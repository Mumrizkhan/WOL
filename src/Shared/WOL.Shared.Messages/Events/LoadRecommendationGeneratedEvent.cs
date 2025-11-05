namespace WOL.Shared.Messages.Events;

public class LoadRecommendationGeneratedEvent
{
    public Guid DriverId { get; set; }
    public string CurrentLocation { get; set; } = string.Empty;
    public List<RecommendedLoadDto> RecommendedLoads { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
}

public class RecommendedLoadDto
{
    public Guid OpportunityId { get; set; }
    public string OriginCity { get; set; } = string.Empty;
    public string DestinationCity { get; set; } = string.Empty;
    public decimal EstimatedEarnings { get; set; }
    public double DistanceFromDriver { get; set; }
    public double MatchScore { get; set; }
    public string ReasonCode { get; set; } = string.Empty;
}
