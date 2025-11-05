using WOL.Shared.Common.Domain;

namespace WOL.Booking.Domain.Entities;

public class WaitingCharge : BaseEntity
{
    public Guid BookingId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public int HoursCharged { get; private set; }
    public decimal HourlyRate { get; private set; }
    public decimal TotalAmount { get; private set; }
    public bool IsPaid { get; private set; }

    private WaitingCharge() { }

    public static WaitingCharge Create(Guid bookingId, DateTime startTime, decimal hourlyRate)
    {
        return new WaitingCharge
        {
            BookingId = bookingId,
            StartTime = startTime,
            HourlyRate = hourlyRate,
            HoursCharged = 0,
            TotalAmount = 0,
            IsPaid = false
        };
    }

    public void AddHour()
    {
        HoursCharged++;
        TotalAmount = HoursCharged * HourlyRate;
        SetUpdatedAt();
    }

    public void Complete(DateTime endTime)
    {
        EndTime = endTime;
        SetUpdatedAt();
    }

    public void MarkPaid()
    {
        IsPaid = true;
        SetUpdatedAt();
    }
}
