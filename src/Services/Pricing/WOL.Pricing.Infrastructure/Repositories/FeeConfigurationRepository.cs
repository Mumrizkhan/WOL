using Microsoft.EntityFrameworkCore;
using WOL.Pricing.Domain.Entities;
using WOL.Pricing.Domain.Repositories;
using WOL.Pricing.Infrastructure.Data;

namespace WOL.Pricing.Infrastructure.Repositories;

public class FeeConfigurationRepository : IFeeConfigurationRepository
{
    private readonly PricingDbContext _context;

    public FeeConfigurationRepository(PricingDbContext context)
    {
        _context = context;
    }

    public async Task<FeeConfiguration?> GetByIdAsync(Guid id)
    {
        return await _context.FeeConfigurations.FindAsync(id);
    }

    public async Task<FeeConfiguration?> GetByTypeAsync(string feeType)
    {
        return await _context.FeeConfigurations
            .FirstOrDefaultAsync(f => f.FeeType == feeType && f.IsActive);
    }

    public async Task<IEnumerable<FeeConfiguration>> GetAllAsync()
    {
        return await _context.FeeConfigurations.ToListAsync();
    }

    public async Task<IEnumerable<FeeConfiguration>> GetActiveAsync()
    {
        return await _context.FeeConfigurations
            .Where(f => f.IsActive)
            .ToListAsync();
    }

    public async Task AddAsync(FeeConfiguration feeConfiguration)
    {
        await _context.FeeConfigurations.AddAsync(feeConfiguration);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(FeeConfiguration feeConfiguration)
    {
        _context.FeeConfigurations.Update(feeConfiguration);
        await _context.SaveChangesAsync();
    }
}
