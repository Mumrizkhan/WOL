using WOL.Shared.Common.Application;
using WOL.Tracking.Domain.Entities;

namespace WOL.Tracking.Domain.Repositories;

public interface ILocationHistoryRepository : IRepository<LocationHistory>
{
    Task<IEnumerable<LocationHistory>> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task<LocationHistory?> GetLatestLocationAsync(Guid bookingId, CancellationToken cancellationToken = default);
}
