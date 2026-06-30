using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
namespace FosterFlow.Web.Services.HttpServices;

public class ImageUploadService
{
    private readonly IHttpClientFactory _httpFactory;

    public ImageUploadService(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    private HttpClient Http => _httpFactory.CreateClient("API");

    public async Task<(bool Success, string? Error, string? Url)> UploadAsync(IBrowserFile file, long maxAllowedSize)
    {
        using var content = new MultipartFormDataContent();
        await using var stream = file.OpenReadStream(maxAllowedSize);
        using var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "file", file.Name);

        var response = await Http.PostAsync("api/attachments/photo", content);
        if (!response.IsSuccessStatusCode)
        {
            return (false, await ApiErrorHelper.GetFirstErrorAsync(response), null);
        }

        var payload = await response.Content.ReadFromJsonAsync<ImageUploadResponse>();
        if (payload is null || string.IsNullOrWhiteSpace(payload.Url))
        {
            return (false, "The server did not return an image URL.", null);
        }

        return (true, null, payload.Url);
    }
}

public sealed record ImageUploadResponse(string Url);
