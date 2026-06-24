using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FosterFlow.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
namespace FosterFlow.Web.Authentication;

public class AppAuthStateProvider : AuthenticationStateProvider
{
    private readonly TokenStorage _storage;

    public AppAuthStateProvider(TokenStorage storage)
    {
        _storage = storage;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = _storage.GetAccessToken();
        if (string.IsNullOrEmpty(token))
        {
            return Unauthenticated();
        }

        // Validate expiry client-side (defence-in-depth)
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        if (jwt.ValidTo < DateTime.UtcNow)
        {
            return Unauthenticated();
        }

        var identity = new ClaimsIdentity(jwt.Claims, "jwt");
        return Task.FromResult(
            new AuthenticationState(new ClaimsPrincipal(identity)));
    }

    public void NotifyLogin(string accessToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(accessToken);
        var identity = new ClaimsIdentity(jwt.Claims, "jwt");
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(
                new ClaimsPrincipal(identity))));
    }

    public void NotifyLogout()
    {
        NotifyAuthenticationStateChanged(Unauthenticated());
    }

    private static Task<AuthenticationState> Unauthenticated()
    {
        return Task.FromResult(
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
    }
}
