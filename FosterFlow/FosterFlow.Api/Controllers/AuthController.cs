using FosterFlow.Application.Features.Auth.Commands.RegisterShelter;
using FosterFlow.Application.Features.Auth.Commands.RegisterUser;
using FosterFlow.Contracts.DTOs.Auth;
using FosterFlow.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LoginRequest=Microsoft.AspNetCore.Identity.Data.LoginRequest;
namespace FosterFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly TokenService _tokens;

    private readonly UserManager<ApplicationUser> _users;

    public AuthController(
        UserManager<ApplicationUser> users,
        TokenService tokens,
        ISender mediator)
    {
        _users = users;
        _tokens = tokens;
        _mediator = mediator;
    }

    // ── POST /api/auth/login ────────────────────────────────────────
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _users.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return Unauthorized("Invalid credentials");
        }

        var valid = await _users.CheckPasswordAsync(user, request.Password);
        if (!valid)
        {
            return Unauthorized("Invalid credentials");
        }

        var roles = await _users.GetRolesAsync(user);
        var (accessToken, expiry) = _tokens.GenerateAccessToken(user, roles);
        var (rawRefresh, _) = await _tokens.GenerateRefreshTokenAsync(user.Id);

        SetRefreshCookie(rawRefresh);

        return Ok(new LoginResponse
        {
            AccessToken = accessToken, Expiration = expiry, Email = user.Email!, Role = roles.FirstOrDefault() ?? "Foster"
        });
    }

    [HttpPost("signin")]
    public async Task<IActionResult> Login(RegisterUserRequest request)
    {
        await _mediator.Send(new RegisterUserCommand(request));
        var user = await _users.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Unauthorized("Invalid credentials");
        }
        var valid = await _users.CheckPasswordAsync(user, request.Password);
        if (!valid)
        {
            return Unauthorized("Invalid credentials");
        }

        var roles = await _users.GetRolesAsync(user);
        var (accessToken, expiry) = _tokens.GenerateAccessToken(user, roles);
        var (rawRefresh, _) = await _tokens.GenerateRefreshTokenAsync(user.Id);

        SetRefreshCookie(rawRefresh);

        return Ok(new LoginResponse
        {
            AccessToken = accessToken, Expiration = expiry, Email = user.Email!, Role = roles.FirstOrDefault() ?? "Foster"
        });
    }
    
    [HttpPost("RegisterShelter")]
    public async Task<IActionResult> RegisterShelter(RegisterShelterRequest request)
    {
        await _mediator.Send(new RegisterShelterCommand(request));
        var user = await _users.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Unauthorized("Invalid credentials");
        }
        var valid = await _users.CheckPasswordAsync(user, request.Password);
        if (!valid)
        {
            return Unauthorized("Invalid credentials");
        }

        var roles = await _users.GetRolesAsync(user);
        var (accessToken, expiry) = _tokens.GenerateAccessToken(user, roles);
        var (rawRefresh, _) = await _tokens.GenerateRefreshTokenAsync(user.Id);

        SetRefreshCookie(rawRefresh);

        return Ok(new LoginResponse
        {
            AccessToken = accessToken, Expiration = expiry, Email = user.Email!, Role = roles.FirstOrDefault() ?? "Foster"
        });
    }

    // ── POST /api/auth/refresh ──────────────────────────────────────
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var raw = Request.Cookies["refreshToken"];
        if (raw is null)
        {
            return Unauthorized();
        }

        var existing = await _tokens.ValidateRefreshTokenAsync(raw);
        if (existing is null)
        {
            return Unauthorized();
        }

        var user = await _users.FindByIdAsync(existing.UserId);
        if (user is null)
        {
            return Unauthorized();
        }

        var roles = await _users.GetRolesAsync(user);
        var (newAccess, expiry) = _tokens.GenerateAccessToken(user, roles);
        var (newRaw, _) = await _tokens.GenerateRefreshTokenAsync(user.Id);

        await _tokens.RevokeTokenAsync(existing, TokenService.HashToken(newRaw));

        SetRefreshCookie(newRaw);

        return Ok(new LoginResponse
        {
            AccessToken = newAccess, Expiration = expiry, Email = user.Email!, Role = roles.FirstOrDefault() ?? "Foster"
        });
    }

    // ── POST /api/auth/logout ───────────────────────────────────────
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var raw = Request.Cookies["refreshToken"];
        if (raw is not null)
        {
            var token = await _tokens.ValidateRefreshTokenAsync(raw);
            if (token is not null)
            {
                await _tokens.RevokeTokenAsync(token);
            }
        }

        Response.Cookies.Delete("refreshToken");
        return Ok();
    }

    // ── Cookie helper ───────────────────────────────────────────────
    private void SetRefreshCookie(string raw)
    {
        Response.Cookies.Append("refreshToken", raw, new CookieOptions
        {
            HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict, Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
    }
}
