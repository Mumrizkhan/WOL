using MassTransit;
using WOL.Shared.Messages.Events;
using WOL.ComplianceWorker.Services;

namespace WOL.ComplianceWorker;

public class VehicleRegisteredComplianceConsumer : IConsumer<VehicleRegisteredEvent>
{
    private readonly ILogger<VehicleRegisteredComplianceConsumer> _logger;
    private readonly IComplianceService _complianceService;

    public VehicleRegisteredComplianceConsumer(
        ILogger<VehicleRegisteredComplianceConsumer> logger,
        IComplianceService complianceService)
    {
        _logger = logger;
        _complianceService = complianceService;
    }

    public async Task Consume(ConsumeContext<VehicleRegisteredEvent> context)
    {
        _logger.LogInformation("Processing compliance check for Vehicle {VehicleId}", context.Message.VehicleId);

        var message = context.Message;

        await _complianceService.CheckVehicleComplianceAsync(
            message.VehicleId,
            message.RegistrationNumber,
            message.RegistrationExpiry,
            message.InsuranceExpiry);

        _logger.LogInformation("Compliance check completed for Vehicle {VehicleId}", context.Message.VehicleId);
    }
}
