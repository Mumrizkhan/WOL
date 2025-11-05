using MediatR;
using WOL.Booking.Domain.Repositories;

namespace WOL.Booking.Application.Commands;

public record ActivateBANTimingCommand : IRequest<ActivateBANTimingResponse>
{
    public Guid Id { get; init; }
}

public record ActivateBANTimingResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}

public class ActivateBANTimingCommandHandler : IRequestHandler<ActivateBANTimingCommand, ActivateBANTimingResponse>
{
    private readonly IBANTimingRepository _banTimingRepository;

    public ActivateBANTimingCommandHandler(IBANTimingRepository banTimingRepository)
    {
        _banTimingRepository = banTimingRepository;
    }

    public async Task<ActivateBANTimingResponse> Handle(ActivateBANTimingCommand request, CancellationToken cancellationToken)
    {
        var banTiming = await _banTimingRepository.GetByIdAsync(request.Id);

        if (banTiming == null)
        {
            return new ActivateBANTimingResponse
            {
                Success = false,
                Message = "BAN timing not found"
            };
        }

        banTiming.Activate();
        await _banTimingRepository.UpdateAsync(banTiming);

        return new ActivateBANTimingResponse
        {
            Success = true,
            Message = "BAN timing activated successfully"
        };
    }
}
