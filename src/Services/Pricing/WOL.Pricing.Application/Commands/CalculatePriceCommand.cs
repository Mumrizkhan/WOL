using MediatR;

namespace WOL.Pricing.Application.Commands;

public record CalculatePriceCommand : IRequest<CalculatePriceResponse>
{
    public string VehicleType { get; init; } = string.Empty;
    public string FromCity { get; init; } = string.Empty;
    public string ToCity { get; init; } = string.Empty;
    public decimal Distance { get; init; }
    public decimal Weight { get; init; }
}

public record CalculatePriceResponse
{
    public decimal TotalPrice { get; init; }
    public decimal BasePrice { get; init; }
    public decimal DistancePrice { get; init; }
    public decimal WeightPrice { get; init; }
}
