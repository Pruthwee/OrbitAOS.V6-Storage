#!/bin/bash
set -e

# Build and push script for OrbitAOS.V6 to AWS ECR
# Run from the repository root

PROJECT_NAME="OrbitAOS.V6"
IMAGE_NAME=$(echo "$PROJECT_NAME" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9' '-' | sed 's/^-*//;s/-*$//')

echo "=== OrbitAOS.V6 Build and Push to AWS ECR ==="
echo ""

# Ask for registry details
echo "1. AWS ECR Registry Configuration"
read -p "Enter AWS region (e.g., us-east-1): " AWS_REGION
read -p "Enter AWS account ID (e.g., 123456789012): " AWS_ACCOUNT_ID

REGISTRY_URL="${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com"
ECR_REPO="${IMAGE_NAME}"

# Ask for image tag
read -p "Enter image tag (or press Enter for 'latest'): " IMAGE_TAG
if [ -z "$IMAGE_TAG" ]; then
    IMAGE_TAG="latest"
fi

# Sanitize tag
IMAGE_TAG=$(echo "$IMAGE_TAG" | tr -cs 'a-z0-9._-' '-' | sed 's/^-*//;s/-*$//')
if [ -z "$IMAGE_TAG" ]; then
    IMAGE_TAG="latest"
fi

FULL_IMAGE_NAME="${REGISTRY_URL}/${ECR_REPO}:${IMAGE_TAG}"

echo ""
echo "=== Build Configuration ==="
echo "Project:      $PROJECT_NAME"
echo "Image name:   $IMAGE_NAME"
echo "Registry:     $REGISTRY_URL"
echo "Repository:   $ECR_REPO"
echo "Tag:          $IMAGE_TAG"
echo "Full image:   $FULL_IMAGE_NAME"
echo ""

# Login to AWS ECR
echo "2. Logging in to AWS ECR..."
aws ecr get-login-password --region "$AWS_REGION" | docker login --username AWS --password-stdin "$REGISTRY_URL"
if [ $? -ne 0 ]; then
    echo "ERROR: ECR login failed"
    exit 1
fi
echo "ECR login successful."
echo ""

# Create ECR repository if it doesn't exist
echo "3. Checking if ECR repository exists..."
aws ecr describe-repositories --repository-names "$ECR_REPO" --region "$AWS_REGION" >/dev/null 2>&1
if [ $? -ne 0 ]; then
    echo "Creating ECR repository: $ECR_REPO"
    aws ecr create-repository --repository-name "$ECR_REPO" --region "$AWS_REGION"
    if [ $? -ne 0 ]; then
        echo "ERROR: Failed to create ECR repository"
        exit 1
    fi
    echo "ECR repository created."
else
    echo "ECR repository already exists."
fi
echo ""

# Build the Docker image
echo "4. Building Docker image..."
docker build -f Dockerfile -t "$FULL_IMAGE_NAME" --platform linux/amd64 .
if [ $? -ne 0 ]; then
    echo "ERROR: Docker build failed"
    exit 1
fi
echo "Docker build successful."
echo ""

# Push the image
echo "5. Pushing Docker image to ECR..."
docker push "$FULL_IMAGE_NAME"
if [ $? -ne 0 ]; then
    echo "ERROR: Docker push failed"
    exit 1
fi
echo ""

echo "=== Success ==="
echo "Image pushed: $FULL_IMAGE_NAME"
echo "Use this image URI when deploying to EKS."
