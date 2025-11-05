using Microsoft.AspNetCore.SignalR;
using WOL.Tracking.API.Hubs;
using WOL.Tracking.Application.Services;

namespace WOL.Tracking.API.Services;

public class SignalRLocationUpdateBroadcaster : ILocationUpdateBroadcaster
{
    private readonly IHubContext<LocationTrackingHub> _hubContext;

    public SignalRLocationUpdateBroadcaster(IHubContext<LocationTrackingHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task BroadcastLocationUpdate(
        string bookingId,
        double latitude,
        double longitude,
        double speed,
        DateTime timestamp)
    {
        await LocationTrackingHub.SendLocationUpdate(
            _hubContext,
            bookingId,
            latitude,
            longitude,
            speed,
            timestamp);
    }

    public async Task BroadcastETAUpdate(
        string bookingId,
        DateTime estimatedArrival,
        TimeSpan remainingTime,
        double remainingDistanceKm)
    {
        await LocationTrackingHub.SendETAUpdate(
            _hubContext,
            bookingId,
            estimatedArrival,
            remainingTime,
            remainingDistanceKm);
    }

    public async Task BroadcastStatusUpdate(
        string bookingId,
        string status,
        string message)
    {
        await LocationTrackingHub.SendStatusUpdate(
            _hubContext,
            bookingId,
            status,
            message);
    }
}
