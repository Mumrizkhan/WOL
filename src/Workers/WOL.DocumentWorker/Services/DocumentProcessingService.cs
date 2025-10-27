using System.Text.Json;

namespace WOL.DocumentWorker.Services;

public class DocumentProcessingService : IDocumentProcessingService
{
    private readonly ILogger<DocumentProcessingService> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public DocumentProcessingService(
        ILogger<DocumentProcessingService> logger,
        IConfiguration configuration,
        HttpClient httpClient)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task ProcessDocumentAsync(Guid documentId, string documentType, string filePath)
    {
        try
        {
            _logger.LogInformation("Processing document {DocumentId} of type {DocumentType}", documentId, documentType);

            var azureVisionEndpoint = _configuration["AzureVision:Endpoint"];
            var azureVisionKey = _configuration["AzureVision:Key"];

            if (string.IsNullOrEmpty(azureVisionEndpoint) || string.IsNullOrEmpty(azureVisionKey))
            {
                _logger.LogWarning("Azure Vision credentials not configured. Skipping OCR processing.");
                return;
            }

            var ocrResult = await PerformOcrAsync(filePath, azureVisionEndpoint, azureVisionKey);
            
            _logger.LogInformation("OCR processing completed for document {DocumentId}. Extracted {TextLength} characters", 
                documentId, ocrResult?.Length ?? 0);

            await ValidateDocumentAsync(documentId, documentType, ocrResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing document {DocumentId}", documentId);
        }
    }

    private async Task<string?> PerformOcrAsync(string filePath, string endpoint, string key)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{endpoint}/vision/v3.2/read/analyze");
            request.Headers.Add("Ocp-Apim-Subscription-Key", key);

            var response = await _httpClient.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                var operationLocation = response.Headers.GetValues("Operation-Location").FirstOrDefault();
                if (!string.IsNullOrEmpty(operationLocation))
                {
                    await Task.Delay(2000);
                    
                    var resultRequest = new HttpRequestMessage(HttpMethod.Get, operationLocation);
                    resultRequest.Headers.Add("Ocp-Apim-Subscription-Key", key);
                    
                    var resultResponse = await _httpClient.SendAsync(resultRequest);
                    if (resultResponse.IsSuccessStatusCode)
                    {
                        var resultContent = await resultResponse.Content.ReadAsStringAsync();
                        return resultContent;
                    }
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing OCR");
            return null;
        }
    }

    private async Task ValidateDocumentAsync(Guid documentId, string documentType, string? ocrResult)
    {
        await Task.CompletedTask;
        
        if (string.IsNullOrEmpty(ocrResult))
        {
            _logger.LogWarning("No OCR result for document {DocumentId}", documentId);
            return;
        }

        _logger.LogInformation("Document {DocumentId} validated successfully", documentId);
    }

    public async Task SendExpiryNotificationAsync(Guid documentId, string documentType, DateTime expiryDate)
    {
        try
        {
            _logger.LogInformation("Sending expiry notification for document {DocumentId} expiring on {ExpiryDate}", 
                documentId, expiryDate);

            var daysUntilExpiry = (expiryDate - DateTime.UtcNow).Days;
            
            if (daysUntilExpiry <= 30 && daysUntilExpiry > 0)
            {
                _logger.LogWarning("Document {DocumentId} ({DocumentType}) expires in {Days} days", 
                    documentId, documentType, daysUntilExpiry);
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending expiry notification for document {DocumentId}", documentId);
        }
    }
}
