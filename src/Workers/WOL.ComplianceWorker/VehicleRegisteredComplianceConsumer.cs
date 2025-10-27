using MassTransit;
using WOL.Shared.Messages.Events;

namespace WOL.ComplianceWorker;

public class VehicleRegisteredComplianceConsumer : IConsumer<VehicleRegisteredEvent>
{
    private readonly ILogger<VehicleRegisteredComplianceConsumer> _logger;

    public VehicleRegisteredComplianceConsumer(ILogger<VehicleRegisteredComplianceConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<VehicleRegisteredEvent> context)
    {
        _logger.LogInformation("Processing compliance check for VehicleRegisteredEvent: {VehicleId}", context.Message.VehicleId);

        await Task.Delay(150);

        _logger.LogInformation("Compliance check completed for Vehicle {VehicleId}", context.Message.VehicleId);
    }
}
