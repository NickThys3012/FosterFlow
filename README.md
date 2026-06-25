# 🐾 FosterFlow

> **#HACKTHEKITTY 2026** — *"Tinder for cat fostering"*

FosterFlow is a real-time cat fostering coordination platform that connects animal shelters with foster families. It replaces the chaotic WhatsApp threads and Facebook posts that most shelters rely on today with a structured, mobile-first web application — complete with smart matching, AI-generated care briefings, and a live notification feed.

---

## ✨ Features

| Feature | Description |
|---|---|
| **Role-based onboarding** | Separate signup flows for Shelters (post cats) and Fosters (offer homes) |
| **Smart match feed** | Fosters only see cats compatible with their home type, pets, availability, and experience level |
| **One-click interest** | Fosters express interest in a cat; shelters accept or decline from their dashboard |
| **AI care briefings** | Claude AI generates a personalised care guide for each matched cat (diet, routine, medical needs, red flags) |
| **Real-time notifications** | SignalR pushes match requests and status updates instantly — no refresh required |
| **Gamification** | Foster badge system and leaderboard reward the most active community members |
| **Full observability** | Prometheus metrics + Loki structured logs + Grafana dashboards, running on Azure Container Instances |

---

## 🏗 Architecture

```
FosterFlow/
├── FinalTest.Api/          # ASP.NET Core 9 — hosts API + Blazor WASM (same origin)
├── FinalTest.Web/          # Blazor WASM client (hosted model — no CORS)
├── FinalTest.Shared/       # DTOs + FluentValidation validators (shared by both)
├── FosterFlow.Domain/      # Pure business entities, enums, repository interfaces
├── FosterFlow.Application/ # MediatR use cases, pipeline behaviours
└── FosterFlow.Infrastructure/ # EF Core, ASP.NET Core Identity, repositories
```

**Key decisions:**

- **Hosted Blazor WASM** — API serves the client on the same origin, eliminating all `SameSite` cookie friction
- **JWT + refresh token rotation** — 15-minute access tokens, 7-day HttpOnly-cookie refresh tokens with reuse detection and family revocation
- **Clean Architecture** — `Domain.User` (pure) vs `Infrastructure.ApplicationUser` (IdentityUser); `UserRepository.MapToDomain()` is the only crossing point
- **Shared validators** — FluentValidation rules in `FinalTest.Shared` run identically on server (MediatR pipeline) and client (Blazor `EditForm`)

---

## 🛠 Tech Stack

| Layer | Technology |
|---|---|
| **Backend** | ASP.NET Core 9, MediatR, FluentValidation, EF Core 9 |
| **Frontend** | Blazor WebAssembly (Hosted), Blazored.FluentValidation |
| **Auth** | ASP.NET Core Identity + custom JWT / refresh token implementation |
| **Database** | Azure SQL (EF Core migrations) |
| **Storage** | Azure Blob Storage (cat photos) |
| **Real-time** | SignalR |
| **AI** | Anthropic Claude API (`claude-sonnet-4-20250514`) — match scoring + care briefings |
| **Observability** | prometheus-net, Serilog → Loki, Grafana |
| **Hosting** | Azure App Service (Free F1) + Azure Container Instances (observability stack) |
| **CI/CD** | GitHub Actions |
| **Security scan** | Aikido |

---

## 🚀 Running Locally

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for observability stack)
- SQL Server LocalDB (ships with Visual Studio) or Azure SQL connection string

### 1. Clone & configure

```bash
git clone https://github.com/<your-handle>/fosterflow.git
cd fosterflow
```

Copy the example secrets file and fill in your values:

```bash
cp src/FinalTest.Api/appsettings.Example.json src/FinalTest.Api/appsettings.Development.json
```

Required values in `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Server=(localdb)\\mssqllocaldb;Database=FosterFlow;Trusted_Connection=True"
  },
  "Jwt": {
    "Secret": "<32+ character random string>",
    "Issuer": "FosterFlow.Api",
    "Audience": "FosterFlow.Web",
    "AccessTokenMinutes": 15,
    "RefreshTokenDays": 7
  },
  "Anthropic": {
    "ApiKey": "<your Claude API key>"
  },
  "Azure": {
    "BlobStorage": "<your connection string>"
  }
}
```

### 2. Apply database migrations

```bash
cd src/FinalTest.Api
dotnet ef database update
```

### 3. Start the observability stack (optional)

```bash
# From solution root
docker compose -f docker-compose.observability.yml up -d
```

| Service | URL | Credentials |
|---|---|---|
| Grafana | http://localhost:3000 | admin / fosterflow-dev |
| Prometheus | http://localhost:9090 | — |
| Loki | http://localhost:3100 | — |

### 4. Run the application

```bash
cd src/FinalTest.Api
dotnet run
```

The app is served at **https://localhost:7200** — this single URL serves both the API and the Blazor WASM client.

**Seed data** is applied automatically on first run (Admin, Shelter, and Foster demo accounts — see console output for credentials).

---

## ☁️ Deployment (Azure)

CI/CD runs through three GitHub Actions workflows (`.github/workflows/`):

| Workflow | File | Trigger | Purpose |
|---|---|---|---|
| **Build & Test** | `build.yml` | push to `develop`/`main`, PRs to `main` | `dotnet restore` → `build -c Release` → `test`, then publishes the API (Blazor WASM baked into `wwwroot`) as an artifact. |
| **Docker Build & Push** | `docker-build.yml` | push to `main` | Builds the multi-stage image and pushes it to **GHCR** (`ghcr.io/<owner>/fosterflow`) tagged `latest` + git SHA. Uses the built-in `GITHUB_TOKEN` — no extra secrets. |
| **Deploy** | `deploy.yml` | push to `main`, manual | Publishes API + Web and deploys to **Azure App Service** (`app-fosterflow-prod-swe`), then runs `/health` and `/` smoke checks. |

> Branch protection on `main` should require the **Build & Test** check so failing builds can't merge.

**Required GitHub Secrets:**

| Secret | Used by | Description |
|---|---|---|
| `AZURE_WEBAPP_PUBLISH_PROFILE` | `deploy.yml` | Publish profile downloaded from the Azure App Service (`app-fosterflow-prod-swe`). |

> The image push uses the automatic `GITHUB_TOKEN`; runtime secrets (DB connection, JWT, blob storage) are
> injected by the App Service from Key Vault — see `infra/`.

To deploy manually:

```bash
dotnet publish FosterFlow/FosterFlow.Api/FosterFlow.Api.csproj -c Release -o ./publish
# Publishing the API also compiles the Blazor WASM client into wwwroot.
az webapp deploy --name app-fosterflow-prod-swe --resource-group rg-fosterflow-prod-swe \
  --type zip --src-path ./publish

# Run the container image locally:
docker run -p 5000:8080 ghcr.io/<owner>/fosterflow:latest
# http://localhost:5000/  -> Blazor app, http://localhost:5000/health -> API health
```

Observability containers are deployed to Azure Container Instances — see `observability/azure-deploy-observability.sh`.

---

## 🔒 Security

- Refresh tokens stored **hashed (SHA-256)** in the database — a DB breach cannot replay tokens
- Refresh token delivered via **HttpOnly cookie** — invisible to JavaScript / XSS
- Access token held **in-memory only** — never `localStorage`
- **Reuse detection** — replaying a revoked refresh token immediately revokes all sessions for that user
- `SameSite=Strict` on refresh token cookie mitigates CSRF
- `ClockSkew = TimeSpan.Zero` enforces strict JWT expiry
- Aikido security scan included in submission ZIP

---

## 🧪 Testing the Auth Flow (Postman / Thunder Client)

```
POST https://localhost:7200/api/auth/login
{ "email": "admin@fosterflow.dev", "password": "Admin1234!" }

→ 200 OK  +  Set-Cookie: refreshToken=...  +  { accessToken, ... }

GET  https://localhost:7200/api/cats
Authorization: Bearer <accessToken>

POST https://localhost:7200/api/auth/refresh   # cookie sent automatically
→ 200 OK  +  new accessToken  +  rotated cookie

POST https://localhost:7200/api/auth/logout
→ 200 OK  +  cookie cleared
```

---

## 📁 Project Documents

Detailed implementation guides are included in the submission ZIP:

| Document | Contents |
|---|---|
| `JWT_Auth_Guide_Blazor_WASM.docx` | Full JWT + refresh token implementation walkthrough |
| `FluentValidation_Guide_FosterFlow.docx` | Server + client validation setup |
| `Observability_Guide_FosterFlow.docx` | Prometheus + Loki + Grafana stack |
| `CleanArchitecture_Reference.md` | Layer diagram, dependency rules, code examples |
| `fosterflow-user-stories.md` | Full product backlog (~200 story points) |

---

## 👤 Author

Built solo by **Nick** for **#HACKTHEKITTY 2026** — a 14-day sprint.

---

*Every cat deserves a foster home. FosterFlow makes the match.*