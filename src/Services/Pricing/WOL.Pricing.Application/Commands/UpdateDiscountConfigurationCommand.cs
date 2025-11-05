using MediatR;
using WOL.Pricing.Domain.Repositories;

namespace WOL.Pricing.Application.Commands;

public record UpdateDiscountConfigurationCommand : IRequest<UpdateDiscountConfigurationResponse>
{
    public Guid Id { get; init; }
    public decimal Percentage { get; init; }
}

public record UpdateDiscountConfigurationResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}

public class UpdateDiscountConfigurationCommandHandler : IRequestHandler<UpdateDiscountConfigurationCommand, UpdateDiscountConfigurationResponse>
{
    private readonly IDiscountConfigurationRepository _discountConfigurationRepository;

    public UpdateDiscountConfigurationCommandHandler(IDiscountConfigurationRepository discountConfigurationRepository)
    {
        _discountConfigurationRepository = discountConfigurationRepository;
    }

    public async Task<UpdateDiscountConfigurationResponse> Handle(UpdateDiscountConfigurationCommand request, CancellationToken cancellationToken)
    {
        var discount = await _discountConfigurationRepository.GetByIdAsync(request.Id);

        if (discount == null)
        {
            return new UpdateDiscountConfigurationResponse
            {
                Success = false,
                Message = "Discount configuration not found"
            };
        }

        try
        {
            discount.UpdatePercentage(request.Percentage);
            await _discountConfigurationRepository.UpdateAsync(discount);

            return new UpdateDiscountConfigurationResponse
            {
                Success = true,
                Message = "Discount configuration updated successfully"
            };
        }
        catch (ArgumentException ex)
        {
            return new UpdateDiscountConfigurationResponse
            {
                Success = false,
                Message = ex.Message
            };
        }
    }
}
