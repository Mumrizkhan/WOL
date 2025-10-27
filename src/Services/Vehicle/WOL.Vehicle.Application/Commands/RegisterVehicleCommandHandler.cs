using MediatR;
using WOL.Shared.Common.Application;
using WOL.Shared.Common.Exceptions;
using WOL.Vehicle.Domain.Repositories;

namespace WOL.Vehicle.Application.Commands;

public class RegisterVehicleCommandHandler : IRequestHandler<RegisterVehicleCommand, RegisterVehicleResponse>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterVehicleCommandHandler(
        IVehicleRepository vehicleRepository,
        IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RegisterVehicleResponse> Handle(RegisterVehicleCommand request, CancellationToken cancellationToken)
    {
        if (await _vehicleRepository.ExistsByPlateNumberAsync(request.PlateNumber, cancellationToken))
        {
            throw new DomainException($"Vehicle with plate number {request.PlateNumber} already exists");
        }

        var vehicle = Domain.Entities.Vehicle.Create(
            request.OwnerId,
            request.VehicleTypeId,
            request.PlateNumber,
            request.Make,
            request.Model,
            request.Year,
            request.Color);

        await _vehicleRepository.AddAsync(vehicle, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterVehicleResponse
        {
            VehicleId = vehicle.Id,
            PlateNumber = vehicle.PlateNumber,
            Message = "Vehicle registered successfully"
        };
    }
}
