using System.Security.Claims;
using System.Text;
using FosterFlow.Api.Middleware;
using FosterFlow.Api.Observability;
using FosterFlow.Application;
using FosterFlow.Application.Common.Interfaces;
using FosterFlow.Infrastructure;
using FosterFlow.Infrastructure.Identity;
using FosterFlow.Infrastructure.Persistence;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

// ── Bootstrap logger ──────────────────────────────────────────────────
// A minimal logger that captures anything thrown while the host is being built,
// before the fully configured Serilog pipeline takes over.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // ── Structured logging (US-INF-4.2, #48) ──────────────────────────
    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId();

        var lokiUrl = context.Configuration["Loki:Url"];
        if (string.IsNullOrWhiteSpace(lokiUrl))
        {
            return;
        }
        var environment = context.Configuration["Loki:Environment"]
            ?? context.HostingEnvironment.EnvironmentName.ToLowerInvariant();

        configuration.WriteTo.GrafanaLoki(
            lokiUrl,
            [
                new LokiLabel
                {
                    Key = "app", Value = "fosterflow-api"
                },
                new LokiLabel
                {
                    Key = "environment", Value = environment
                }
            ]);
    });

    // ── Layer registrations ───────────────────────────────────────────
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // ── Metrics (US-INF-4.1, #47) ─────────────────────────────────────
    builder.Services.AddSingleton<IBusinessMetrics, PrometheusBusinessMetrics>();

    // ── Identity ──────────────────────────────────────────────────────
    builder.Services
        .AddIdentity<ApplicationUser, IdentityRole>(opts =>
        {
            opts.Password.RequireDigit = true;
            opts.Password.RequiredLength = 8;
            opts.Password.RequireNonAlphanumeric = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

    // ── JWT ───────────────────────────────────────────────────────────
    builder.Services
        .AddAuthentication(opts =>
        {
            opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(opts =>
        {
            // Ensure JWT "role"/"nameid" claims are mapped to ClaimTypes so
            // [Authorize(Roles=...)] and CurrentUserService continue to work.
            opts.MapInboundClaims = true;
            opts.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
                ClockSkew = TimeSpan.Zero
            };
        });

    var conStr = builder.Configuration.GetConnectionString("Database");
    if (string.IsNullOrEmpty(conStr))
    {
        throw new InvalidOperationException(
            "Could not find a connection string named 'DefaultConnection'.");
    }
    builder.Services.AddHealthChecks()
        .AddSqlServer(conStr)
        .AddDbContextCheck<AppDbContext>();

    builder.Services.AddHsts(options =>
    {
        options.MaxAge = TimeSpan.FromDays(365); // 31536000 seconds — matches your AC
        options.IncludeSubDomains = true;
        options.Preload = false; // Don't set true unless you're submitting to the HSTS preload list
    });

    builder.Services.AddAuthorization();
    builder.Services.AddScoped<TokenService>();

    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    var app = builder.Build();
    await app.Services.MigrateDatabaseAsync();
    await app.Services.SeedUsers();
    await app.Services.InitialiseBusinessMetricsAsync();

    // ── Middleware pipeline ───────────────────────────────────────────
    app.UseMiddleware<ExceptionHandlingMiddleware>(); // ← must be first
    app.UseSerilogRequestLogging();                   // HTTP request logging (#48)
    app.UseHttpsRedirection();
    app.UseHsts(); // Only sends the header over HTTPS — correct behaviour
    app.UseStaticFiles();

    // Serve the Blazor WASM app (hosted model). MapStaticAssets replaces
    // UseBlazorFrameworkFiles/UseStaticFiles and exposes every framework asset at a
    // stable URL (e.g. _framework/blazor.webassembly.js) that maps to the
    // fingerprinted file. index.html references those stable names directly, so the
    // raw SPA fallback below works without any #[.{fingerprint}] placeholder resolution.
    app.MapStaticAssets();

    // Auto-track HTTP metrics: http_requests_received_total,
    // http_request_duration_seconds, http_requests_in_progress (#47).
    app.UseHttpMetrics();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResultStatusCodes =
        {
            [HealthStatus.Healthy] = StatusCodes.Status200OK, [HealthStatus.Degraded] = StatusCodes.Status200OK, [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
        },
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        Predicate = _ => true
    });

    app.MapOpenApi();
    app.MapScalarApiReference();

    app.MapControllers();
    app.MapMetrics();                    // Prometheus scrape endpoint at /metrics (#47)
    app.MapFallbackToFile("index.html"); // Blazor client-side routing

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "FosterFlow API terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
