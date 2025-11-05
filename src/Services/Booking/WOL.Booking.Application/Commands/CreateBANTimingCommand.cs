using MediatR;
using WOL.Booking.Domain.Entities;
using WOL.Booking.Domain.Repositories;

namespace WOL.Booking.Application.Commands;

public record CreateBANTimingCommand : IRequest<CreateBANTimingResponse>
{
    public string City { get; init; } = string.Empty;
    public DayOfWeek DayOfWeek { get; init; }
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
    public string? Description { get; init; }
}

public record CreateBANTimingResponse
{
    public bool Success { get; init; }
    public Guid Id { get; init; }
    public string Message { get; init; } = string.Empty;
}

public class CreateBANTimingCommandHandler : IRequestHandler<CreateBANTimingCommand, CreateBANTimingResponse>
{
    private readonly IBANTimingRepository _banTimingRepository;

    public CreateBANTimingCommandHandler(IBANTimingRepository banTimingRepository)
    {
        _banTimingRepository = banTimingRepository;
    }

    public async Task<CreateBANTimingResponse> Handle(CreateBANTimingCommand request, CancellationToken cancellationToken)
    {
        var banTiming = BANTiming.Create(
            request.City,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            request.Description);

        await _banTimingRepository.AddAsync(banTiming);

        return new CreateBANTimingResponse
        {
            Success = true,
            Id = banTiming.Id,
            Message = "BAN timing created successfully"
        };
    }
}
