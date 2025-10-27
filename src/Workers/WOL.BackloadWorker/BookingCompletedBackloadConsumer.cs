using MassTransit;
using WOL.Shared.Messages.Events;
using WOL.BackloadWorker.Services;

namespace WOL.BackloadWorker;

public class BookingCompletedBackloadConsumer : IConsumer<BookingCompletedEvent>
{
    private readonly ILogger<BookingCompletedBackloadConsumer> _logger;
    private readonly IBackloadMatchingService _backloadMatchingService;

    public BookingCompletedBackloadConsumer(
        ILogger<BookingCompletedBackloadConsumer> logger,
        IBackloadMatchingService backloadMatchingService)
    {
        _logger = logger;
        _backloadMatchingService = backloadMatchingService;
    }

    public async Task Consume(ConsumeContext<BookingCompletedEvent> context)
    {
        _logger.LogInformation("Updating backload availability for completed booking: {BookingId}", context.Message.BookingId);

        var message = context.Message;

        await _backloadMatchingService.UpdateBackloadAvailabilityAsync(
            message.BookingId,
            "completed");

        _logger.LogInformation("Backload availability updated for Booking {BookingId}", context.Message.BookingId);
    }
}
