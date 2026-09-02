@echo off
setlocal enabledelayedexpansion

:: =============================================================================
:: build-push.bat - Build and Push Docker Image for OrbitAOS.V6
:: Target Platform: Azure AKS
:: =============================================================================

echo ==============================================
echo   OrbitAOS.V6 - Docker Build ^& Push Script
echo ==============================================
echo.

set "PROJECT_NAME=orbitaos-v6"

:: Sanitize image name using PowerShell
for /f "delims=" %%i in ('powershell -Command "$n = 'orbitaos-v6'; $n = $n.ToLower() -replace '[^a-z0-9]','-'; $n = $n.Trim('-'); Write-Output $n"') do set "IMAGE_NAME=%%i"

echo Select container registry:
echo   1. Azure Container Registry (ACR)
echo   2. Docker Hub
echo.
set /p "REGISTRY_CHOICE=Enter choice [1 or 2]: "

echo.
set /p "IMAGE_TAG_INPUT=Enter image tag (press Enter for 'latest'): "

:: Sanitize tag
if "!IMAGE_TAG_INPUT!"=="" (
  set "IMAGE_TAG=latest"
) else (
  for /f "delims=" %%t in ('powershell -Command "$t = '!IMAGE_TAG_INPUT!'; $t = $t.ToLower() -replace '[^a-z0-9._-]','-'; $t = $t.Trim('-'); if ($t -eq '') { $t = 'latest' }; Write-Output $t"') do set "IMAGE_TAG=%%t"
)

echo.

if "!REGISTRY_CHOICE!"=="1" (
  :: ---- Azure Container Registry ----
  set /p "ACR_NAME=Enter ACR name (e.g., myregistry): "

  for /f "delims=" %%a in ('powershell -Command "Write-Output '!ACR_NAME!'.ToLower().Replace(' ','')"') do set "ACR_NAME=%%a"

  set "FULL_IMAGE_NAME=!ACR_NAME!.azurecr.io/!IMAGE_NAME!:!IMAGE_TAG!"

  echo.
  echo Logging in to Azure Container Registry: !ACR_NAME! ...
  az acr login --name !ACR_NAME!
  if !ERRORLEVEL! neq 0 (
    echo ERROR: ACR login failed. Ensure you are logged in with 'az login'.
    exit /b 1
  )

) else if "!REGISTRY_CHOICE!"=="2" (
  :: ---- Docker Hub ----
  set /p "DOCKER_USERNAME=Enter Docker Hub username: "
  set /p "DOCKER_PASSWORD=Enter Docker Hub password/token: "

  set "FULL_IMAGE_NAME=!DOCKER_USERNAME!/!IMAGE_NAME!:!IMAGE_TAG!"

  echo.
  echo Logging in to Docker Hub ...
  echo !DOCKER_PASSWORD! | docker login --username !DOCKER_USERNAME! --password-stdin
  if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker Hub login failed.
    exit /b 1
  )

) else (
  echo ERROR: Invalid choice. Please enter 1 or 2.
  exit /b 1
)

echo.
echo Building Docker image: !FULL_IMAGE_NAME!
echo Build context: . (repository root)
echo Dockerfile: OrbitAOS.V6\Dockerfile
echo.

docker build -f OrbitAOS.V6\Dockerfile -t "!FULL_IMAGE_NAME!" .
if !ERRORLEVEL! neq 0 (
  echo ERROR: Docker build failed.
  exit /b 1
)

echo.
echo Pushing image: !FULL_IMAGE_NAME! ...
docker push "!FULL_IMAGE_NAME!"
if !ERRORLEVEL! neq 0 (
  echo ERROR: Docker push failed.
  exit /b 1
)

echo.
echo ==============================================
echo   Build and push completed successfully!
echo   Image: !FULL_IMAGE_NAME!
echo ==============================================

endlocal
