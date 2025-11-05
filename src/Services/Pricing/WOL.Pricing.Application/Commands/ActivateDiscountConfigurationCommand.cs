using MediatR;
using WOL.Pricing.Domain.Repositories;

namespace WOL.Pricing.Application.Commands;

public record ActivateDiscountConfigurationCommand : IRequest<ActivateDiscountConfigurationResponse>
{
    public Guid Id { get; init; }
}

public record ActivateDiscountConfigurationResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}

public class ActivateDiscountConfigurationCommandHandler : IRequestHandler<ActivateDiscountConfigurationCommand, ActivateDiscountConfigurationResponse>
{
    private readonly IDiscountConfigurationRepository _discountConfigurationRepository;

    public ActivateDiscountConfigurationCommandHandler(IDiscountConfigurationRepository discountConfigurationRepository)
    {
        _discountConfigurationRepository = discountConfigurationRepository;
    }

    public async Task<ActivateDiscountConfigurationResponse> Handle(ActivateDiscountConfigurationCommand request, CancellationToken cancellationToken)
    {
        var discount = await _discountConfigurationRepository.GetByIdAsync(request.Id);

        if (discount == null)
        {
            return new ActivateDiscountConfigurationResponse
            {
                Success = false,
                Message = "Discount configuration not found"
            };
        }

        discount.Activate();
        await _discountConfigurationRepository.UpdateAsync(discount);

        return new ActivateDiscountConfigurationResponse
        {
            Success = true,
            Message = "Discount configuration activated successfully"
        };
    }
}
