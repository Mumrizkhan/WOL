using MediatR;
using WOL.Shared.Common.Application;
using WOL.Notification.Domain.Repositories;

namespace WOL.Notification.Application.Commands;

public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, SendNotificationResponse>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SendNotificationCommandHandler(
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SendNotificationResponse> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = Domain.Entities.Notification.Create(
            request.UserId,
            request.Title,
            request.Body,
            request.Type,
            request.Data);

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SendNotificationResponse
        {
            NotificationId = notification.Id,
            Message = "Notification sent successfully"
        };
    }
}
