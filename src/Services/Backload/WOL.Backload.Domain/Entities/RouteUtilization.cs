using WOL.Shared.Common.Domain;

namespace WOL.Backload.Domain.Entities;

public class RouteUtilization : BaseEntity
{
    public string OriginCity { get; private set; } = string.Empty;
    public string DestinationCity { get; private set; } = string.Empty;
    public int OutboundBookings { get; private set; }
    public int ReturnBookings { get; private set; }
    public decimal UtilizationPercentage { get; private set; }
    public decimal EmptyKmTotal { get; private set; }
    public decimal EmptyKmSaved { get; private set; }
    public DateTime PeriodStart { get; private set; }
    public DateTime PeriodEnd { get; private set; }

    private RouteUtilization() { }

    public static RouteUtilization Create(
        string originCity,
        string destinationCity,
        DateTime periodStart,
        DateTime periodEnd)
    {
        return new RouteUtilization
        {
            OriginCity = originCity,
            DestinationCity = destinationCity,
            OutboundBookings = 0,
            ReturnBookings = 0,
            UtilizationPercentage = 0,
            EmptyKmTotal = 0,
            EmptyKmSaved = 0,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd
        };
    }

    public void RecordOutboundBooking()
    {
        OutboundBookings++;
        CalculateUtilization();
        SetUpdatedAt();
    }

    public void RecordReturnBooking(decimal distanceKm)
    {
        ReturnBookings++;
        EmptyKmSaved += distanceKm;
        CalculateUtilization();
        SetUpdatedAt();
    }

    public void RecordEmptyReturn(decimal distanceKm)
    {
        EmptyKmTotal += distanceKm;
        CalculateUtilization();
        SetUpdatedAt();
    }

    private void CalculateUtilization()
    {
        if (OutboundBookings == 0)
        {
            UtilizationPercentage = 0;
            return;
        }

        UtilizationPercentage = (decimal)ReturnBookings / OutboundBookings * 100;
    }

    public decimal GetEmptyKmPercentage()
    {
        var totalKm = EmptyKmTotal + EmptyKmSaved;
        if (totalKm == 0) return 0;
        
        return (EmptyKmTotal / totalKm) * 100;
    }
}
