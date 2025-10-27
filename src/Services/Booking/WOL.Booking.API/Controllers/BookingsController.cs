using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WOL.Booking.Application.Commands;

namespace WOL.Booking.API.Controllers;

[ApiController]
[Route("api/bookings")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(IMediator mediator, ILogger<BookingsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<CreateBookingResponse>> CreateBooking([FromBody] CreateBookingCommand command)
    {
        try
        {
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(CreateBooking), new { id = response.BookingId }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            return BadRequest(new { message = ex.Message });
        }
    }
}
