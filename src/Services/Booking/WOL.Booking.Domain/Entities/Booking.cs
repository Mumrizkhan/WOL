using WOL.Shared.Common.Domain;
using WOL.Shared.Common.Exceptions;
using WOL.Booking.Domain.Enums;
using WOL.Booking.Domain.ValueObjects;
using WOL.Booking.Domain.Events;

namespace WOL.Booking.Domain.Entities;

public class Booking : BaseEntity
{
    public string BookingNumber { get; private set; } = string.Empty;
    public Guid CustomerId { get; private set; }
    public Guid VehicleTypeId { get; private set; }
    public Guid? VehicleId { get; private set; }
    public Guid? DriverId { get; private set; }
    public Location Origin { get; private set; } = null!;
    public Location Destination { get; private set; } = null!;
    public DateTime PickupDate { get; private set; }
    public TimeSpan PickupTime { get; private set; }
    public CargoDetails Cargo { get; private set; } = null!;
    public ContactInfo Shipper { get; private set; } = null!;
    public ContactInfo Receiver { get; private set; } = null!;
    public BookingType BookingType { get; private set; }
    public BookingStatus Status { get; private set; }
    public decimal TotalFare { get; private set; }
    public decimal? DiscountAmount { get; private set; }
    public decimal FinalFare { get; private set; }
    public DateTime? DriverAssignedAt { get; private set; }
    public DateTime? DriverAcceptedAt { get; private set; }
    public DateTime? DriverReachedAt { get; private set; }
    public DateTime? LoadingStartedAt { get; private set; }
    public DateTime? InTransitAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public string? CancellationReason { get; private set; }

    private Booking() { }

    public static Booking Create(
        Guid customerId,
        Guid vehicleTypeId,
        Location origin,
        Location destination,
        DateTime pickupDate,
        TimeSpan pickupTime,
        CargoDetails cargo,
        ContactInfo shipper,
        ContactInfo receiver,
        BookingType bookingType,
        decimal totalFare)
    {
        var booking = new Booking
        {
            BookingNumber = GenerateBookingNumber(),
            CustomerId = customerId,
            VehicleTypeId = vehicleTypeId,
            Origin = origin,
            Destination = destination,
            PickupDate = pickupDate,
            PickupTime = pickupTime,
            Cargo = cargo,
            Shipper = shipper,
            Receiver = receiver,
            BookingType = bookingType,
            Status = BookingStatus.Pending,
            TotalFare = totalFare,
            FinalFare = totalFare
        };

        booking.AddDomainEvent(new BookingCreatedEvent
        {
            BookingId = booking.Id,
            BookingNumber = booking.BookingNumber,
            CustomerId = booking.CustomerId,
            VehicleTypeId = booking.VehicleTypeId,
            OriginAddress = booking.Origin.Address,
            DestinationAddress = booking.Destination.Address,
            PickupDate = booking.PickupDate,
            PickupTime = booking.PickupTime,
            TotalFare = booking.TotalFare,
            OccurredOn = DateTime.UtcNow
        });

        return booking;
    }

    public void AssignDriver(Guid vehicleId, Guid driverId)
    {
        if (Status != BookingStatus.Pending)
            throw new DomainException("Cannot assign driver to booking that is not pending");

        VehicleId = vehicleId;
        DriverId = driverId;
        Status = BookingStatus.DriverAssigned;
        DriverAssignedAt = DateTime.UtcNow;
        SetUpdatedAt();

        AddDomainEvent(new BookingAssignedEvent
        {
            BookingId = Id,
            BookingNumber = BookingNumber,
            CustomerId = CustomerId,
            VehicleId = vehicleId,
            DriverId = driverId,
            OccurredOn = DateTime.UtcNow
        });
    }

    public void AcceptByDriver()
    {
        if (Status != BookingStatus.DriverAssigned)
            throw new DomainException("Cannot accept booking that is not assigned");

        Status = BookingStatus.DriverAccepted;
        DriverAcceptedAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void MarkDriverReached()
    {
        if (Status != BookingStatus.DriverAccepted)
            throw new DomainException("Driver must accept booking before reaching pickup location");

        Status = BookingStatus.DriverReached;
        DriverReachedAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void StartLoading()
    {
        if (Status != BookingStatus.DriverReached)
            throw new DomainException("Driver must reach pickup location before starting loading");

        Status = BookingStatus.LoadingStarted;
        LoadingStartedAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void StartTransit()
    {
        if (Status != BookingStatus.LoadingStarted)
            throw new DomainException("Loading must be started before transit");

        Status = BookingStatus.InTransit;
        InTransitAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void MarkDelivered()
    {
        if (Status != BookingStatus.InTransit)
            throw new DomainException("Booking must be in transit before delivery");

        Status = BookingStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void Complete()
    {
        if (Status != BookingStatus.Delivered)
            throw new DomainException("Booking must be delivered before completion");

        Status = BookingStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        SetUpdatedAt();

        AddDomainEvent(new BookingCompletedEvent
        {
            BookingId = Id,
            BookingNumber = BookingNumber,
            CustomerId = CustomerId,
            DriverId = DriverId!.Value,
            TotalFare = FinalFare,
            OccurredOn = DateTime.UtcNow
        });
    }

    public void Cancel(string reason)
    {
        if (Status == BookingStatus.Completed || Status == BookingStatus.Cancelled)
            throw new DomainException("Cannot cancel completed or already cancelled booking");

        Status = BookingStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancellationReason = reason;
        SetUpdatedAt();
    }

    public void ApplyDiscount(decimal discountAmount)
    {
        if (discountAmount < 0 || discountAmount > TotalFare)
            throw new DomainException("Invalid discount amount");

        DiscountAmount = discountAmount;
        FinalFare = TotalFare - discountAmount;
        SetUpdatedAt();
    }

    private static string GenerateBookingNumber()
    {
        return $"BK{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }
}
