#!/bin/bash
set -e

# =============================================================================
# build-push.sh - Build and Push Docker Image for OrbitAOS.V6
# Target Platform: Azure AKS
# =============================================================================

echo "=============================================="
echo "  OrbitAOS.V6 - Docker Build & Push Script"
echo "=============================================="
echo ""

PROJECT_NAME="orbitaos-v6"

# Sanitize image name: lowercase, replace non-alphanumeric with hyphens, trim hyphens
IMAGE_NAME=$(echo "$PROJECT_NAME" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9' '-' | sed 's/^-*//;s/-*$//')

echo "Select container registry:"
echo "  1. Azure Container Registry (ACR)"
echo "  2. Docker Hub"
echo ""
read -p "Enter choice [1 or 2]: " REGISTRY_CHOICE

echo ""
read -p "Enter image tag (press Enter for 'latest'): " IMAGE_TAG_INPUT

# Sanitize tag: lowercase, replace non-alphanumeric with hyphens, trim hyphens
if [ -z "$IMAGE_TAG_INPUT" ]; then
  IMAGE_TAG="latest"
else
  IMAGE_TAG=$(echo "$IMAGE_TAG_INPUT" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9._-' '-' | sed 's/^-*//;s/-*$//')
  if [ -z "$IMAGE_TAG" ]; then
    IMAGE_TAG="latest"
  fi
fi

echo ""

if [ "$REGISTRY_CHOICE" = "1" ]; then
  # ---- Azure Container Registry ----
  read -p "Enter ACR name (e.g., myregistry): " ACR_NAME
  ACR_NAME=$(echo "$ACR_NAME" | tr '[:upper:]' '[:lower:]' | tr -d ' ')

  FULL_IMAGE_NAME="${ACR_NAME}.azurecr.io/${IMAGE_NAME}:${IMAGE_TAG}"

  echo ""
  echo "Logging in to Azure Container Registry: $ACR_NAME ..."
  az acr login --name "$ACR_NAME"
  if [ $? -ne 0 ]; then
    echo "ERROR: ACR login failed. Ensure you are logged in with 'az login'."
    exit 1
  fi

elif [ "$REGISTRY_CHOICE" = "2" ]; then
  # ---- Docker Hub ----
  read -p "Enter Docker Hub username: " DOCKER_USERNAME
  read -s -p "Enter Docker Hub password/token: " DOCKER_PASSWORD
  echo ""

  FULL_IMAGE_NAME="${DOCKER_USERNAME}/${IMAGE_NAME}:${IMAGE_TAG}"

  echo ""
  echo "Logging in to Docker Hub ..."
  echo "$DOCKER_PASSWORD" | docker login --username "$DOCKER_USERNAME" --password-stdin
  if [ $? -ne 0 ]; then
    echo "ERROR: Docker Hub login failed."
    exit 1
  fi

else
  echo "ERROR: Invalid choice. Please enter 1 or 2."
  exit 1
fi

echo ""
echo "Building Docker image: $FULL_IMAGE_NAME"
echo "Build context: . (repository root)"
echo "Dockerfile: OrbitAOS.V6/Dockerfile"
echo ""

docker build -f OrbitAOS.V6/Dockerfile -t "$FULL_IMAGE_NAME" .
if [ $? -ne 0 ]; then
  echo "ERROR: Docker build failed."
  exit 1
fi

echo ""
echo "Pushing image: $FULL_IMAGE_NAME ..."
docker push "$FULL_IMAGE_NAME"
if [ $? -ne 0 ]; then
  echo "ERROR: Docker push failed."
  exit 1
fi

echo ""
echo "=============================================="
echo "  Build and push completed successfully!"
echo "  Image: $FULL_IMAGE_NAME"
echo "=============================================="
