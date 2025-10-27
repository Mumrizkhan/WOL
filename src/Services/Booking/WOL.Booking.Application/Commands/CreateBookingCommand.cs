using MediatR;
using WOL.Booking.Domain.Enums;

namespace WOL.Booking.Application.Commands;

public record CreateBookingCommand : IRequest<CreateBookingResponse>
{
    public Guid CustomerId { get; init; }
    public Guid VehicleTypeId { get; init; }
    public LocationDto Origin { get; init; } = null!;
    public LocationDto Destination { get; init; } = null!;
    public DateTime PickupDate { get; init; }
    public TimeSpan PickupTime { get; init; }
    public CargoDetailsDto Cargo { get; init; } = null!;
    public ContactInfoDto Shipper { get; init; } = null!;
    public ContactInfoDto Receiver { get; init; } = null!;
    public BookingType BookingType { get; init; }
}

public record LocationDto
{
    public string Address { get; init; } = string.Empty;
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
    public string City { get; init; } = string.Empty;
}

public record CargoDetailsDto
{
    public string Type { get; init; } = string.Empty;
    public decimal GrossWeightKg { get; init; }
    public decimal? NetWeightKg { get; init; }
    public int? NumberOfBoxes { get; init; }
    public string? Description { get; init; }
}

public record ContactInfoDto
{
    public string Name { get; init; } = string.Empty;
    public string Mobile { get; init; } = string.Empty;
    public string? Email { get; init; }
}

public record CreateBookingResponse
{
    public Guid BookingId { get; init; }
    public string BookingNumber { get; init; } = string.Empty;
    public decimal TotalFare { get; init; }
    public string Message { get; init; } = string.Empty;
}
