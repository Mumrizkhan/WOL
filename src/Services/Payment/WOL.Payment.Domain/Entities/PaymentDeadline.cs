using WOL.Shared.Common.Domain;

namespace WOL.Payment.Domain.Entities;

public class PaymentDeadline : BaseEntity
{
    public Guid BookingId { get; private set; }
    public Guid CustomerId { get; private set; }
    public string CustomerType { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public DateTime DeadlineAt { get; private set; }
    public bool IsPaid { get; private set; }
    public bool IsExpired { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public DateTime? ExpiredAt { get; private set; }

    private PaymentDeadline() { }

    public static PaymentDeadline Create(
        Guid bookingId,
        Guid customerId,
        string customerType,
        decimal amount,
        int deadlineMinutes)
    {
        return new PaymentDeadline
        {
            BookingId = bookingId,
            CustomerId = customerId,
            CustomerType = customerType,
            Amount = amount,
            DeadlineAt = DateTime.UtcNow.AddMinutes(deadlineMinutes),
            IsPaid = false,
            IsExpired = false
        };
    }

    public void MarkPaid()
    {
        IsPaid = true;
        PaidAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void MarkExpired()
    {
        IsExpired = true;
        ExpiredAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public bool HasExpired()
    {
        return DateTime.UtcNow > DeadlineAt && !IsPaid;
    }
}
