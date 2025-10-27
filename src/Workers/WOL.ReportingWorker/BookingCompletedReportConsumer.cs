using MassTransit;
using WOL.Shared.Messages.Events;

namespace WOL.ReportingWorker;

public class BookingCompletedReportConsumer : IConsumer<BookingCompletedEvent>
{
    private readonly ILogger<BookingCompletedReportConsumer> _logger;

    public BookingCompletedReportConsumer(ILogger<BookingCompletedReportConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BookingCompletedEvent> context)
    {
        _logger.LogInformation("Processing report data for BookingCompletedEvent: {BookingId}", context.Message.BookingId);

        await Task.Delay(100);

        _logger.LogInformation("Report data recorded for Booking {BookingId}", context.Message.BookingId);
    }
}
