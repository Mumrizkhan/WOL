using MediatR;
using WOL.Pricing.Domain.Repositories;

namespace WOL.Pricing.Application.Queries;

public record GetAllDiscountConfigurationsQuery : IRequest<GetAllDiscountConfigurationsResponse>;

public record GetAllDiscountConfigurationsResponse
{
    public List<DiscountConfigurationDto> Discounts { get; init; } = new();
}

public record DiscountConfigurationDto
{
    public Guid Id { get; init; }
    public string DiscountType { get; init; } = string.Empty;
    public decimal Percentage { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; }
}

public class GetAllDiscountConfigurationsQueryHandler : IRequestHandler<GetAllDiscountConfigurationsQuery, GetAllDiscountConfigurationsResponse>
{
    private readonly IDiscountConfigurationRepository _discountConfigurationRepository;

    public GetAllDiscountConfigurationsQueryHandler(IDiscountConfigurationRepository discountConfigurationRepository)
    {
        _discountConfigurationRepository = discountConfigurationRepository;
    }

    public async Task<GetAllDiscountConfigurationsResponse> Handle(GetAllDiscountConfigurationsQuery request, CancellationToken cancellationToken)
    {
        var discounts = await _discountConfigurationRepository.GetAllAsync();

        var dtos = discounts.Select(d => new DiscountConfigurationDto
        {
            Id = d.Id,
            DiscountType = d.DiscountType,
            Percentage = d.Percentage,
            Description = d.Description,
            IsActive = d.IsActive
        }).ToList();

        return new GetAllDiscountConfigurationsResponse
        {
            Discounts = dtos
        };
    }
}
