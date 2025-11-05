using WOL.Shared.Common.Domain;

namespace WOL.Booking.Domain.Entities;

public class BANTiming : BaseEntity
{
    public string City { get; private set; } = string.Empty;
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public bool IsActive { get; private set; }
    public string? Description { get; private set; }

    private BANTiming() { }

    public static BANTiming Create(
        string city,
        DayOfWeek dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        string? description = null)
    {
        return new BANTiming
        {
            City = city,
            DayOfWeek = dayOfWeek,
            StartTime = startTime,
            EndTime = endTime,
            IsActive = true,
            Description = description
        };
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }

    public bool IsWithinBANPeriod(DateTime dateTime)
    {
        if (!IsActive) return false;
        if (dateTime.DayOfWeek != DayOfWeek) return false;

        var timeOfDay = dateTime.TimeOfDay;
        return timeOfDay >= StartTime && timeOfDay <= EndTime;
    }
}
