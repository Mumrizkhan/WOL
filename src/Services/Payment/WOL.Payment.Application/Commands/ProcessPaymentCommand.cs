using MediatR;
using WOL.Payment.Domain.Enums;

namespace WOL.Payment.Application.Commands;

public record ProcessPaymentCommand : IRequest<ProcessPaymentResponse>
{
    public Guid BookingId { get; init; }
    public Guid CustomerId { get; init; }
    public decimal Amount { get; init; }
    public PaymentMethod PaymentMethod { get; init; }
}

public record ProcessPaymentResponse
{
    public Guid PaymentId { get; init; }
    public string TransactionId { get; init; } = string.Empty;
    public PaymentStatus Status { get; init; }
    public string Message { get; init; } = string.Empty;
}
