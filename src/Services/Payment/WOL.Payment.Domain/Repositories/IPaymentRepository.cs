using WOL.Shared.Common.Application;

namespace WOL.Payment.Domain.Repositories;

public interface IPaymentRepository : IRepository<Entities.Payment>
{
    Task<Entities.Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default);
    Task<Entities.Payment?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Entities.Payment>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
}
