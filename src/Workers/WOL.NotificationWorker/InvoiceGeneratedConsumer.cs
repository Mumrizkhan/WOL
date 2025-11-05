using MassTransit;
using Microsoft.Extensions.Logging;
using WOL.NotificationWorker.Services;
using WOL.Shared.Messages.Events;

namespace WOL.NotificationWorker;

public class InvoiceGeneratedConsumer : IConsumer<InvoiceGeneratedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<InvoiceGeneratedConsumer> _logger;

    public InvoiceGeneratedConsumer(
        INotificationService notificationService,
        ILogger<InvoiceGeneratedConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<InvoiceGeneratedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing InvoiceGenerated notification for Invoice {InvoiceNumber}", message.InvoiceNumber);

        try
        {
            var notificationMessage = $"Invoice {message.InvoiceNumber} has been generated. " +
                                    $"Amount: SAR {message.TotalAmount:F2}. " +
                                    $"Payment Terms: {message.PaymentTerms}. " +
                                    $"Due Date: {message.DueDate:yyyy-MM-dd}.";

            await _notificationService.SendPushNotificationAsync(
                message.CustomerId,
                "Invoice Generated",
                notificationMessage);

            await _notificationService.SendEmailAsync(
                message.CustomerId,
                $"Invoice {message.InvoiceNumber} - World of Logistics",
                notificationMessage);

            _logger.LogInformation("InvoiceGenerated notification sent successfully for Invoice {InvoiceNumber}", message.InvoiceNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending InvoiceGenerated notification for Invoice {InvoiceNumber}", message.InvoiceNumber);
            throw;
        }
    }
}
