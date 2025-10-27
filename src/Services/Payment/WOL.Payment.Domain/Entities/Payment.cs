using WOL.Shared.Common.Domain;
using WOL.Shared.Common.Exceptions;
using WOL.Payment.Domain.Enums;
using WOL.Payment.Domain.Events;

namespace WOL.Payment.Domain.Entities;

public class Payment : BaseEntity
{
    public string TransactionId { get; private set; } = string.Empty;
    public Guid BookingId { get; private set; }
    public Guid CustomerId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? PaymentGatewayReference { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? FailureReason { get; private set; }

    private Payment() { }

    public static Payment Create(
        Guid bookingId,
        Guid customerId,
        decimal amount,
        PaymentMethod paymentMethod)
    {
        if (amount <= 0)
            throw new DomainException("Payment amount must be greater than zero");

        return new Payment
        {
            TransactionId = GenerateTransactionId(),
            BookingId = bookingId,
            CustomerId = customerId,
            Amount = amount,
            PaymentMethod = paymentMethod,
            Status = PaymentStatus.Pending
        };
    }

    public void MarkProcessing(string paymentGatewayReference)
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException("Only pending payments can be marked as processing");

        Status = PaymentStatus.Processing;
        PaymentGatewayReference = paymentGatewayReference;
        ProcessedAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void MarkCompleted()
    {
        if (Status != PaymentStatus.Processing)
            throw new DomainException("Only processing payments can be marked as completed");

        Status = PaymentStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        SetUpdatedAt();

        AddDomainEvent(new PaymentProcessedEvent
        {
            PaymentId = Id,
            BookingId = BookingId,
            CustomerId = CustomerId,
            Amount = Amount,
            PaymentMethod = PaymentMethod.ToString(),
            OccurredOn = DateTime.UtcNow
        });
    }

    public void MarkFailed(string reason)
    {
        if (Status == PaymentStatus.Completed || Status == PaymentStatus.Refunded)
            throw new DomainException("Cannot mark completed or refunded payment as failed");

        Status = PaymentStatus.Failed;
        FailureReason = reason;
        SetUpdatedAt();
    }

    public void MarkRefunded()
    {
        if (Status != PaymentStatus.Completed)
            throw new DomainException("Only completed payments can be refunded");

        Status = PaymentStatus.Refunded;
        SetUpdatedAt();
    }

    public void Cancel()
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException("Only pending payments can be cancelled");

        Status = PaymentStatus.Cancelled;
        SetUpdatedAt();
    }

    private static string GenerateTransactionId()
    {
        return $"TXN{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }
}
