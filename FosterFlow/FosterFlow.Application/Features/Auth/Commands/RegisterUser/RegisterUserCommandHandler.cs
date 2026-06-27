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
    private readonly IBusinessMetrics _metrics;
    private readonly IUserRepository _userRepository;
    public RegisterUserCommandHandler(IUserRepository userRepository, IIdentityService identityService, IBusinessMetrics metrics)
    {
        _userRepository = userRepository;
        _identityService = identityService;
        _metrics = metrics;
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

        var role = Enum.Parse<UserRole>(cmd.Request.Role);
        //await _identityService.RegisterAsync(cmd.Request.Email, cmd.Request.Password, $"{cmd.Request.FirstName} {cmd.Request.Name}", role);

        if (role == UserRole.Foster)
        {
            _metrics.IncrementActiveFosters();
        }
    }
}
