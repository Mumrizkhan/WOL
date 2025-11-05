using WOL.Shared.Common.Domain;

namespace WOL.Payment.Domain.Entities;

public class DriverPayout : BaseEntity
{
    public Guid DriverId { get; private set; }
    public DateTime PeriodStart { get; private set; }
    public DateTime PeriodEnd { get; private set; }
    public decimal TotalEarnings { get; private set; }
    public decimal TotalPenalties { get; private set; }
    public decimal NetPayout { get; private set; }
    public string Status { get; private set; } = "Pending";
    public DateTime? ProcessedAt { get; private set; }
    public string? ProcessedBy { get; private set; }

    private readonly List<PayoutLineItem> _lineItems = new();
    public IReadOnlyCollection<PayoutLineItem> LineItems => _lineItems.AsReadOnly();

    private DriverPayout() { }

    public static DriverPayout Create(Guid driverId, DateTime periodStart, DateTime periodEnd)
    {
        return new DriverPayout
        {
            DriverId = driverId,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            TotalEarnings = 0,
            TotalPenalties = 0,
            NetPayout = 0,
            Status = "Pending"
        };
    }

    public void AddEarning(Guid bookingId, decimal amount, string description)
    {
        _lineItems.Add(new PayoutLineItem
        {
            Id = Guid.NewGuid(),
            Type = "Earning",
            ReferenceId = bookingId,
            Amount = amount,
            Description = description
        });

        TotalEarnings += amount;
        CalculateNetPayout();
        SetUpdatedAt();
    }

    public void AddPenalty(Guid referenceId, decimal amount, string description)
    {
        _lineItems.Add(new PayoutLineItem
        {
            Id = Guid.NewGuid(),
            Type = "Penalty",
            ReferenceId = referenceId,
            Amount = amount,
            Description = description
        });

        TotalPenalties += amount;
        CalculateNetPayout();
        SetUpdatedAt();
    }

    public void Process(string processedBy)
    {
        Status = "Processed";
        ProcessedAt = DateTime.UtcNow;
        ProcessedBy = processedBy;
        SetUpdatedAt();
    }

    private void CalculateNetPayout()
    {
        NetPayout = TotalEarnings - TotalPenalties;
    }
}

public class PayoutLineItem
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public Guid ReferenceId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}
