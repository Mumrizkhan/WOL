using WOL.Shared.Common.Application;

namespace WOL.Vehicle.Domain.Repositories;

public interface IVehicleRepository : IRepository<Entities.Vehicle>
{
    Task<Entities.Vehicle?> GetByPlateNumberAsync(string plateNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Entities.Vehicle>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Entities.Vehicle>> GetAvailableVehiclesAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByPlateNumberAsync(string plateNumber, CancellationToken cancellationToken = default);
}
