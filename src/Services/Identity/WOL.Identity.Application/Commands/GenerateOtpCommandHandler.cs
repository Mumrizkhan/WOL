using MediatR;
using WOL.Identity.Domain.Enums;
using WOL.Identity.Domain.Repositories;
using WOL.Identity.Infrastructure.Services;

namespace WOL.Identity.Application.Commands;

public class GenerateOtpCommandHandler : IRequestHandler<GenerateOtpCommand, string>
{
    private readonly IOtpService _otpService;
    private readonly IAuditLogRepository _auditLogRepository;

    public GenerateOtpCommandHandler(
        IOtpService otpService,
        IAuditLogRepository auditLogRepository)
    {
        _otpService = otpService;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<string> Handle(GenerateOtpCommand request, CancellationToken cancellationToken)
    {
        var otp = await _otpService.GenerateOtpAsync(
            request.UserId,
            request.Purpose,
            request.PhoneNumber,
            request.Email);

        await _auditLogRepository.AddAsync(new Domain.Entities.AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Action = AuditAction.OtpGenerated,
            EntityName = "OtpCode",
            EntityId = otp.Id.ToString(),
            NewValues = $"Purpose: {request.Purpose}",
            Timestamp = DateTime.UtcNow
        }, cancellationToken);

        return otp.Code;
    }
}
