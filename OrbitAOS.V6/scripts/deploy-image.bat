@echo off
setlocal enabledelayedexpansion

:: =============================================================================
:: deploy-image.bat - Deploy OrbitAOS.V6 to AWS EKS (Windows)
:: =============================================================================

set APP_NAME=orbitaos-v6
set NAMESPACE=orbitaos-v6
set SCRIPT_DIR=%~dp0
set K8S_DIR=%SCRIPT_DIR%..\kubernetes

echo ==============================================
echo   OrbitAOS.V6 - AWS EKS Deployment Script
echo ==============================================

:: Prompt for AWS region
set /p AWS_REGION="Enter AWS region (e.g. us-east-1): "
if "!AWS_REGION!"=="" (
    echo ERROR: AWS region is required.
    exit /b 1
)

:: Prompt for EKS cluster name
set /p CLUSTER_NAME="Enter EKS cluster name: "
if "!CLUSTER_NAME!"=="" (
    echo ERROR: EKS cluster name is required.
    exit /b 1
)

:: Prompt for Docker image URI
set /p IMAGE_URI="Enter full Docker image URI (e.g. 123456789.dkr.ecr.us-east-1.amazonaws.com/orbitaos-v6:latest): "
if "!IMAGE_URI!"=="" (
    echo ERROR: Docker image URI is required.
    exit /b 1
)

echo.
echo --- Optional: Application Environment Variables ---
echo Press Enter to skip any variable.

set /p CONNECTION_STRING="Enter value for CONNECTION_STRING (SQL Server connection string): "
if "!CONNECTION_STRING!"=="" set CONNECTION_STRING=Server=your-db-server;Database=OrbitAOS;User Id=sa;Password=YourStrong@Passw0rd;MultipleActiveResultSets=true

set /p ASPNETCORE_DOCS_URL="Enter value for ASPNETCORE_DOCS_URL [https://docs.microsoft.com/aspnet/core]: "
if "!ASPNETCORE_DOCS_URL!"=="" set ASPNETCORE_DOCS_URL=https://docs.microsoft.com/aspnet/core

set /p STATIC_FILES_CACHE_MAX_AGE_SECONDS="Enter value for STATIC_FILES_CACHE_MAX_AGE_SECONDS [86400]: "
if "!STATIC_FILES_CACHE_MAX_AGE_SECONDS!"=="" set STATIC_FILES_CACHE_MAX_AGE_SECONDS=86400

echo.
echo Configuring kubectl for EKS cluster: !CLUSTER_NAME! in !AWS_REGION!...
aws eks update-kubeconfig --region !AWS_REGION! --name !CLUSTER_NAME!
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to configure kubectl.
    exit /b 1
)

echo Verifying cluster connectivity...
kubectl cluster-info
if !ERRORLEVEL! neq 0 (
    echo ERROR: Cannot connect to cluster. Check your credentials and cluster name.
    exit /b 1
)

echo.
echo Updating Kubernetes manifests with deployment values...

:: Copy manifests to temp directory
copy "!K8S_DIR!\deployment.yaml" "%TEMP%\deployment.yaml" >nul
copy "!K8S_DIR!\service.yaml" "%TEMP%\service.yaml" >nul
copy "!K8S_DIR!\ingress.yaml" "%TEMP%\ingress.yaml" >nul
copy "!K8S_DIR!\namespace.yaml" "%TEMP%\namespace.yaml" >nul

:: Replace placeholders using PowerShell
powershell -Command "(Get-Content '%TEMP%\deployment.yaml') -replace '\{\{IMAGE_URI\}\}', '!IMAGE_URI!' | Set-Content '%TEMP%\deployment.yaml'"
powershell -Command "(Get-Content '%TEMP%\deployment.yaml') -replace '\{\{CONNECTION_STRING\}\}', '!CONNECTION_STRING!' | Set-Content '%TEMP%\deployment.yaml'"
powershell -Command "(Get-Content '%TEMP%\deployment.yaml') -replace '\{\{ASPNETCORE_DOCS_URL\}\}', '!ASPNETCORE_DOCS_URL!' | Set-Content '%TEMP%\deployment.yaml'"
powershell -Command "(Get-Content '%TEMP%\deployment.yaml') -replace '\{\{STATIC_FILES_CACHE_MAX_AGE_SECONDS\}\}', '!STATIC_FILES_CACHE_MAX_AGE_SECONDS!' | Set-Content '%TEMP%\deployment.yaml'"

echo.
echo Applying Kubernetes manifests...

echo   [1/4] Applying namespace...
kubectl apply -f "%TEMP%\namespace.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply namespace. & exit /b 1 )

echo   [2/4] Applying deployment...
kubectl apply -f "%TEMP%\deployment.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply deployment. & exit /b 1 )

echo   [3/4] Applying service...
kubectl apply -f "%TEMP%\service.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply service. & exit /b 1 )

echo   [4/4] Applying ingress...
kubectl apply -f "%TEMP%\ingress.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply ingress. & exit /b 1 )

echo.
echo Waiting for deployment rollout to complete...
kubectl rollout status deployment/!APP_NAME! -n !NAMESPACE! --timeout=300s
if !ERRORLEVEL! neq 0 (
    echo ERROR: Deployment rollout failed.
    echo Rollback command: kubectl rollout undo deployment/!APP_NAME! -n !NAMESPACE!
    exit /b 1
)

echo.
echo Verifying deployed resources...
kubectl get pods,svc,ingress -n !NAMESPACE!

echo.
echo ==============================================
echo   Deployment completed successfully!
echo   App: !APP_NAME!
echo   Namespace: !NAMESPACE!
echo   Image: !IMAGE_URI!
echo ==============================================
echo.
echo Rollback command (if needed):
echo   kubectl rollout undo deployment/!APP_NAME! -n !NAMESPACE!

endlocal
