# OrbitAOS.V6 - Deployment Guide

## Overview

This guide covers building, containerizing, and deploying the **OrbitAOS.V6** ASP.NET Core 6.0 web application to **AWS EKS (Elastic Kubernetes Service)**.

- **Technology**: ASP.NET Core 6.0 MVC with Identity
- **Runtime Image**: `mcr.microsoft.com/dotnet/sdk:6.0`
- **Application Port**: 80 (HTTP)
- **Health Endpoint**: `/health`
- **Target Platform**: AWS EKS

---

## Prerequisites

### Local Development
- [Docker Desktop](https://www.docker.com/products/docker-desktop) 20.10+
- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- Docker Compose v2+

### AWS EKS Deployment
- [AWS CLI v2](https://docs.aws.amazon.com/cli/latest/userguide/install-cliv2.html) configured with appropriate IAM permissions
- [kubectl](https://kubernetes.io/docs/tasks/tools/) v1.24+
- [eksctl](https://eksctl.io/) (optional, for cluster creation)
- An existing EKS cluster with the [AWS Load Balancer Controller](https://kubernetes-sigs.github.io/aws-load-balancer-controller/) installed
- ECR repository access or Docker Hub account

### Required IAM Permissions
```
ecr:GetAuthorizationToken
ecr:BatchCheckLayerAvailability
ecr:GetDownloadUrlForLayer
ecr:BatchGetImage
ecr:CreateRepository
ecr:DescribeRepositories
eks:DescribeCluster
eks:ListClusters
```

---

## Project Structure

```
OrbitAOSmono/
├── OrbitAOS.V6/
│   ├── Dockerfile
│   ├── docker-compose.yml
│   ├── .dockerignore
│   ├── kubernetes/
│   │   ├── namespace.yaml
│   │   ├── deployment.yaml
│   │   ├── service.yaml
│   │   └── ingress.yaml
│   ├── scripts/
│   │   ├── build-push.sh
│   │   ├── build-push.bat
│   │   ├── deploy-image.sh
│   │   └── deploy-image.bat
│   └── docs/
│       └── DEPLOYMENT.md
```

---

## Local Development with Docker Compose

### 1. Configure Environment Variables

Create a `.env` file in the `OrbitAOS.V6/` directory:

```env
CONNECTION_STRING=Server=your-sql-server;Database=OrbitAOS;User Id=sa;Password=YourStrong@Passw0rd;MultipleActiveResultSets=true
ASPNETCORE_DOCS_URL=https://docs.microsoft.com/aspnet/core
STATIC_FILES_CACHE_MAX_AGE_SECONDS=86400
```

### 2. Build and Start the Application

```bash
# From the OrbitAOSmono/ directory (repository root)
cd OrbitAOSmono

# Build and start
docker compose -f OrbitAOS.V6/docker-compose.yml up --build

# Run in background
docker compose -f OrbitAOS.V6/docker-compose.yml up --build -d
```

### 3. Access the Application

- **Application**: http://localhost:8080
- **Health Check**: http://localhost:8080/health

### 4. View Logs

```bash
docker compose -f OrbitAOS.V6/docker-compose.yml logs -f orbitaos-v6
```

### 5. Stop the Application

```bash
docker compose -f OrbitAOS.V6/docker-compose.yml down
```

---

## Building and Pushing the Docker Image

### Linux/macOS

```bash
# From the OrbitAOSmono/ directory (repository root)
chmod +x OrbitAOS.V6/scripts/build-push.sh
./OrbitAOS.V6/scripts/build-push.sh
```

The script will prompt you to:
1. Enter an image tag (defaults to `latest`)
2. Select registry type (AWS ECR or Docker Hub)
3. Provide registry credentials

### Windows

```cmd
# From the OrbitAOSmono\ directory (repository root)
OrbitAOS.V6\scripts\build-push.bat
```

### Manual Build

```bash
# From the OrbitAOSmono/ directory (repository root)
docker build -f OrbitAOS.V6/Dockerfile -t orbitaos-v6:latest .
```

---

## AWS EKS Deployment

### Step 1: Configure AWS CLI

```bash
aws configure
# Enter: AWS Access Key ID, Secret Access Key, Region, Output format
```

### Step 2: Verify EKS Cluster Access

```bash
aws eks update-kubeconfig --region <your-region> --name <your-cluster-name>
kubectl cluster-info
kubectl get nodes
```

### Step 3: Build and Push Image to ECR

```bash
# Run the build-push script and select AWS ECR
./OrbitAOS.V6/scripts/build-push.sh
```

Or manually:
```bash
AWS_REGION=us-east-1
AWS_ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)
ECR_REPO=orbitaos-v6
REGISTRY_URL=${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com

# Login to ECR
aws ecr get-login-password --region $AWS_REGION | docker login --username AWS --password-stdin $REGISTRY_URL

# Create repository if needed
aws ecr create-repository --repository-name $ECR_REPO --region $AWS_REGION 2>/dev/null || true

# Build and push
docker build -f OrbitAOS.V6/Dockerfile -t ${REGISTRY_URL}/${ECR_REPO}:latest .
docker push ${REGISTRY_URL}/${ECR_REPO}:latest
```

### Step 4: Deploy to EKS

```bash
# Run the deploy script
chmod +x OrbitAOS.V6/scripts/deploy-image.sh
./OrbitAOS.V6/scripts/deploy-image.sh
```

The script will prompt for:
- AWS region
- EKS cluster name
- Full Docker image URI
- Application environment variables (CONNECTION_STRING, etc.)

### Step 5: Verify Deployment

```bash
# Check pods
kubectl get pods -n orbitaos-v6

# Check services
kubectl get svc -n orbitaos-v6

# Check ingress
kubectl get ingress -n orbitaos-v6

# View pod logs
kubectl logs -l app=orbitaos-v6 -n orbitaos-v6 --tail=100

# Describe deployment
kubectl describe deployment orbitaos-v6 -n orbitaos-v6
```

---

## Kubernetes Manifest Descriptions

### namespace.yaml
Creates the `orbitaos-v6` Kubernetes namespace to isolate all application resources.

### deployment.yaml
Defines the application deployment with:
- **2 replicas** for high availability
- **Rolling update** strategy (zero-downtime deployments)
- **Resource limits**: CPU 500m / Memory 1Gi
- **Resource requests**: CPU 250m / Memory 512Mi
- **Liveness probe**: HTTP GET `/health` (starts after 60s, every 30s)
- **Readiness probe**: HTTP GET `/health` (starts after 30s, every 15s)
- **Non-root security context** for container security

### service.yaml
Creates a `ClusterIP` service exposing port 80, routing traffic to application pods.

### ingress.yaml
Configures an AWS Application Load Balancer (ALB) via the AWS Load Balancer Controller:
- Internet-facing scheme
- IP target type for direct pod routing
- Health check on `/health`
- Routes all traffic (`/`) to the application service

---

## Configuration Management

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | `Production` |
| `ASPNETCORE_URLS` | Kestrel binding URL | `http://+:80` |
| `ConnectionStrings__DefaultConnection` | SQL Server connection string | Required |
| `ApiUrls__AspNetCoreDocsUrl` | External API URL | `https://docs.microsoft.com/aspnet/core` |
| `STATIC_FILES_CACHE_MAX_AGE_SECONDS` | Static file cache duration | `86400` |

### Using Kubernetes Secrets (Recommended for Production)

```bash
# Create a secret for the database connection string
kubectl create secret generic orbitaos-v6-secrets \
  --from-literal=connection-string="Server=prod-server;Database=OrbitAOS;..." \
  -n orbitaos-v6
```

Then reference in deployment.yaml:
```yaml
env:
  - name: ConnectionStrings__DefaultConnection
    valueFrom:
      secretKeyRef:
        name: orbitaos-v6-secrets
        key: connection-string
```

---

## Scaling and Management

### Manual Scaling

```bash
kubectl scale deployment orbitaos-v6 --replicas=3 -n orbitaos-v6
```

### Horizontal Pod Autoscaler (HPA)

```bash
kubectl autoscale deployment orbitaos-v6 \
  --cpu-percent=70 \
  --min=2 \
  --max=10 \
  -n orbitaos-v6
```

### Rolling Update

```bash
# Update image
kubectl set image deployment/orbitaos-v6 \
  orbitaos-v6=<new-image-uri> \
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
kubectl logs <pod-name> -n orbitaos-v6 --previous  # crashed pod logs
```

### Common Issues

**ImagePullBackOff**
- Verify ECR repository exists and image tag is correct
- Check IAM permissions for ECR access
- Ensure EKS node role has `AmazonEC2ContainerRegistryReadOnly` policy

**CrashLoopBackOff**
- Check application logs: `kubectl logs <pod-name> -n orbitaos-v6`
- Verify `CONNECTION_STRING` environment variable is correct
- Ensure SQL Server is accessible from the EKS cluster

**Readiness Probe Failing**
- Verify `/health` endpoint is responding: `kubectl exec -it <pod-name> -n orbitaos-v6 -- wget -qO- http://localhost:80/health`
- Check if database migrations have run
- Increase `initialDelaySeconds` if application startup is slow

**Ingress Not Getting External IP**
- Verify AWS Load Balancer Controller is installed: `kubectl get pods -n kube-system | grep aws-load-balancer`
- Check ingress annotations are correct
- Review ALB controller logs: `kubectl logs -n kube-system -l app.kubernetes.io/name=aws-load-balancer-controller`

### Health Check

```bash
# Port-forward to test locally
kubectl port-forward svc/orbitaos-v6-service 8080:80 -n orbitaos-v6

# Test health endpoint
curl http://localhost:8080/health
```

---

## Security Considerations

1. **Non-root container**: The application runs as a non-root user (`appuser`) inside the container
2. **Secrets management**: Use Kubernetes Secrets or AWS Secrets Manager for sensitive values
3. **Network policies**: Consider adding Kubernetes NetworkPolicies to restrict pod-to-pod communication
4. **Image scanning**: Enable ECR image scanning for vulnerability detection
5. **HTTPS**: Configure TLS termination at the ALB level using ACM certificates
6. **RBAC**: Apply least-privilege RBAC policies for service accounts
7. **Pod Security**: The deployment enforces `runAsNonRoot: true` and `runAsUser: 1000`

### Enable HTTPS on ALB

Add to `ingress.yaml` annotations:
```yaml
alb.ingress.kubernetes.io/listen-ports: '[{"HTTP": 80}, {"HTTPS": 443}]'
alb.ingress.kubernetes.io/ssl-redirect: '443'
alb.ingress.kubernetes.io/certificate-arn: arn:aws:acm:<region>:<account>:certificate/<cert-id>
```

---

## .NET-Specific Notes

- **Framework**: ASP.NET Core 6.0 with MVC, Razor Pages, and ASP.NET Core Identity
- **Database**: Entity Framework Core with SQL Server provider
- **Health Checks**: Configured via `app.MapHealthChecks("/health")` in `Program.cs`
- **Static Files**: Configured with CDN-friendly cache-control headers
- **Authentication**: ASP.NET Core Identity with confirmed account requirement
- **Build**: Multi-stage Docker build using `mcr.microsoft.com/dotnet/sdk:6.0`
- **Runtime**: `mcr.microsoft.com/dotnet/sdk:6.0` (explicit base image as specified)
- **Startup Time**: Allow 60 seconds for initial liveness probe due to EF Core migrations

### Database Migrations

Run EF Core migrations before or during deployment:

```bash
# Option 1: Run migrations from local machine
dotnet ef database update --project OrbitAOS.V6 --connection "Server=prod-server;..."

# Option 2: Add migration job to Kubernetes (init container pattern)
# Add to deployment.yaml spec.initContainers:
# - name: migrate
#   image: {{IMAGE_URI}}
#   command: ["dotnet", "OrbitAOS.V6.dll", "--migrate"]
```

---

## Support

For issues with this deployment configuration, review:
- [ASP.NET Core on Kubernetes](https://docs.microsoft.com/aspnet/core/host-and-deploy/kubernetes)
- [AWS EKS Documentation](https://docs.aws.amazon.com/eks/latest/userguide/)
- [AWS Load Balancer Controller](https://kubernetes-sigs.github.io/aws-load-balancer-controller/)
