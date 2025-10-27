using MongoDB.Driver;
using MongoDB.Bson;

namespace WOL.ComplianceWorker.Services;

public class ComplianceService : IComplianceService
{
    private readonly ILogger<ComplianceService> _logger;
    private readonly IMongoDatabase _database;

    public ComplianceService(ILogger<ComplianceService> logger, IMongoDatabase database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task CheckVehicleComplianceAsync(Guid vehicleId, string registrationNumber, DateTime registrationExpiry, DateTime insuranceExpiry)
    {
        try
        {
            var collection = _database.GetCollection<BsonDocument>("compliance_checks");
            
            var registrationDaysRemaining = (registrationExpiry - DateTime.UtcNow).Days;
            var insuranceDaysRemaining = (insuranceExpiry - DateTime.UtcNow).Days;

            var isCompliant = registrationDaysRemaining > 0 && insuranceDaysRemaining > 0;
            var issues = new List<string>();

            if (registrationDaysRemaining <= 0)
            {
                issues.Add("Registration expired");
                _logger.LogWarning("Vehicle {VehicleId} registration has expired", vehicleId);
            }
            else if (registrationDaysRemaining <= 30)
            {
                issues.Add($"Registration expires in {registrationDaysRemaining} days");
                _logger.LogWarning("Vehicle {VehicleId} registration expires in {Days} days", vehicleId, registrationDaysRemaining);
            }

            if (insuranceDaysRemaining <= 0)
            {
                issues.Add("Insurance expired");
                _logger.LogWarning("Vehicle {VehicleId} insurance has expired", vehicleId);
            }
            else if (insuranceDaysRemaining <= 30)
            {
                issues.Add($"Insurance expires in {insuranceDaysRemaining} days");
                _logger.LogWarning("Vehicle {VehicleId} insurance expires in {Days} days", vehicleId, insuranceDaysRemaining);
            }

            var document = new BsonDocument
            {
                { "vehicleId", vehicleId.ToString() },
                { "registrationNumber", registrationNumber },
                { "registrationExpiry", registrationExpiry },
                { "insuranceExpiry", insuranceExpiry },
                { "isCompliant", isCompliant },
                { "issues", new BsonArray(issues) },
                { "checkedAt", DateTime.UtcNow }
            };

            await collection.InsertOneAsync(document);
            _logger.LogInformation("Compliance check completed for vehicle {VehicleId}. Compliant: {IsCompliant}", 
                vehicleId, isCompliant);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking vehicle compliance for {VehicleId}", vehicleId);
        }
    }

    public async Task CheckDocumentComplianceAsync(Guid documentId, string documentType, DateTime expiryDate)
    {
        try
        {
            var collection = _database.GetCollection<BsonDocument>("document_compliance");
            
            var daysRemaining = (expiryDate - DateTime.UtcNow).Days;
            var isCompliant = daysRemaining > 0;

            var document = new BsonDocument
            {
                { "documentId", documentId.ToString() },
                { "documentType", documentType },
                { "expiryDate", expiryDate },
                { "daysRemaining", daysRemaining },
                { "isCompliant", isCompliant },
                { "checkedAt", DateTime.UtcNow }
            };

            await collection.InsertOneAsync(document);

            if (!isCompliant)
            {
                _logger.LogWarning("Document {DocumentId} ({DocumentType}) has expired", documentId, documentType);
            }
            else if (daysRemaining <= 30)
            {
                _logger.LogWarning("Document {DocumentId} ({DocumentType}) expires in {Days} days", 
                    documentId, documentType, daysRemaining);
            }

            _logger.LogInformation("Document compliance check completed for {DocumentId}", documentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking document compliance for {DocumentId}", documentId);
        }
    }
}
