@echo off
setlocal enabledelayedexpansion

:: =============================================================================
:: deploy-image.bat - Deploy OrbitAOS.V6 to Azure AKS
:: =============================================================================

echo ==============================================
echo   OrbitAOS.V6 - AKS Deployment Script
echo ==============================================
echo.

set "APP_NAME=orbitaos-v6"
set "NAMESPACE=orbitaos-v6"

:: Determine project directory (parent of scripts folder)
set "SCRIPT_DIR=%~dp0"
for %%i in ("%SCRIPT_DIR%..") do set "PROJECT_DIR=%%~fi"
set "DEPLOY_DIR=%PROJECT_DIR%\kubernetes"
set "TMP_DIR=%TEMP%\orbitaos-v6-deploy"

:: ---- Prompt for Azure / AKS details ----
set /p "RESOURCE_GROUP=Enter Azure Resource Group name: "
if "!RESOURCE_GROUP!"=="" (
  echo ERROR: Resource group cannot be empty.
  exit /b 1
)

set /p "CLUSTER_NAME=Enter AKS Cluster name: "
if "!CLUSTER_NAME!"=="" (
  echo ERROR: AKS cluster name cannot be empty.
  exit /b 1
)

set /p "IMAGE_URI=Enter full Docker image URI (e.g., myregistry.azurecr.io/orbitaos-v6:latest): "
if "!IMAGE_URI!"=="" (
  echo ERROR: Image URI cannot be empty.
  exit /b 1
)

echo.
echo ---- Application Environment Variables ----
echo Press Enter to skip any optional variable.
echo.

set /p "CONNECTION_STRING=Enter SQL Server connection string (ConnectionStrings__DefaultConnection): "
if "!CONNECTION_STRING!"=="" (
  set "CONNECTION_STRING=Server=^<your-sql-server^>;Database=OrbitAOSDb;User Id=^<user^>;Password=^<password^>;MultipleActiveResultSets=true"
  echo   Using placeholder connection string.
)

set /p "STATIC_CACHE_AGE=Enter static file cache max age in seconds [86400]: "
if "!STATIC_CACHE_AGE!"=="" (
  set "STATIC_CACHE_AGE=86400"
)

echo.
echo ---- Configuring kubectl for AKS ----
az aks get-credentials --resource-group !RESOURCE_GROUP! --name !CLUSTER_NAME! --overwrite-existing
if !ERRORLEVEL! neq 0 (
  echo ERROR: Failed to get AKS credentials. Check resource group and cluster name.
  exit /b 1
)

echo.
echo Verifying cluster connectivity ...
kubectl cluster-info
if !ERRORLEVEL! neq 0 (
  echo ERROR: Cannot connect to AKS cluster.
  exit /b 1
)

echo.
echo ---- Preparing manifest copies ----
if exist "!TMP_DIR!" rmdir /s /q "!TMP_DIR!"
mkdir "!TMP_DIR!"
copy "!DEPLOY_DIR!\*.yaml" "!TMP_DIR!\" >nul

echo Updating image URI in deployment manifest ...
powershell -Command "(Get-Content '!TMP_DIR!\deployment.yaml') -replace '\{\{IMAGE_URI\}\}', '!IMAGE_URI!' | Set-Content '!TMP_DIR!\deployment.yaml'"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to update IMAGE_URI. & exit /b 1 )

powershell -Command "(Get-Content '!TMP_DIR!\deployment.yaml') -replace '\{\{CONNECTION_STRING\}\}', '!CONNECTION_STRING!' | Set-Content '!TMP_DIR!\deployment.yaml'"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to update CONNECTION_STRING. & exit /b 1 )

powershell -Command "(Get-Content '!TMP_DIR!\deployment.yaml') -replace '\{\{STATIC_FILE_CACHE_MAX_AGE_SECONDS\}\}', '!STATIC_CACHE_AGE!' | Set-Content '!TMP_DIR!\deployment.yaml'"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to update STATIC_FILE_CACHE_MAX_AGE_SECONDS. & exit /b 1 )

echo Manifests updated successfully.

echo.
echo ---- Applying Kubernetes manifests ----

echo Applying namespace ...
kubectl apply -f "!TMP_DIR!\namespace.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply namespace. & exit /b 1 )

echo Applying deployment ...
kubectl apply -f "!TMP_DIR!\deployment.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply deployment. & exit /b 1 )

echo Applying service ...
kubectl apply -f "!TMP_DIR!\service.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply service. & exit /b 1 )

echo Applying ingress ...
kubectl apply -f "!TMP_DIR!\ingress.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply ingress. & exit /b 1 )

echo.
echo ---- Waiting for deployment rollout ----
kubectl rollout status deployment/!APP_NAME! -n !NAMESPACE! --timeout=300s
if !ERRORLEVEL! neq 0 (
  echo ERROR: Deployment rollout failed or timed out.
  echo Run: kubectl describe deployment/!APP_NAME! -n !NAMESPACE!
  echo Run: kubectl logs -l app=!APP_NAME! -n !NAMESPACE!
  echo.
  echo To rollback: kubectl rollout undo deployment/!APP_NAME! -n !NAMESPACE!
  rmdir /s /q "!TMP_DIR!"
  exit /b 1
)

echo.
echo ---- Verifying deployed resources ----
kubectl get pods,svc,ingress -n !NAMESPACE!

echo.
echo ---- Application Access ----
echo Application URL: http://orbitaos-v6.example.com
echo Health Check:    http://orbitaos-v6.example.com/health

echo.
echo ==============================================
echo   Deployment completed successfully!
echo   App: !APP_NAME! ^| Namespace: !NAMESPACE!
echo ==============================================

rmdir /s /q "!TMP_DIR!"
endlocal
