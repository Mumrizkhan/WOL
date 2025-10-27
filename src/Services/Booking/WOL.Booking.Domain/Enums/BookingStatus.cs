namespace WOL.Booking.Domain.Enums;

public enum BookingStatus
{
    Pending = 1,
    DriverAssigned = 2,
    DriverAccepted = 3,
    DriverReached = 4,
    LoadingStarted = 5,
    InTransit = 6,
    Delivered = 7,
    Completed = 8,
    Cancelled = 9,
    Rejected = 10
}
