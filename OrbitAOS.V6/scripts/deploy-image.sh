#!/bin/bash
set -e
set -o pipefail

# =============================================================================
# deploy-image.sh - Deploy OrbitAOS.V6 to AWS EKS
# =============================================================================

APP_NAME="orbitaos-v6"
NAMESPACE="orbitaos-v6"
K8S_DIR="$(cd "$(dirname "$0")/.." && pwd)/kubernetes"

echo "=============================================="
echo "  OrbitAOS.V6 - AWS EKS Deployment Script"
echo "=============================================="

# Prompt for AWS region
read -p "Enter AWS region (e.g. us-east-1): " AWS_REGION
if [ -z "$AWS_REGION" ]; then
  echo "ERROR: AWS region is required."
  exit 1
fi

# Prompt for EKS cluster name
read -p "Enter EKS cluster name: " CLUSTER_NAME
if [ -z "$CLUSTER_NAME" ]; then
  echo "ERROR: EKS cluster name is required."
  exit 1
fi

# Prompt for Docker image URI
read -p "Enter full Docker image URI (e.g. 123456789.dkr.ecr.us-east-1.amazonaws.com/orbitaos-v6:latest): " IMAGE_URI
if [ -z "$IMAGE_URI" ]; then
  echo "ERROR: Docker image URI is required."
  exit 1
fi

echo ""
echo "--- Optional: Application Environment Variables ---"
echo "Press Enter to skip any variable."

read -p "Enter value for CONNECTION_STRING (SQL Server connection string): " CONNECTION_STRING
CONNECTION_STRING="${CONNECTION_STRING:-Server=your-db-server;Database=OrbitAOS;User Id=sa;Password=YourStrong@Passw0rd;MultipleActiveResultSets=true}"

read -p "Enter value for ASPNETCORE_DOCS_URL [https://docs.microsoft.com/aspnet/core]: " ASPNETCORE_DOCS_URL
ASPNETCORE_DOCS_URL="${ASPNETCORE_DOCS_URL:-https://docs.microsoft.com/aspnet/core}"

read -p "Enter value for STATIC_FILES_CACHE_MAX_AGE_SECONDS [86400]: " STATIC_FILES_CACHE_MAX_AGE_SECONDS
STATIC_FILES_CACHE_MAX_AGE_SECONDS="${STATIC_FILES_CACHE_MAX_AGE_SECONDS:-86400}"

echo ""
echo "Configuring kubectl for EKS cluster: $CLUSTER_NAME in $AWS_REGION..."
aws eks update-kubeconfig --region "$AWS_REGION" --name "$CLUSTER_NAME"

echo "Verifying cluster connectivity..."
kubectl cluster-info || { echo "ERROR: Cannot connect to cluster. Check your credentials and cluster name."; exit 1; }

echo ""
echo "Updating Kubernetes manifests with deployment values..."

# Work on copies to avoid modifying originals
cp "$K8S_DIR/deployment.yaml" /tmp/deployment.yaml
cp "$K8S_DIR/service.yaml" /tmp/service.yaml
cp "$K8S_DIR/ingress.yaml" /tmp/ingress.yaml
cp "$K8S_DIR/namespace.yaml" /tmp/namespace.yaml

# Replace placeholders using pipe delimiter
sed -i 's|{{IMAGE_URI}}|'"$IMAGE_URI"'|g' /tmp/deployment.yaml
sed -i 's|{{CONNECTION_STRING}}|'"$CONNECTION_STRING"'|g' /tmp/deployment.yaml
sed -i 's|{{ASPNETCORE_DOCS_URL}}|'"$ASPNETCORE_DOCS_URL"'|g' /tmp/deployment.yaml
sed -i 's|{{STATIC_FILES_CACHE_MAX_AGE_SECONDS}}|'"$STATIC_FILES_CACHE_MAX_AGE_SECONDS"'|g' /tmp/deployment.yaml

echo ""
echo "Applying Kubernetes manifests..."

echo "  [1/4] Applying namespace..."
kubectl apply -f /tmp/namespace.yaml

echo "  [2/4] Applying deployment..."
kubectl apply -f /tmp/deployment.yaml

echo "  [3/4] Applying service..."
kubectl apply -f /tmp/service.yaml

echo "  [4/4] Applying ingress..."
kubectl apply -f /tmp/ingress.yaml

echo ""
echo "Waiting for deployment rollout to complete..."
kubectl rollout status deployment/"$APP_NAME" -n "$NAMESPACE" --timeout=300s

echo ""
echo "Verifying deployed resources..."
kubectl get pods,svc,ingress -n "$NAMESPACE"

echo ""
echo "Fetching application URL from ingress..."
INGRESS_HOST=$(kubectl get ingress "${APP_NAME}-ingress" -n "$NAMESPACE" -o jsonpath='{.status.loadBalancer.ingress[0].hostname}' 2>/dev/null || echo "pending")
if [ "$INGRESS_HOST" != "pending" ] && [ -n "$INGRESS_HOST" ]; then
  echo "  Application URL: http://$INGRESS_HOST"
else
  echo "  Ingress hostname is still provisioning. Run the following to check:"
  echo "  kubectl get ingress ${APP_NAME}-ingress -n ${NAMESPACE}"
fi

echo ""
echo "=============================================="
echo "  Deployment completed successfully!"
echo "  App: $APP_NAME"
echo "  Namespace: $NAMESPACE"
echo "  Image: $IMAGE_URI"
echo "=============================================="
echo ""
echo "Rollback command (if needed):"
echo "  kubectl rollout undo deployment/$APP_NAME -n $NAMESPACE"
