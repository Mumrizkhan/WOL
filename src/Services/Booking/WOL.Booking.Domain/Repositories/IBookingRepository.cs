using WOL.Shared.Common.Application;
using WOL.Booking.Domain.Entities;
using WOL.Booking.Domain.Enums;

namespace WOL.Booking.Domain.Repositories;

public interface IBookingRepository : IRepository<Booking>
{
    Task<Booking?> GetByBookingNumberAsync(string bookingNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>> GetByDriverIdAsync(Guid driverId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>> GetByStatusAsync(BookingStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>> GetPendingBookingsAsync(CancellationToken cancellationToken = default);
}
