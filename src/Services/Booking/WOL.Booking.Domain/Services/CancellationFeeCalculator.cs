using WOL.Booking.Domain.Entities;
using WOL.Booking.Domain.Enums;
using WOL.Identity.Domain.Enums;

namespace WOL.Booking.Domain.Services;

public class CancellationFeeCalculator
{
    private const decimal SHIPPER_NO_SHOW_FEE_COMMERCIAL = 250m; // SAR
    private const decimal SHIPPER_NO_SHOW_FEE_INDIVIDUAL = 0m;   // No charge for individuals
    private const decimal DRIVER_WAITED_ONE_HOUR_FEE = 500m;     // SAR
    private const decimal ADMIN_FEE_AFTER_30_MINS = 100m;        // SAR
    private const decimal DRIVER_EARLY_CANCEL_PERCENTAGE = 0.50m; // 50% of trip fare

    public static CancellationFee? CalculateFee(
        Booking booking,
        CancellationReason reason,
        CancellationParty cancelledBy,
        UserType customerType,
        DateTime? driverReachedAt = null)
    {
        switch (reason)
        {
            case CancellationReason.CustomerNoShow:
                if (customerType == UserType.Commercial || customerType == UserType.Supplier)
                {
                    return CancellationFee.Create(
                        booking.Id,
                        reason,
                        CancellationParty.Customer,
                        CancellationParty.Customer,
                        SHIPPER_NO_SHOW_FEE_COMMERCIAL,
                        "Commercial customer no-show after 30 minutes");
                }
                return null;

            case CancellationReason.CustomerSlowLoading:
                if (customerType == UserType.Commercial || customerType == UserType.Supplier)
                {
                    return CancellationFee.Create(
                        booking.Id,
                        reason,
                        CancellationParty.Customer,
                        CancellationParty.Customer,
                        SHIPPER_NO_SHOW_FEE_COMMERCIAL,
                        "Commercial customer slow loading - exceeded free time");
                }
                return null;

            case CancellationReason.CustomerRequestWithin30Minutes:
                return null;

            case CancellationReason.CustomerRequestAfter30Minutes:
                if (!booking.DriverId.HasValue)
                {
                    return CancellationFee.Create(
                        booking.Id,
                        reason,
                        CancellationParty.Customer,
                        CancellationParty.Customer,
                        ADMIN_FEE_AFTER_30_MINS,
                        "Cancellation after 30 minutes, no driver assigned");
                }
                return null;

            case CancellationReason.DriverWaitedOneHour:
                return CancellationFee.Create(
                    booking.Id,
                    reason,
                    CancellationParty.Driver,
                    CancellationParty.Customer,
                    DRIVER_WAITED_ONE_HOUR_FEE,
                    "Driver waited 1+ hour with no loading");

            case CancellationReason.DriverEarlyCancel:
                var penaltyAmount = booking.FinalFare * DRIVER_EARLY_CANCEL_PERCENTAGE;
                return CancellationFee.Create(
                    booking.Id,
                    reason,
                    CancellationParty.Driver,
                    CancellationParty.Driver,
                    penaltyAmount,
                    "Driver cancelled before free time ended - 50% penalty");

            case CancellationReason.NoDriverAssigned:
                return null;

            case CancellationReason.VehicleBreakdown:
            case CancellationReason.SafetyConcern:
                return null;

            default:
                return null;
        }
    }

    public static bool CanCustomerCancel(Booking booking, DateTime requestTime)
    {
        var timeSinceBooking = requestTime - booking.CreatedAt;
        
        if (timeSinceBooking.TotalMinutes < 30)
            return true;

        if (!booking.DriverId.HasValue)
            return true;

        return false;
    }

    public static bool CanDriverCancel(Booking booking, DateTime requestTime, DateTime? driverReachedAt)
    {
        if (driverReachedAt.HasValue)
        {
            var waitTime = requestTime - driverReachedAt.Value;
            return waitTime.TotalHours >= 1;
        }

        return true;
    }
}
