@echo off
setlocal enabledelayedexpansion

REM Deploy script for OrbitAOS.V6 to AWS EKS
REM Run from the repository root

set "PROJECT_NAME=OrbitAOS.V6"
REM Sanitize name: lowercase, non-alphanumeric to hyphen, trim leading/trailing hyphens
for /f "usebackq delims=" %%i in (`powershell -Command "$n='%PROJECT_NAME%'; ($n.ToLower() -replace '[^a-z0-9]', '-') -replace '^-+|-+$',''"`) do set "NAME=%%i"

echo === OrbitAOS.V6 Deploy to AWS EKS ===
echo.

REM Ask for cluster details
echo 1. AWS EKS Cluster Configuration
set /p AWS_REGION="Enter AWS region (e.g., us-east-1): "
set /p CLUSTER_NAME="Enter EKS cluster name: "

REM Ask for image URI
set /p IMAGE_URI="Enter full image URI (registry/repo:tag): "

REM Configure kubectl for the target cluster
echo.
echo 2. Configuring kubectl for EKS cluster: !CLUSTER_NAME!
aws eks update-kubeconfig --region !AWS_REGION! --name !CLUSTER_NAME!
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to configure kubectl
    exit /b 1
)
kubectl cluster-info
if !ERRORLEVEL! neq 0 (
    echo ERROR: kubectl cluster-info failed
    exit /b 1
)
echo kubectl configured successfully.
echo.

REM Ask for environment variables needed by the application
echo 3. Application Environment Variables
echo Enter values for the environment variables the application needs.
echo Press Enter to skip (the placeholder will remain and you must set it manually).
echo.

REM Connection string (credential - should not be echoed)
set /p DB_CONN_STR="Enter value for ConnectionStrings__DefaultConnection (database connection string): "
echo.

REM Create the Kubernetes namespace
echo 4. Creating namespace: !NAME!
kubectl apply -f kubernetes/namespace.yaml
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to create namespace
    exit /b 1
)
echo Namespace created.
echo.

REM Create the Secret with credentials
echo 5. Creating Kubernetes Secret: !NAME!-secrets
set "SECRET_KEY=connection-strings--default-connection"
kubectl create secret generic "!NAME!-secrets" -n "!NAME!" --from-literal=!SECRET_KEY!=!DB_CONN_STR! --dry-run=client -o yaml | kubectl apply -f -
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to create secret
    exit /b 1
)
echo Secret created.
echo.

REM Apply deployment with placeholder substitution
echo 6. Deploying application to EKS

REM Replace placeholders in deployment.yaml using PowerShell
powershell -Command "(Get-Content kubernetes/deployment.yaml -Raw) -replace '{{IMAGE_URI}}', '!IMAGE_URI!' | Set-Content kubernetes/deployment.yaml"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to substitute IMAGE_URI placeholder
    exit /b 1
)
powershell -Command "(Get-Content kubernetes/deployment.yaml -Raw) -replace '{{NAMESPACE}}', '!NAME!' | Set-Content kubernetes/deployment.yaml"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to substitute NAMESPACE placeholder
    exit /b 1
)

REM Apply deployment, service, and ingress
kubectl apply -f kubernetes/deployment.yaml
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to apply deployment
    exit /b 1
)
echo Deployment applied.
echo.

echo 7. Creating Service
kubectl apply -f kubernetes/service.yaml
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to create service
    exit /b 1
)
echo Service created.
echo.

echo 8. Creating Ingress
kubectl apply -f kubernetes/ingress.yaml
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to create ingress
    exit /b 1
)
echo Ingress created.
echo.

REM Wait for deployment rollout
echo 9. Waiting for deployment rollout...
kubectl rollout status deployment/!NAME! -n !NAME!
if !ERRORLEVEL! neq 0 (
    echo ERROR: Deployment rollout failed
    echo Rolling back...
    kubectl rollout undo deployment/!NAME! -n !NAME!
    exit /b 1
)
echo Deployment is ready.
echo.

REM Show resources
echo 10. Deployment Status
kubectl get pods,svc,ingress -n !NAME!
echo.

REM Show application URL
echo === Deployment Complete ===
echo Application URL: https://!NAME!.example.com
echo.
echo To check logs: kubectl logs -f deployment/!NAME! -n !NAME!
echo To rollback: kubectl rollout undo deployment/!NAME! -n !NAME!

endlocal
