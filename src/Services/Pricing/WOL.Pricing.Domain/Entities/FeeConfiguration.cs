using WOL.Shared.Common.Domain;

namespace WOL.Pricing.Domain.Entities;

public class FeeConfiguration : BaseEntity
{
    public string FeeType { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    private FeeConfiguration() { }

    public static FeeConfiguration Create(string feeType, decimal amount, string? description = null)
    {
        return new FeeConfiguration
        {
            FeeType = feeType,
            Amount = amount,
            Description = description,
            IsActive = true
        };
    }

    public void UpdateAmount(decimal newAmount)
    {
        Amount = newAmount;
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
}
