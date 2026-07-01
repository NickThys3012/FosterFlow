using System.Net;
using System.Net.Http.Json;
using FosterFlow.Contracts.DTOs.Cats;
using FosterFlow.Contracts.DTOs.Cats.GetAllCats;
namespace FosterFlow.Web.Services.HttpServices;

public class CatService
{
    private readonly IHttpClientFactory _httpFactory;

    public CatService(
        IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }
    private HttpClient Http => _httpFactory.CreateClient("API");

    public async Task<CatDto?> GetCatByIdAsync(Guid id)
    {
        var res = await Http.GetAsync($"api/cats/{id}");
        if (!res.IsSuccessStatusCode)
        {
            return null;
        }

        var data = await res.Content.ReadFromJsonAsync<CatDto>();
        return data;
    }

    public async Task<GetAllCatsResponse?> GetAllCatsAsync()
    {
        var res = await Http.GetAsync("api/cats");
        if (!res.IsSuccessStatusCode)
        {
            return null;
        }

        var data = await res.Content.ReadFromJsonAsync<GetAllCatsResponse>();
        return data;
    }
    public async Task<(bool Success, string? Error, Guid? catId)> CreateCatAsync(CreateCatRequest request)
    {
        var res = await Http.PostAsJsonAsync("api/cats", request);
        if (!res.IsSuccessStatusCode)
        {
            var error = res.StatusCode == HttpStatusCode.Unauthorized
                ? "You are not authorized to create cat listings."
                : await ApiErrorHelper.GetFirstErrorAsync(res);
            return (false, error, null);
        }

        var data = await res.Content.ReadFromJsonAsync<CreateCatResponse>();
        if (data is null)
        {
            return (false, "The server did not return a cat id.", null);
        }


        return (true, null, data.Id);
    }
}
