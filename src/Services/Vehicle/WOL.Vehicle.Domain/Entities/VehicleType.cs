using WOL.Shared.Common.Domain;

namespace WOL.Vehicle.Domain.Entities;

public class VehicleType : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string NameAr { get; private set; } = string.Empty;
    public decimal LoadCapacityKg { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    private VehicleType() { }

    public static VehicleType Create(string name, string nameAr, decimal loadCapacityKg, string? description)
    {
        return new VehicleType
        {
            Name = name,
            NameAr = nameAr,
            LoadCapacityKg = loadCapacityKg,
            Description = description,
            IsActive = true
        };
    }

    public void Update(string name, string nameAr, decimal loadCapacityKg, string? description)
    {
        Name = name;
        NameAr = nameAr;
        LoadCapacityKg = loadCapacityKg;
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
}
