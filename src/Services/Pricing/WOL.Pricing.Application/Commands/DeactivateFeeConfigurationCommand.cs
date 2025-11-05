using MediatR;
using WOL.Pricing.Domain.Repositories;

namespace WOL.Pricing.Application.Commands;

public record DeactivateFeeConfigurationCommand : IRequest<DeactivateFeeConfigurationResponse>
{
    public Guid Id { get; init; }
}

public record DeactivateFeeConfigurationResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}

public class DeactivateFeeConfigurationCommandHandler : IRequestHandler<DeactivateFeeConfigurationCommand, DeactivateFeeConfigurationResponse>
{
    private readonly IFeeConfigurationRepository _feeConfigurationRepository;

    public DeactivateFeeConfigurationCommandHandler(IFeeConfigurationRepository feeConfigurationRepository)
    {
        _feeConfigurationRepository = feeConfigurationRepository;
    }

    public async Task<DeactivateFeeConfigurationResponse> Handle(DeactivateFeeConfigurationCommand request, CancellationToken cancellationToken)
    {
        var fee = await _feeConfigurationRepository.GetByIdAsync(request.Id);

        if (fee == null)
        {
            return new DeactivateFeeConfigurationResponse
            {
                Success = false,
                Message = "Fee configuration not found"
            };
        }

        fee.Deactivate();
        await _feeConfigurationRepository.UpdateAsync(fee);

        return new DeactivateFeeConfigurationResponse
        {
            Success = true,
            Message = "Fee configuration deactivated successfully"
        };
    }
}
