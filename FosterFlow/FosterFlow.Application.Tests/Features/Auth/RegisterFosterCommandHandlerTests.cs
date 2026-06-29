using FosterFlow.Application.Common.Exceptions;
using FosterFlow.Application.Common.Interfaces;
using FosterFlow.Application.Features.Auth.Commands.RegisterFoster;
using FosterFlow.Contracts.DTOs.Auth;
using FosterFlow.Domain.Entities;
using FosterFlow.Domain.Enums;
using FosterFlow.Domain.Interfaces.Repositories;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
namespace FosterFlow.Application.Tests.Features.Auth;

[TestFixture]
public class RegisterFosterCommandHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _identityService = Substitute.For<IIdentityService>();
        _metrics = Substitute.For<IBusinessMetrics>();
        _handler = new RegisterFosterCommandHandler(_identityService, _metrics, _userRepository);
    }

    private IUserRepository _userRepository = null!;
    private IIdentityService _identityService = null!;
    private IBusinessMetrics _metrics = null!;
    private RegisterFosterCommandHandler _handler = null!;

    private static RegisterFosterCommand ValidCommand(string email = "Foster@example.com")
    {
        return new RegisterFosterCommand(new RegisterFosterRequest
        {
            Name = "Happy Paws Foster",
            Email = email,
            Password = "Passw0rd!",
            Phone = "+3212345678",
            Street = "Main Street 1",
            PostalCode = "2000",
            City = "Antwerp",
            Country = "Belgium"
        });
    }

    private static User ExistingUser(string email)
    {
        return new User(Guid.NewGuid(), email, "Existing", UserRole.Foster, "Street", "1000", "City", "Country", null);
    }

    [Test]
    public void Handle_WhenEmailAlreadyExists_ThrowsValidationException()
    {
        var command = ValidCommand();
        _userRepository.GetByEmailAsync(command.Cmd.Email).Returns(ExistingUser(command.Cmd.Email));

        Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_WhenEmailAlreadyExists_ReportsEmailError()
    {
        var command = ValidCommand();
        _userRepository.GetByEmailAsync(command.Cmd.Email).Returns(ExistingUser(command.Cmd.Email));

        var ex = Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.That(ex!.Errors, Does.ContainKey("Email"));
    }

    [Test]
    public async Task Handle_WhenEmailAlreadyExists_DoesNotCallIdentityService()
    {
        var command = ValidCommand();
        _userRepository.GetByEmailAsync(command.Cmd.Email).Returns(ExistingUser(command.Cmd.Email));

        try
        {
            await _handler.Handle(command, CancellationToken.None);
        }
        catch (ValidationException)
        {
            // expected
        }

        await _identityService.DidNotReceiveWithAnyArgs().RegisterFosterAsync(
            null!, null!, null!, null!, null!, null!, null!, null!, null!, default!, default!, false, false, 0, default!, default!);
    }

    [Test]
    public async Task Handle_WhenEmailAlreadyExists_DoesNotIncrementActiveFosters()
    {
        var command = ValidCommand();
        _userRepository.GetByEmailAsync(command.Cmd.Email).Returns(ExistingUser(command.Cmd.Email));

        try
        {
            await _handler.Handle(command, CancellationToken.None);
        }
        catch (ValidationException)
        {
            // expected
        }

        _metrics.DidNotReceive().IncrementActiveFosters();
    }

    [Test]
    public async Task Handle_WithNewEmail_LooksUpUserByRequestEmail()
    {
        var command = ValidCommand();

        await _handler.Handle(command, CancellationToken.None);

        await _userRepository.Received(1).GetByEmailAsync(command.Cmd.Email);
    }

    [Test]
    public async Task Handle_WithNewEmail_CallsRegisterFosterAsyncWithAllRequestValues()
    {
        var command = ValidCommand();

        await _handler.Handle(command, CancellationToken.None);

        await _identityService.Received(1).RegisterFosterAsync(
            command.Cmd.Email,
            command.Cmd.Password,
            command.Cmd.FirstName,
            command.Cmd.Name,
            command.Cmd.Phone,
            command.Cmd.Street,
            command.Cmd.PostalCode,
            command.Cmd.City,
            command.Cmd.Country,
            command.Cmd.ExperienceLevel,
            command.Cmd.HomeType,
            command.Cmd.HasKids,
            command.Cmd.HasDogs,
            command.Cmd.MaxCats,
            command.Cmd.AvailableFrom,
            command.Cmd.AvailableTo);
    }

    [Test]
    public async Task Handle_WithNewEmail_IncrementsActiveFostersExactlyOnce()
    {
        var command = ValidCommand();

        await _handler.Handle(command, CancellationToken.None);

        _metrics.Received(1).IncrementActiveFosters();
    }

    [Test]
    public void Handle_WhenIdentityServiceThrows_PropagatesException()
    {
        var command = ValidCommand();
        _identityService.RegisterFosterAsync(
                Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ExperienceLevel>(), Arg.Any<HomeType>(), Arg.Any<bool>(), Arg.Any<bool>(), Arg.Any<int>(),
                Arg.Any<DateOnly>(), Arg.Any<DateOnly>())
            .ThrowsAsync(new InvalidOperationException("identity failure"));

        Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public async Task Handle_WhenIdentityServiceThrows_DoesNotIncrementActiveFosters()
    {
        var command = ValidCommand();
        _identityService.RegisterFosterAsync(
                Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ExperienceLevel>(), Arg.Any<HomeType>(), Arg.Any<bool>(), Arg.Any<bool>(), Arg.Any<int>(),
                Arg.Any<DateOnly>(), Arg.Any<DateOnly>())
            .ThrowsAsync(new InvalidOperationException("identity failure"));

        try
        {
            await _handler.Handle(command, CancellationToken.None);
        }
        catch (InvalidOperationException)
        {
            // expected
        }

        _metrics.DidNotReceive().IncrementActiveFosters();
    }
}
