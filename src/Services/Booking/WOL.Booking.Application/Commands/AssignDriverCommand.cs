using MediatR;
using MassTransit;
using WOL.Booking.Domain.Repositories;
using WOL.Compliance.Application.Services;
using WOL.Shared.Messages.Events;

namespace WOL.Booking.Application.Commands;

public record AssignDriverCommand : IRequest<AssignDriverResponse>
{
    public Guid BookingId { get; init; }
    public Guid DriverId { get; init; }
    public Guid VehicleId { get; init; }
}

public record AssignDriverResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public ComplianceCheckResult? ComplianceResult { get; init; }
}

public class AssignDriverCommandHandler : IRequestHandler<AssignDriverCommand, AssignDriverResponse>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ComplianceCheckService _complianceCheckService;
    private readonly IPublishEndpoint _publishEndpoint;

    public AssignDriverCommandHandler(
        IBookingRepository bookingRepository,
        ComplianceCheckService complianceCheckService,
        IPublishEndpoint publishEndpoint)
    {
        _bookingRepository = bookingRepository;
        _complianceCheckService = complianceCheckService;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<AssignDriverResponse> Handle(AssignDriverCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId);

        if (booking == null)
        {
            return new AssignDriverResponse
            {
                Success = false,
                Message = "Booking not found"
            };
        }

        var complianceResult = await _complianceCheckService.CheckBookingComplianceAsync(
            request.DriverId, 
            request.VehicleId);

        if (!complianceResult.IsCompliant)
        {
            await _publishEndpoint.Publish(new ComplianceCheckFailedEvent
            {
                BookingId = request.BookingId,
                DriverId = request.DriverId,
                VehicleId = request.VehicleId,
                ExpiredDocuments = complianceResult.ExpiredDocuments,
                MissingDocuments = complianceResult.MissingDocuments,
                Reason = complianceResult.Message,
                Timestamp = DateTime.UtcNow
            }, cancellationToken);

            return new AssignDriverResponse
            {
                Success = false,
                Message = $"Cannot assign driver: {complianceResult.Message}",
                ComplianceResult = complianceResult
            };
        }

        booking.AssignDriver(request.DriverId, request.VehicleId);
        await _bookingRepository.UpdateAsync(booking);

        await _publishEndpoint.Publish(new BookingAssignedEvent
        {
            BookingId = booking.Id,
            DriverId = request.DriverId,
            VehicleId = request.VehicleId,
            CustomerId = booking.CustomerId,
            AssignedAt = DateTime.UtcNow
        }, cancellationToken);

        return new AssignDriverResponse
        {
            Success = true,
            Message = "Driver assigned successfully",
            ComplianceResult = complianceResult
        };
    }
}
