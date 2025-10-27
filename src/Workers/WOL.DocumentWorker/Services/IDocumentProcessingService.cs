namespace WOL.DocumentWorker.Services;

public interface IDocumentProcessingService
{
    Task ProcessDocumentAsync(Guid documentId, string documentType, string filePath);
    Task SendExpiryNotificationAsync(Guid documentId, string documentType, DateTime expiryDate);
}
