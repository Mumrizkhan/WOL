using MassTransit;
using WOL.Shared.Messages.Events;
using WOL.BackloadWorker.Services;

namespace WOL.BackloadWorker;

public class BookingCreatedBackloadConsumer : IConsumer<BookingCreatedEvent>
{
    private readonly ILogger<BookingCreatedBackloadConsumer> _logger;
    private readonly IBackloadMatchingService _backloadMatchingService;

    public BookingCreatedBackloadConsumer(
        ILogger<BookingCreatedBackloadConsumer> logger,
        IBackloadMatchingService backloadMatchingService)
    {
        _logger = logger;
        _backloadMatchingService = backloadMatchingService;
    }

    public async Task Consume(ConsumeContext<BookingCreatedEvent> context)
    {
        _logger.LogInformation("Processing backload matching for BookingCreatedEvent: {BookingId}", context.Message.BookingId);

        var message = context.Message;

        await _backloadMatchingService.FindBackloadOpportunitiesAsync(
            message.BookingId,
            message.PickupLocation,
            message.DeliveryLocation,
            message.PickupDate);

        _logger.LogInformation("Backload matching completed for Booking {BookingId}", context.Message.BookingId);
    }
}
