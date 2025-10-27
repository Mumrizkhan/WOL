using MediatR;
using WOL.Shared.Common.Application;
using WOL.Booking.Domain.Entities;
using WOL.Booking.Domain.Repositories;
using WOL.Booking.Domain.ValueObjects;
using WOL.Booking.Application.Services;

namespace WOL.Booking.Application.Commands;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, CreateBookingResponse>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPricingService _pricingService;

    public CreateBookingCommandHandler(
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork,
        IPricingService pricingService)
    {
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _pricingService = pricingService;
    }

    public async Task<CreateBookingResponse> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var totalFare = await _pricingService.CalculateFareAsync(
            request.VehicleTypeId,
            request.Origin.Latitude,
            request.Origin.Longitude,
            request.Destination.Latitude,
            request.Destination.Longitude,
            request.BookingType,
            cancellationToken);

        var origin = new Location(
            request.Origin.Address,
            request.Origin.Latitude,
            request.Origin.Longitude,
            request.Origin.City);

        var destination = new Location(
            request.Destination.Address,
            request.Destination.Latitude,
            request.Destination.Longitude,
            request.Destination.City);

        var cargo = new CargoDetails(
            request.Cargo.Type,
            request.Cargo.GrossWeightKg,
            request.Cargo.NetWeightKg,
            request.Cargo.NumberOfBoxes,
            request.Cargo.Description);

        var shipper = new ContactInfo(
            request.Shipper.Name,
            request.Shipper.Mobile,
            request.Shipper.Email);

        var receiver = new ContactInfo(
            request.Receiver.Name,
            request.Receiver.Mobile,
            request.Receiver.Email);

        var booking = Domain.Entities.Booking.Create(
            request.CustomerId,
            request.VehicleTypeId,
            origin,
            destination,
            request.PickupDate,
            request.PickupTime,
            cargo,
            shipper,
            receiver,
            request.BookingType,
            totalFare);

        await _bookingRepository.AddAsync(booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateBookingResponse
        {
            BookingId = booking.Id,
            BookingNumber = booking.BookingNumber,
            TotalFare = booking.TotalFare,
            Message = "Booking created successfully"
        };
    }
}
