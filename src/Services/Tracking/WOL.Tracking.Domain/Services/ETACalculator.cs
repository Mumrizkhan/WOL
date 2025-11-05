namespace WOL.Tracking.Domain.Services;

public class ETACalculator
{
    private const double AVERAGE_SPEED_KMH = 60.0;
    private const double TRAFFIC_MULTIPLIER_PEAK = 1.5;
    private const double TRAFFIC_MULTIPLIER_NORMAL = 1.0;

    public static DateTime CalculateETA(
        double remainingDistanceKm,
        DateTime currentTime,
        bool isPeakHour = false)
    {
        var multiplier = isPeakHour ? TRAFFIC_MULTIPLIER_PEAK : TRAFFIC_MULTIPLIER_NORMAL;
        var adjustedSpeed = AVERAGE_SPEED_KMH / multiplier;
        var hoursRemaining = remainingDistanceKm / adjustedSpeed;
        
        return currentTime.AddHours(hoursRemaining);
    }

    public static TimeSpan CalculateRemainingTime(
        double remainingDistanceKm,
        bool isPeakHour = false)
    {
        var multiplier = isPeakHour ? TRAFFIC_MULTIPLIER_PEAK : TRAFFIC_MULTIPLIER_NORMAL;
        var adjustedSpeed = AVERAGE_SPEED_KMH / multiplier;
        var hoursRemaining = remainingDistanceKm / adjustedSpeed;
        
        return TimeSpan.FromHours(hoursRemaining);
    }

    public static bool IsPeakHour(DateTime dateTime)
    {
        var hour = dateTime.Hour;
        return (hour >= 7 && hour <= 9) || (hour >= 17 && hour <= 19);
    }

    public static double CalculateRemainingDistance(
        double currentLatitude,
        double currentLongitude,
        double destinationLatitude,
        double destinationLongitude)
    {
        return CalculateHaversineDistance(
            currentLatitude,
            currentLongitude,
            destinationLatitude,
            destinationLongitude);
    }

    private static double CalculateHaversineDistance(
        double lat1,
        double lon1,
        double lat2,
        double lon2)
    {
        const double R = 6371;
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
