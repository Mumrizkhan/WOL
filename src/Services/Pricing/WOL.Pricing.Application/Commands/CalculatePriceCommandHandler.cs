using MediatR;
using WOL.Pricing.Domain.Repositories;
using WOL.Pricing.Domain.Services;

namespace WOL.Pricing.Application.Commands;

public class CalculatePriceCommandHandler : IRequestHandler<CalculatePriceCommand, CalculatePriceResponse>
{
    private readonly IPricingRuleRepository _pricingRuleRepository;

    public CalculatePriceCommandHandler(IPricingRuleRepository pricingRuleRepository)
    {
        _pricingRuleRepository = pricingRuleRepository;
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

        var basePrice = rule.BasePrice;
        var distancePrice = rule.PricePerKm * request.Distance;
        var weightPrice = rule.PricePerKg * request.Weight;
        var subTotal = basePrice + distancePrice + weightPrice;

        decimal backloadDiscount = 0;
        decimal flexibleDateDiscount = 0;
        decimal sharedLoadDiscount = 0;

        if (request.IsBackload || request.IsFlexibleDate)
        {
            var discount = BackloadDiscountCalculator.CalculateBackloadDiscount(
                subTotal, 
                request.IsBackload, 
                request.IsFlexibleDate);
            
            if (request.IsBackload)
            {
                backloadDiscount = subTotal * BackloadDiscountCalculator.GetBackloadDiscountPercentage();
            }
            
            if (request.IsFlexibleDate)
            {
                flexibleDateDiscount = subTotal * 0.05m; // 5%
            }
        }

        if (request.IsSharedLoad && request.CapacityUtilization.HasValue)
        {
            sharedLoadDiscount = BackloadDiscountCalculator.CalculateSharedLoadDiscount(
                subTotal, 
                request.CapacityUtilization.Value);
        }

        var totalDiscount = backloadDiscount + flexibleDateDiscount + sharedLoadDiscount;
        var totalPrice = subTotal - totalDiscount;

        return new CalculatePriceResponse
        {
            BasePrice = basePrice,
            DistancePrice = distancePrice,
            WeightPrice = weightPrice,
            SubTotal = subTotal,
            BackloadDiscount = backloadDiscount,
            FlexibleDateDiscount = flexibleDateDiscount,
            SharedLoadDiscount = sharedLoadDiscount,
            TotalDiscount = totalDiscount,
            TotalPrice = totalPrice
        };
    }
}
