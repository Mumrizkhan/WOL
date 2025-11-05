using Hangfire;
using MassTransit;
using Microsoft.Extensions.Logging;
using WOL.Booking.Domain.Repositories;
using WOL.Shared.Messages.Events;

namespace WOL.BookingWorker.Jobs;

public class DriverAssignmentTimeoutJob
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<DriverAssignmentTimeoutJob> _logger;

    public DriverAssignmentTimeoutJob(
        IBookingRepository bookingRepository,
        IPublishEndpoint publishEndpoint,
        ILogger<DriverAssignmentTimeoutJob> logger)
    {
        _bookingRepository = bookingRepository;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task Execute()
    {
        _logger.LogInformation("Starting driver assignment timeout check job at {Time}", DateTime.UtcNow);

        try
        {
            var allBookings = await _bookingRepository.GetAllAsync();
            var paidBookingsWithoutDriver = allBookings
                .Where(b => 
                    b.Status == "Pending" && 
                    b.PaymentStatus == "Paid" &&
                    !b.DriverId.HasValue &&
                    b.CreatedAt.AddMinutes(30) <= DateTime.UtcNow)
                .ToList();

            _logger.LogInformation("Found {Count} bookings without driver assignment after 30 minutes", paidBookingsWithoutDriver.Count);

            foreach (var booking in paidBookingsWithoutDriver)
            {
                booking.Cancel("No driver assigned within 30 minutes - automatic cancellation");
                await _bookingRepository.UpdateAsync(booking);

                await _publishEndpoint.Publish(new AutoRefundInitiatedEvent
                {
                    BookingId = booking.Id,
                    CustomerId = booking.CustomerId,
                    Amount = booking.TotalPrice,
                    Reason = "No driver assigned within 30 minutes",
                    Timestamp = DateTime.UtcNow
                });

                _logger.LogWarning(
                    "Published AutoRefundInitiatedEvent for booking {BookingId}, customer {CustomerId}, amount {Amount}",
                    booking.Id,
                    booking.CustomerId,
                    booking.TotalPrice);
            }

            _logger.LogInformation("Driver assignment timeout check job completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing driver assignment timeout check job");
            throw;
        }
    }
}
