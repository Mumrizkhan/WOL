using WOL.Shared.Common.Application;

namespace WOL.Backload.Domain.Repositories;

public interface IBackloadOpportunityRepository : IRepository<Entities.BackloadOpportunity>
{
    Task<IEnumerable<Entities.BackloadOpportunity>> GetAvailableOpportunitiesAsync(string fromCity, string toCity, CancellationToken cancellationToken = default);
    Task<IEnumerable<Entities.BackloadOpportunity>> GetByDriverIdAsync(Guid driverId, CancellationToken cancellationToken = default);
}
