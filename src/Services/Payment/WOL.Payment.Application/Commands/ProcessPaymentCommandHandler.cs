using MediatR;
using WOL.Shared.Common.Application;
using WOL.Payment.Domain.Repositories;
using WOL.Payment.Application.Services;

namespace WOL.Payment.Application.Commands;

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, ProcessPaymentResponse>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentGatewayService _paymentGatewayService;

    public ProcessPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork,
        IPaymentGatewayService paymentGatewayService)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
        _paymentGatewayService = paymentGatewayService;
    }

    public async Task<ProcessPaymentResponse> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = Domain.Entities.Payment.Create(
            request.BookingId,
            request.CustomerId,
            request.Amount,
            request.PaymentMethod);

        await _paymentRepository.AddAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var gatewayReference = await _paymentGatewayService.ProcessPaymentAsync(
            payment.TransactionId,
            request.Amount,
            request.PaymentMethod,
            cancellationToken);

        payment.MarkProcessing(gatewayReference);
        payment.MarkCompleted();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProcessPaymentResponse
        {
            PaymentId = payment.Id,
            TransactionId = payment.TransactionId,
            Status = payment.Status,
            Message = "Payment processed successfully"
        };
    }
}
