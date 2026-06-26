using System.Net.Http.Json;
using System.Text.Json;
namespace FosterFlow.Web.Services;

public static class ApiErrorHelper
{
    /// <summary>
    /// Extracts the first validation error string from a 400 ProblemDetails response.
    /// Falls back to a generic message if the body cannot be parsed.
    /// </summary>
    public static async Task<string> GetFirstErrorAsync(HttpResponseMessage response)
    {
        try
        {
            var problem = await response.Content
                .ReadFromJsonAsync<ProblemDetails>();

            if (problem?.Extensions.TryGetValue("errors", out var errObj) == true
                && errObj is JsonElement json)
            {
                foreach (var prop in json.EnumerateObject())
                foreach (var msg  in prop.Value.EnumerateArray())
                    return msg.GetString() ?? "Validation error.";
            }
        }
        catch { /* swallow — fall through to generic */ }

        return "Something went wrong. Please try again.";
    }
}

// Minimal ProblemDetails record (avoids pulling in ASP.NET Core packages to WASM)
public record ProblemDetails(
    string?                          Title,
    int?                             Status,
    Dictionary<string, object?>?     Extensions);
