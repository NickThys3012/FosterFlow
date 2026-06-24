using FluentValidation.Results;
using FosterFlow.Application.Common.Exceptions;
using FosterFlow.Application.Features.Auth.Commands.RegisterUser;
using FosterFlow.Domain.Enums;
using FosterFlow.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
namespace FosterFlow.Infrastructure.Features.Auth;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RegisterUserCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task Handle(RegisterUserCommand cmd, CancellationToken cancellationToken)
    {
        var existing = await _userManager.FindByEmailAsync(cmd.Request.Email);
        if (existing is not null)
        {
            throw new ValidationException([
                new ValidationFailure("Email", "Email already exists")
            ]);
        }

        var appUser = new ApplicationUser
        {
            UserName = cmd.Request.Email, Email = cmd.Request.Email, DisplayName = cmd.Request.Name, Role = Enum.Parse<UserRole>(cmd.Request.Role)
        };

        var result = await _userManager.CreateAsync(appUser, cmd.Request.Password);
        if (!result.Succeeded)
        {
            throw new ProcessingException(nameof(ApplicationUser), appUser.Id);
        }

        await _userManager.AddToRoleAsync(appUser, cmd.Request.Role);
    }
}
