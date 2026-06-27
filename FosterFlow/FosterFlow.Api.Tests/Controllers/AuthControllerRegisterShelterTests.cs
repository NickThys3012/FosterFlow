using FosterFlow.Api.Controllers;
using FosterFlow.Application.Common.Exceptions;
using FosterFlow.Application.Features.Auth.Commands.RegisterShelter;
using FosterFlow.Contracts.DTOs.Auth;
using FosterFlow.Infrastructure.Identity;
using FosterFlow.Infrastructure.Persistence;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace FosterFlow.Api.Tests.Controllers;

[TestFixture]
public class AuthControllerRegisterShelterTests
{
    [SetUp]
    public void SetUp()
    {
        _mediator = Substitute.For<ISender>();
        _users = CreateUserManagerSubstitute();
        _tokens = CreateTokenService();

        _controller = new AuthController(_users, _tokens, _mediator)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [TearDown]
    public void TearDown()
    {
        _users?.Dispose();
        _dbContext?.Dispose();
    }

    private ISender _mediator = null!;
    private UserManager<ApplicationUser> _users = null!;
    private TokenService _tokens = null!;
    private AppDbContext _dbContext = null!;
    private AuthController _controller = null!;

    private static RegisterShelterRequest ValidRequest(string email = "shelter@example.com")
    {
        return new RegisterShelterRequest
        {
            Name = "Happy Paws Shelter",
            Email = email,
            Password = "Passw0rd!",
            Phone = "+3212345678",
            Street = "Main Street 1",
            PostalCode = "2000",
            City = "Antwerp",
            Country = "Belgium"
        };
    }

    private static ApplicationUser ShelterUser(string email)
    {
        return new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = email,
            UserName = email,
            Name = "Happy Paws Shelter"
        };
    }

    private static UserManager<ApplicationUser> CreateUserManagerSubstitute()
    {
        var store = Substitute.For<IUserStore<ApplicationUser>>();
        return Substitute.For<UserManager<ApplicationUser>>(
            store, null, null, null, null, null, null, null, null);
    }

    private TokenService CreateTokenService()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"auth-tests-{Guid.NewGuid()}")
            .Options;
        _dbContext = new AppDbContext(options);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = "super-secret-signing-key-for-tests-1234567890",
                ["Jwt:Issuer"] = "fosterflow-tests",
                ["Jwt:Audience"] = "fosterflow-tests",
                ["Jwt:AccessTokenMinutes"] = "15",
                ["Jwt:RefreshTokenDays"] = "7"
            })
            .Build();

        return new TokenService(config, _dbContext);
    }

    [Test]
    public async Task RegisterShelter_WithValidRequest_SendsRegisterShelterCommand()
    {
        var request = ValidRequest();
        _users.FindByEmailAsync(request.Email).Returns(ShelterUser(request.Email));
        _users.CheckPasswordAsync(Arg.Any<ApplicationUser>(), request.Password).Returns(true);
        _users.GetRolesAsync(Arg.Any<ApplicationUser>()).Returns(new List<string> { "Shelter" });

        await _controller.RegisterShelter(request);

        await _mediator.Received(1).Send(
            Arg.Is<RegisterShelterCommand>(c => c.Request == request),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task RegisterShelter_WithValidRequest_ReturnsOk()
    {
        var request = ValidRequest();
        _users.FindByEmailAsync(request.Email).Returns(ShelterUser(request.Email));
        _users.CheckPasswordAsync(Arg.Any<ApplicationUser>(), request.Password).Returns(true);
        _users.GetRolesAsync(Arg.Any<ApplicationUser>()).Returns(new List<string> { "Shelter" });

        var result = await _controller.RegisterShelter(request);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task RegisterShelter_WithValidRequest_ReturnsLoginResponseWithTokenAndRole()
    {
        var request = ValidRequest();
        _users.FindByEmailAsync(request.Email).Returns(ShelterUser(request.Email));
        _users.CheckPasswordAsync(Arg.Any<ApplicationUser>(), request.Password).Returns(true);
        _users.GetRolesAsync(Arg.Any<ApplicationUser>()).Returns(new List<string> { "Shelter" });

        var result = await _controller.RegisterShelter(request) as OkObjectResult;
        var body = result!.Value as LoginResponse;

        Assert.That(body, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(body!.Email, Is.EqualTo(request.Email));
            Assert.That(body.Role, Is.EqualTo("Shelter"));
            Assert.That(body.AccessToken, Is.Not.Empty);
        });
    }

    [Test]
    public async Task RegisterShelter_WhenUserHasNoRoles_DefaultsRoleToShelter()
    {
        var request = ValidRequest();
        _users.FindByEmailAsync(request.Email).Returns(ShelterUser(request.Email));
        _users.CheckPasswordAsync(Arg.Any<ApplicationUser>(), request.Password).Returns(true);
        _users.GetRolesAsync(Arg.Any<ApplicationUser>()).Returns(new List<string>());

        var result = await _controller.RegisterShelter(request) as OkObjectResult;
        var body = result!.Value as LoginResponse;

        Assert.That(body!.Role, Is.EqualTo("Shelter"));
    }

    [Test]
    public async Task RegisterShelter_OnSuccess_SetsRefreshTokenCookie()
    {
        var request = ValidRequest();
        _users.FindByEmailAsync(request.Email).Returns(ShelterUser(request.Email));
        _users.CheckPasswordAsync(Arg.Any<ApplicationUser>(), request.Password).Returns(true);
        _users.GetRolesAsync(Arg.Any<ApplicationUser>()).Returns(new List<string> { "Shelter" });

        await _controller.RegisterShelter(request);

        var setCookie = _controller.Response.Headers.SetCookie.ToString();
        Assert.That(setCookie, Does.Contain("refreshToken"));
    }

    [Test]
    public async Task RegisterShelter_WhenUserNotFoundAfterRegistration_ReturnsUnauthorized()
    {
        var request = ValidRequest();
        _users.FindByEmailAsync(request.Email).Returns((ApplicationUser?)null);

        var result = await _controller.RegisterShelter(request);

        Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
    }

    [Test]
    public async Task RegisterShelter_WhenPasswordCheckFails_ReturnsUnauthorized()
    {
        var request = ValidRequest();
        _users.FindByEmailAsync(request.Email).Returns(ShelterUser(request.Email));
        _users.CheckPasswordAsync(Arg.Any<ApplicationUser>(), request.Password).Returns(false);

        var result = await _controller.RegisterShelter(request);

        Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
    }

    [Test]
    public void RegisterShelter_WhenMediatorThrowsValidationException_PropagatesException()
    {
        var request = ValidRequest();
        _mediator.Send(Arg.Any<RegisterShelterCommand>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new ValidationException([new ValidationFailure("Email", "Email already exists")]));

        Assert.ThrowsAsync<ValidationException>(() => _controller.RegisterShelter(request));
    }

    [Test]
    public async Task RegisterShelter_WhenMediatorThrows_DoesNotLookUpUser()
    {
        var request = ValidRequest();
        _mediator.Send(Arg.Any<RegisterShelterCommand>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new ValidationException([new ValidationFailure("Email", "Email already exists")]));

        try
        {
            await _controller.RegisterShelter(request);
        }
        catch (ValidationException)
        {
            // expected
        }

        await _users.DidNotReceive().FindByEmailAsync(Arg.Any<string>());
    }
}
