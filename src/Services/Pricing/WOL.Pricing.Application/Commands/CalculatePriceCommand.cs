using MediatR;

namespace WOL.Pricing.Application.Commands;

public record CalculatePriceCommand : IRequest<CalculatePriceResponse>
{
    public string VehicleType { get; init; } = string.Empty;
    public string FromCity { get; init; } = string.Empty;
    public string ToCity { get; init; } = string.Empty;
    public decimal Distance { get; init; }
    public decimal Weight { get; init; }
    public bool IsBackload { get; init; }
    public bool IsFlexibleDate { get; init; }
    public bool IsSharedLoad { get; init; }
    public decimal? CapacityUtilization { get; init; }
}

public record CalculatePriceResponse
{
    public decimal BasePrice { get; init; }
    public decimal DistancePrice { get; init; }
    public decimal WeightPrice { get; init; }
    public decimal SubTotal { get; init; }
    public decimal BackloadDiscount { get; init; }
    public decimal FlexibleDateDiscount { get; init; }
    public decimal SharedLoadDiscount { get; init; }
    public decimal TotalDiscount { get; init; }
    public decimal TotalPrice { get; init; }
}
