using System.Net.Http.Json;
using FosterFlow.Contracts.DTOs.Auth;
using FosterFlow.Web.Authentication;
namespace FosterFlow.Web.Services;

public class AuthService
{
    private readonly AppAuthStateProvider _authState;
    private readonly IHttpClientFactory _httpFactory;
    private readonly TokenStorage _storage;

    public AuthService(
        IHttpClientFactory httpFactory,
        TokenStorage storage,
        AppAuthStateProvider authState)
    {
        _httpFactory = httpFactory;
        _storage = storage;
        _authState = authState;
    }
    private HttpClient Http => _httpFactory.CreateClient("API");


    public async Task<bool> LoginAsync(LoginRequest request)
    {
        var res = await Http.PostAsJsonAsync("api/auth/login", request);
        if (!res.IsSuccessStatusCode)
        {
            return false;
        }

        var data = await res.Content.ReadFromJsonAsync<LoginResponse>();
        if (data is null)
        {
            return false;
        }

        _storage.SetAccessToken(data.AccessToken);
        _authState.NotifyLogin(data.AccessToken);
        return true;
    }

    public async Task<bool> SigninAsync(RegisterUserRequest request)
    {
        var res = await Http.PostAsJsonAsync("api/auth/signin", request);
        if (!res.IsSuccessStatusCode)
        {
            return false;
        }

        var data = await res.Content.ReadFromJsonAsync<LoginResponse>();
        if (data is null)
        {
            return false;
        }

        _storage.SetAccessToken(data.AccessToken);
        _authState.NotifyLogin(data.AccessToken);
        return true;
    }

    public async Task<(bool Success, string? Error)> RegisterShelterAsync(RegisterShelterRequest request)
    {
        var res = await Http.PostAsJsonAsync("api/auth/RegisterShelter", request);
        if (!res.IsSuccessStatusCode)
        {
            return (false, await ApiErrorHelper.GetFirstErrorAsync(res));
        }

        var data = await res.Content.ReadFromJsonAsync<LoginResponse>();
        if (data is null)
        {
            return (false, null);
        }

        _storage.SetAccessToken(data.AccessToken);
        _authState.NotifyLogin(data.AccessToken);
        return (true, null);
    }
    
    public async Task<(bool Success, string? Error)> RegisterFosterAsync(RegisterFosterRequest request)
    {
        var res = await Http.PostAsJsonAsync("api/auth/RegisterFoster", request);
        if (!res.IsSuccessStatusCode)
        {
            return (false, await ApiErrorHelper.GetFirstErrorAsync(res));
        }

        var data = await res.Content.ReadFromJsonAsync<LoginResponse>();
        if (data is null)
        {
            return (false, null);
        }

        _storage.SetAccessToken(data.AccessToken);
        _authState.NotifyLogin(data.AccessToken);
        return (true, null);
    }

    public async Task<bool> RefreshAsync()
    {
        // Cookie is sent automatically by the browser
        var res = await Http.PostAsync("api/auth/refresh", null);
        if (!res.IsSuccessStatusCode)
        {
            return false;
        }

        var data = await res.Content.ReadFromJsonAsync<LoginResponse>();
        if (data is null)
        {
            return false;
        }

        _storage.SetAccessToken(data.AccessToken);
        _authState.NotifyLogin(data.AccessToken);
        return true;
    }

    public async Task LogoutAsync()
    {
        await Http.PostAsync(new Uri("api/auth/logout"), null);
        _storage.ClearAccessToken();
        _authState.NotifyLogout();
    }
}
