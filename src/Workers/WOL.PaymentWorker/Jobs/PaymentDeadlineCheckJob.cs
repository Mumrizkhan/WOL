using Hangfire;
using MassTransit;
using Microsoft.Extensions.Logging;
using WOL.Payment.Domain.Repositories;
using WOL.Shared.Messages.Events;

namespace WOL.PaymentWorker.Jobs;

public class PaymentDeadlineCheckJob
{
    private readonly IPaymentDeadlineRepository _paymentDeadlineRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<PaymentDeadlineCheckJob> _logger;

    public PaymentDeadlineCheckJob(
        IPaymentDeadlineRepository paymentDeadlineRepository,
        IPublishEndpoint publishEndpoint,
        ILogger<PaymentDeadlineCheckJob> _logger)
    {
        _paymentDeadlineRepository = paymentDeadlineRepository;
        _publishEndpoint = publishEndpoint;
        this._logger = _logger;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task Execute()
    {
        _logger.LogInformation("Starting payment deadline check job at {Time}", DateTime.UtcNow);

        try
        {
            var allDeadlines = await _paymentDeadlineRepository.GetAllAsync();
            var expiredDeadlines = allDeadlines
                .Where(d => d.Status == "Pending" && d.DeadlineAt <= DateTime.UtcNow)
                .ToList();

            _logger.LogInformation("Found {Count} expired payment deadlines", expiredDeadlines.Count);

            foreach (var deadline in expiredDeadlines)
            {
                deadline.MarkAsExpired();
                await _paymentDeadlineRepository.UpdateAsync(deadline);

                await _publishEndpoint.Publish(new PaymentDeadlineExpiredEvent
                {
                    BookingId = deadline.BookingId,
                    CustomerId = deadline.CustomerId,
                    Amount = deadline.Amount,
                    DeadlineAt = deadline.DeadlineAt,
                    Timestamp = DateTime.UtcNow
                });

                _logger.LogWarning(
                    "Published PaymentDeadlineExpiredEvent for booking {BookingId}, customer {CustomerId}",
                    deadline.BookingId,
                    deadline.CustomerId);
            }

            _logger.LogInformation("Payment deadline check job completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing payment deadline check job");
            throw;
        }
    }
}
