using MediatR;
using MassTransit;
using WOL.Shared.Common.Application;
using WOL.Tracking.Domain.Entities;
using WOL.Tracking.Domain.Repositories;
using WOL.Tracking.Domain.Services;
using WOL.Tracking.Application.Services;
using WOL.Shared.Messages.Events;

namespace WOL.Tracking.Application.Commands;

public class RecordLocationCommandHandler : IRequestHandler<RecordLocationCommand, RecordLocationResponse>
{
    private readonly ILocationHistoryRepository _locationHistoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILocationUpdateBroadcaster _locationBroadcaster;
    private readonly IPublishEndpoint _publishEndpoint;

    public RecordLocationCommandHandler(
        ILocationHistoryRepository locationHistoryRepository,
        IUnitOfWork unitOfWork,
        ILocationUpdateBroadcaster locationBroadcaster,
        IPublishEndpoint publishEndpoint)
    {
        _locationHistoryRepository = locationHistoryRepository;
        _unitOfWork = unitOfWork;
        _locationBroadcaster = locationBroadcaster;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<RecordLocationResponse> Handle(RecordLocationCommand request, CancellationToken cancellationToken)
    {
        var location = LocationHistory.Create(
            request.BookingId,
            request.VehicleId,
            request.DriverId,
            request.Latitude,
            request.Longitude,
            request.Speed,
            request.Heading);

        await _locationHistoryRepository.AddAsync(location, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _locationBroadcaster.BroadcastLocationUpdate(
            request.BookingId.ToString(),
            request.Latitude,
            request.Longitude,
            request.Speed,
            DateTime.UtcNow);

        await _publishEndpoint.Publish(new LocationUpdatedEvent
        {
            BookingId = request.BookingId,
            DriverId = request.DriverId,
            VehicleId = request.VehicleId,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Speed = request.Speed,
            Heading = request.Heading,
            Timestamp = DateTime.UtcNow
        }, cancellationToken);

        if (request.DestinationLatitude.HasValue && request.DestinationLongitude.HasValue)
        {
            var remainingDistance = ETACalculator.CalculateRemainingDistance(
                request.Latitude,
                request.Longitude,
                request.DestinationLatitude.Value,
                request.DestinationLongitude.Value);

            var isPeakHour = ETACalculator.IsPeakHour(DateTime.UtcNow);
            var eta = ETACalculator.CalculateETA(remainingDistance, DateTime.UtcNow, isPeakHour);
            var remainingTime = ETACalculator.CalculateRemainingTime(remainingDistance, isPeakHour);

            await _locationBroadcaster.BroadcastETAUpdate(
                request.BookingId.ToString(),
                eta,
                remainingTime,
                remainingDistance);

            await _publishEndpoint.Publish(new ETAUpdatedEvent
            {
                BookingId = request.BookingId,
                DriverId = request.DriverId,
                CustomerId = request.CustomerId,
                CurrentLatitude = request.Latitude,
                CurrentLongitude = request.Longitude,
                RemainingDistanceKm = remainingDistance,
                EstimatedArrival = eta,
                RemainingTime = remainingTime,
                Timestamp = DateTime.UtcNow
            }, cancellationToken);
        }

        return new RecordLocationResponse
        {
            LocationId = location.Id,
            Message = "Location recorded successfully"
        };
    }
}
