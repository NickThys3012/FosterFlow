#!/usr/bin/env bash
# Uploads the FosterFlow monitoring configuration + dashboards to the Azure Files
# share that backs the ACI monitoring container group (US-INF-4.3, #49).
#
# Run this AFTER `az deployment sub create ... --parameters deployMonitoring=true`,
# then restart the container group so the containers pick up the config:
#   az container restart -g <rg> -n <containerGroupName>
#
# Usage:
#   ./upload-monitoring-config.sh <storageAccountName> <apiHost> [shareName]
# Example:
#   ./upload-monitoring-config.sh stfosterflowmon app-fosterflow-prod-swe.azurewebsites.net
set -euo pipefail

STORAGE_ACCOUNT="${1:?storage account name required}"
API_HOST="${2:?API host (e.g. app-fosterflow-prod-swe.azurewebsites.net) required}"
SHARE_NAME="${3:-monitoring-config}"

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SRC="$ROOT/monitoring"
TMP="$(mktemp -d)"
trap 'rm -rf "$TMP"' EXIT

# Materialise the production Prometheus config with the real API host injected.
mkdir -p "$TMP/prometheus" "$TMP/loki" "$TMP/grafana"
sed "s/__API_HOST__/${API_HOST}/g" "$SRC/prometheus/prometheus.prod.yml" > "$TMP/prometheus/prometheus.prod.yml"
cp "$SRC/loki/loki-config.yml" "$TMP/loki/loki-config.yml"
cp -R "$SRC/grafana/provisioning" "$TMP/grafana/provisioning"
cp -R "$SRC/grafana/dashboards" "$TMP/grafana/dashboards"

# The provisioned dashboards provider points at /var/lib/grafana/dashboards, but on
# ACI the share is mounted at /mnt/config, so repoint it for the cloud layout.
sed -i.bak 's#/var/lib/grafana/dashboards#/mnt/config/grafana/dashboards#' \
  "$TMP/grafana/provisioning/dashboards/dashboards.yml"
rm -f "$TMP/grafana/provisioning/dashboards/dashboards.yml.bak"

# In a docker-compose network Grafana reaches Prometheus/Loki by service name, but
# in an ACI container group all containers share one network namespace and talk over
# localhost. Rewrite the datasource URLs for the cloud layout.
sed -i.bak \
  -e 's#http://prometheus:9090#http://localhost:9090#' \
  -e 's#http://loki:3100#http://localhost:3100#' \
  "$TMP/grafana/provisioning/datasources/datasources.yml"
rm -f "$TMP/grafana/provisioning/datasources/datasources.yml.bak"

KEY="$(az storage account keys list -n "$STORAGE_ACCOUNT" --query '[0].value' -o tsv)"

echo "Uploading monitoring config to share '$SHARE_NAME' in '$STORAGE_ACCOUNT'..."
az storage file upload-batch \
  --account-name "$STORAGE_ACCOUNT" \
  --account-key "$KEY" \
  --destination "$SHARE_NAME" \
  --source "$TMP"

echo "Done. Restart the container group to apply:"
echo "  az container restart -g <resourceGroup> -n <containerGroupName>"
