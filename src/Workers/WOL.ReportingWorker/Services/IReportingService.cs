namespace WOL.ReportingWorker.Services;

public interface IReportingService
{
    Task AggregateBookingDataAsync(Guid bookingId, Guid customerId, decimal totalAmount, DateTime completedAt);
    Task AggregatePaymentDataAsync(Guid paymentId, Guid bookingId, decimal amount, string paymentMethod, DateTime processedAt);
}
