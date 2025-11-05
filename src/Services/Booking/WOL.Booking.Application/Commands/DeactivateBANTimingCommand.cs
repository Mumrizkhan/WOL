using MediatR;
using WOL.Booking.Domain.Repositories;

namespace WOL.Booking.Application.Commands;

public record DeactivateBANTimingCommand : IRequest<DeactivateBANTimingResponse>
{
    public Guid Id { get; init; }
}

public record DeactivateBANTimingResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}

public class DeactivateBANTimingCommandHandler : IRequestHandler<DeactivateBANTimingCommand, DeactivateBANTimingResponse>
{
    private readonly IBANTimingRepository _banTimingRepository;

    public DeactivateBANTimingCommandHandler(IBANTimingRepository banTimingRepository)
    {
        _banTimingRepository = banTimingRepository;
    }

    public async Task<DeactivateBANTimingResponse> Handle(DeactivateBANTimingCommand request, CancellationToken cancellationToken)
    {
        var banTiming = await _banTimingRepository.GetByIdAsync(request.Id);

        if (banTiming == null)
        {
            return new DeactivateBANTimingResponse
            {
                Success = false,
                Message = "BAN timing not found"
            };
        }

        banTiming.Deactivate();
        await _banTimingRepository.UpdateAsync(banTiming);

        return new DeactivateBANTimingResponse
        {
            Success = true,
            Message = "BAN timing deactivated successfully"
        };
    }
}
