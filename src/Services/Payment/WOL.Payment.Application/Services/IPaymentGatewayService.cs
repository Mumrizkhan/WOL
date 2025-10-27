using WOL.Payment.Domain.Enums;

namespace WOL.Payment.Application.Services;

public interface IPaymentGatewayService
{
    Task<string> ProcessPaymentAsync(
        string transactionId,
        decimal amount,
        PaymentMethod paymentMethod,
        CancellationToken cancellationToken = default);
}
