using Microsoft.EntityFrameworkCore;
using WOL.Booking.Domain.Entities;
using WOL.Booking.Domain.Repositories;
using WOL.Booking.Infrastructure.Data;

namespace WOL.Booking.Infrastructure.Repositories;

public class SharedLoadBookingRepository : ISharedLoadBookingRepository
{
    private readonly BookingDbContext _context;

    public SharedLoadBookingRepository(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<SharedLoadBooking?> GetByIdAsync(Guid id)
    {
        return await _context.SharedLoadBookings.FindAsync(id);
    }

    public async Task<IEnumerable<SharedLoadBooking>> GetOpenPoolsAsync(
        string originCity, 
        string destinationCity, 
        DateTime pickupDate, 
        string vehicleType)
    {
        var startOfDay = pickupDate.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _context.SharedLoadBookings
            .Where(s => 
                s.OriginCity == originCity &&
                s.DestinationCity == destinationCity &&
                s.PickupDate >= startOfDay &&
                s.PickupDate < endOfDay &&
                s.VehicleType == vehicleType &&
                s.Status == "Open")
            .ToListAsync();
    }

    public async Task<IEnumerable<SharedLoadBooking>> GetAllAsync()
    {
        return await _context.SharedLoadBookings.ToListAsync();
    }

    public async Task AddAsync(SharedLoadBooking sharedLoadBooking)
    {
        await _context.SharedLoadBookings.AddAsync(sharedLoadBooking);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(SharedLoadBooking sharedLoadBooking)
    {
        _context.SharedLoadBookings.Update(sharedLoadBooking);
        await _context.SaveChangesAsync();
    }
}
