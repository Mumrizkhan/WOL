using MediatR;
using WOL.Booking.Domain.Repositories;

namespace WOL.Booking.Application.Queries;

public record GetAllBANTimingsQuery : IRequest<GetAllBANTimingsResponse>;

public record GetAllBANTimingsResponse
{
    public List<BANTimingDto> BANTimings { get; init; } = new();
}

public record BANTimingDto
{
    public Guid Id { get; init; }
    public string City { get; init; } = string.Empty;
    public DayOfWeek DayOfWeek { get; init; }
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; }
}

public class GetAllBANTimingsQueryHandler : IRequestHandler<GetAllBANTimingsQuery, GetAllBANTimingsResponse>
{
    private readonly IBANTimingRepository _banTimingRepository;

    public GetAllBANTimingsQueryHandler(IBANTimingRepository banTimingRepository)
    {
        _banTimingRepository = banTimingRepository;
    }

    public async Task<GetAllBANTimingsResponse> Handle(GetAllBANTimingsQuery request, CancellationToken cancellationToken)
    {
        var banTimings = await _banTimingRepository.GetAllAsync();

        var dtos = banTimings.Select(b => new BANTimingDto
        {
            Id = b.Id,
            City = b.City,
            DayOfWeek = b.DayOfWeek,
            StartTime = b.StartTime,
            EndTime = b.EndTime,
            Description = b.Description,
            IsActive = b.IsActive
        }).ToList();

        return new GetAllBANTimingsResponse
        {
            BANTimings = dtos
        };
    }
}
