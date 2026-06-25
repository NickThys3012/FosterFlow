#!/usr/bin/env bash
# Deploy FosterFlow production infrastructure (issues #37-#40).
# Usage:
#   export SQL_ADMIN_PASSWORD='...'      # 16+ chars, meets Azure SQL complexity rules
#   export JWT_SECRET='...'              # 32+ char random string
#   export LOKI_URL='https://...'        # optional
#   export CLIENT_IP="$(curl -s ifconfig.me)"   # optional, for local SQL access
#   ./deploy.sh
set -euo pipefail

LOCATION="${LOCATION:-westeurope}"
DEPLOYMENT_NAME="fosterflow-$(date +%Y%m%d%H%M%S)"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

: "${SQL_ADMIN_PASSWORD:?Set SQL_ADMIN_PASSWORD}"
: "${JWT_SECRET:?Set JWT_SECRET}"

echo "Validating Bicep (what-if)..."
az deployment sub what-if \
  --name "$DEPLOYMENT_NAME" \
  --location "$LOCATION" \
  --template-file "$SCRIPT_DIR/main.bicep" \
  --parameters \
      sqlAdminPassword="$SQL_ADMIN_PASSWORD" \
      jwtSecret="$JWT_SECRET" \
      lokiUrl="${LOKI_URL:-}" \
      allowedClientIp="${CLIENT_IP:-}"

echo "Deploying..."
az deployment sub create \
  --name "$DEPLOYMENT_NAME" \
  --location "$LOCATION" \
  --template-file "$SCRIPT_DIR/main.bicep" \
  --parameters \
      sqlAdminPassword="$SQL_ADMIN_PASSWORD" \
      jwtSecret="$JWT_SECRET" \
      lokiUrl="${LOKI_URL:-}" \
      allowedClientIp="${CLIENT_IP:-}"

echo "Done. Outputs:"
az deployment sub show --name "$DEPLOYMENT_NAME" --query properties.outputs
