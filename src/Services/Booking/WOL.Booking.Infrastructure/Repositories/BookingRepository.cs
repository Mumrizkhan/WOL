using Microsoft.EntityFrameworkCore;
using WOL.Booking.Domain.Entities;
using WOL.Booking.Domain.Repositories;
using WOL.Booking.Domain.Enums;
using WOL.Booking.Infrastructure.Data;

namespace WOL.Booking.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly BookingDbContext _context;

    public BookingRepository(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Booking>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Bookings.ToListAsync(cancellationToken);
    }

    public async Task<Domain.Entities.Booking> AddAsync(Domain.Entities.Booking entity, CancellationToken cancellationToken = default)
    {
        await _context.Bookings.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task UpdateAsync(Domain.Entities.Booking entity, CancellationToken cancellationToken = default)
    {
        _context.Bookings.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Domain.Entities.Booking entity, CancellationToken cancellationToken = default)
    {
        _context.Bookings.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<Domain.Entities.Booking?> GetByBookingNumberAsync(string bookingNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .FirstOrDefaultAsync(b => b.BookingNumber == bookingNumber, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Booking>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Where(b => b.CustomerId == customerId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Booking>> GetByDriverIdAsync(Guid driverId, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Where(b => b.DriverId == driverId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Booking>> GetByStatusAsync(BookingStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Where(b => b.Status == status)
            .OrderBy(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Booking>> GetPendingBookingsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Where(b => b.Status == BookingStatus.Pending)
            .OrderBy(b => b.PickupDate)
            .ThenBy(b => b.PickupTime)
            .ToListAsync(cancellationToken);
    }
}
