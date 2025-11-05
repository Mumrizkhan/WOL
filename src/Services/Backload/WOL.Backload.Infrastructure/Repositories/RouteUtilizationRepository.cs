using Microsoft.EntityFrameworkCore;
using WOL.Backload.Domain.Entities;
using WOL.Backload.Domain.Repositories;
using WOL.Backload.Infrastructure.Data;

namespace WOL.Backload.Infrastructure.Repositories;

public class RouteUtilizationRepository : IRouteUtilizationRepository
{
    private readonly BackloadDbContext _context;

    public RouteUtilizationRepository(BackloadDbContext context)
    {
        _context = context;
    }

    public async Task<RouteUtilization?> GetByIdAsync(Guid id)
    {
        return await _context.RouteUtilizations.FindAsync(id);
    }

    public async Task<RouteUtilization?> GetByRouteAsync(string originCity, string destinationCity)
    {
        return await _context.RouteUtilizations
            .FirstOrDefaultAsync(r => 
                r.OriginCity == originCity && 
                r.DestinationCity == destinationCity);
    }

    public async Task<IEnumerable<RouteUtilization>> GetAllAsync()
    {
        return await _context.RouteUtilizations.ToListAsync();
    }

    public async Task<IEnumerable<RouteUtilization>> GetByOriginCityAsync(string originCity)
    {
        return await _context.RouteUtilizations
            .Where(r => r.OriginCity == originCity)
            .ToListAsync();
    }

    public async Task<IEnumerable<RouteUtilization>> GetByDestinationCityAsync(string destinationCity)
    {
        return await _context.RouteUtilizations
            .Where(r => r.DestinationCity == destinationCity)
            .ToListAsync();
    }

    public async Task AddAsync(RouteUtilization routeUtilization)
    {
        await _context.RouteUtilizations.AddAsync(routeUtilization);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RouteUtilization routeUtilization)
    {
        _context.RouteUtilizations.Update(routeUtilization);
        await _context.SaveChangesAsync();
    }
}
