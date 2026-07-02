using System.Net.Http.Json;
namespace FosterFlow.Web.Services;

public static class ApiErrorHelper
{
    /// <summary>
    ///     Extracts the first validation error string from a ProblemDetails response
    ///     (e.g. a 422 from the API with an <c>errors</c> dictionary).
    ///     Falls back to a generic message if the body cannot be parsed.
    /// </summary>
    public static async Task<string> GetFirstErrorAsync(HttpResponseMessage response)
    {
        try
        {
            var problem = await response.Content
                .ReadFromJsonAsync<ProblemDetails>();

            if (problem?.Errors is {} errors)
            {
                foreach (var msg in errors.Values.SelectMany(messages => messages))
                {
                    return msg;
                }
            }
        }
        catch
        { /* swallow — fall through to generic */
        }

        return "Something went wrong. Please try again.";
    }
}

// Minimal ProblemDetails record (avoids pulling in ASP.NET Core packages to WASM).
// Matches the shape written by the API's ExceptionHandlingMiddleware: { title, status, errors }.
public record ProblemDetails(
    string? Title,
    int? Status,
    Dictionary<string, string[]>? Errors);
