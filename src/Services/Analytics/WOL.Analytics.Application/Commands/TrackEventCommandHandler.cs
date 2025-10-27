using MediatR;
using WOL.Shared.Common.Application;
using WOL.Analytics.Domain.Repositories;

namespace WOL.Analytics.Application.Commands;

public class TrackEventCommandHandler : IRequestHandler<TrackEventCommand, TrackEventResponse>
{
    private readonly IAnalyticsEventRepository _analyticsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TrackEventCommandHandler(IAnalyticsEventRepository analyticsRepository, IUnitOfWork unitOfWork)
    {
        _analyticsRepository = analyticsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TrackEventResponse> Handle(TrackEventCommand request, CancellationToken cancellationToken)
    {
        var analyticsEvent = Domain.Entities.AnalyticsEvent.Create(
            request.EventType,
            request.EventCategory,
            request.UserId,
            request.EntityId,
            request.EntityType,
            request.Metadata);

        await _analyticsRepository.AddAsync(analyticsEvent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TrackEventResponse
        {
            EventId = analyticsEvent.Id,
            Message = "Event tracked successfully"
        };
    }
}
