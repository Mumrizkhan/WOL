using Microsoft.EntityFrameworkCore;
using WOL.Payment.Domain.Repositories;
using WOL.Payment.Infrastructure.Data;

namespace WOL.Payment.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly PaymentDbContext _context;

    public PaymentRepository(PaymentDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Payments.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Payment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Payments.ToListAsync(cancellationToken);
    }

    public async Task<Domain.Entities.Payment> AddAsync(Domain.Entities.Payment entity, CancellationToken cancellationToken = default)
    {
        await _context.Payments.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task UpdateAsync(Domain.Entities.Payment entity, CancellationToken cancellationToken = default)
    {
        _context.Payments.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Domain.Entities.Payment entity, CancellationToken cancellationToken = default)
    {
        _context.Payments.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<Domain.Entities.Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(p => p.TransactionId == transactionId, cancellationToken);
    }

    public async Task<Domain.Entities.Payment?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(p => p.BookingId == bookingId, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Payment>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Where(p => p.CustomerId == customerId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
