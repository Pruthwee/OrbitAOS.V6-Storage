@echo off
setlocal enabledelayedexpansion

echo === OrbitAOS - MM Deployment to AWS EKS ===
echo.

set "PROJECT_NAME=OrbitAOS.V6"
for /f "delims=" %%i in ('powershell -command "$p='%PROJECT_NAME%'; ($p.ToLower() -replace '[^a-z0-9]+', '-') -replace '^-+|-+$', ''"') do set "NAME=%%i"

set /p AWS_REGION="Enter AWS region (e.g., us-east-1): "
set /p CLUSTER_NAME="Enter EKS cluster name: "
set /p IMAGE_URI="Enter image URI (full path with registry and tag): "

echo.
echo Configuring kubectl for EKS cluster: !CLUSTER_NAME! in !AWS_REGION!...
aws eks update-kubeconfig --region !AWS_REGION! --name !CLUSTER_NAME!
if !ERRORLEVEL! neq 0 (
    echo Failed to configure kubectl
    exit /b 1
)

echo.
echo Verifying cluster connectivity...
kubectl cluster-info
if !ERRORLEVEL! neq 0 (
    echo Cluster connectivity check failed
    exit /b 1
)

echo.
echo Enter values for environment variables (press Enter to skip):
set /p ENV_VALUE="Enter value for ASPNETCORE_ENVIRONMENT (or press Enter to skip): "
set /p URLS_VALUE="Enter value for ASPNETCORE_URLS (or press Enter to skip): "

echo.
echo Enter credentials for Kubernetes Secret:
set /p CONNECTION_STRING="Enter database connection string: "

echo.
echo Applying namespace...
kubectl apply -f kubernetes/namespace.yaml
if !ERRORLEVEL! neq 0 (
    echo Failed to apply namespace
    exit /b 1
)

echo.
echo Creating Kubernetes Secret for credentials...
kubectl create secret generic orbitaossmm-secrets -n orbitaossmm --from-literal=connection-string="!CONNECTION_STRING!" --dry-run=client -o yaml | kubectl apply -f -
if !ERRORLEVEL! neq 0 (
    echo Failed to create secret
    exit /b 1
)

echo.
echo Applying deployment with substituted values...
sed -i.bak "s|{{IMAGE_URI}}|!IMAGE_URI!|g" kubernetes/deployment.yaml
sed -i.bak "s|{{NAMESPACE}}|orbitaossmm|g" kubernetes/deployment.yaml
if not "!ENV_VALUE!"=="" (
    sed -i.bak "s|{{ASPNETCORE_ENVIRONMENT}}|!ENV_VALUE!|g" kubernetes/deployment.yaml
)
if not "!URLS_VALUE!"=="" (
    sed -i.bak "s|{{ASPNETCORE_URLS}}|!URLS_VALUE!|g" kubernetes/deployment.yaml
)
del kubernetes/deployment.yaml.bak 2>nul

kubectl apply -f kubernetes/deployment.yaml
if !ERRORLEVEL! neq 0 (
    echo Failed to apply deployment
    exit /b 1
)

echo.
echo Applying service...
kubectl apply -f kubernetes/service.yaml
if !ERRORLEVEL! neq 0 (
    echo Failed to apply service
    exit /b 1
)

echo.
echo Applying ingress...
kubectl apply -f kubernetes/ingress.yaml
if !ERRORLEVEL! neq 0 (
    echo Failed to apply ingress
    exit /b 1
)

echo.
echo Waiting for deployment rollout...
kubectl rollout status deployment/orbitaossmm -n orbitaossmm
if !ERRORLEVEL! neq 0 (
    echo Deployment rollout failed
    exit /b 1
)

echo.
echo === Deployment complete ===
echo.
echo Pods:
kubectl get pods -n orbitaossmm
echo.
echo Services:
kubectl get svc -n orbitaossmm
echo.
echo Ingress:
kubectl get ingress -n orbitaossmm
echo.
echo Application URL: http://orbitaossmm.example.com
echo.
echo To rollback on failure:
echo   kubectl rollout undo deployment/orbitaossmm -n orbitaossmm
echo   kubectl rollout status deployment/orbitaossmm -n orbitaossmm

endlocal
