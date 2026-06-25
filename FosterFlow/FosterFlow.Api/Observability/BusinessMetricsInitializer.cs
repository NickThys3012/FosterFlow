using FosterFlow.Application.Common.Interfaces;
using FosterFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace FosterFlow.Api.Observability;

public static class BusinessMetricsInitializer
{
    /// <summary>
    /// Seeds the <c>fosterflow_active_fosters</c> gauge with the current number of users in the
    /// Foster role so the metric reflects reality on a cold start (US-INF-4.1, #47).
    /// </summary>
    public static async Task InitialiseBusinessMetricsAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var metrics = scope.ServiceProvider.GetRequiredService<IBusinessMetrics>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var fosters = await users.GetUsersInRoleAsync("Foster");
        metrics.SetActiveFosters(fosters.Count);
    }
}
