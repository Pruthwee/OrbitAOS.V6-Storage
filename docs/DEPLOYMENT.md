# OrbitAOS.V6 - Deployment Guide

## Overview

OrbitAOS.V6 is an ASP.NET Core 6.0 web application deployed as a container on AWS Elastic Kubernetes Service (EKS). This document covers the full deployment lifecycle: local development, Docker image building, pushing to Amazon ECR, and deploying to EKS.

## Prerequisites

### System Requirements

- **.NET 6.0 SDK** (for local development and building)
- **Docker** 20.10+ (for building and running container images)
- **AWS CLI** 2.x (for ECR login and EKS configuration)
- **kubectl** 1.21+ (for interacting with the EKS cluster)
- **eksctl** (optional, for creating EKS clusters)

### AWS Permissions

The deploying user/role must have the following IAM permissions:

- `ecr:GetAuthorizationToken`
- `ecr:BatchCheckLayerAvailability`
- `ecr:CompleteLayerUpload`
- `ecr:UploadLayerPart`
- `ecr:PutImage`
- `ecr:InitiateLayerUpload`
- `ecr:BatchGetImage`
- `ecr:DescribeRepositories`
- `ecr:CreateRepository`
- `eks:DescribeCluster`
- `eks:ListClusters`

### EKS Cluster Setup

If you do not already have an EKS cluster, create one with eksctl:

```bash
eksctl create cluster \
  --name orbitaos-v6 \
  --region us-east-1 \
  --nodegroup-name orbitaos-v6-nodes \
  --node-type t3.medium \
  --nodes 2 \
  --nodes-max 4 \
  --managed
```

Configure kubectl to use the cluster:

```bash
aws eks update-kubeconfig --region us-east-1 --name orbitaos-v6
kubectl cluster-info
```

## Local Setup with Docker Compose

For local development, use Docker Compose:

```bash
# Set required environment variables
export DB_CONNECTION_STRING="Server=your-sql-server;Database=OrbitAOS;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=true;"

# Build and run
docker-compose up --build
```

The application will be available at `http://localhost:8080`.

## Building and Pushing the Docker Image

### Using the Shell Script (Linux/macOS)

```bash
./scripts/build-push.sh
```

The script will prompt for:
1. AWS region
2. AWS account ID
3. Image tag (or press Enter for `latest`)

It will then:
- Log in to AWS ECR
- Create the ECR repository if it does not exist
- Build the Docker image
- Push the image to ECR

### Using the Batch Script (Windows)

```cmd
scripts\build-push.bat
```

## Deploying to AWS EKS

### Using the Shell Script (Linux/macOS)

```bash
./scripts/deploy-image.sh
```

The script will prompt for:
1. AWS region
2. EKS cluster name
3. Full image URI (from the build-push step)
4. Database connection string (entered without echo for security)

It will then:
- Configure kubectl for the EKS cluster
- Create the Kubernetes namespace
- Create a Kubernetes Secret with the database connection string
- Substitute placeholders in the deployment manifest
- Apply the namespace, deployment, service, and ingress manifests
- Wait for the deployment to become ready
- Show pod, service, and ingress status

### Using the Batch Script (Windows)

```cmd
scripts\deploy-image.bat
```

### Manual Deployment

If you prefer to deploy manually:

```bash
# 1. Configure kubectl
aws eks update-kubeconfig --region us-east-1 --name orbitaos-v6

# 2. Create the namespace
kubectl apply -f kubernetes/namespace.yaml

# 3. Create the secret
kubectl create secret generic orbitaos-v6-secrets -n orbitaos-v6 \
  --from-literal=connection-strings--default-connection="your-connection-string" \
  --dry-run=client -o yaml | kubectl apply -f -

# 4. Substitute placeholders and apply deployment
sed -e 's|{{IMAGE_URI}}|123456789012.dkr.ecr.us-east-1.amazonaws.com/orbitaos-v6:latest|g' \
    -e 's|{{NAMESPACE}}|orbitaos-v6|g' \
    kubernetes/deployment.yaml | kubectl apply -f -

# 5. Apply service and ingress
kubectl apply -f kubernetes/service.yaml
kubectl apply -f kubernetes/ingress.yaml

# 6. Wait for rollout
kubectl rollout status deployment/orbitaos-v6 -n orbitaos-v6
```

## Kubernetes Manifests

### Namespace (`kubernetes/namespace.yaml`)

Creates a dedicated namespace `orbitaos-v6` for the application.

### Deployment (`kubernetes/deployment.yaml`)

- **Replicas**: 2 (for high availability)
- **Container**: `orbitaos-v6` running on port 8080
- **Image**: `{{IMAGE_URI}}` (replaced at deploy time)
- **Environment**: `ASPNETCORE_ENVIRONMENT=Production`
- **Secrets**: Database connection string from Kubernetes Secret
- **Resources**: 250m CPU / 512Mi RAM requests, 500m CPU / 1Gi RAM limits
- **Liveness Probe**: HTTP GET `/health` on port 8080 (30s initial delay, 10s interval)
- **Readiness Probe**: HTTP GET `/health` on port 8080 (10s initial delay, 5s interval)

### Service (`kubernetes/service.yaml`)

- **Type**: ClusterIP
- **Port**: 80 → 8080 (container port)
- **Selector**: `app: orbitaos-v6`

### Ingress (`kubernetes/ingress.yaml`)

- **Annotations**: AWS Load Balancer Controller (ALB, internet-facing)
- **Host**: `orbitaos-v6.example.com`
- **Path**: `/` (Prefix)
- **Target**: `orbitaos-v6-service` on port 80

## Configuration Management

### Environment Variables

The application reads configuration from the following environment variables:

| Variable | Description | Secret? |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | SQL Server database connection string | Yes |
| `ASPNETCORE_ENVIRONMENT` | ASP.NET Core environment (set to `Production`) | No |
| `ASPNETCORE_URLS` | Kestrel binding URL (set to `http://+:8080`) | No |

### Kubernetes Secrets

The database connection string is stored in a Kubernetes Secret named `orbitaos-v6-secrets`. The secret key is `connection-strings--default-connection`, which maps to the `ConnectionStrings:DefaultConnection` configuration key via ASP.NET Core's `__` hierarchy separator.

### Local Configuration

The `appsettings.json` file contains an empty `ConnectionStrings:DefaultConnection` value. The actual connection string is provided at runtime via the `ConnectionStrings__DefaultConnection` environment variable. This ensures no credentials are baked into the container image.

## Health Check Endpoint

The application exposes a health check endpoint at `/health`. This endpoint is used by Kubernetes for liveness and readiness probes. It returns HTTP 200 when the application is running and healthy.

## Troubleshooting

### Pod Failures

```bash
# Check pod status
kubectl get pods -n orbitaos-v6

# Check pod logs
kubectl logs -f deployment/orbitaos-v6 -n orbitaos-v6

# Describe pod for events
kubectl describe pod <pod-name> -n orbitaos-v6
```

### Service Issues

```bash
# Check service
kubectl get svc -n orbitaos-v6

# Check endpoints
kubectl get endpoints -n orbitaos-v6
```

### Ingress Problems

```bash
# Check ingress
kubectl get ingress -n orbitaos-v6

# Check ingress events
kubectl describe ingress orbitaos-v6-ingress -n orbitaos-v6
```

### Rollback

If the deployment fails:

```bash
# Rollback to previous revision
kubectl rollout undo deployment/orbitaos-v6 -n orbitaos-v6

# Check rollout status
kubectl rollout status deployment/orbitaos-v6 -n orbitaos-v6
```

## Scaling and Management

### Horizontal Pod Autoscaler (HPA)

To enable auto-scaling based on CPU utilization:

```bash
kubectl autoscale deployment orbitaos-v6 -n orbitaos-v6 \
  --cpu-percent=60 \
  --min=2 \
  --max=10
```

### Rolling Updates

```bash
# Update the image
kubectl set image deployment/orbitaos-v6 \
  orbitaos-v6=123456789012.dkr.ecr.us-east-1.amazonaws.com/orbitaos-v6:v2 \
  -n orbitaos-v6

# Monitor rollout
kubectl rollout status deployment/orbitaos-v6 -n orbitaos-v6
```

### Manual Scaling

```bash
kubectl scale deployment orbitaos-v6 --replicas=3 -n orbitaos-v6
```

## .NET Notes

### Runtime Settings

- **Target Framework**: .NET 6.0
- **Runtime Image**: `mcr.microsoft.com/dotnet/aspnet:6.0`
- **Listening Port**: 8080 (configured via `ASPNETCORE_URLS=http://+:8080`)
- **Environment**: `ASPNETCORE_ENVIRONMENT=Production`
- **HTTPS Redirection**: Enabled via `app.UseHttpsRedirection()` — in the container, the ALB terminates TLS and forwards HTTP to the container on port 8080.

### Garbage Collector Tuning

The Docker image sets the following GC environment variables for container-optimized behavior:

- `DOTNET_gcServer=true` — enables server GC for multi-core containers
- `DOTNET_gcConcurrent=true` — enables concurrent GC
- `DOTNET_GCHeapHardLimit=0x18000000` — limits the managed heap to approximately 384 MB, respecting the 1Gi memory limit

### Security

- **Non-root User**: The container runs as a non-root user (`appuser`) created in the runtime stage.
- **HTTPS Redirection**: Enforced in production via `UseHttpsRedirection()`.
- **Security Headers**: HSTS is configured for 30 days in production.
- **Authentication and Authorization**: ASP.NET Core Identity with cookie authentication is configured via `AddDefaultIdentity`.
- **CORS**: No CORS policy is configured — the application is same-origin by default.
- **Data Protection**: ASP.NET Core Identity uses the default data protection provider. For multi-instance deployments, configure a shared data protection keys store (e.g., mounted from a shared volume or AWS Secrets Manager).

### Static File Caching

Static files served from `wwwroot` include cache-control headers:

- **Fingerprinted assets** (with hash suffixes): `Cache-Control: public, max-age=31536000, immutable` (1 year)
- **Non-fingerprinted assets**: `Cache-Control: public, max-age=86400, must-revalidate` (1 day)

### CSS Optimization

The Dockerfile is structured as a multi-stage build. For production deployments, PurgeCSS should be integrated into the build stage to strip unused CSS from the Bootstrap library, reducing container image size. This is documented in the Dockerfile comments.

### Application Insights / Logging

The application uses the built-in ASP.NET Core logging infrastructure. Logs are written to stdout/stderr, which Kubernetes captures via `kubectl logs`. No Application Insights SDK is configured — consider adding it for production monitoring.
