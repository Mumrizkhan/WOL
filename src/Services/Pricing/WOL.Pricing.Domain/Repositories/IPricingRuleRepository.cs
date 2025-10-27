using WOL.Shared.Common.Application;

namespace WOL.Pricing.Domain.Repositories;

public interface IPricingRuleRepository : IRepository<Entities.PricingRule>
{
    Task<Entities.PricingRule?> GetByRouteAndVehicleAsync(string fromCity, string toCity, string vehicleType, CancellationToken cancellationToken = default);
    Task<IEnumerable<Entities.PricingRule>> GetActiveRulesAsync(CancellationToken cancellationToken = default);
}
