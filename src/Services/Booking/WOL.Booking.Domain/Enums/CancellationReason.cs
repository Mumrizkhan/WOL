namespace WOL.Booking.Domain.Enums;

public enum CancellationReason
{
    CustomerNoShow = 1,
    CustomerSlowLoading = 2,
    CustomerRequestWithin30Minutes = 3,
    CustomerRequestAfter30Minutes = 4,
    DriverEarlyCancel = 5,
    DriverWaitedOneHour = 6,
    NoDriverAssigned = 7,
    VehicleBreakdown = 8,
    SafetyConcern = 9,
    Other = 10
}
