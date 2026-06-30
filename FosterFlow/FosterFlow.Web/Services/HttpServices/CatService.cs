using System.Net;
using System.Net.Http.Json;
using FosterFlow.Contracts.DTOs.Cats;
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
            return (false, null, null);
        }


        return (true, null, null);
    }
}
