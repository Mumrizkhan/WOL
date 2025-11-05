using MediatR;
using WOL.Identity.Domain.Enums;
using WOL.Identity.Domain.Repositories;
using WOL.Identity.Infrastructure.Services;

namespace WOL.Identity.Application.Commands;

public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, bool>
{
    private readonly IOtpService _otpService;
    private readonly IAuditLogRepository _auditLogRepository;

    public VerifyOtpCommandHandler(
        IOtpService otpService,
        IAuditLogRepository auditLogRepository)
    {
        _otpService = otpService;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<bool> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        var isValid = await _otpService.ValidateOtpAsync(request.UserId, request.Code, request.Purpose);

        if (isValid)
        {
            await _auditLogRepository.AddAsync(new Domain.Entities.AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Action = AuditAction.OtpVerified,
                EntityName = "OtpCode",
                EntityId = request.UserId.ToString(),
                NewValues = $"Purpose: {request.Purpose}",
                Timestamp = DateTime.UtcNow
            }, cancellationToken);
        }

        return isValid;
    }
}
