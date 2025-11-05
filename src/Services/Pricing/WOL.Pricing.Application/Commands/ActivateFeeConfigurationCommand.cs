using MediatR;
using WOL.Pricing.Domain.Repositories;

namespace WOL.Pricing.Application.Commands;

public record ActivateFeeConfigurationCommand : IRequest<ActivateFeeConfigurationResponse>
{
    public Guid Id { get; init; }
}

public record ActivateFeeConfigurationResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}

public class ActivateFeeConfigurationCommandHandler : IRequestHandler<ActivateFeeConfigurationCommand, ActivateFeeConfigurationResponse>
{
    private readonly IFeeConfigurationRepository _feeConfigurationRepository;

    public ActivateFeeConfigurationCommandHandler(IFeeConfigurationRepository feeConfigurationRepository)
    {
        _feeConfigurationRepository = feeConfigurationRepository;
    }

    public async Task<ActivateFeeConfigurationResponse> Handle(ActivateFeeConfigurationCommand request, CancellationToken cancellationToken)
    {
        var fee = await _feeConfigurationRepository.GetByIdAsync(request.Id);

        if (fee == null)
        {
            return new ActivateFeeConfigurationResponse
            {
                Success = false,
                Message = "Fee configuration not found"
            };
        }

        fee.Activate();
        await _feeConfigurationRepository.UpdateAsync(fee);

        return new ActivateFeeConfigurationResponse
        {
            Success = true,
            Message = "Fee configuration activated successfully"
        };
    }
}
