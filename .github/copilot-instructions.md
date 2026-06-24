# FosterFlow

FosterFlow helps animal shelters find foster families for cats. It is a **.NET 10** solution using
**Clean Architecture** with a hosted **Blazor WebAssembly** front end served by an ASP.NET Core API.

> The Visual Studio solution lives in the nested folder `FosterFlow/FosterFlow/`. The git root is the
> outer `FosterFlow/` directory (which also holds planning docs: `fosterflow-mvp-brief_1.md`,
> `fosterflow-user-stories.md`, `fosterflow-monitoring-setup.md`, `fosterflow-mockups.html`).

## Build, test, run

Run all commands from the solution directory `FosterFlow/FosterFlow/` (where `FosterFlow.sln` lives).

```bash
dotnet build FosterFlow.sln
dotnet test FosterFlow.sln                                   # all test projects (NUnit)
dotnet test FosterFlow.Domain.Tests/FosterFlow.Domain.Tests.csproj   # one project
dotnet test --filter "FullyQualifiedName~CreateCat"          # a single test / class
dotnet run --project FosterFlow.Api                          # starts API + hosted Blazor client
```

- The **API is the startup project**. It references `FosterFlow.Web` and serves the WASM client via
  `UseBlazorFrameworkFiles()` + `MapFallbackToFile("index.html")`. Do not run `FosterFlow.Web` standalone.
- Default URLs (Development): API `https://localhost:7214` / `http://localhost:5001`. The Blazor client
  hard-codes the API base address `https://localhost:7214` in `FosterFlow.Web/Program.cs` — keep these in sync.
- OpenAPI/Scalar docs are exposed at `/scalar`; health at `/health`.

## Configuration & database

- SQL Server via EF Core. The connection string is named **`Database`** (see `ConnectionStrings:Database`
  in `appsettings.json`, empty by default — set it through user secrets or `appsettings.Development.json`).
- `Jwt:Secret` plus `Issuer`/`Audience` must be set; the placeholder in `appsettings.json` is not usable in prod.
- `AppDbContext` (`FosterFlow.Infrastructure/Persistence`) extends `IdentityDbContext<ApplicationUser>`.
- **EF migrations live in `FosterFlow.Infrastructure/Persistence/EntityFramework/Migrations`** (non-default
  path). The API is the startup project and Infrastructure holds the context:
  ```bash
  dotnet ef migrations add <Name> \
    --project FosterFlow.Infrastructure --startup-project FosterFlow.Api \
    -o Persistence/EntityFramework/Migrations
  dotnet ef database update --project FosterFlow.Infrastructure --startup-project FosterFlow.Api
  ```
- On startup the API calls `app.Services.SeedUsers()` (`IdentitySeed.cs`), which seeds roles
  `Admin`, `Shelter`, `Foster` and a default admin `admin@fosterflow.dev` / `Admin1234!`.

## Architecture & project dependencies

Dependencies flow inward; reference direction is enforced by project references:

- **FosterFlow.Domain** — entities (`Cat`, `User`), enums (`CatStatus`, `UserRole`), repository interfaces
  (`Interfaces/Repositories`), domain exceptions. No outward dependencies.
- **FosterFlow.Contracts** — request/response DTOs and FluentValidation validators. **Shared by both the
  Application layer and the Blazor Web client**, so it must stay free of server-only dependencies.
- **FosterFlow.Application** — MediatR command/query handlers + `ValidationBehaviour` pipeline. References
  Contracts only.
- **FosterFlow.Infrastructure** — EF Core `AppDbContext`, repository implementations, ASP.NET Identity
  (`ApplicationUser`, `TokenService`, JWT refresh tokens), and `IIdentityService`/`ICurrentUserService`.
  References Application + Domain.
- **FosterFlow.Api** — controllers, JWT auth setup, `ExceptionHandlingMiddleware`. References Infrastructure
  + Web.
- **FosterFlow.Web** — Blazor WASM client. References Contracts only; talks to the API over HTTP.

Each layer wires itself up via a `DependencyInjection.AddXxx()` extension called from the relevant `Program.cs`.

## Key conventions

- **CQRS with MediatR.** Features are organized as
  `Features/<Area>/Commands|Queries/<Name>/{<Name>Command|Query}.cs` + `<Name>Handler.cs`. Commands/queries
  are `record`s implementing `IRequest<T>`; controllers inject `ISender`/`IMediator` and `Send` them.
- **Validation** is automatic: validators live in `FosterFlow.Contracts/Validators` (e.g.
  `CreateCatRequestValidator`), are registered from the Contracts assembly, and run via `ValidationBehaviour`
  in the MediatR pipeline. A failure throws `Application.Common.Exceptions.ValidationException`. Add a
  validator class rather than validating inside a handler.
- **Exceptions → HTTP** is centralized in `ExceptionHandlingMiddleware` (registered first in the pipeline).
  Throw the domain/application exceptions (`NotFoundException`, `ValidationException`, `ProcessingException`,
  `DomainException`) instead of returning error status codes from handlers.
- **Repositories** are interfaces in `Domain/Interfaces/Repositories`, implemented in
  `Infrastructure/Persistence/Repositories`, registered scoped in `Infrastructure/DependencyInjection.cs`.
- **DTOs vs entities:** controllers and the Web client only exchange Contracts DTOs; domain entities never
  leave the server.
- **Auth flow:** API issues a JWT access token + an HTTP-only refresh cookie (`TokenService`). The Blazor
  client stores tokens via `TokenStorage`, attaches them through `AuthMessageHandler`, and surfaces auth
  state through `AppAuthStateProvider`. Roles in use are `Admin`, `Shelter`, `Foster`.
- Targeting .NET 10 with `Nullable` and `ImplicitUsings` enabled across all projects. Tests use **NUnit**
  (global `using NUnit.Framework;`).
- Some scaffolding is still boilerplate (e.g. `Cat` entity is marked TODO; `UnitTest1.cs`, `Counter`/`Weather`
  pages). Replace these as features land rather than building on them.
