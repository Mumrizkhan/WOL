namespace WOL.AnalyticsWorker.Services;

public interface IAnalyticsService
{
    Task RecordBookingCreatedAsync(Guid bookingId, Guid customerId, string bookingType, decimal estimatedPrice, DateTime createdAt);
    Task RecordPaymentProcessedAsync(Guid paymentId, Guid bookingId, decimal amount, string paymentMethod, DateTime processedAt);
    Task RecordLocationUpdateAsync(Guid bookingId, Guid driverId, double latitude, double longitude, DateTime timestamp);
}
