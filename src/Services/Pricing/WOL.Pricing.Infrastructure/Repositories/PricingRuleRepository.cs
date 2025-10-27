using Microsoft.EntityFrameworkCore;
using WOL.Pricing.Domain.Entities;
using WOL.Pricing.Domain.Repositories;
using WOL.Pricing.Infrastructure.Data;

namespace WOL.Pricing.Infrastructure.Repositories;

public class PricingRuleRepository : IPricingRuleRepository
{
    private readonly PricingDbContext _context;

    public PricingRuleRepository(PricingDbContext context)
    {
        _context = context;
    }

    public async Task<PricingRule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.PricingRules.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<PricingRule>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PricingRules.ToListAsync(cancellationToken);
    }

    public async Task<PricingRule> AddAsync(PricingRule entity, CancellationToken cancellationToken = default)
    {
        await _context.PricingRules.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task UpdateAsync(PricingRule entity, CancellationToken cancellationToken = default)
    {
        _context.PricingRules.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(PricingRule entity, CancellationToken cancellationToken = default)
    {
        _context.PricingRules.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<PricingRule?> GetByRouteAndVehicleAsync(string fromCity, string toCity, string vehicleType, CancellationToken cancellationToken = default)
    {
        return await _context.PricingRules
            .Where(p => p.FromCity == fromCity && p.ToCity == toCity && p.VehicleType == vehicleType && p.IsActive)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<PricingRule>> GetActiveRulesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PricingRules
            .Where(p => p.IsActive)
            .ToListAsync(cancellationToken);
    }
}
