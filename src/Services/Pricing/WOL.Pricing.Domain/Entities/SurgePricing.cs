using WOL.Shared.Common.Domain;

namespace WOL.Pricing.Domain.Entities;

public class SurgePricing : BaseEntity
{
    public string City { get; private set; } = string.Empty;
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public decimal Multiplier { get; private set; }
    public bool IsActive { get; private set; }
    public string? Description { get; private set; }

    private SurgePricing() { }

    public static SurgePricing Create(
        string city,
        DayOfWeek dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        decimal multiplier,
        string? description = null)
    {
        return new SurgePricing
        {
            City = city,
            DayOfWeek = dayOfWeek,
            StartTime = startTime,
            EndTime = endTime,
            Multiplier = multiplier,
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

    public bool IsApplicable(DateTime dateTime, string city)
    {
        if (!IsActive) return false;
        if (!City.Equals(city, StringComparison.OrdinalIgnoreCase)) return false;
        if (dateTime.DayOfWeek != DayOfWeek) return false;
        
        var timeOfDay = dateTime.TimeOfDay;
        return timeOfDay >= StartTime && timeOfDay <= EndTime;
    }

    public decimal ApplySurge(decimal baseAmount)
    {
        return baseAmount * Multiplier;
    }
}
