using WOL.Backload.Domain.Entities;

namespace WOL.Backload.Domain.Repositories;

public interface IRouteUtilizationRepository
{
    Task<RouteUtilization?> GetByIdAsync(Guid id);
    Task<RouteUtilization?> GetByRouteAsync(string originCity, string destinationCity);
    Task<IEnumerable<RouteUtilization>> GetAllAsync();
    Task<IEnumerable<RouteUtilization>> GetByOriginCityAsync(string originCity);
    Task<IEnumerable<RouteUtilization>> GetByDestinationCityAsync(string destinationCity);
    Task AddAsync(RouteUtilization routeUtilization);
    Task UpdateAsync(RouteUtilization routeUtilization);
}
