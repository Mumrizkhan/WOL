using MongoDB.Driver;
using MongoDB.Bson;

namespace WOL.BackloadWorker.Services;

public class BackloadMatchingService : IBackloadMatchingService
{
    private readonly ILogger<BackloadMatchingService> _logger;
    private readonly IMongoDatabase _database;

    public BackloadMatchingService(ILogger<BackloadMatchingService> logger, IMongoDatabase database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task FindBackloadOpportunitiesAsync(Guid bookingId, string pickupLocation, string deliveryLocation, DateTime pickupDate)
    {
        try
        {
            var collection = _database.GetCollection<BsonDocument>("backload_opportunities");

            var filter = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Eq("status", "available"),
                Builders<BsonDocument>.Filter.Gte("pickupDate", pickupDate.AddDays(-2)),
                Builders<BsonDocument>.Filter.Lte("pickupDate", pickupDate.AddDays(2))
            );

            var potentialMatches = await collection.Find(filter).ToListAsync();

            var matches = new List<BsonDocument>();
            foreach (var match in potentialMatches)
            {
                var matchPickup = match["pickupLocation"].AsString;
                var matchDelivery = match["deliveryLocation"].AsString;

                var score = CalculateMatchScore(pickupLocation, deliveryLocation, matchPickup, matchDelivery);
                
                if (score > 0.5)
                {
                    matches.Add(new BsonDocument
                    {
                        { "bookingId", bookingId.ToString() },
                        { "matchedBookingId", match["bookingId"].AsString },
                        { "score", score },
                        { "pickupLocation", pickupLocation },
                        { "deliveryLocation", deliveryLocation },
                        { "matchPickupLocation", matchPickup },
                        { "matchDeliveryLocation", matchDelivery },
                        { "createdAt", DateTime.UtcNow }
                    });
                }
            }

            if (matches.Count > 0)
            {
                var matchesCollection = _database.GetCollection<BsonDocument>("backload_matches");
                await matchesCollection.InsertManyAsync(matches);
                
                _logger.LogInformation("Found {Count} backload opportunities for booking {BookingId}", 
                    matches.Count, bookingId);
            }
            else
            {
                _logger.LogInformation("No backload opportunities found for booking {BookingId}", bookingId);
            }

            var opportunityDocument = new BsonDocument
            {
                { "bookingId", bookingId.ToString() },
                { "pickupLocation", pickupLocation },
                { "deliveryLocation", deliveryLocation },
                { "pickupDate", pickupDate },
                { "status", "available" },
                { "createdAt", DateTime.UtcNow }
            };

            await collection.InsertOneAsync(opportunityDocument);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding backload opportunities for {BookingId}", bookingId);
        }
    }

    public async Task UpdateBackloadAvailabilityAsync(Guid bookingId, string status)
    {
        try
        {
            var collection = _database.GetCollection<BsonDocument>("backload_opportunities");

            var filter = Builders<BsonDocument>.Filter.Eq("bookingId", bookingId.ToString());
            var update = Builders<BsonDocument>.Update
                .Set("status", status)
                .Set("updatedAt", DateTime.UtcNow);

            await collection.UpdateOneAsync(filter, update);

            _logger.LogInformation("Updated backload availability for booking {BookingId} to {Status}", 
                bookingId, status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating backload availability for {BookingId}", bookingId);
        }
    }

    private double CalculateMatchScore(string pickup1, string delivery1, string pickup2, string delivery2)
    {
        var score = 0.0;

        if (IsLocationNearby(delivery1, pickup2))
        {
            score += 0.5;
        }

        if (IsLocationNearby(pickup1, delivery2))
        {
            score += 0.3;
        }

        if (IsSameCity(pickup1, pickup2) || IsSameCity(delivery1, delivery2))
        {
            score += 0.2;
        }

        return Math.Min(score, 1.0);
    }

    private bool IsLocationNearby(string location1, string location2)
    {
        if (string.IsNullOrEmpty(location1) || string.IsNullOrEmpty(location2))
            return false;

        var city1 = ExtractCity(location1);
        var city2 = ExtractCity(location2);

        return city1.Equals(city2, StringComparison.OrdinalIgnoreCase);
    }

    private bool IsSameCity(string location1, string location2)
    {
        if (string.IsNullOrEmpty(location1) || string.IsNullOrEmpty(location2))
            return false;

        var city1 = ExtractCity(location1);
        var city2 = ExtractCity(location2);

        return city1.Equals(city2, StringComparison.OrdinalIgnoreCase);
    }

    private string ExtractCity(string location)
    {
        var parts = location.Split(',');
        return parts.Length > 0 ? parts[0].Trim() : location;
    }
}
