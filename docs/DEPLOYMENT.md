# Deployment Guide — OrbitAOS - MM

## Overview

OrbitAOS - MM is an ASP.NET Core 6.0 web application deployed as a container on **AWS EKS** (Elastic Kubernetes Service). The application is containerized with a multi-stage Dockerfile, published to **Amazon ECR**, and deployed to EKS with Kubernetes manifests.

## Prerequisites

### System Requirements

- **.NET 6.0 SDK** (for local builds)
- **Docker** (for local container builds)
- **AWS CLI** v2
- **kubectl** (matching your EKS cluster version)
- **eksctl** (optional, for cluster creation)
- **PowerShell** 5.1+ (for `.bat` script execution on Windows)

### AWS Permissions

The following IAM permissions are required:

- `ecr:BatchGetImage`
- `ecr:CompleteLayerUpload`
- `ecr:UploadLayerPart`
- `ecr:InitiateLayerUpload`
- `ecr:PutImage`
- `ecr:GetAuthorizationToken`
- `ecr:DescribeRepositories`
- `ecr:CreateRepository`
- `eks:DescribeCluster`
- `eks:ListClusters`

## Local Setup with Docker Compose

### Build and run locally

```bash
# Set the connection string environment variable
export ConnectionStrings__DefaultConnection="Server=your-sql-server;Database=OrbitAOS;User Id=sa;Password=YourStrong!Passw0rd;MultipleActiveResultSets=true"

# Build and start the application
docker-compose up --build
```

The application will be available at `http://localhost:8080`.

### Environment variables

| Variable | Description |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | Set to `Production` |
| `ASPNETCORE_URLS` | Set to `http://+:8080` (container listens on port 8080) |
| `ConnectionStrings__DefaultConnection` | SQL Server connection string (contains credentials — inject via Secret in production) |

## Building and Pushing the Container Image

### Using the shell script (Linux/macOS)

```bash
./scripts/build-push.sh
```

The script will:
1. Ask you to select the target platform (AWS ECR or Docker Hub).
2. Prompt for the registry details (AWS region, account ID, ECR repository name, or Docker Hub credentials).
3. Prompt for an image tag (defaults to `latest`).
4. Log in to the registry.
5. Build the Docker image using the multi-stage `Dockerfile`.
6. Push the image to the registry.

### Using the batch script (Windows)

```cmd
scripts\build-push.bat
```

### Dockerfile details

The Dockerfile uses a **multi-stage build**:

1. **Build stage**: `mcr.microsoft.com/dotnet/sdk:6.0` — restores NuGet packages, compiles the application, and publishes to `/app/publish`.
2. **Runtime stage**: `mcr.microsoft.com/dotnet/aspnet:6.0` — copies the published output and runs the application as a non-root user on port 8080.

Key settings in the runtime image:
- `ASPNETCORE_URLS=http://+:8080` — the application listens on port 8080 inside the container.
- `ASPNETCORE_ENVIRONMENT=Production` — production configuration is used.
- `DOTNET_RUNNING_IN_CONTAINER=true` — enables .NET container-aware GC tuning.
- The application runs as a non-root user (`appuser`, UID 1000) for security.
- No package manager or extra tools (curl, wget) are installed in the runtime image.

## AWS EKS Deployment

### EKS Cluster Setup

If you do not already have an EKS cluster, create one:

```bash
# Create a cluster with eksctl
eksctl create cluster --name orbitaossmm-cluster --region us-east-1 --nodegroup-name orbitaossmm-nodes --node-type t3.medium --nodes 2 --nodes-max 4

# Or using AWS CLI
aws eks create-cluster --name orbitaossmm-cluster --role-arn <eks-role-arn> --resources-vpc-config file://vpc-config.json --region us-east-1
```

### Configure kubectl

```bash
aws eks update-kubeconfig --name orbitaossmm-cluster --region us-east-1
```

Verify connectivity:

```bash
kubectl cluster-info
```

### Deploy to EKS

```bash
./scripts/deploy-image.sh
```

The script will:
1. Ask for the AWS region and EKS cluster name.
2. Ask for the image URI (full path with registry and tag).
3. Ask for non-credential environment variable values (press Enter to skip).
4. Ask for credentials (database connection string) without echoing.
5. Apply the namespace.
6. Create a Kubernetes Secret (`orbitaossmm-secrets`) for the connection string.
7. Apply the deployment with placeholder substitution.
8. Apply the service and ingress.
9. Wait for the rollout to complete.
10. Display pod, service, and ingress status.
11. Print the application URL and rollback instructions.

### Deployment walkthrough

#### Namespace (`kubernetes/namespace.yaml`)

Creates a dedicated namespace `orbitaossmm` for the application.

#### Deployment (`kubernetes/deployment.yaml`)

- **Replicas**: 2 (for high availability).
- **Container image**: `{{IMAGE_URI}}` — replaced at deploy time with the ECR image URI.
- **Container port**: 8080 (matches `ASPNETCORE_URLS`).
- **Environment variables**: `ASPNETCORE_ENVIRONMENT` and `ASPNETCORE_URLS` are set as fixed values. The database connection string is sourced from the `orbitaossmm-secrets` Kubernetes Secret.
- **Resources**: requests (250m CPU, 512Mi memory), limits (500m CPU, 1Gi memory).
- **Liveness probe**: HTTP GET to `/health` on port 8080, with a 30-second initial delay.
- **Readiness probe**: HTTP GET to `/health` on port 8080, with a 10-second initial delay.

#### Service (`kubernetes/service.yaml`)

- **Type**: ClusterIP.
- **Port**: 80 → targetPort 8080.

#### Ingress (`kubernetes/ingress.yaml`)

- **Ingress class**: `alb` (AWS Load Balancer Controller).
- **Scheme**: `internet-facing`.
- **Host**: `orbitaossmm.example.com`.
- **Health check path**: `/health`.

### Rollback

```bash
kubectl rollout undo deployment/orbitaossmm -n orbitaossmm
kubectl rollout status deployment/orbitaossmm -n orbitaossmm
```

## Configuration Management

### Environment variables

All environment-specific configuration is injected via Kubernetes environment variables and Secrets:

| Variable | Source |
|---|---|
| `ConnectionStrings__DefaultConnection` | Kubernetes Secret (`orbitaossmm-secrets`) |
| `ASPNETCORE_ENVIRONMENT` | Deployment env (fixed: `Production`) |
| `ASPNETCORE_URLS` | Deployment env (fixed: `http://+:8080`) |

The connection string contains credentials and must never be hardcoded. It is provided at deploy time and stored in a Kubernetes Secret.

### Updating configuration

To update a Secret:

```bash
kubectl create secret generic orbitaossmm-secrets -n orbitaossmm \
    --from-literal=connection-string="new-connection-string" \
    --dry-run=client -o yaml | kubectl apply -f -
```

Then restart the deployment:

```bash
kubectl rollout restart deployment/orbitaossmm -n orbitaossmm
```

## Security Considerations

### Non-root user

The container runs as a non-root user (`appuser`, UID 1000) for defense in depth.

### HTTPS redirection

The application enforces HTTPS redirection via `app.UseHttpsRedirection()`. In EKS, the ALB terminates TLS and forwards traffic to the container over HTTP on port 8080.

### Secrets management

- Database credentials are stored in Kubernetes Secrets, never in the image or manifest files.
- The `appsettings.json` connection string is set to an empty string; the real value is injected via the `ConnectionStrings__DefaultConnection` environment variable at runtime.

### Security headers

The application uses ASP.NET Core's built-in security features:
- HSTS (30 days) in production.
- HTTPS redirection.
- Authentication and authorization middleware.

### CORS

The application does not configure CORS explicitly, so all cross-origin requests are blocked by default. If cross-origin access is needed, add a named CORS policy in `Program.cs` and configure allowed origins via configuration.

### Data protection

ASP.NET Core Identity uses data protection for authentication tokens. In a multi-pod deployment, configure a shared data protection key ring (e.g., via a Kubernetes Secret volume mount) to ensure tokens remain valid across pod restarts.

## .NET Notes

### Runtime settings

- **Target framework**: .NET 6.0
- **Base image**: `mcr.microsoft.com/dotnet/aspnet:6.0`
- **GC tuning**: `DOTNET_RUNNING_IN_CONTAINER=true` enables container-aware garbage collection, which respects cgroup memory limits.
- **Kestrel configuration**: The application uses the default Kestrel server configured via `ASPNETCORE_URLS=http://+:8080`.

### Garbage collector

In the container, .NET automatically detects the cgroup memory limit and adjusts the GC heap size accordingly. The deployment sets memory limits (512Mi requests, 1Gi limits) to ensure the GC has appropriate bounds.

### Monitoring

The application uses:
- **Built-in ASP.NET Core logging** with `Information` level for the default logger and `Warning` for Microsoft.AspNetCore.
- **Health checks endpoint** at `/health` for Kubernetes liveness and readiness probes.

### Container image size

The multi-stage build produces a minimal runtime image. The SDK stage is discarded, and only the published output is copied into the `aspnet:6.0` runtime image. The `.dockerignore` file excludes `bin/`, `obj/`, `.git/`, and other unnecessary files from the build context.

### Static file caching

The application configures `StaticFileOptions` with cache headers:
- **Fingerprinted assets** (e.g., `site.css?v=...` or hashed filenames): `Cache-Control: public, max-age=31536000, immutable`.
- **Non-fingerprinted assets**: `Cache-Control: public, max-age=86400, must-revalidate` with `Vary: Accept-Encoding`.

This enables CDN (CloudFront) caching in front of the Ingress, reducing origin pod load.
