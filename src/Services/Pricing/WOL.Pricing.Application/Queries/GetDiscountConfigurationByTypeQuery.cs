using MediatR;
using WOL.Pricing.Domain.Repositories;

namespace WOL.Pricing.Application.Queries;

public record GetDiscountConfigurationByTypeQuery : IRequest<DiscountConfigurationDto?>
{
    public string DiscountType { get; init; } = string.Empty;
}

public class GetDiscountConfigurationByTypeQueryHandler : IRequestHandler<GetDiscountConfigurationByTypeQuery, DiscountConfigurationDto?>
{
    private readonly IDiscountConfigurationRepository _discountConfigurationRepository;

    public GetDiscountConfigurationByTypeQueryHandler(IDiscountConfigurationRepository discountConfigurationRepository)
    {
        _discountConfigurationRepository = discountConfigurationRepository;
    }

    public async Task<DiscountConfigurationDto?> Handle(GetDiscountConfigurationByTypeQuery request, CancellationToken cancellationToken)
    {
        var discount = await _discountConfigurationRepository.GetByTypeAsync(request.DiscountType);

        if (discount == null)
            return null;

        return new DiscountConfigurationDto
        {
            Id = discount.Id,
            DiscountType = discount.DiscountType,
            Percentage = discount.Percentage,
            Description = discount.Description,
            IsActive = discount.IsActive
        };
    }
}
