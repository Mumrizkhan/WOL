namespace WOL.Tracking.Application.Services;

public interface ILocationUpdateBroadcaster
{
    Task BroadcastLocationUpdate(
        string bookingId,
        double latitude,
        double longitude,
        double speed,
        DateTime timestamp);

    Task BroadcastETAUpdate(
        string bookingId,
        DateTime estimatedArrival,
        TimeSpan remainingTime,
        double remainingDistanceKm);

    Task BroadcastStatusUpdate(
        string bookingId,
        string status,
        string message);
}
