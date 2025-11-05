using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using WOL.Shared.Messages.Events;

namespace WOL.AnalyticsWorker;

public class LoadRecommendationGeneratedConsumer : IConsumer<LoadRecommendationGeneratedEvent>
{
    private readonly IMongoDatabase _mongoDatabase;
    private readonly ILogger<LoadRecommendationGeneratedConsumer> _logger;

    public LoadRecommendationGeneratedConsumer(
        IMongoDatabase mongoDatabase,
        ILogger<LoadRecommendationGeneratedConsumer> logger)
    {
        _mongoDatabase = mongoDatabase;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<LoadRecommendationGeneratedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Recording load recommendation analytics for Driver {DriverId}", message.DriverId);

        try
        {
            var collection = _mongoDatabase.GetCollection<LoadRecommendationAnalytics>("load_recommendation_analytics");

            foreach (var recommendation in message.RecommendedLoads)
            {
                var analytics = new LoadRecommendationAnalytics
                {
                    Id = Guid.NewGuid().ToString(),
                    DriverId = message.DriverId,
                    CurrentLocation = message.CurrentLocation,
                    OpportunityId = recommendation.OpportunityId,
                    OriginCity = recommendation.OriginCity,
                    DestinationCity = recommendation.DestinationCity,
                    EstimatedEarnings = recommendation.EstimatedEarnings,
                    DistanceFromDriver = recommendation.DistanceFromDriver,
                    MatchScore = recommendation.MatchScore,
                    ReasonCode = recommendation.ReasonCode,
                    GeneratedAt = message.GeneratedAt,
                    WasAccepted = false,
                    AcceptedAt = null
                };

                await collection.InsertOneAsync(analytics);
            }

            _logger.LogInformation("Recorded {Count} recommendations for Driver {DriverId}", 
                message.RecommendedLoads.Count, message.DriverId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording load recommendation analytics for Driver {DriverId}", 
                message.DriverId);
            throw;
        }
    }
}

public class LoadRecommendationAnalytics
{
    public string Id { get; set; } = string.Empty;
    public Guid DriverId { get; set; }
    public string CurrentLocation { get; set; } = string.Empty;
    public Guid OpportunityId { get; set; }
    public string OriginCity { get; set; } = string.Empty;
    public string DestinationCity { get; set; } = string.Empty;
    public decimal EstimatedEarnings { get; set; }
    public double DistanceFromDriver { get; set; }
    public double MatchScore { get; set; }
    public string ReasonCode { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public bool WasAccepted { get; set; }
    public DateTime? AcceptedAt { get; set; }
}
