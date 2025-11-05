using MediatR;

namespace WOL.Tracking.Application.Commands;

public record RecordLocationCommand : IRequest<RecordLocationResponse>
{
    public Guid BookingId { get; init; }
    public Guid VehicleId { get; init; }
    public Guid DriverId { get; init; }
    public Guid CustomerId { get; init; }
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
    public decimal? Speed { get; init; }
    public decimal? Heading { get; init; }
    public decimal? DestinationLatitude { get; init; }
    public decimal? DestinationLongitude { get; init; }
}

public record RecordLocationResponse
{
    public Guid LocationId { get; init; }
    public string Message { get; init; } = string.Empty;
}
