using WOL.Shared.Common.Domain;

namespace WOL.Pricing.Domain.Entities;

public class DiscountConfiguration : BaseEntity
{
    public string DiscountType { get; private set; } = string.Empty;
    public decimal Percentage { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    private DiscountConfiguration() { }

    public static DiscountConfiguration Create(
        string discountType, 
        decimal percentage, 
        string? description = null)
    {
        return new DiscountConfiguration
        {
            DiscountType = discountType,
            Percentage = percentage,
            Description = description,
            IsActive = true
        };
    }

    public void UpdatePercentage(decimal newPercentage)
    {
        if (newPercentage < 0 || newPercentage > 1)
            throw new ArgumentException("Percentage must be between 0 and 1");

        Percentage = newPercentage;
        SetUpdatedAt();
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

    public decimal CalculateDiscount(decimal baseAmount)
    {
        return baseAmount * Percentage;
    }
}
