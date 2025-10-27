using WOL.Shared.Common.Application;
using WOL.Vehicle.Domain.Entities;

namespace WOL.Vehicle.Domain.Repositories;

public interface IVehicleTypeRepository : IRepository<VehicleType>
{
    Task<IEnumerable<VehicleType>> GetActiveTypesAsync(CancellationToken cancellationToken = default);
}
