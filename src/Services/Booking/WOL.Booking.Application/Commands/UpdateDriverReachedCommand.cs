using MediatR;
using MassTransit;
using WOL.Booking.Domain.Repositories;
using WOL.Shared.Messages.Events;

namespace WOL.Booking.Application.Commands;

public record UpdateDriverReachedCommand : IRequest<UpdateDriverReachedResponse>
{
    public Guid BookingId { get; init; }
    public Guid DriverId { get; init; }
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
    public string PhotoPath { get; init; } = string.Empty;
    public DateTime ReachedAt { get; init; }
}

public record UpdateDriverReachedResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}

public class UpdateDriverReachedCommandHandler : IRequestHandler<UpdateDriverReachedCommand, UpdateDriverReachedResponse>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateDriverReachedCommandHandler(
        IBookingRepository bookingRepository,
        IPublishEndpoint publishEndpoint)
    {
        _bookingRepository = bookingRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<UpdateDriverReachedResponse> Handle(UpdateDriverReachedCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId);

        if (booking == null)
        {
            return new UpdateDriverReachedResponse
            {
                Success = false,
                Message = "Booking not found"
            };
        }

        if (booking.DriverId != request.DriverId)
        {
            return new UpdateDriverReachedResponse
            {
                Success = false,
                Message = "Driver not assigned to this booking"
            };
        }

        var pickupLatitude = booking.PickupLocation.Latitude;
        var pickupLongitude = booking.PickupLocation.Longitude;
        var distance = CalculateDistance(
            (double)pickupLatitude, 
            (double)pickupLongitude, 
            (double)request.Latitude, 
            (double)request.Longitude);

        if (distance > 0.5)
        {
            return new UpdateDriverReachedResponse
            {
                Success = false,
                Message = $"Driver location is {distance:F2} km away from pickup location. Must be within 500m."
            };
        }

        booking.UpdateDriverReached(request.PhotoPath, request.Latitude, request.Longitude, request.ReachedAt);
        await _bookingRepository.UpdateAsync(booking);

        await _publishEndpoint.Publish(new DriverReachedEvent
        {
            BookingId = booking.Id,
            DriverId = request.DriverId,
            CustomerId = booking.CustomerId,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            PhotoPath = request.PhotoPath,
            ReachedAt = request.ReachedAt
        }, cancellationToken);

        return new UpdateDriverReachedResponse
        {
            Success = true,
            Message = "Driver reached status updated successfully"
        };
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371;
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}
