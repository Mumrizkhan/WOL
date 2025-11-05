using MediatR;
using MassTransit;
using WOL.Booking.Domain.Repositories;
using WOL.Shared.Messages.Events;

namespace WOL.Booking.Application.Commands;

public record CompleteBookingCommand : IRequest<CompleteBookingResponse>
{
    public Guid BookingId { get; init; }
    public DateTime CompletedAt { get; init; }
}

public record CompleteBookingResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}

public class CompleteBookingCommandHandler : IRequestHandler<CompleteBookingCommand, CompleteBookingResponse>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public CompleteBookingCommandHandler(
        IBookingRepository bookingRepository,
        IPublishEndpoint publishEndpoint)
    {
        _bookingRepository = bookingRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<CompleteBookingResponse> Handle(CompleteBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId);

        if (booking == null)
        {
            return new CompleteBookingResponse
            {
                Success = false,
                Message = "Booking not found"
            };
        }

        booking.Complete(request.CompletedAt);
        await _bookingRepository.UpdateAsync(booking);

        await _publishEndpoint.Publish(new BookingCompletedEvent
        {
            BookingId = booking.Id,
            DriverId = booking.DriverId ?? Guid.Empty,
            VehicleId = booking.VehicleId ?? Guid.Empty,
            OriginCity = booking.PickupLocation.City,
            DestinationCity = booking.DeliveryLocation.City,
            CompletedAt = request.CompletedAt,
            TotalAmount = booking.TotalPrice
        }, cancellationToken);

        return new CompleteBookingResponse
        {
            Success = true,
            Message = "Booking completed successfully"
        };
    }
}
