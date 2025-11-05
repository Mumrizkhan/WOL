using MediatR;
using WOL.Pricing.Domain.Repositories;

namespace WOL.Pricing.Application.Queries;

public record GetAllFeeConfigurationsQuery : IRequest<GetAllFeeConfigurationsResponse>;

public record GetAllFeeConfigurationsResponse
{
    public List<FeeConfigurationDto> Fees { get; init; } = new();
}

public record FeeConfigurationDto
{
    public Guid Id { get; init; }
    public string FeeType { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; }
}

public class GetAllFeeConfigurationsQueryHandler : IRequestHandler<GetAllFeeConfigurationsQuery, GetAllFeeConfigurationsResponse>
{
    private readonly IFeeConfigurationRepository _feeConfigurationRepository;

    public GetAllFeeConfigurationsQueryHandler(IFeeConfigurationRepository feeConfigurationRepository)
    {
        _feeConfigurationRepository = feeConfigurationRepository;
    }

    public async Task<GetAllFeeConfigurationsResponse> Handle(GetAllFeeConfigurationsQuery request, CancellationToken cancellationToken)
    {
        var fees = await _feeConfigurationRepository.GetAllAsync();

        var dtos = fees.Select(f => new FeeConfigurationDto
        {
            Id = f.Id,
            FeeType = f.FeeType,
            Amount = f.Amount,
            Description = f.Description,
            IsActive = f.IsActive
        }).ToList();

        return new GetAllFeeConfigurationsResponse
        {
            Fees = dtos
        };
    }
}
