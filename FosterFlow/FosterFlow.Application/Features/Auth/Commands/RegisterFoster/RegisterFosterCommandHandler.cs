using FluentValidation.Results;
using FosterFlow.Application.Common.Exceptions;
using FosterFlow.Application.Common.Interfaces;
using FosterFlow.Domain.Interfaces.Repositories;
using MediatR;
namespace FosterFlow.Application.Features.Auth.Commands.RegisterFoster;

public class RegisterFosterCommandHandler : IRequestHandler<RegisterFosterCommand>
{

    private readonly IIdentityService _identityService;
    private readonly IBusinessMetrics _metrics;
    private readonly IUserRepository _userRepository;

    public RegisterFosterCommandHandler(IIdentityService identityService, IBusinessMetrics metrics, IUserRepository userRepository)
    {
        _identityService = identityService;
        _metrics = metrics;
        _userRepository = userRepository;
    }

    public async Task Handle(RegisterFosterCommand request, CancellationToken cancellationToken)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Cmd.Email);
        if (existing is not null)
        {
            throw new ValidationException([
                new ValidationFailure("Email", "Email already exists")
            ]);
        }

        await _identityService.RegisterFosterAsync(request.Cmd.Email, request.Cmd.Password, request.Cmd.Name, request.Cmd.Phone!, request.Cmd.Street!, request.Cmd.PostalCode!,
            request.Cmd.City!, request.Cmd.Country!, request.Cmd.ExperienceLevel, request.Cmd.HomeType, request.Cmd.HasKids, request.Cmd.HasDogs, request.Cmd.MaxCats, request.Cmd.AvailableFrom,
            request.Cmd.AvailableTo);

        _metrics.IncrementActiveFosters();
    }
}
