using WOL.Shared.Common.Domain;

namespace WOL.Pricing.Domain.Entities;

public class PricingRule : BaseEntity
{
    public string VehicleType { get; private set; } = string.Empty;
    public decimal BasePrice { get; private set; }
    public decimal PricePerKm { get; private set; }
    public decimal PricePerKg { get; private set; }
    public string FromCity { get; private set; } = string.Empty;
    public string ToCity { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    private PricingRule() { }

    public static PricingRule Create(string vehicleType, decimal basePrice, decimal pricePerKm, decimal pricePerKg, string fromCity, string toCity)
    {
        return new PricingRule
        {
            VehicleType = vehicleType,
            BasePrice = basePrice,
            PricePerKm = pricePerKm,
            PricePerKg = pricePerKg,
            FromCity = fromCity,
            ToCity = toCity,
            IsActive = true
        };
    }

    public void UpdatePricing(decimal basePrice, decimal pricePerKm, decimal pricePerKg)
    {
        BasePrice = basePrice;
        PricePerKm = pricePerKm;
        PricePerKg = pricePerKg;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }
}
