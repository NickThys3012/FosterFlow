using System.Security.Claims;
using FosterFlow.Application.Common.Interfaces;
using FosterFlow.Domain.Entities;
using FosterFlow.Domain.Interfaces.Repositories;
namespace FosterFlow.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _http;
    private readonly IUserRepository _users;

    public CurrentUserService(IHttpContextAccessor http, IUserRepository users)
    {
        _http = http;
        _users = users;
    }

    public Guid? UserId
    {
        get
        {
            var raw = _http.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(raw, out var id) ? id : null;
        }
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        if (UserId is null)
        {
            return null;
        }
        return await _users.GetByIdAsync(UserId.Value);
    }
}
