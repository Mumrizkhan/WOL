using MassTransit;
using WOL.Shared.Messages.Events;
using WOL.ReportingWorker.Services;

namespace WOL.ReportingWorker;

public class BookingCompletedReportConsumer : IConsumer<BookingCompletedEvent>
{
    private readonly ILogger<BookingCompletedReportConsumer> _logger;
    private readonly IReportingService _reportingService;

    public BookingCompletedReportConsumer(
        ILogger<BookingCompletedReportConsumer> logger,
        IReportingService reportingService)
    {
        _logger = logger;
        _reportingService = reportingService;
    }

    public async Task Consume(ConsumeContext<BookingCompletedEvent> context)
    {
        _logger.LogInformation("Processing report data for BookingCompletedEvent: {BookingId}", context.Message.BookingId);

        var message = context.Message;

        await _reportingService.AggregateBookingDataAsync(
            message.BookingId,
            message.CustomerId,
            message.TotalAmount,
            message.CompletedAt);

        _logger.LogInformation("Report data aggregated for Booking {BookingId}", context.Message.BookingId);
    }
}
