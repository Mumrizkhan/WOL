using MediatR;
using WOL.Shared.Common.Application;
using WOL.Backload.Domain.Repositories;

namespace WOL.Backload.Application.Commands;

public class CreateBackloadOpportunityCommandHandler : IRequestHandler<CreateBackloadOpportunityCommand, CreateBackloadOpportunityResponse>
{
    private readonly IBackloadOpportunityRepository _backloadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBackloadOpportunityCommandHandler(IBackloadOpportunityRepository backloadRepository, IUnitOfWork unitOfWork)
    {
        _backloadRepository = backloadRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateBackloadOpportunityResponse> Handle(CreateBackloadOpportunityCommand request, CancellationToken cancellationToken)
    {
        var opportunity = Domain.Entities.BackloadOpportunity.Create(
            request.VehicleId,
            request.DriverId,
            request.FromCity,
            request.ToCity,
            request.AvailableFrom,
            request.AvailableTo,
            request.AvailableCapacity);

        await _backloadRepository.AddAsync(opportunity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateBackloadOpportunityResponse
        {
            OpportunityId = opportunity.Id,
            Message = "Backload opportunity created successfully"
        };
    }
}
