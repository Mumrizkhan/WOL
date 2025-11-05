using MediatR;
using MassTransit;
using WOL.Pricing.Domain.Repositories;
using WOL.Pricing.Domain.Services;
using WOL.Pricing.Domain.Entities;
using WOL.Shared.Messages.Events;

namespace WOL.Pricing.Application.Commands;

public class CalculatePriceCommandHandler : IRequestHandler<CalculatePriceCommand, CalculatePriceResponse>
{
    private readonly IPricingRuleRepository _pricingRuleRepository;
    private readonly IFeeConfigurationRepository _feeConfigurationRepository;
    private readonly IDiscountConfigurationRepository _discountConfigurationRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public CalculatePriceCommandHandler(
        IPricingRuleRepository pricingRuleRepository,
        IFeeConfigurationRepository feeConfigurationRepository,
        IDiscountConfigurationRepository discountConfigurationRepository,
        IPublishEndpoint publishEndpoint)
    {
        _pricingRuleRepository = pricingRuleRepository;
        _feeConfigurationRepository = feeConfigurationRepository;
        _discountConfigurationRepository = discountConfigurationRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<CalculatePriceResponse> Handle(CalculatePriceCommand request, CancellationToken cancellationToken)
    {
        var rule = await _pricingRuleRepository.GetByRouteAndVehicleAsync(
            request.FromCity, 
            request.ToCity, 
            request.VehicleType, 
            cancellationToken);

        if (rule == null)
        {
            throw new InvalidOperationException("No pricing rule found for the specified route and vehicle type");
        }

        var lineItems = new List<PriceLineItem>();

        var basePrice = rule.BasePrice;
        lineItems.Add(new PriceLineItem { Description = "Base Fare", Amount = basePrice, Type = "Charge" });

        var distancePrice = rule.PricePerKm * request.Distance;
        lineItems.Add(new PriceLineItem { Description = $"Distance ({request.Distance} km)", Amount = distancePrice, Type = "Charge" });

        var weightPrice = rule.PricePerKg * request.Weight;
        lineItems.Add(new PriceLineItem { Description = $"Weight ({request.Weight} kg)", Amount = weightPrice, Type = "Charge" });

        var subTotal = basePrice + distancePrice + weightPrice;

        decimal backloadDiscount = 0;
        if (request.IsBackload)
        {
            var backloadConfig = await _discountConfigurationRepository.GetByTypeAsync("Backload");
            var discountPercentage = backloadConfig?.DiscountPercentage ?? 0.15m;
            backloadDiscount = subTotal * discountPercentage;
            lineItems.Add(new PriceLineItem { Description = $"Backload Discount ({discountPercentage * 100}%)", Amount = -backloadDiscount, Type = "Discount" });
            
            await _publishEndpoint.Publish(new BackloadDiscountAppliedEvent
            {
                CustomerId = request.CustomerId,
                BookingId = Guid.NewGuid(),
                DiscountPercentage = discountPercentage,
                DiscountAmount = backloadDiscount,
                Timestamp = DateTime.UtcNow
            }, cancellationToken);
        }

        decimal flexibleDateDiscount = 0;
        if (request.IsFlexibleDate)
        {
            var flexibleConfig = await _discountConfigurationRepository.GetByTypeAsync("FlexibleDate");
            var discountPercentage = flexibleConfig?.DiscountPercentage ?? 0.05m;
            flexibleDateDiscount = subTotal * discountPercentage;
            lineItems.Add(new PriceLineItem { Description = $"Flexible Date Discount ({discountPercentage * 100}%)", Amount = -flexibleDateDiscount, Type = "Discount" });
        }

        decimal sharedLoadDiscount = 0;
        if (request.IsSharedLoad && request.CapacityUtilization.HasValue)
        {
            sharedLoadDiscount = BackloadDiscountCalculator.CalculateSharedLoadDiscount(subTotal, request.CapacityUtilization.Value);
            var discountPercentage = sharedLoadDiscount / subTotal;
            lineItems.Add(new PriceLineItem { Description = $"Shared Load Discount ({discountPercentage * 100:F1}%)", Amount = -sharedLoadDiscount, Type = "Discount" });
        }

        decimal loyaltyDiscount = 0;

        decimal surgeAmount = 0;
        var surgePricing = await _pricingRuleRepository.GetSurgePricingAsync(request.FromCity, request.BookingDateTime.DayOfWeek, request.BookingDateTime.TimeOfDay);
        if (surgePricing != null && surgePricing.IsApplicable(request.BookingDateTime))
        {
            surgeAmount = surgePricing.ApplySurge(subTotal);
            lineItems.Add(new PriceLineItem { Description = $"Surge Pricing ({surgePricing.Multiplier}x)", Amount = surgeAmount, Type = "Charge" });
            
            await _publishEndpoint.Publish(new SurgePricingAppliedEvent
            {
                CustomerId = request.CustomerId,
                BookingId = Guid.NewGuid(),
                City = request.FromCity,
                Multiplier = surgePricing.Multiplier,
                SurgeAmount = surgeAmount,
                Timestamp = DateTime.UtcNow
            }, cancellationToken);
        }

        decimal waitingCharges = 0;
        if (request.WaitingHours.HasValue && request.WaitingHours.Value > 2)
        {
            var waitingFeeConfig = await _feeConfigurationRepository.GetByTypeAsync("WaitingCharge");
            var hourlyRate = waitingFeeConfig?.Amount ?? 100m;
            var chargeableHours = request.WaitingHours.Value - 2;
            waitingCharges = chargeableHours * hourlyRate;
            lineItems.Add(new PriceLineItem { Description = $"Waiting Charges ({chargeableHours} hrs @ {hourlyRate} SAR/hr)", Amount = waitingCharges, Type = "Charge" });
        }

        decimal cancellationFee = 0;
        if (request.IsCancelled)
        {
            var cancellationFeeConfig = await _feeConfigurationRepository.GetByTypeAsync("CancellationFee");
            cancellationFee = cancellationFeeConfig?.Amount ?? 500m;
            lineItems.Add(new PriceLineItem { Description = "Cancellation Fee", Amount = cancellationFee, Type = "Charge" });
        }

        var totalDiscount = backloadDiscount + flexibleDateDiscount + sharedLoadDiscount + loyaltyDiscount;
        var totalPrice = subTotal - totalDiscount + surgeAmount + waitingCharges + cancellationFee;

        return new CalculatePriceResponse
        {
            BasePrice = basePrice,
            DistancePrice = distancePrice,
            WeightPrice = weightPrice,
            SubTotal = subTotal,
            BackloadDiscount = backloadDiscount,
            FlexibleDateDiscount = flexibleDateDiscount,
            SharedLoadDiscount = sharedLoadDiscount,
            LoyaltyDiscount = loyaltyDiscount,
            TotalDiscount = totalDiscount,
            SurgeAmount = surgeAmount,
            WaitingCharges = waitingCharges,
            CancellationFee = cancellationFee,
            TotalPrice = totalPrice,
            LineItems = lineItems
        };
    }
}
