using Hangfire;
using Microsoft.Extensions.Logging;
using WOL.Backload.Domain.Entities;
using WOL.Backload.Domain.Repositories;
using WOL.Booking.Domain.Repositories;

namespace WOL.BackloadWorker.Jobs;

public class RouteUtilizationAggregationJob
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRouteUtilizationRepository _routeUtilizationRepository;
    private readonly ILogger<RouteUtilizationAggregationJob> _logger;

    public RouteUtilizationAggregationJob(
        IBookingRepository bookingRepository,
        IRouteUtilizationRepository routeUtilizationRepository,
        ILogger<RouteUtilizationAggregationJob> logger)
    {
        _bookingRepository = bookingRepository;
        _routeUtilizationRepository = routeUtilizationRepository;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task Execute()
    {
        _logger.LogInformation("Starting route utilization aggregation job at {Time}", DateTime.UtcNow);

        try
        {
            var startOfHour = DateTime.UtcNow.Date.AddHours(DateTime.UtcNow.Hour);
            var endOfHour = startOfHour.AddHours(1);

            var allBookings = await _bookingRepository.GetAllAsync();
            var completedBookings = allBookings
                .Where(b => 
                    b.Status == "Completed" &&
                    b.CompletedAt >= startOfHour &&
                    b.CompletedAt < endOfHour)
                .ToList();

            var routeGroups = completedBookings
                .GroupBy(b => new { b.OriginCity, b.DestinationCity });

            _logger.LogInformation("Processing {Count} routes for utilization aggregation", routeGroups.Count());

            foreach (var routeGroup in routeGroups)
            {
                var originCity = routeGroup.Key.OriginCity;
                var destinationCity = routeGroup.Key.DestinationCity;

                var outboundBookings = routeGroup.Count();
                
                var returnBookings = allBookings
                    .Count(b => 
                        b.Status == "Completed" &&
                        b.CompletedAt >= startOfHour &&
                        b.CompletedAt < endOfHour &&
                        b.OriginCity == destinationCity &&
                        b.DestinationCity == originCity);

                var totalCapacity = routeGroup.Sum(b => b.VehicleCapacity ?? 0);
                var utilizedCapacity = routeGroup.Sum(b => b.ActualWeight ?? 0);
                var utilizationPercentage = totalCapacity > 0 ? (utilizedCapacity / totalCapacity) * 100 : 0;

                var averageDistance = routeGroup.Average(b => b.Distance);
                var emptyKmTotal = returnBookings == 0 ? averageDistance * outboundBookings : 0;
                var emptyKmSaved = returnBookings > 0 ? averageDistance * Math.Min(outboundBookings, returnBookings) : 0;

                var existingUtilization = await _routeUtilizationRepository.GetByRouteAsync(originCity, destinationCity);

                if (existingUtilization != null)
                {
                    existingUtilization.UpdateStats(
                        outboundBookings,
                        returnBookings,
                        utilizationPercentage,
                        emptyKmTotal,
                        emptyKmSaved);
                    
                    await _routeUtilizationRepository.UpdateAsync(existingUtilization);
                }
                else
                {
                    var newUtilization = RouteUtilization.Create(
                        originCity,
                        destinationCity,
                        startOfHour,
                        endOfHour,
                        outboundBookings,
                        returnBookings,
                        utilizationPercentage,
                        emptyKmTotal,
                        emptyKmSaved);

                    await _routeUtilizationRepository.AddAsync(newUtilization);
                }

                _logger.LogInformation(
                    "Updated route utilization for {Origin} -> {Destination}: {Outbound} outbound, {Return} return, {Utilization}% utilization",
                    originCity,
                    destinationCity,
                    outboundBookings,
                    returnBookings,
                    utilizationPercentage);
            }

            _logger.LogInformation("Route utilization aggregation job completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing route utilization aggregation job");
            throw;
        }
    }
}
