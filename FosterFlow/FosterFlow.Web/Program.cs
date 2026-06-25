using FosterFlow.Web;
using FosterFlow.Web.Authentication;
using FosterFlow.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ── Auth services ────────────────────────────────────────────────────
builder.Services.AddScoped<TokenStorage>();
builder.Services.AddScoped<AppAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    sp => sp.GetRequiredService<AppAuthStateProvider>());
builder.Services.AddAuthorizationCore();

// ── HttpClient with auth handler ─────────────────────────────────────
// IMPORTANT: Register AuthService before AuthMessageHandler
// IMPORTANT: Use AddScoped for the base HttpClient so handler can resolve AuthService
// The client is hosted by the API on the same origin, so target the host's own
// base address. This resolves to https://localhost:7214 locally and to the App
// Service URL in production — no hard-coded host that breaks once deployed.
builder.Services.AddHttpClient("API",
        client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<AuthMessageHandler>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddTransient<AuthMessageHandler>();

// Inject the named client as the default HttpClient
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));
await builder.Build().RunAsync();
