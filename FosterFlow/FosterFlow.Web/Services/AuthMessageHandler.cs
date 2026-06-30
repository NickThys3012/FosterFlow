using System.Net;
using System.Net.Http.Headers;
using FosterFlow.Web.Services.HttpServices;
namespace FosterFlow.Web.Services;

public class AuthMessageHandler : DelegatingHandler
{
    private readonly AuthService _auth;
    private readonly TokenStorage _storage;

    public AuthMessageHandler(TokenStorage storage, AuthService auth)
    {
        _storage = storage;
        _auth = auth;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Never intercept auth endpoints — would cause infinite loop
        if (IsAuthEndpoint(request.RequestUri))
        {
            return await base.SendAsync(request, cancellationToken);
        }

        AttachToken(request);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return response;
        }

        var refreshed = await _auth.RefreshAsync();
        if (!refreshed)
        {
            return response;
        }

        var retry = CloneRequest(request);
        AttachToken(retry);
        return await base.SendAsync(retry, cancellationToken);
    }

    private static bool IsAuthEndpoint(Uri? uri)
    {
        if (uri is null)
        {
            return false;
        }
        var path = uri.AbsolutePath.ToLowerInvariant();
        return path.Contains("/api/auth/");
    }

    private void AttachToken(HttpRequestMessage request)
    {
        var token = _storage.GetAccessToken();
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    private static HttpRequestMessage CloneRequest(HttpRequestMessage req)
    {
        var clone = new HttpRequestMessage(req.Method, req.RequestUri);
        foreach (var h in req.Headers)
        {
            clone.Headers.TryAddWithoutValidation(h.Key, h.Value);
        }
        return clone;
    }
}
