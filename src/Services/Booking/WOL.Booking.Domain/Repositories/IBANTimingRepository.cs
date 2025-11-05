using WOL.Booking.Domain.Entities;

namespace WOL.Booking.Domain.Repositories;

public interface IBANTimingRepository
{
    Task<IEnumerable<BANTiming>> GetByCityAsync(string city, CancellationToken cancellationToken = default);
    Task<IEnumerable<BANTiming>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<BANTiming?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(BANTiming banTiming, CancellationToken cancellationToken = default);
    Task UpdateAsync(BANTiming banTiming, CancellationToken cancellationToken = default);
    Task<bool> IsWithinBANPeriodAsync(string city, DateTime dateTime, CancellationToken cancellationToken = default);
}
