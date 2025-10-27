using Microsoft.EntityFrameworkCore;
using WOL.Vehicle.Domain.Entities;
using WOL.Vehicle.Domain.Repositories;
using WOL.Vehicle.Infrastructure.Data;

namespace WOL.Vehicle.Infrastructure.Repositories;

public class VehicleTypeRepository : IVehicleTypeRepository
{
    private readonly VehicleDbContext _context;

    public VehicleTypeRepository(VehicleDbContext context)
    {
        _context = context;
    }

    public async Task<VehicleType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.VehicleTypes.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<VehicleType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.VehicleTypes.ToListAsync(cancellationToken);
    }

    public async Task<VehicleType> AddAsync(VehicleType entity, CancellationToken cancellationToken = default)
    {
        await _context.VehicleTypes.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task UpdateAsync(VehicleType entity, CancellationToken cancellationToken = default)
    {
        _context.VehicleTypes.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(VehicleType entity, CancellationToken cancellationToken = default)
    {
        _context.VehicleTypes.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<VehicleType>> GetActiveTypesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.VehicleTypes
            .Where(vt => vt.IsActive)
            .ToListAsync(cancellationToken);
    }
}
