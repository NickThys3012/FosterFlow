# FosterFlow Monitoring (US-INF-4.1 / 4.2 / 4.3 — #47 #48 #49)

Observability stack for the FosterFlow API: **Prometheus** (metrics), **Loki** (logs)
and **Grafana** (dashboards).

## What the API exposes

- **`GET /metrics`** — Prometheus text format (prometheus-net).
  - Auto HTTP metrics: `http_requests_received_total`, `http_request_duration_seconds`,
    `http_requests_in_progress`.
  - Business metrics: `fosterflow_cat_listings_created_total`,
    `fosterflow_matches_created_total`, `fosterflow_matches_accepted_total`,
    `fosterflow_care_briefing_duration_seconds`, `fosterflow_active_fosters`.
- **Structured logs** via Serilog. When `Loki:Url` is set the API streams logs to Loki with
  labels `app=fosterflow-api` and `environment=<env>`. Query in Grafana with `{app="fosterflow-api"}`.

## Run locally (docker-compose)

```bash
cp .env.example .env      # first time only
docker compose up --build
```

| Service    | URL                       | Notes                          |
|------------|---------------------------|--------------------------------|
| API        | http://localhost:5000     | `/metrics`, `/health`, `/scalar` |
| Prometheus | http://localhost:9090     | scrapes `api:8080/metrics`     |
| Loki       | http://localhost:3100     | Serilog sink target            |
| Grafana    | http://localhost:3000     | admin / admin (override in `.env`) |

Grafana auto-provisions the Prometheus + Loki datasources and the three dashboards
under the **FosterFlow** folder.

## Dashboards (exported JSON in `grafana/dashboards/`)

1. **API Overview** — request rate, latency (p50/p95/p99), 5xx error rate, in-progress requests, active users.
2. **Business Metrics** — cats created, matches created/accepted, acceptance rate, active fosters, care-briefing p95.
3. **Logs Explorer** — searchable Loki logs with level + free-text filters.

All dashboards refresh every **30 s**. Re-export from Grafana (Share → Export → Save to file)
to update the JSON checked into this folder.

## Deploy to Azure (ACI)

The monitoring stack ships as an optional ACI container group in the Bicep deployment.

```bash
az deployment sub create \
  --location swedencentral \
  --template-file infra/main.bicep \
  --parameters infra/main.bicepparam \
  --parameters deployMonitoring=true grafanaAdminPassword='<strong-password>'

# Upload config + dashboards to the Azure Files share, then restart the group:
infra/upload-monitoring-config.sh stfosterflowmon <appServiceName>.azurewebsites.net
az container restart -g <resourceGroup> -n ci-fosterflow-monitoring
```

The deployment outputs `grafanaUrl`. Point the API at Loki by setting the
`LokiUrl` Key Vault secret (the App Service already reads `Loki__Url` from it).

> Loki retention is configured for **30 days** (`loki/loki-config.yml`).
