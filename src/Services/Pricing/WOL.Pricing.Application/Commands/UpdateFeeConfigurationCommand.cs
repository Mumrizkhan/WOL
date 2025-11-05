using MediatR;
using WOL.Pricing.Domain.Repositories;

namespace WOL.Pricing.Application.Commands;

public record UpdateFeeConfigurationCommand : IRequest<UpdateFeeConfigurationResponse>
{
    public Guid Id { get; init; }
    public decimal Amount { get; init; }
}

public record UpdateFeeConfigurationResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}

public class UpdateFeeConfigurationCommandHandler : IRequestHandler<UpdateFeeConfigurationCommand, UpdateFeeConfigurationResponse>
{
    private readonly IFeeConfigurationRepository _feeConfigurationRepository;

    public UpdateFeeConfigurationCommandHandler(IFeeConfigurationRepository feeConfigurationRepository)
    {
        _feeConfigurationRepository = feeConfigurationRepository;
    }

    public async Task<UpdateFeeConfigurationResponse> Handle(UpdateFeeConfigurationCommand request, CancellationToken cancellationToken)
    {
        var fee = await _feeConfigurationRepository.GetByIdAsync(request.Id);

        if (fee == null)
        {
            return new UpdateFeeConfigurationResponse
            {
                Success = false,
                Message = "Fee configuration not found"
            };
        }

        fee.UpdateAmount(request.Amount);
        await _feeConfigurationRepository.UpdateAsync(fee);

        return new UpdateFeeConfigurationResponse
        {
            Success = true,
            Message = "Fee configuration updated successfully"
        };
    }
}
