using MassTransit;
using Microsoft.Extensions.Logging;
using WOL.Backload.Application.Commands;
using WOL.Shared.Messages.Events;
using MediatR;

namespace WOL.BackloadWorker;

public class BookingCompletedConsumer : IConsumer<BookingCompletedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<BookingCompletedConsumer> _logger;

    public BookingCompletedConsumer(
        IMediator mediator,
        ILogger<BookingCompletedConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BookingCompletedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Generating load recommendations for Driver {DriverId} after completing booking {BookingId}", 
            message.DriverId, message.BookingId);

        try
        {
            var command = new GenerateLoadRecommendationsCommand
            {
                DriverId = message.DriverId,
                CurrentCity = message.OriginCity,
                DestinationCity = message.DestinationCity,
                CompletionTime = message.CompletedAt
            };

            var result = await _mediator.Send(command);

            if (result.Success && result.RecommendationCount > 0)
            {
                _logger.LogInformation("Generated {Count} load recommendations for Driver {DriverId}", 
                    result.RecommendationCount, message.DriverId);
            }
            else
            {
                _logger.LogInformation("No suitable load recommendations found for Driver {DriverId}", 
                    message.DriverId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating load recommendations for Driver {DriverId}", 
                message.DriverId);
            throw;
        }
    }
}
