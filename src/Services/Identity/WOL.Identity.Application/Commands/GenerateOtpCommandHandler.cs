using MediatR;
using MassTransit;
using WOL.Identity.Domain.Enums;
using WOL.Identity.Infrastructure.Services;
using WOL.Shared.Messages.Events;

namespace WOL.Identity.Application.Commands;

public class GenerateOtpCommandHandler : IRequestHandler<GenerateOtpCommand, string>
{
    private readonly IOtpService _otpService;
    private readonly IPublishEndpoint _publishEndpoint;

    public GenerateOtpCommandHandler(
        IOtpService otpService,
        IPublishEndpoint publishEndpoint)
    {
        _otpService = otpService;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<string> Handle(GenerateOtpCommand request, CancellationToken cancellationToken)
    {
        var otp = await _otpService.GenerateOtpAsync(
            request.UserId,
            request.Purpose,
            request.PhoneNumber,
            request.Email);

        await _publishEndpoint.Publish(new AuditLogCreatedEvent
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Action = AuditAction.OtpGenerated.ToString(),
            EntityName = "OtpCode",
            EntityId = otp.Id.ToString(),
            NewValues = $"Purpose: {request.Purpose}",
            Timestamp = DateTime.UtcNow
        }, cancellationToken);

        return otp.Code;
    }
}
