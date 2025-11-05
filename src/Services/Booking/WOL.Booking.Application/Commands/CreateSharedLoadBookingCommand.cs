using MediatR;
using MassTransit;
using WOL.Booking.Domain.Entities;
using WOL.Booking.Domain.Repositories;
using WOL.Shared.Messages.Events;

namespace WOL.Booking.Application.Commands;

public record CreateSharedLoadBookingCommand : IRequest<CreateSharedLoadBookingResponse>
{
    public Guid CustomerId { get; init; }
    public string PickupCity { get; init; } = string.Empty;
    public string DeliveryCity { get; init; } = string.Empty;
    public DateTime PickupDate { get; init; }
    public decimal Weight { get; init; }
    public decimal Volume { get; init; }
    public string VehicleType { get; init; } = string.Empty;
    public decimal VehicleCapacity { get; init; }
}

public record CreateSharedLoadBookingResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public Guid? BookingId { get; init; }
    public Guid? SharedLoadPoolId { get; init; }
    public bool IsNewPool { get; init; }
    public decimal CurrentUtilization { get; init; }
}

public class CreateSharedLoadBookingCommandHandler : IRequestHandler<CreateSharedLoadBookingCommand, CreateSharedLoadBookingResponse>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ISharedLoadBookingRepository _sharedLoadRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateSharedLoadBookingCommandHandler(
        IBookingRepository bookingRepository,
        ISharedLoadBookingRepository sharedLoadRepository,
        IPublishEndpoint publishEndpoint)
    {
        _bookingRepository = bookingRepository;
        _sharedLoadRepository = sharedLoadRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<CreateSharedLoadBookingResponse> Handle(CreateSharedLoadBookingCommand request, CancellationToken cancellationToken)
    {
        var existingPools = await _sharedLoadRepository.GetOpenPoolsAsync(
            request.PickupCity,
            request.DeliveryCity,
            request.PickupDate,
            request.VehicleType);

        SharedLoadBooking? pool = null;
        bool isNewPool = false;

        foreach (var existingPool in existingPools)
        {
            if (existingPool.CanAddBooking(request.Weight, request.Volume))
            {
                pool = existingPool;
                break;
            }
        }

        if (pool == null)
        {
            pool = SharedLoadBooking.Create(
                request.PickupCity,
                request.DeliveryCity,
                request.PickupDate,
                request.VehicleType,
                request.VehicleCapacity);

            await _sharedLoadRepository.AddAsync(pool);
            isNewPool = true;
        }

        var booking = Booking.CreateSharedLoad(
            request.CustomerId,
            request.PickupCity,
            request.DeliveryCity,
            request.PickupDate,
            request.Weight,
            request.Volume,
            request.VehicleType);

        pool.AddBooking(booking.Id, request.Weight, request.Volume);
        await _sharedLoadRepository.UpdateAsync(pool);
        await _bookingRepository.AddAsync(booking);

        await _publishEndpoint.Publish(new SharedLoadCapacityUpdatedEvent
        {
            SharedLoadId = pool.Id,
            OriginCity = request.PickupCity,
            DestinationCity = request.DeliveryCity,
            CurrentWeight = pool.CurrentWeight,
            CurrentVolume = pool.CurrentVolume,
            TotalCapacity = pool.TotalCapacity,
            UtilizationPercentage = pool.UtilizationPercentage,
            Status = pool.Status,
            Timestamp = DateTime.UtcNow
        }, cancellationToken);

        if (pool.Status == "Full")
        {
            await _publishEndpoint.Publish(new SharedLoadPoolFullEvent
            {
                SharedLoadId = pool.Id,
                OriginCity = request.PickupCity,
                DestinationCity = request.DeliveryCity,
                TotalBookings = pool.BookingIds.Count,
                TotalWeight = pool.CurrentWeight,
                TotalVolume = pool.CurrentVolume,
                Timestamp = DateTime.UtcNow
            }, cancellationToken);
        }

        return new CreateSharedLoadBookingResponse
        {
            Success = true,
            Message = isNewPool ? "New shared load pool created" : "Added to existing shared load pool",
            BookingId = booking.Id,
            SharedLoadPoolId = pool.Id,
            IsNewPool = isNewPool,
            CurrentUtilization = pool.UtilizationPercentage
        };
    }
}
