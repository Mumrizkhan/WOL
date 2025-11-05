using MediatR;
using WOL.Booking.Domain.Repositories;

namespace WOL.Booking.Application.Queries;

public record GetBANTimingsByCityQuery : IRequest<GetBANTimingsByCityResponse>
{
    public string City { get; init; } = string.Empty;
}

public record GetBANTimingsByCityResponse
{
    public List<BANTimingDto> BANTimings { get; init; } = new();
}

public class GetBANTimingsByCityQueryHandler : IRequestHandler<GetBANTimingsByCityQuery, GetBANTimingsByCityResponse>
{
    private readonly IBANTimingRepository _banTimingRepository;

    public GetBANTimingsByCityQueryHandler(IBANTimingRepository banTimingRepository)
    {
        _banTimingRepository = banTimingRepository;
    }

    public async Task<GetBANTimingsByCityResponse> Handle(GetBANTimingsByCityQuery request, CancellationToken cancellationToken)
    {
        var banTimings = await _banTimingRepository.GetByCityAsync(request.City);

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

        return new GetBANTimingsByCityResponse
        {
            BANTimings = dtos
        };
    }
}
