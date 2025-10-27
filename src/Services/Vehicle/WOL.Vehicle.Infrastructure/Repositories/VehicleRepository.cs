using Microsoft.EntityFrameworkCore;
using WOL.Vehicle.Domain.Repositories;
using WOL.Vehicle.Infrastructure.Data;

namespace WOL.Vehicle.Infrastructure.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly VehicleDbContext _context;

    public VehicleRepository(VehicleDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Vehicle>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles.ToListAsync(cancellationToken);
    }

    public async Task<Domain.Entities.Vehicle> AddAsync(Domain.Entities.Vehicle entity, CancellationToken cancellationToken = default)
    {
        await _context.Vehicles.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task UpdateAsync(Domain.Entities.Vehicle entity, CancellationToken cancellationToken = default)
    {
        _context.Vehicles.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Domain.Entities.Vehicle entity, CancellationToken cancellationToken = default)
    {
        _context.Vehicles.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<Domain.Entities.Vehicle?> GetByPlateNumberAsync(string plateNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles
            .FirstOrDefaultAsync(v => v.PlateNumber == plateNumber, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Vehicle>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles
            .Where(v => v.OwnerId == ownerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Vehicle>> GetAvailableVehiclesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles
            .Where(v => v.IsAvailable)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByPlateNumberAsync(string plateNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles
            .AnyAsync(v => v.PlateNumber == plateNumber, cancellationToken);
    }
}
