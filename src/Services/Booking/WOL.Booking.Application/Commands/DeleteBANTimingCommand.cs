using MediatR;
using WOL.Booking.Domain.Repositories;

namespace WOL.Booking.Application.Commands;

public record DeleteBANTimingCommand : IRequest<DeleteBANTimingResponse>
{
    public Guid Id { get; init; }
}

public record DeleteBANTimingResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}

public class DeleteBANTimingCommandHandler : IRequestHandler<DeleteBANTimingCommand, DeleteBANTimingResponse>
{
    private readonly IBANTimingRepository _banTimingRepository;

    public DeleteBANTimingCommandHandler(IBANTimingRepository banTimingRepository)
    {
        _banTimingRepository = banTimingRepository;
    }

    public async Task<DeleteBANTimingResponse> Handle(DeleteBANTimingCommand request, CancellationToken cancellationToken)
    {
        var banTiming = await _banTimingRepository.GetByIdAsync(request.Id);

        if (banTiming == null)
        {
            return new DeleteBANTimingResponse
            {
                Success = false,
                Message = "BAN timing not found"
            };
        }

        await _banTimingRepository.DeleteAsync(banTiming);

        return new DeleteBANTimingResponse
        {
            Success = true,
            Message = "BAN timing deleted successfully"
        };
    }
}
