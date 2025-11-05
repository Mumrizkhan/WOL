using MediatR;
using WOL.Pricing.Domain.Repositories;

namespace WOL.Pricing.Application.Commands;

public record DeactivateDiscountConfigurationCommand : IRequest<DeactivateDiscountConfigurationResponse>
{
    public Guid Id { get; init; }
}

public record DeactivateDiscountConfigurationResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}

public class DeactivateDiscountConfigurationCommandHandler : IRequestHandler<DeactivateDiscountConfigurationCommand, DeactivateDiscountConfigurationResponse>
{
    private readonly IDiscountConfigurationRepository _discountConfigurationRepository;

    public DeactivateDiscountConfigurationCommandHandler(IDiscountConfigurationRepository discountConfigurationRepository)
    {
        _discountConfigurationRepository = discountConfigurationRepository;
    }

    public async Task<DeactivateDiscountConfigurationResponse> Handle(DeactivateDiscountConfigurationCommand request, CancellationToken cancellationToken)
    {
        var discount = await _discountConfigurationRepository.GetByIdAsync(request.Id);

        if (discount == null)
        {
            return new DeactivateDiscountConfigurationResponse
            {
                Success = false,
                Message = "Discount configuration not found"
            };
        }

        discount.Deactivate();
        await _discountConfigurationRepository.UpdateAsync(discount);

        return new DeactivateDiscountConfigurationResponse
        {
            Success = true,
            Message = "Discount configuration deactivated successfully"
        };
    }
}
