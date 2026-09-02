# OrbitAOS.V6 - Deployment Guide

## Overview

This guide covers the complete deployment process for **OrbitAOS.V6**, an ASP.NET Core 6.0 MVC web application with Identity authentication and Entity Framework Core (SQL Server), targeting **Azure Kubernetes Service (AKS)**.

---

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Project Analysis](#project-analysis)
3. [Local Development with Docker Compose](#local-development-with-docker-compose)
4. [Build and Push Docker Image](#build-and-push-docker-image)
5. [Azure AKS Deployment](#azure-aks-deployment)
6. [Kubernetes Manifest Descriptions](#kubernetes-manifest-descriptions)
7. [Configuration Management](#configuration-management)
8. [Scaling and Management](#scaling-and-management)
9. [Troubleshooting](#troubleshooting)
10. [Security Considerations](#security-considerations)
11. [.NET-Specific Notes](#net-specific-notes)

---

## Prerequisites

### Local Development
- [Docker Desktop](https://www.docker.com/products/docker-desktop) 20.10+
- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- Docker Compose 2.x

### Azure AKS Deployment
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) 2.40+
- [kubectl](https://kubernetes.io/docs/tasks/tools/) 1.25+
- Active Azure subscription
- Azure Container Registry (ACR) or Docker Hub account
- AKS cluster with Application Gateway Ingress Controller (AGIC) enabled

### Verify Tools
```bash
az --version
kubectl version --client
docker --version
dotnet --version
```

---

## Project Analysis

| Property              | Value                                      |
|-----------------------|--------------------------------------------|
| Framework             | .NET 6.0 (net6.0)                          |
| Application Type      | ASP.NET Core MVC Web Application           |
| Authentication        | ASP.NET Core Identity                      |
| Database              | SQL Server (Entity Framework Core 6.0.25)  |
| Application Port      | 80 (HTTP)                                  |
| Health Endpoint       | `/health`                                  |
| Base Image (Build)    | mcr.microsoft.com/dotnet/sdk:6.0           |
| Base Image (Runtime)  | mcr.microsoft.com/dotnet/sdk:6.0           |
| Target Platform       | Azure AKS                                  |

---

## Local Development with Docker Compose

### 1. Configure Environment Variables

Create a `.env` file in the project root:

```env
CONNECTION_STRING=Server=<your-sql-server>;Database=OrbitAOSDb;User Id=<user>;Password=<password>;MultipleActiveResultSets=true
STATIC_FILE_CACHE_MAX_AGE_SECONDS=86400
```

### 2. Build and Start the Application

```bash
# From the OrbitAOS.V6 directory
docker-compose up --build
```

### 3. Access the Application

- Application: [http://localhost:80](http://localhost:80)
- Health Check: [http://localhost:80/health](http://localhost:80/health)

### 4. Stop the Application

```bash
docker-compose down
```

### 5. View Logs

```bash
docker-compose logs -f orbitaos-v6
```

---

## Build and Push Docker Image

### Linux/macOS

```bash
# Make the script executable
chmod +x scripts/build-push.sh

# Run from the OrbitAOS.V6 directory
./scripts/build-push.sh
```

### Windows

```cmd
scripts\build-push.bat
```

### Script Prompts

The script will ask for:
1. **Registry type**: Azure ACR (1) or Docker Hub (2)
2. **Image tag**: e.g., `v1.0.0` or press Enter for `latest`
3. **Registry credentials**: ACR name or Docker Hub username/password

### Manual Build (Alternative)

```bash
# Build from repository root
docker build -f OrbitAOS.V6/Dockerfile -t <registry>/orbitaos-v6:latest .

# Push to registry
docker push <registry>/orbitaos-v6:latest
```

---

## Azure AKS Deployment

### Step 1: Login to Azure

```bash
az login
az account set --subscription "<your-subscription-id>"
```

### Step 2: Create or Configure ACR (if using Azure ACR)

```bash
# Create ACR (if not exists)
az acr create --resource-group <resource-group> --name <acr-name> --sku Basic

# Attach ACR to AKS cluster
az aks update --resource-group <resource-group> --name <cluster-name> --attach-acr <acr-name>
```

### Step 3: Create AKS Cluster (if not exists)

```bash
az aks create \
  --resource-group <resource-group> \
  --name <cluster-name> \
  --node-count 2 \
  --enable-addons monitoring \
  --generate-ssh-keys \
  --enable-managed-identity
```

### Step 4: Enable Application Gateway Ingress Controller

```bash
az aks enable-addons \
  --resource-group <resource-group> \
  --name <cluster-name> \
  --addons ingress-appgw \
  --appgw-name <appgw-name> \
  --appgw-subnet-cidr "10.225.0.0/16"
```

### Step 5: Run Deployment Script

#### Linux/macOS
```bash
chmod +x scripts/deploy-image.sh
./scripts/deploy-image.sh
```

#### Windows
```cmd
scripts\deploy-image.bat
```

### Step 6: Verify Deployment

```bash
# Check all resources
kubectl get pods,svc,ingress -n orbitaos-v6

# Check pod logs
kubectl logs -l app=orbitaos-v6 -n orbitaos-v6

# Describe deployment
kubectl describe deployment/orbitaos-v6 -n orbitaos-v6
```

### Step 7: Access the Application

```bash
# Get ingress address
kubectl get ingress orbitaos-v6-ingress -n orbitaos-v6

# Health check
curl http://<ingress-address>/health
```

---

## Kubernetes Manifest Descriptions

### `kubernetes/namespace.yaml`
Creates the `orbitaos-v6` namespace to isolate all application resources.

### `kubernetes/deployment.yaml`
Defines the application deployment with:
- **2 replicas** for high availability
- **Rolling update** strategy (zero-downtime deployments)
- **Resource limits**: CPU 500m / Memory 1Gi
- **Resource requests**: CPU 250m / Memory 512Mi
- **Liveness probe**: GET `/health` every 30s (starts after 60s)
- **Readiness probe**: GET `/health` every 15s (starts after 30s)
- **Non-root security context** (UID 1001)
- Environment variable placeholders for runtime configuration

### `kubernetes/service.yaml`
Exposes the deployment internally via ClusterIP on port 80.

### `kubernetes/ingress.yaml`
Configures Azure Application Gateway Ingress Controller to route external traffic to the service.
- Update `host: orbitaos-v6.example.com` to your actual domain.

---

## Configuration Management

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | `Production` |
| `ASPNETCORE_URLS` | Kestrel binding URL | `http://+:80` |
| `ConnectionStrings__DefaultConnection` | SQL Server connection string | *(required)* |
| `STATIC_FILE_CACHE_MAX_AGE_SECONDS` | Static file CDN cache duration | `86400` |
| `TZ` | Container timezone | `UTC` |

### Using Kubernetes Secrets for Sensitive Data

```bash
# Create secret for database connection string
kubectl create secret generic orbitaos-v6-secrets \
  --from-literal=connection-string="Server=<server>;Database=OrbitAOSDb;..." \
  -n orbitaos-v6
```

Update `deployment.yaml` to reference the secret:
```yaml
- name: ConnectionStrings__DefaultConnection
  valueFrom:
    secretKeyRef:
      name: orbitaos-v6-secrets
      key: connection-string
```

### Using ConfigMaps for Non-Sensitive Config

```bash
kubectl create configmap orbitaos-v6-config \
  --from-literal=STATIC_FILE_CACHE_MAX_AGE_SECONDS=86400 \
  -n orbitaos-v6
```

---

## Scaling and Management

### Manual Scaling

```bash
kubectl scale deployment/orbitaos-v6 --replicas=3 -n orbitaos-v6
```

### Horizontal Pod Autoscaler (HPA)

```bash
kubectl autoscale deployment/orbitaos-v6 \
  --cpu-percent=70 \
  --min=2 \
  --max=10 \
  -n orbitaos-v6
```

### Rolling Update (New Image)

```bash
kubectl set image deployment/orbitaos-v6 \
  orbitaos-v6=<registry>/orbitaos-v6:<new-tag> \
  -n orbitaos-v6

# Monitor rollout
kubectl rollout status deployment/orbitaos-v6 -n orbitaos-v6
```

### Rollback

```bash
# Rollback to previous version
kubectl rollout undo deployment/orbitaos-v6 -n orbitaos-v6

# Rollback to specific revision
kubectl rollout history deployment/orbitaos-v6 -n orbitaos-v6
kubectl rollout undo deployment/orbitaos-v6 --to-revision=2 -n orbitaos-v6
```

---

## Troubleshooting

### Pod Not Starting

```bash
# Check pod status
kubectl get pods -n orbitaos-v6

# Describe pod for events
kubectl describe pod <pod-name> -n orbitaos-v6

# Check logs
kubectl logs <pod-name> -n orbitaos-v6
kubectl logs <pod-name> -n orbitaos-v6 --previous
```

### Common Issues

| Issue | Cause | Solution |
|-------|-------|----------|
| `ImagePullBackOff` | Cannot pull image | Verify ACR credentials and image URI |
| `CrashLoopBackOff` | App crashes on start | Check logs; verify DB connection string |
| `Pending` pods | Insufficient resources | Scale node pool or reduce resource requests |
| Health probe failing | App not ready | Increase `initialDelaySeconds` in probes |
| DB connection error | Wrong connection string | Verify `ConnectionStrings__DefaultConnection` |

### Database Migration

Run EF Core migrations as a Kubernetes Job or init container:

```bash
# Run migrations manually from local machine
dotnet ef database update --connection "<connection-string>"
```

### Ingress Not Accessible

```bash
# Check ingress status
kubectl describe ingress orbitaos-v6-ingress -n orbitaos-v6

# Verify Application Gateway is running
az network application-gateway show \
  --resource-group <resource-group> \
  --name <appgw-name>
```

---

## Security Considerations

1. **Non-root container**: The application runs as UID 1001 (non-root).
2. **Secrets management**: Use Kubernetes Secrets or Azure Key Vault for sensitive data.
3. **Network policies**: Restrict pod-to-pod communication with NetworkPolicy resources.
4. **HTTPS**: Configure TLS termination at the Application Gateway level.
5. **Image scanning**: Enable Azure Defender for container registries.
6. **RBAC**: Use Kubernetes RBAC to limit access to namespaces.
7. **Data Protection**: Configure ASP.NET Core Data Protection keys for distributed deployments.

```bash
# Example: Store Data Protection keys in Azure Blob Storage
# Add to appsettings.json or environment variables:
# AzureStorage__ConnectionString=<storage-connection-string>
# AzureStorage__ContainerName=dataprotection
```

---

## .NET-Specific Notes

### ASP.NET Core Identity
- The application uses ASP.NET Core Identity with SQL Server.
- Ensure the database is provisioned and migrations are applied before first run.
- `RequireConfirmedAccount = true` is set — configure email confirmation for production.

### Entity Framework Core Migrations
```bash
# Apply migrations (run from project directory)
dotnet ef database update --project OrbitAOS.V6.csproj
```

### Static File Caching
- The `STATIC_FILE_CACHE_MAX_AGE_SECONDS` environment variable controls CDN cache headers.
- Default is 86400 seconds (1 day).
- Adjust for your CDN configuration.

### Health Checks
- Health endpoint: `/health` (registered via `app.MapHealthChecks("/health")`)
- Used by both Kubernetes liveness and readiness probes.
- Consider adding database health checks:
  ```csharp
  builder.Services.AddHealthChecks()
      .AddDbContextCheck<ApplicationDbContext>();
  ```

### Performance Tuning
- Set `DOTNET_GCHeapHardLimit` to limit GC heap size in containers.
- Use `ReadyToRun` compilation for faster startup:
  ```xml
  <PublishReadyToRun>true</PublishReadyToRun>
  ```
- Enable response compression middleware for better throughput.

### Logging
- Default logging is configured via `appsettings.json`.
- For production, consider integrating Application Insights:
  ```bash
  dotnet add package Microsoft.ApplicationInsights.AspNetCore
  ```
  Set `APPLICATIONINSIGHTS_CONNECTION_STRING` environment variable.

---

## Quick Reference

```bash
# Deploy
./scripts/deploy-image.sh

# Check status
kubectl get all -n orbitaos-v6

# View logs
kubectl logs -l app=orbitaos-v6 -n orbitaos-v6 -f

# Scale
kubectl scale deployment/orbitaos-v6 --replicas=3 -n orbitaos-v6

# Rollback
kubectl rollout undo deployment/orbitaos-v6 -n orbitaos-v6

# Delete all resources
kubectl delete namespace orbitaos-v6
```
