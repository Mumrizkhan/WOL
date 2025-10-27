using WOL.Booking.Application.Services;
using WOL.Booking.Domain.Enums;

namespace WOL.Booking.Infrastructure.Services;

public class PricingService : IPricingService
{
    public async Task<decimal> CalculateFareAsync(
        Guid vehicleTypeId,
        decimal originLat,
        decimal originLng,
        decimal destLat,
        decimal destLng,
        BookingType bookingType,
        CancellationToken cancellationToken = default)
    {
        var distance = CalculateDistance(originLat, originLng, destLat, destLng);

        decimal baseFare = 50; // Base fare in SAR
        decimal perKmRate = 2.5m; // Rate per km in SAR
        decimal totalFare = baseFare + (distance * perKmRate);

        if (bookingType == BookingType.Backload)
        {
            totalFare *= 0.85m; // 15% discount for backload
        }
        else if (bookingType == BookingType.SharedLoad)
        {
            totalFare *= 0.70m; // 30% discount for shared load
        }

        return await Task.FromResult(Math.Round(totalFare, 2));
    }

    private static decimal CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
    {
        const decimal R = 6371; // Earth's radius in km

        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = (decimal)(Math.Sin((double)dLat / 2) * Math.Sin((double)dLat / 2) +
                Math.Cos((double)ToRadians(lat1)) * Math.Cos((double)ToRadians(lat2)) *
                Math.Sin((double)dLon / 2) * Math.Sin((double)dLon / 2));

        var c = (decimal)(2 * Math.Atan2(Math.Sqrt((double)a), Math.Sqrt((double)(1 - a))));

        return R * c;
    }

    private static decimal ToRadians(decimal degrees)
    {
        return degrees * (decimal)Math.PI / 180m;
    }
}
