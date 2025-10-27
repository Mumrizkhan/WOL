using MediatR;
using WOL.Shared.Common.Application;
using WOL.Shared.Common.Exceptions;
using WOL.Identity.Domain.Entities;
using WOL.Identity.Domain.Repositories;
using WOL.Identity.Application.Services;

namespace WOL.Identity.Application.Commands;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByMobileNumberAsync(request.MobileNumber, cancellationToken))
        {
            throw new DomainException($"User with mobile number {request.MobileNumber} already exists");
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = User.Create(
            request.UserType,
            request.MobileNumber,
            passwordHash,
            request.FirstName,
            request.LastName,
            request.Email,
            request.IqamaNumber,
            request.CompanyName);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterUserResponse
        {
            UserId = user.Id,
            MobileNumber = user.MobileNumber,
            Message = "User registered successfully. Please verify your mobile number."
        };
    }
}
