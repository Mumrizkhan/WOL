using MediatR;
using WOL.Pricing.Domain.Repositories;

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
        var totalPrice = basePrice + distancePrice + weightPrice;

        return new CalculatePriceResponse
        {
            TotalPrice = totalPrice,
            BasePrice = basePrice,
            DistancePrice = distancePrice,
            WeightPrice = weightPrice
        };
    }
}
