using MediatR;
using WOL.Shared.Common.Application;
using WOL.Compliance.Domain.Repositories;

namespace WOL.Compliance.Application.Commands;

public class CreateComplianceCheckCommandHandler : IRequestHandler<CreateComplianceCheckCommand, CreateComplianceCheckResponse>
{
    private readonly IComplianceCheckRepository _complianceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateComplianceCheckCommandHandler(IComplianceCheckRepository complianceRepository, IUnitOfWork unitOfWork)
    {
        _complianceRepository = complianceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateComplianceCheckResponse> Handle(CreateComplianceCheckCommand request, CancellationToken cancellationToken)
    {
        var check = Domain.Entities.ComplianceCheck.Create(
            request.EntityId,
            request.EntityType,
            request.CheckType);

        await _complianceRepository.AddAsync(check, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateComplianceCheckResponse
        {
            CheckId = check.Id,
            Message = "Compliance check created successfully"
        };
    }
}
