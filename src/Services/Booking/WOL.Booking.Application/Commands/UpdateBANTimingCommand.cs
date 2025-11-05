using MediatR;
using WOL.Booking.Domain.Repositories;

namespace WOL.Booking.Application.Commands;

public record UpdateBANTimingCommand : IRequest<UpdateBANTimingResponse>
{
    public Guid Id { get; init; }
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
    public string? Description { get; init; }
}

public record UpdateBANTimingResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}

public class UpdateBANTimingCommandHandler : IRequestHandler<UpdateBANTimingCommand, UpdateBANTimingResponse>
{
    private readonly IBANTimingRepository _banTimingRepository;

    public UpdateBANTimingCommandHandler(IBANTimingRepository banTimingRepository)
    {
        _banTimingRepository = banTimingRepository;
    }

    public async Task<UpdateBANTimingResponse> Handle(UpdateBANTimingCommand request, CancellationToken cancellationToken)
    {
        var banTiming = await _banTimingRepository.GetByIdAsync(request.Id);

        if (banTiming == null)
        {
            return new UpdateBANTimingResponse
            {
                Success = false,
                Message = "BAN timing not found"
            };
        }

        banTiming.UpdateTiming(request.StartTime, request.EndTime, request.Description);
        await _banTimingRepository.UpdateAsync(banTiming);

        return new UpdateBANTimingResponse
        {
            Success = true,
            Message = "BAN timing updated successfully"
        };
    }
}
