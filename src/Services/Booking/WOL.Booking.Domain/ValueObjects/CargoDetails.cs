using WOL.Shared.Common.Domain;

namespace WOL.Booking.Domain.ValueObjects;

public class CargoDetails : ValueObject
{
    public string Type { get; private set; }
    public decimal GrossWeightKg { get; private set; }
    public decimal? NetWeightKg { get; private set; }
    public int? NumberOfBoxes { get; private set; }
    public string? Description { get; private set; }

    private CargoDetails() 
    {
        Type = string.Empty;
    }

    public CargoDetails(string type, decimal grossWeightKg, decimal? netWeightKg, int? numberOfBoxes, string? description)
    {
        Type = type;
        GrossWeightKg = grossWeightKg;
        NetWeightKg = netWeightKg;
        NumberOfBoxes = numberOfBoxes;
        Description = description;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Type;
        yield return GrossWeightKg;
        yield return NetWeightKg;
        yield return NumberOfBoxes;
        yield return Description;
    }
}
