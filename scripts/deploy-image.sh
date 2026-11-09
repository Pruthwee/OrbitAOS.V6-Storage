#!/bin/bash
set -e
set -o pipefail

# Deploy script for OrbitAOS.V6 to AWS EKS
# Run from the repository root

PROJECT_NAME="OrbitAOS.V6"
NAME=$(echo "$PROJECT_NAME" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9' '-' | sed 's/^-*//;s-*$//')

echo "=== OrbitAOS.V6 Deploy to AWS EKS ==="
echo ""

# Ask for cluster details
echo "1. AWS EKS Cluster Configuration"
read -p "Enter AWS region (e.g., us-east-1): " AWS_REGION
read -p "Enter EKS cluster name: " CLUSTER_NAME

# Ask for image URI
read -p "Enter full image URI (registry/repo:tag): " IMAGE_URI

# Configure kubectl for the target cluster
echo ""
echo "2. Configuring kubectl for EKS cluster: $CLUSTER_NAME"
aws eks update-kubeconfig --region "$AWS_REGION" --name "$CLUSTER_NAME"
if [ $? -ne 0 ]; then
    echo "ERROR: Failed to configure kubectl"
    exit 1
fi
kubectl cluster-info || exit 1
echo "kubectl configured successfully."
echo ""

# Ask for environment variables needed by the application
echo "3. Application Environment Variables"
echo "Enter values for the environment variables the application needs."
echo "Press Enter to skip (the placeholder will remain and you must set it manually)."
echo ""

# Collect environment variable values
VARS=()
VALUES=()

# Connection string (credential - should not be echoed)
read -s -p "Enter value for ConnectionStrings__DefaultConnection (database connection string): " DB_CONN_STR
echo ""
VARS+=("ConnectionStrings__DefaultConnection")
VALUES+=("$DB_CONN_STR")

# Ask for any additional non-credential environment variables
echo ""
echo "Enter any additional environment variables (name and value)."
echo "Leave the name empty to finish."
while true; do
    read -p "Enter variable name (or press Enter to finish): " VAR_NAME
    if [ -z "$VAR_NAME" ]; then
        break
    fi
    read -p "Enter value for $VAR_NAME: " VAR_VALUE
    VARS+=("$VAR_NAME")
    VALUES+=("$VAR_VALUE")
done

# Create the Kubernetes namespace
echo ""
echo "4. Creating namespace: $NAME"
kubectl apply -f kubernetes/namespace.yaml
echo "Namespace created."
echo ""

# Create the Secret with credentials
echo "5. Creating Kubernetes Secret: $NAME-secrets"
SECRET_ARGS=""
for i in "${!VARS[@]}"; do
    VAR="${VARS[$i]}"
    VAL="${VALUES[$i]}"
    if [ -n "$VAL" ]; then
        # Convert variable name to a valid secret key (replace __ with --, lowercase)
        SECRET_KEY=$(echo "$VAR" | tr '[:upper:]' '[:lower:]' | sed 's/__/-/g')
        SECRET_ARGS="$SECRET_ARGS --from-literal=$SECRET_KEY=$VAL"
    fi
done

if [ -n "$SECRET_ARGS" ]; then
    kubectl create secret generic "$NAME-secrets" -n "$NAME" $SECRET_ARGS --dry-run=client -o yaml | kubectl apply -f -
    echo "Secret created."
else
    echo "No secret values provided."
fi
echo ""

# Apply deployment with placeholder substitution
echo "6. Deploying application to EKS"

# Replace placeholders in deployment.yaml
sed -i.bak "s|{{IMAGE_URI}}|$IMAGE_URI|g" kubernetes/deployment.yaml
sed -i.bak "s|{{NAMESPACE}}|$NAME|g" kubernetes/deployment.yaml
for i in "${!VARS[@]}"; do
    VAR="${VARS[$i]}"
    VAL="${VALUES[$i]}"
    if [ -n "$VAL" ]; then
        # Replace {{VAR_NAME}} placeholders with actual values
        sed -i.bak "s|{{${VAR}}}|${VAL}|g" kubernetes/deployment.yaml
    fi
done

# Remove .bak files
rm -f kubernetes/deployment.yaml.bak

# Apply deployment, service, and ingress
kubectl apply -f kubernetes/deployment.yaml
echo "Deployment applied."
echo ""

echo "7. Creating Service"
kubectl apply -f kubernetes/service.yaml
echo "Service created."
echo ""

echo "8. Creating Ingress"
kubectl apply -f kubernetes/ingress.yaml
echo "Ingress created."
echo ""

# Wait for deployment rollout
echo "9. Waiting for deployment rollout..."
kubectl rollout status deployment/"$NAME" -n "$NAME"
if [ $? -ne 0 ]; then
    echo "ERROR: Deployment rollout failed"
    echo "Rolling back..."
    kubectl rollout undo deployment/"$NAME" -n "$NAME"
    exit 1
fi
echo "Deployment is ready."
echo ""

# Show resources
echo "10. Deployment Status"
kubectl get pods,svc,ingress -n "$NAME"
echo ""

# Show application URL
echo "=== Deployment Complete ==="
echo "Application URL: https://$NAME.example.com"
echo ""
echo "To check logs: kubectl logs -f deployment/$NAME -n $NAME"
echo "To rollback: kubectl rollout undo deployment/$NAME -n $NAME"
