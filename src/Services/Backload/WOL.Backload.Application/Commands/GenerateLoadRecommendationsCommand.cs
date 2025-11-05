using MediatR;
using MassTransit;
using WOL.Backload.Domain.Repositories;
using WOL.Backload.Domain.Services;
using WOL.Shared.Messages.Events;

namespace WOL.Backload.Application.Commands;

public record GenerateLoadRecommendationsCommand : IRequest<GenerateLoadRecommendationsResponse>
{
    public Guid DriverId { get; init; }
    public string CurrentCity { get; init; } = string.Empty;
    public string DestinationCity { get; init; } = string.Empty;
    public DateTime CompletionTime { get; init; }
}

public record GenerateLoadRecommendationsResponse
{
    public bool Success { get; init; }
    public int RecommendationCount { get; init; }
    public List<RecommendedLoadDto> Recommendations { get; init; } = new();
}

public class GenerateLoadRecommendationsCommandHandler 
    : IRequestHandler<GenerateLoadRecommendationsCommand, GenerateLoadRecommendationsResponse>
{
    private readonly IBackloadOpportunityRepository _opportunityRepository;
    private readonly IRouteUtilizationRepository _routeUtilizationRepository;
    private readonly LoadRecommendationEngine _recommendationEngine;
    private readonly IPublishEndpoint _publishEndpoint;

    public GenerateLoadRecommendationsCommandHandler(
        IBackloadOpportunityRepository opportunityRepository,
        IRouteUtilizationRepository routeUtilizationRepository,
        LoadRecommendationEngine recommendationEngine,
        IPublishEndpoint publishEndpoint)
    {
        _opportunityRepository = opportunityRepository;
        _routeUtilizationRepository = routeUtilizationRepository;
        _recommendationEngine = recommendationEngine;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<GenerateLoadRecommendationsResponse> Handle(
        GenerateLoadRecommendationsCommand request, 
        CancellationToken cancellationToken)
    {
        var availableOpportunities = await _opportunityRepository.GetAvailableOpportunitiesAsync();
        
        var historicalData = await _routeUtilizationRepository.GetAllAsync();

        var recommendation = _recommendationEngine.GenerateRecommendation(
            request.DriverId,
            request.CurrentCity,
            request.DestinationCity,
            request.CompletionTime,
            availableOpportunities.ToList(),
            historicalData.ToList());

        if (recommendation.RecommendedLoads.Any())
        {
            await _publishEndpoint.Publish(new LoadRecommendationGeneratedEvent
            {
                DriverId = recommendation.DriverId,
                CurrentLocation = recommendation.CurrentLocation,
                RecommendedLoads = recommendation.RecommendedLoads.Select(r => new RecommendedLoadDto
                {
                    OpportunityId = r.OpportunityId,
                    OriginCity = r.OriginCity,
                    DestinationCity = r.DestinationCity,
                    EstimatedEarnings = r.EstimatedEarnings,
                    DistanceFromDriver = r.DistanceFromDriver,
                    MatchScore = r.MatchScore,
                    ReasonCode = r.ReasonCode
                }).ToList(),
                GeneratedAt = recommendation.GeneratedAt
            }, cancellationToken);
        }

        return new GenerateLoadRecommendationsResponse
        {
            Success = true,
            RecommendationCount = recommendation.RecommendedLoads.Count,
            Recommendations = recommendation.RecommendedLoads.Select(r => new RecommendedLoadDto
            {
                OpportunityId = r.OpportunityId,
                OriginCity = r.OriginCity,
                DestinationCity = r.DestinationCity,
                EstimatedEarnings = r.EstimatedEarnings,
                DistanceFromDriver = r.DistanceFromDriver,
                MatchScore = r.MatchScore,
                ReasonCode = r.ReasonCode
            }).ToList()
        };
    }
}
