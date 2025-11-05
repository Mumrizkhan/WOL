using WOL.Booking.Domain.Entities;

namespace WOL.Booking.Domain.Repositories;

public interface ISharedLoadBookingRepository
{
    Task<SharedLoadBooking?> GetByIdAsync(Guid id);
    Task<IEnumerable<SharedLoadBooking>> GetOpenPoolsAsync(string originCity, string destinationCity, DateTime pickupDate, string vehicleType);
    Task<IEnumerable<SharedLoadBooking>> GetAllAsync();
    Task AddAsync(SharedLoadBooking sharedLoadBooking);
    Task UpdateAsync(SharedLoadBooking sharedLoadBooking);
}
