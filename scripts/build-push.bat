@echo off
setlocal enabledelayedexpansion

echo === OrbitAOS - MM Container Build and Push ===
echo.

set "PROJECT_NAME=OrbitAOS.V6"
set "IMAGE_NAME=%PROJECT_NAME%"
REM Sanitize: lowercase and replace non-alphanumeric with hyphen
for /f "delims=" %%i in ('powershell -command "$p='%PROJECT_NAME%'; ($p.ToLower() -replace '[^a-z0-9]+', '-') -replace '^-+|-+$', ''"') do set "IMAGE_NAME=%%i"

echo Select target platform:
echo   1. AWS ECR (Amazon Elastic Container Registry)
echo   2. Docker Hub
echo.
set /p PLATFORM_CHOICE="Enter choice (1 or 2): "

if "%PLATFORM_CHOICE%"=="1" (
    set /p AWS_REGION="Enter AWS region (e.g., us-east-1): "
    set /p ECR_REPO="Enter ECR repository name: "
    set /p AWS_ACCOUNT_ID="Enter AWS account ID: "
    set "REGISTRY_URL=!AWS_ACCOUNT_ID!.dkr.ecr.!AWS_REGION!.amazonaws.com"
    set "FULL_REPO=!REGISTRY_URL!/!ECR_REPO!"

    set /p IMAGE_TAG="Enter image tag (or press Enter for 'latest'): "
    if "!IMAGE_TAG!"=="" set "IMAGE_TAG=latest"
    REM Sanitize tag with PowerShell
    for /f "delims=" %%i in ('powershell -command "$t='!IMAGE_TAG!'; ($t -replace '[^a-z0-9.\-]+', '-') -replace '^-+|-+$', ''"') do set "IMAGE_TAG=%%i"

    set "FULL_IMAGE_NAME=!FULL_REPO!:!IMAGE_TAG!"

    echo.
    echo Logging in to AWS ECR...
    for /f "delims=" %%p in ('aws ecr get-login-password --region !AWS_REGION!') do set "ECR_PASSWORD=%%p"
    echo !ECR_PASSWORD! | docker login --username AWS --password-stdin !REGISTRY_URL!
    if !ERRORLEVEL! neq 0 (
        echo ECR login failed
        exit /b 1
    )

    echo.
    echo Creating ECR repository if it does not exist...
    aws ecr describe-repositories --repository-names !ECR_REPO! --region !AWS_REGION! >nul 2>&1
    if !ERRORLEVEL! neq 0 (
        echo Creating ECR repository...
        aws ecr create-repository --repository-name !ECR_REPO! --region !AWS_REGION!
    )

    echo.
    echo Building Docker image...
    echo   Image: !FULL_IMAGE_NAME!
    docker build -f Dockerfile -t "!FULL_IMAGE_NAME!" .
    if !ERRORLEVEL! neq 0 (
        echo Docker build failed
        exit /b 1
    )

    echo.
    echo Pushing Docker image...
    docker push "!FULL_IMAGE_NAME!"
    if !ERRORLEVEL! neq 0 (
        echo Docker push failed
        exit /b 1
    )

    echo.
    echo === Build and push complete ===
    echo Image: !FULL_IMAGE_NAME!

) else if "%PLATFORM_CHOICE%"=="2" (
    set /p DOCKER_USERNAME="Enter Docker Hub username: "
    set /p IMAGE_TAG="Enter image tag (or press Enter for 'latest'): "
    if "!IMAGE_TAG!"=="" set "IMAGE_TAG=latest"
    for /f "delims=" %%i in ('powershell -command "$t='!IMAGE_TAG!'; ($t -replace '[^a-z0-9.\-]+', '-') -replace '^-+|-+$', ''"') do set "IMAGE_TAG=%%i"

    set "FULL_IMAGE_NAME=!DOCKER_USERNAME!/!IMAGE_NAME!:!IMAGE_TAG!"

    set /p DOCKER_PASSWORD="Enter Docker Hub password: "

    echo.
    echo Logging in to Docker Hub...
    echo !DOCKER_PASSWORD! | docker login --username !DOCKER_USERNAME! --password-stdin
    if !ERRORLEVEL! neq 0 (
        echo Docker Hub login failed
        exit /b 1
    )

    echo.
    echo Building Docker image...
    echo   Image: !FULL_IMAGE_NAME!
    docker build -f Dockerfile -t "!FULL_IMAGE_NAME!" .
    if !ERRORLEVEL! neq 0 (
        echo Docker build failed
        exit /b 1
    )

    echo.
    echo Pushing Docker image...
    docker push "!FULL_IMAGE_NAME!"
    if !ERRORLEVEL! neq 0 (
        echo Docker push failed
        exit /b 1
    )

    echo.
    echo === Build and push complete ===
    echo Image: !FULL_IMAGE_NAME!

) else (
    echo Invalid choice. Exiting.
    exit /b 1
)

endlocal
