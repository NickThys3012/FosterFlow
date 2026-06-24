using FluentValidation.Results;
using FosterFlow.Application.Common.Exceptions;
using FosterFlow.Application.Common.Interfaces;
using FosterFlow.Domain.Enums;
using FosterFlow.Domain.Interfaces.Repositories;
using MediatR;
namespace FosterFlow.Application.Features.Auth.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand>
{
    private readonly IIdentityService _identityService;
    private readonly IUserRepository _userRepository;
    public RegisterUserCommandHandler(IUserRepository userRepository, IIdentityService identityService)
    {
        _userRepository = userRepository;
        _identityService = identityService;
    }

    public async Task Handle(RegisterUserCommand cmd, CancellationToken cancellationToken)
    {
        var existing = await _userRepository.GetByEmailAsync(cmd.Request.Email);
        if (existing is not null)
        {
            throw new ValidationException([
                new ValidationFailure("Email", "Email already exists")
            ]);
        }

        await _identityService.RegisterAsync(cmd.Request.Email, cmd.Request.Password, $"{cmd.Request.FirstName} {cmd.Request.Name}", Enum.Parse<UserRole>(cmd.Request.Role));
    }
}
