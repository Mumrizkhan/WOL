using MediatR;
using WOL.Pricing.Domain.Repositories;

namespace WOL.Pricing.Application.Queries;

public record GetFeeConfigurationByTypeQuery : IRequest<FeeConfigurationDto?>
{
    public string FeeType { get; init; } = string.Empty;
}

public class GetFeeConfigurationByTypeQueryHandler : IRequestHandler<GetFeeConfigurationByTypeQuery, FeeConfigurationDto?>
{
    private readonly IFeeConfigurationRepository _feeConfigurationRepository;

    public GetFeeConfigurationByTypeQueryHandler(IFeeConfigurationRepository feeConfigurationRepository)
    {
        _feeConfigurationRepository = feeConfigurationRepository;
    }

    public async Task<FeeConfigurationDto?> Handle(GetFeeConfigurationByTypeQuery request, CancellationToken cancellationToken)
    {
        var fee = await _feeConfigurationRepository.GetByTypeAsync(request.FeeType);

        if (fee == null)
            return null;

        return new FeeConfigurationDto
        {
            Id = fee.Id,
            FeeType = fee.FeeType,
            Amount = fee.Amount,
            Description = fee.Description,
            IsActive = fee.IsActive
        };
    }
}
