using MediatR;
using WOL.Shared.Common.Application;
using WOL.Tracking.Domain.Entities;
using WOL.Tracking.Domain.Repositories;

namespace WOL.Tracking.Application.Commands;

public class RecordLocationCommandHandler : IRequestHandler<RecordLocationCommand, RecordLocationResponse>
{
    private readonly ILocationHistoryRepository _locationHistoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RecordLocationCommandHandler(
        ILocationHistoryRepository locationHistoryRepository,
        IUnitOfWork unitOfWork)
    {
        _locationHistoryRepository = locationHistoryRepository;
        _unitOfWork = unitOfWork;
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

        return new RecordLocationResponse
        {
            LocationId = location.Id,
            Message = "Location recorded successfully"
        };
    }
}
