using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace WOL.Tracking.API.Hubs;

[Authorize]
public class LocationTrackingHub : Hub
{
    private static readonly Dictionary<string, HashSet<string>> _bookingConnections = new();
    private static readonly object _lock = new();

    public async Task JoinBookingTracking(string bookingId)
    {
        var groupName = $"booking_{bookingId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        lock (_lock)
        {
            if (!_bookingConnections.ContainsKey(bookingId))
            {
                _bookingConnections[bookingId] = new HashSet<string>();
            }
            _bookingConnections[bookingId].Add(Context.ConnectionId);
        }

        await Clients.Caller.SendAsync("JoinedBookingTracking", bookingId);
    }

    public async Task LeaveBookingTracking(string bookingId)
    {
        var groupName = $"booking_{bookingId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        lock (_lock)
        {
            if (_bookingConnections.ContainsKey(bookingId))
            {
                _bookingConnections[bookingId].Remove(Context.ConnectionId);
                if (_bookingConnections[bookingId].Count == 0)
                {
                    _bookingConnections.Remove(bookingId);
                }
            }
        }

        await Clients.Caller.SendAsync("LeftBookingTracking", bookingId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        lock (_lock)
        {
            var bookingsToRemove = new List<string>();
            foreach (var kvp in _bookingConnections)
            {
                kvp.Value.Remove(Context.ConnectionId);
                if (kvp.Value.Count == 0)
                {
                    bookingsToRemove.Add(kvp.Key);
                }
            }

            foreach (var bookingId in bookingsToRemove)
            {
                _bookingConnections.Remove(bookingId);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    public static async Task SendLocationUpdate(
        IHubContext<LocationTrackingHub> hubContext,
        string bookingId,
        double latitude,
        double longitude,
        double speed,
        DateTime timestamp)
    {
        var groupName = $"booking_{bookingId}";
        await hubContext.Clients.Group(groupName).SendAsync("LocationUpdated", new
        {
            BookingId = bookingId,
            Latitude = latitude,
            Longitude = longitude,
            Speed = speed,
            Timestamp = timestamp
        });
    }

    public static async Task SendETAUpdate(
        IHubContext<LocationTrackingHub> hubContext,
        string bookingId,
        DateTime estimatedArrival,
        TimeSpan remainingTime,
        double remainingDistanceKm)
    {
        var groupName = $"booking_{bookingId}";
        await hubContext.Clients.Group(groupName).SendAsync("ETAUpdated", new
        {
            BookingId = bookingId,
            EstimatedArrival = estimatedArrival,
            RemainingTime = remainingTime,
            RemainingDistanceKm = remainingDistanceKm
        });
    }

    public static async Task SendStatusUpdate(
        IHubContext<LocationTrackingHub> hubContext,
        string bookingId,
        string status,
        string message)
    {
        var groupName = $"booking_{bookingId}";
        await hubContext.Clients.Group(groupName).SendAsync("StatusUpdated", new
        {
            BookingId = bookingId,
            Status = status,
            Message = message,
            Timestamp = DateTime.UtcNow
        });
    }
}
