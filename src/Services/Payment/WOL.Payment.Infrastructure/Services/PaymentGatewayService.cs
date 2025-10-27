using WOL.Payment.Application.Services;
using WOL.Payment.Domain.Enums;

namespace WOL.Payment.Infrastructure.Services;

public class PaymentGatewayService : IPaymentGatewayService
{
    public async Task<string> ProcessPaymentAsync(
        string transactionId,
        decimal amount,
        PaymentMethod paymentMethod,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(100, cancellationToken);
        
        return $"GW-{transactionId}-{Guid.NewGuid():N}";
    }
}
