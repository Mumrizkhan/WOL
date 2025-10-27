namespace WOL.BackloadWorker.Services;

public interface IBackloadMatchingService
{
    Task FindBackloadOpportunitiesAsync(Guid bookingId, string pickupLocation, string deliveryLocation, DateTime pickupDate);
    Task UpdateBackloadAvailabilityAsync(Guid bookingId, string status);
}
