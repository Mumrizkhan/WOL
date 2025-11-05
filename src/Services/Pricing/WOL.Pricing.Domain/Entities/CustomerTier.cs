using WOL.Shared.Common.Domain;

namespace WOL.Pricing.Domain.Entities;

public class CustomerTier : BaseEntity
{
    public Guid CustomerId { get; private set; }
    public string TierLevel { get; private set; } = "Bronze";
    public int TotalBookings { get; private set; }
    public decimal TotalSpent { get; private set; }
    public decimal DiscountPercentage { get; private set; }
    public DateTime? LastBookingDate { get; private set; }

    private CustomerTier() { }

    public static CustomerTier Create(Guid customerId)
    {
        return new CustomerTier
        {
            CustomerId = customerId,
            TierLevel = "Bronze",
            TotalBookings = 0,
            TotalSpent = 0,
            DiscountPercentage = 0
        };
    }

    public void RecordBooking(decimal bookingAmount)
    {
        TotalBookings++;
        TotalSpent += bookingAmount;
        LastBookingDate = DateTime.UtcNow;
        UpdateTier();
        SetUpdatedAt();
    }

    private void UpdateTier()
    {
        if (TotalBookings >= 50 || TotalSpent >= 50000)
        {
            TierLevel = "Gold";
            DiscountPercentage = 0.10m;
        }
        else if (TotalBookings >= 20 || TotalSpent >= 20000)
        {
            TierLevel = "Silver";
            DiscountPercentage = 0.05m;
        }
        else
        {
            TierLevel = "Bronze";
            DiscountPercentage = 0m;
        }
    }

    public decimal GetDiscount(decimal baseAmount)
    {
        return baseAmount * DiscountPercentage;
    }
}
