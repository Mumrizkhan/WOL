using MediatR;
using MassTransit;
using WOL.Identity.Domain.Enums;
using WOL.Identity.Infrastructure.Services;
using WOL.Shared.Messages.Events;

namespace WOL.Identity.Application.Commands;

public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, bool>
{
    private readonly IOtpService _otpService;
    private readonly IPublishEndpoint _publishEndpoint;

    public VerifyOtpCommandHandler(
        IOtpService otpService,
        IPublishEndpoint publishEndpoint)
    {
        _otpService = otpService;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<bool> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        var isValid = await _otpService.ValidateOtpAsync(request.UserId, request.Code, request.Purpose);

        if (isValid)
        {
            await _publishEndpoint.Publish(new AuditLogCreatedEvent
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Action = AuditAction.OtpVerified.ToString(),
                EntityName = "OtpCode",
                EntityId = request.UserId.ToString(),
                NewValues = $"Purpose: {request.Purpose}",
                Timestamp = DateTime.UtcNow
            }, cancellationToken);
        }

        return isValid;
    }
}
