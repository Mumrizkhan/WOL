using WOL.Booking.Domain.Enums;

namespace WOL.Booking.Application.Services;

public interface IPricingService
{
    Task<decimal> CalculateFareAsync(
        Guid vehicleTypeId,
        decimal originLat,
        decimal originLng,
        decimal destLat,
        decimal destLng,
        BookingType bookingType,
        CancellationToken cancellationToken = default);
}
