using MediatR;

namespace WOL.Pricing.Application.Commands;

public record CalculatePriceCommand : IRequest<CalculatePriceResponse>
{
    public Guid CustomerId { get; init; }
    public string VehicleType { get; init; } = string.Empty;
    public string FromCity { get; init; } = string.Empty;
    public string ToCity { get; init; } = string.Empty;
    public decimal Distance { get; init; }
    public decimal Weight { get; init; }
    public bool IsBackload { get; init; }
    public bool IsFlexibleDate { get; init; }
    public bool IsSharedLoad { get; init; }
    public decimal? CapacityUtilization { get; init; }
    public DateTime BookingDateTime { get; init; }
    public decimal? WaitingHours { get; init; }
    public bool IsCancelled { get; init; }
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
    public decimal LoyaltyDiscount { get; init; }
    public decimal TotalDiscount { get; init; }
    public decimal SurgeAmount { get; init; }
    public decimal WaitingCharges { get; init; }
    public decimal CancellationFee { get; init; }
    public decimal TotalPrice { get; init; }
    public List<PriceLineItem> LineItems { get; init; } = new();
}

public record PriceLineItem
{
    public string Description { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Type { get; init; } = string.Empty;
}
