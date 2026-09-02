#!/bin/bash
set -e
set -o pipefail

# =============================================================================
# deploy-image.sh - Deploy OrbitAOS.V6 to Azure AKS
# =============================================================================

echo "=============================================="
echo "  OrbitAOS.V6 - AKS Deployment Script"
echo "=============================================="
echo ""

APP_NAME="orbitaos-v6"
NAMESPACE="orbitaos-v6"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$(dirname "$SCRIPT_DIR")"

# ---- Prompt for Azure / AKS details ----
read -p "Enter Azure Resource Group name: " RESOURCE_GROUP
if [ -z "$RESOURCE_GROUP" ]; then
  echo "ERROR: Resource group cannot be empty."
  exit 1
fi

read -p "Enter AKS Cluster name: " CLUSTER_NAME
if [ -z "$CLUSTER_NAME" ]; then
  echo "ERROR: AKS cluster name cannot be empty."
  exit 1
fi

read -p "Enter full Docker image URI (e.g., myregistry.azurecr.io/orbitaos-v6:latest): " IMAGE_URI
if [ -z "$IMAGE_URI" ]; then
  echo "ERROR: Image URI cannot be empty."
  exit 1
fi

echo ""
echo "---- Application Environment Variables ----"
echo "Press Enter to skip any optional variable."
echo ""

read -p "Enter SQL Server connection string (ConnectionStrings__DefaultConnection): " CONNECTION_STRING
if [ -z "$CONNECTION_STRING" ]; then
  CONNECTION_STRING="Server=<your-sql-server>;Database=OrbitAOSDb;User Id=<user>;Password=<password>;MultipleActiveResultSets=true"
  echo "  Using placeholder: $CONNECTION_STRING"
fi

read -p "Enter static file cache max age in seconds (STATIC_FILE_CACHE_MAX_AGE_SECONDS) [86400]: " STATIC_CACHE_AGE
if [ -z "$STATIC_CACHE_AGE" ]; then
  STATIC_CACHE_AGE="86400"
fi

echo ""
echo "---- Configuring kubectl for AKS ----"
az aks get-credentials --resource-group "$RESOURCE_GROUP" --name "$CLUSTER_NAME" --overwrite-existing
if [ $? -ne 0 ]; then
  echo "ERROR: Failed to get AKS credentials. Check resource group and cluster name."
  exit 1
fi

echo ""
echo "Verifying cluster connectivity ..."
kubectl cluster-info || { echo "ERROR: Cannot connect to AKS cluster."; exit 1; }

echo ""
echo "---- Updating Kubernetes manifests ----"

# Work on copies to avoid modifying originals
DEPLOY_DIR="$PROJECT_DIR/kubernetes"
TMP_DIR=$(mktemp -d)
cp "$DEPLOY_DIR"/*.yaml "$TMP_DIR/"

# Replace placeholders using pipe delimiter
sed -i "s|{{IMAGE_URI}}|${IMAGE_URI}|g"                                   "$TMP_DIR/deployment.yaml"
sed -i "s|{{CONNECTION_STRING}}|${CONNECTION_STRING}|g"                   "$TMP_DIR/deployment.yaml"
sed -i "s|{{STATIC_FILE_CACHE_MAX_AGE_SECONDS}}|${STATIC_CACHE_AGE}|g"   "$TMP_DIR/deployment.yaml"

echo "Manifests updated successfully."

echo ""
echo "---- Applying Kubernetes manifests ----"

echo "Applying namespace ..."
kubectl apply -f "$TMP_DIR/namespace.yaml"

echo "Applying deployment ..."
kubectl apply -f "$TMP_DIR/deployment.yaml"

echo "Applying service ..."
kubectl apply -f "$TMP_DIR/service.yaml"

echo "Applying ingress ..."
kubectl apply -f "$TMP_DIR/ingress.yaml"

echo ""
echo "---- Waiting for deployment rollout ----"
kubectl rollout status deployment/"$APP_NAME" -n "$NAMESPACE" --timeout=300s
if [ $? -ne 0 ]; then
  echo "ERROR: Deployment rollout failed or timed out."
  echo "Run: kubectl describe deployment/$APP_NAME -n $NAMESPACE"
  echo "Run: kubectl logs -l app=$APP_NAME -n $NAMESPACE"
  echo ""
  echo "To rollback: kubectl rollout undo deployment/$APP_NAME -n $NAMESPACE"
  rm -rf "$TMP_DIR"
  exit 1
fi

echo ""
echo "---- Verifying deployed resources ----"
kubectl get pods,svc,ingress -n "$NAMESPACE"

echo ""
echo "---- Application Access ----"
INGRESS_HOST=$(kubectl get ingress "${APP_NAME}-ingress" -n "$NAMESPACE" -o jsonpath='{.spec.rules[0].host}' 2>/dev/null || echo "orbitaos-v6.example.com")
echo "Application URL: http://$INGRESS_HOST"
echo "Health Check:    http://$INGRESS_HOST/health"

echo ""
echo "=============================================="
echo "  Deployment completed successfully!"
echo "  App: $APP_NAME | Namespace: $NAMESPACE"
echo "=============================================="

# Cleanup temp files
rm -rf "$TMP_DIR"
