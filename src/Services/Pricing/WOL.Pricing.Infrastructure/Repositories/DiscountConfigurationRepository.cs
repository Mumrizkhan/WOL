using Microsoft.EntityFrameworkCore;
using WOL.Pricing.Domain.Entities;
using WOL.Pricing.Domain.Repositories;
using WOL.Pricing.Infrastructure.Data;

namespace WOL.Pricing.Infrastructure.Repositories;

public class DiscountConfigurationRepository : IDiscountConfigurationRepository
{
    private readonly PricingDbContext _context;

    public DiscountConfigurationRepository(PricingDbContext context)
    {
        _context = context;
    }

    public async Task<DiscountConfiguration?> GetByIdAsync(Guid id)
    {
        return await _context.DiscountConfigurations.FindAsync(id);
    }

    public async Task<DiscountConfiguration?> GetByTypeAsync(string discountType)
    {
        return await _context.DiscountConfigurations
            .FirstOrDefaultAsync(d => d.DiscountType == discountType && d.IsActive);
    }

    public async Task<IEnumerable<DiscountConfiguration>> GetAllAsync()
    {
        return await _context.DiscountConfigurations.ToListAsync();
    }

    public async Task<IEnumerable<DiscountConfiguration>> GetActiveAsync()
    {
        return await _context.DiscountConfigurations
            .Where(d => d.IsActive)
            .ToListAsync();
    }

    public async Task AddAsync(DiscountConfiguration discountConfiguration)
    {
        await _context.DiscountConfigurations.AddAsync(discountConfiguration);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DiscountConfiguration discountConfiguration)
    {
        _context.DiscountConfigurations.Update(discountConfiguration);
        await _context.SaveChangesAsync();
    }
}
