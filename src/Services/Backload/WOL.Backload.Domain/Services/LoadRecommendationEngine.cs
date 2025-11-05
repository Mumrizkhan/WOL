using WOL.Backload.Domain.Entities;

namespace WOL.Backload.Domain.Services;

public class LoadRecommendationEngine
{
    private readonly List<RoutePattern> _historicalPatterns = new();

    public LoadRecommendation GenerateRecommendation(
        Guid driverId,
        string currentCity,
        string destinationCity,
        DateTime completionTime,
        List<BackloadOpportunity> availableOpportunities,
        List<RouteUtilization> historicalData)
    {
        var recommendations = new List<RecommendedLoad>();

        foreach (var opportunity in availableOpportunities)
        {
            var score = CalculateRecommendationScore(
                currentCity,
                destinationCity,
                opportunity,
                completionTime,
                historicalData);

            if (score > 0.5)
            {
                recommendations.Add(new RecommendedLoad
                {
                    OpportunityId = opportunity.Id,
                    OriginCity = opportunity.OriginCity,
                    DestinationCity = opportunity.DestinationCity,
                    EstimatedEarnings = opportunity.EstimatedPrice,
                    DistanceFromDriver = CalculateDistance(destinationCity, opportunity.OriginCity),
                    MatchScore = score,
                    ReasonCode = DetermineReasonCode(score, opportunity, historicalData)
                });
            }
        }

        return new LoadRecommendation
        {
            DriverId = driverId,
            CurrentLocation = destinationCity,
            RecommendedLoads = recommendations.OrderByDescending(r => r.MatchScore).Take(5).ToList(),
            GeneratedAt = DateTime.UtcNow
        };
    }

    private double CalculateRecommendationScore(
        string currentCity,
        string destinationCity,
        BackloadOpportunity opportunity,
        DateTime completionTime,
        List<RouteUtilization> historicalData)
    {
        double score = 0.0;

        var proximityScore = CalculateProximityScore(destinationCity, opportunity.OriginCity);
        score += proximityScore * 0.4;

        var timingScore = CalculateTimingScore(completionTime, opportunity.PickupDate);
        score += timingScore * 0.2;

        var historicalScore = CalculateHistoricalScore(opportunity.OriginCity, opportunity.DestinationCity, historicalData);
        score += historicalScore * 0.3;

        var priceScore = CalculatePriceScore(opportunity.EstimatedPrice);
        score += priceScore * 0.1;

        return Math.Min(score, 1.0);
    }

    private double CalculateProximityScore(string driverDestination, string opportunityOrigin)
    {
        if (driverDestination.Equals(opportunityOrigin, StringComparison.OrdinalIgnoreCase))
            return 1.0;

        var distance = GetCityDistance(driverDestination, opportunityOrigin);
        if (distance <= 50) return 0.9;
        if (distance <= 100) return 0.7;
        if (distance <= 200) return 0.5;
        return 0.3;
    }

    private double CalculateTimingScore(DateTime completionTime, DateTime pickupDate)
    {
        var timeDiff = (pickupDate - completionTime).TotalHours;
        
        if (timeDiff < 0) return 0.0;
        if (timeDiff <= 2) return 1.0;
        if (timeDiff <= 6) return 0.8;
        if (timeDiff <= 24) return 0.6;
        return 0.4;
    }

    private double CalculateHistoricalScore(string origin, string destination, List<RouteUtilization> historicalData)
    {
        var routeData = historicalData.FirstOrDefault(r => 
            r.OriginCity.Equals(origin, StringComparison.OrdinalIgnoreCase) &&
            r.DestinationCity.Equals(destination, StringComparison.OrdinalIgnoreCase));

        if (routeData == null) return 0.5;

        var utilizationScore = routeData.UtilizationPercentage / 100.0;
        return Math.Min(utilizationScore, 1.0);
    }

    private double CalculatePriceScore(decimal estimatedPrice)
    {
        if (estimatedPrice >= 5000) return 1.0;
        if (estimatedPrice >= 3000) return 0.8;
        if (estimatedPrice >= 2000) return 0.6;
        if (estimatedPrice >= 1000) return 0.4;
        return 0.2;
    }

    private double GetCityDistance(string city1, string city2)
    {
        var cityDistances = new Dictionary<string, Dictionary<string, double>>
        {
            ["Riyadh"] = new() { ["Jeddah"] = 950, ["Dammam"] = 400, ["Mecca"] = 870, ["Medina"] = 850 },
            ["Jeddah"] = new() { ["Riyadh"] = 950, ["Mecca"] = 80, ["Medina"] = 420, ["Dammam"] = 1350 },
            ["Dammam"] = new() { ["Riyadh"] = 400, ["Jeddah"] = 1350, ["Mecca"] = 1270, ["Medina"] = 1250 },
            ["Mecca"] = new() { ["Jeddah"] = 80, ["Riyadh"] = 870, ["Medina"] = 340, ["Dammam"] = 1270 },
            ["Medina"] = new() { ["Jeddah"] = 420, ["Riyadh"] = 850, ["Mecca"] = 340, ["Dammam"] = 1250 }
        };

        if (cityDistances.TryGetValue(city1, out var distances) && 
            distances.TryGetValue(city2, out var distance))
        {
            return distance;
        }

        return 500;
    }

    private string DetermineReasonCode(double score, BackloadOpportunity opportunity, List<RouteUtilization> historicalData)
    {
        if (score >= 0.9) return "PERFECT_MATCH";
        if (score >= 0.8) return "HIGH_EARNINGS";
        if (score >= 0.7) return "NEARBY_PICKUP";
        if (score >= 0.6) return "POPULAR_ROUTE";
        return "AVAILABLE_LOAD";
    }

    private double CalculateDistance(string city1, string city2)
    {
        return GetCityDistance(city1, city2);
    }
}

public class LoadRecommendation
{
    public Guid DriverId { get; set; }
    public string CurrentLocation { get; set; } = string.Empty;
    public List<RecommendedLoad> RecommendedLoads { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
}

public class RecommendedLoad
{
    public Guid OpportunityId { get; set; }
    public string OriginCity { get; set; } = string.Empty;
    public string DestinationCity { get; set; } = string.Empty;
    public decimal EstimatedEarnings { get; set; }
    public double DistanceFromDriver { get; set; }
    public double MatchScore { get; set; }
    public string ReasonCode { get; set; } = string.Empty;
}

public class RoutePattern
{
    public string OriginCity { get; set; } = string.Empty;
    public string DestinationCity { get; set; } = string.Empty;
    public int Frequency { get; set; }
    public decimal AveragePrice { get; set; }
}
