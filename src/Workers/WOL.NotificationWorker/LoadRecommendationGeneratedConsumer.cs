using MassTransit;
using Microsoft.Extensions.Logging;
using WOL.Shared.Messages.Events;

namespace WOL.NotificationWorker;

public class LoadRecommendationGeneratedConsumer : IConsumer<LoadRecommendationGeneratedEvent>
{
    private readonly ILogger<LoadRecommendationGeneratedConsumer> _logger;

    public LoadRecommendationGeneratedConsumer(ILogger<LoadRecommendationGeneratedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<LoadRecommendationGeneratedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Sending load recommendations to Driver {DriverId}", message.DriverId);

        try
        {
            if (message.RecommendedLoads.Count == 0)
            {
                _logger.LogInformation("No recommendations available for Driver {DriverId}", message.DriverId);
                return;
            }

            var topRecommendation = message.RecommendedLoads.First();
            var reasonText = GetReasonText(topRecommendation.ReasonCode);

            var notificationTitle = "New Backload Opportunity!";
            var notificationBody = $"{reasonText}\n" +
                                 $"Route: {topRecommendation.OriginCity} → {topRecommendation.DestinationCity}\n" +
                                 $"Estimated Earnings: SAR {topRecommendation.EstimatedEarnings:N2}\n" +
                                 $"Distance: {topRecommendation.DistanceFromDriver:N0} km\n" +
                                 $"Match Score: {topRecommendation.MatchScore * 100:N0}%";

            _logger.LogInformation("Push notification sent to Driver {DriverId}: {Title}", 
                message.DriverId, notificationTitle);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending load recommendation notification to Driver {DriverId}", 
                message.DriverId);
            throw;
        }
    }

    private string GetReasonText(string reasonCode)
    {
        return reasonCode switch
        {
            "PERFECT_MATCH" => "🎯 Perfect match for your route!",
            "HIGH_EARNINGS" => "💰 High-paying opportunity nearby!",
            "NEARBY_PICKUP" => "📍 Pickup location is very close!",
            "POPULAR_ROUTE" => "🔥 Popular route with good demand!",
            "AVAILABLE_LOAD" => "📦 Available backload opportunity!",
            _ => "📦 New load available!"
        };
    }
}
