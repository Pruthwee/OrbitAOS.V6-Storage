#!/bin/bash
set -e
set -o pipefail

echo "=== OrbitAOS - MM Deployment to AWS EKS ==="
echo ""

PROJECT_NAME="OrbitAOS.V6"
NAME=$(echo "$PROJECT_NAME" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9' '-' | sed 's/^-*//;s-*$//')

# Ask for AWS region and EKS cluster name
read -p "Enter AWS region (e.g., us-east-1): " AWS_REGION
read -p "Enter EKS cluster name: " CLUSTER_NAME
read -p "Enter image URI (full path with registry and tag, e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/orbitaossmm:latest): " IMAGE_URI

echo ""
echo "Configuring kubectl for EKS cluster: $CLUSTER_NAME in $AWS_REGION..."
aws eks update-kubeconfig --region "$AWS_REGION" --name "$CLUSTER_NAME"

echo ""
echo "Verifying cluster connectivity..."
kubectl cluster-info || exit 1

# Ask for environment variables (non-credential)
echo ""
echo "Enter values for environment variables (press Enter to skip):"
read -p "Enter value for ASPNETCORE_ENVIRONMENT (or press Enter to skip): " ENV_VALUE
read -p "Enter value for ASPNETCORE_URLS (or press Enter to skip): " URLS_VALUE

# Ask for credentials (passwords/secrets)
echo ""
echo "Enter credentials for Kubernetes Secret:"
read -s -p "Enter database connection string: " CONNECTION_STRING
echo ""

echo ""
echo "Applying namespace..."
kubectl apply -f kubernetes/namespace.yaml

echo ""
echo "Creating Kubernetes Secret for credentials..."
kubectl create secret generic orbitaossmm-secrets -n orbitaossmm \
    --from-literal=connection-string="$CONNECTION_STRING" \
    --dry-run=client -o yaml | kubectl apply -f -

echo ""
echo "Applying deployment with substituted values..."
# Replace placeholders in deployment.yaml
sed -i.bak "s|{{IMAGE_URI}}|$IMAGE_URI|g" kubernetes/deployment.yaml
sed -i.bak "s|{{NAMESPACE}}|orbitaossmm|g" kubernetes/deployment.yaml
if [ -n "$ENV_VALUE" ]; then
    sed -i.bak "s|{{ASPNETCORE_ENVIRONMENT}}|$ENV_VALUE|g" kubernetes/deployment.yaml
fi
if [ -n "$URLS_VALUE" ]; then
    sed -i.bak "s|{{ASPNETCORE_URLS}}|$URLS_VALUE|g" kubernetes/deployment.yaml
fi
# Remove .bak files
rm -f kubernetes/deployment.yaml.bak

kubectl apply -f kubernetes/deployment.yaml

echo ""
echo "Applying service..."
kubectl apply -f kubernetes/service.yaml

echo ""
echo "Applying ingress..."
kubectl apply -f kubernetes/ingress.yaml

echo ""
echo "Waiting for deployment rollout..."
kubectl rollout status deployment/orbitaossmm -n orbitaossmm

echo ""
echo "=== Deployment complete ==="
echo ""
echo "Pods:"
kubectl get pods -n orbitaossmm
echo ""
echo "Services:"
kubectl get svc -n orbitaossmm
echo ""
echo "Ingress:"
kubectl get ingress -n orbitaossmm
echo ""
echo "Application URL: http://orbitaossmm.example.com"
echo ""
echo "To rollback on failure:"
echo "  kubectl rollout undo deployment/orbitaossmm -n orbitaossmm"
echo "  kubectl rollout status deployment/orbitaossmm -n orbitaossmm"
