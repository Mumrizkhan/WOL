using WOL.Shared.Common.Domain;

namespace WOL.Vehicle.Domain.Entities;

public class VehicleType : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string NameAr { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public decimal LoadCapacityKg { get; private set; }
    public decimal LengthMeters { get; private set; }
    public decimal WidthMeters { get; private set; }
    public decimal HeightMeters { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    private VehicleType() { }

    public static VehicleType Create(
        string name, 
        string nameAr, 
        string category,
        decimal loadCapacityKg, 
        decimal lengthMeters,
        decimal widthMeters,
        decimal heightMeters,
        string? description)
    {
        return new VehicleType
        {
            Name = name,
            NameAr = nameAr,
            Category = category,
            LoadCapacityKg = loadCapacityKg,
            LengthMeters = lengthMeters,
            WidthMeters = widthMeters,
            HeightMeters = heightMeters,
            Description = description,
            IsActive = true
        };
    }

    public void Update(
        string name, 
        string nameAr, 
        string category,
        decimal loadCapacityKg,
        decimal lengthMeters,
        decimal widthMeters,
        decimal heightMeters,
        string? description)
    {
        Name = name;
        NameAr = nameAr;
        Category = category;
        LoadCapacityKg = loadCapacityKg;
        LengthMeters = lengthMeters;
        WidthMeters = widthMeters;
        HeightMeters = heightMeters;
        Description = description;
        SetUpdatedAt();
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }

    public decimal GetVolumeCubicMeters()
    {
        return LengthMeters * WidthMeters * HeightMeters;
    }
}
