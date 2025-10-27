using WOL.Shared.Common.Domain;
using WOL.Shared.Common.Exceptions;
using WOL.Vehicle.Domain.Enums;
using WOL.Vehicle.Domain.Events;

namespace WOL.Vehicle.Domain.Entities;

public class Vehicle : BaseEntity
{
    public Guid OwnerId { get; private set; }
    public Guid VehicleTypeId { get; private set; }
    public string PlateNumber { get; private set; } = string.Empty;
    public string Make { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public int Year { get; private set; }
    public string Color { get; private set; } = string.Empty;
    public VehicleStatus Status { get; private set; }
    public bool IsAvailable { get; private set; }
    public DateTime? LastMaintenanceDate { get; private set; }
    public DateTime? NextMaintenanceDate { get; private set; }

    private Vehicle() { }

    public static Vehicle Create(
        Guid ownerId,
        Guid vehicleTypeId,
        string plateNumber,
        string make,
        string model,
        int year,
        string color)
    {
        var vehicle = new Vehicle
        {
            OwnerId = ownerId,
            VehicleTypeId = vehicleTypeId,
            PlateNumber = plateNumber,
            Make = make,
            Model = model,
            Year = year,
            Color = color,
            Status = VehicleStatus.Active,
            IsAvailable = true
        };

        vehicle.AddDomainEvent(new VehicleRegisteredEvent
        {
            VehicleId = vehicle.Id,
            OwnerId = vehicle.OwnerId,
            PlateNumber = vehicle.PlateNumber,
            OccurredOn = DateTime.UtcNow
        });

        return vehicle;
    }

    public void UpdateDetails(string make, string model, int year, string color)
    {
        Make = make;
        Model = model;
        Year = year;
        Color = color;
        SetUpdatedAt();
    }

    public void MarkAvailable()
    {
        if (Status != VehicleStatus.Active)
            throw new DomainException("Vehicle must be active to be marked as available");

        IsAvailable = true;
        SetUpdatedAt();
    }

    public void MarkUnavailable()
    {
        IsAvailable = false;
        SetUpdatedAt();
    }

    public void StartMaintenance()
    {
        Status = VehicleStatus.UnderMaintenance;
        IsAvailable = false;
        LastMaintenanceDate = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void CompleteMaintenance(DateTime? nextMaintenanceDate)
    {
        if (Status != VehicleStatus.UnderMaintenance)
            throw new DomainException("Vehicle is not under maintenance");

        Status = VehicleStatus.Active;
        IsAvailable = true;
        NextMaintenanceDate = nextMaintenanceDate;
        SetUpdatedAt();
    }

    public void Suspend()
    {
        Status = VehicleStatus.Suspended;
        IsAvailable = false;
        SetUpdatedAt();
    }

    public void Activate()
    {
        Status = VehicleStatus.Active;
        IsAvailable = true;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        Status = VehicleStatus.Inactive;
        IsAvailable = false;
        SetUpdatedAt();
    }
}
