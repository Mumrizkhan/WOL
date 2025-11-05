using Microsoft.EntityFrameworkCore;
using WOL.Booking.Domain.Entities;
using WOL.Booking.Domain.Repositories;
using WOL.Booking.Infrastructure.Data;

namespace WOL.Booking.Infrastructure.Repositories;

public class BANTimingRepository : IBANTimingRepository
{
    private readonly BookingDbContext _context;

    public BANTimingRepository(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BANTiming>> GetByCityAsync(string city, CancellationToken cancellationToken = default)
    {
        return await _context.BANTimings
            .Where(b => b.City == city && b.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BANTiming>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BANTimings
            .Where(b => b.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<BANTiming?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.BANTimings
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task AddAsync(BANTiming banTiming, CancellationToken cancellationToken = default)
    {
        await _context.BANTimings.AddAsync(banTiming, cancellationToken);
    }

    public async Task UpdateAsync(BANTiming banTiming, CancellationToken cancellationToken = default)
    {
        _context.BANTimings.Update(banTiming);
    }

    public async Task<bool> IsWithinBANPeriodAsync(string city, DateTime dateTime, CancellationToken cancellationToken = default)
    {
        var banTimings = await GetByCityAsync(city, cancellationToken);
        return banTimings.Any(b => b.IsWithinBANPeriod(dateTime));
    }
}
