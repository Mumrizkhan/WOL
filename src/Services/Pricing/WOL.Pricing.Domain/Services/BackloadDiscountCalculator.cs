namespace WOL.Pricing.Domain.Services;

public class BackloadDiscountCalculator
{
    private const decimal MAX_BACKLOAD_DISCOUNT_PERCENTAGE = 0.15m; // 15%
    private const decimal FLEXIBLE_DATE_DISCOUNT_PERCENTAGE = 0.05m; // 5%
    private const decimal SHARED_LOAD_MIN_DISCOUNT_PERCENTAGE = 0.10m; // 10%
    private const decimal SHARED_LOAD_MAX_DISCOUNT_PERCENTAGE = 0.20m; // 20%

    public static decimal CalculateBackloadDiscount(decimal baseFare, bool isBackload, bool isFlexibleDate = false)
    {
        decimal totalDiscount = 0;

        if (isBackload)
        {
            totalDiscount += baseFare * MAX_BACKLOAD_DISCOUNT_PERCENTAGE;
        }

        if (isFlexibleDate)
        {
            totalDiscount += baseFare * FLEXIBLE_DATE_DISCOUNT_PERCENTAGE;
        }

        return totalDiscount;
    }

    public static decimal CalculateSharedLoadDiscount(decimal baseFare, decimal capacityUtilization)
    {
        var discountPercentage = SHARED_LOAD_MIN_DISCOUNT_PERCENTAGE + 
            ((capacityUtilization / 100m) * (SHARED_LOAD_MAX_DISCOUNT_PERCENTAGE - SHARED_LOAD_MIN_DISCOUNT_PERCENTAGE));

        return baseFare * discountPercentage;
    }

    public static decimal GetBackloadDiscountPercentage()
    {
        return MAX_BACKLOAD_DISCOUNT_PERCENTAGE;
    }
}
