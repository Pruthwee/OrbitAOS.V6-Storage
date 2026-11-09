@echo off
setlocal enabledelayedexpansion

REM Build and push script for OrbitAOS.V6 to AWS ECR
REM Run from the repository root

set "PROJECT_NAME=OrbitAOS.V6"
REM Sanitize image name: lowercase, non-alphanumeric to hyphen, trim leading/trailing hyphens
set "IMAGE_NAME=%PROJECT_NAME%"
REM Convert to lowercase using PowerShell
for /f "usebackq delims=" %%i in (`powershell -Command "$n='%PROJECT_NAME%'; ($n.ToLower() -replace '[^a-z0-9]', '-') -replace '^-+|-+$',''"`) do set "IMAGE_NAME=%%i"

echo === OrbitAOS.V6 Build and Push to AWS ECR ===
echo.

REM Ask for registry details
echo 1. AWS ECR Registry Configuration
set /p AWS_REGION="Enter AWS region (e.g., us-east-1): "
set /p AWS_ACCOUNT_ID="Enter AWS account ID (e.g., 123456789012): "

set "REGISTRY_URL=!AWS_ACCOUNT_ID!.dkr.ecr.!AWS_REGION!.amazonaws.com"
set "ECR_REPO=!IMAGE_NAME!"

REM Ask for image tag
set /p IMAGE_TAG="Enter image tag (or press Enter for 'latest'): "
if "!IMAGE_TAG!"=="" set "IMAGE_TAG=latest"

REM Sanitize tag using PowerShell
for /f "usebackq delims=" %%i in (`powershell -Command "($n='%IMAGE_TAG%').ToLower() -replace '[^a-z0-9._-]','-' -replace '^-+|-+$',''"`) do set "IMAGE_TAG=%%i"
if "!IMAGE_TAG!"=="" set "IMAGE_TAG=latest"

set "FULL_IMAGE_NAME=!REGISTRY_URL!/!ECR_REPO!:!IMAGE_TAG!"

echo.
echo === Build Configuration ===
echo Project:      %PROJECT_NAME%
echo Image name:   !IMAGE_NAME!
echo Registry:     !REGISTRY_URL!
echo Repository:   !ECR_REPO!
echo Tag:          !IMAGE_TAG!
echo Full image:   !FULL_IMAGE_NAME!
echo.

REM Login to AWS ECR
echo 2. Logging in to AWS ECR...
aws ecr get-login-password --region !AWS_REGION! | docker login --username AWS --password-stdin !REGISTRY_URL!
if !ERRORLEVEL! neq 0 (
    echo ERROR: ECR login failed
    exit /b 1
)
echo ECR login successful.
echo.

REM Create ECR repository if it doesn't exist
echo 3. Checking if ECR repository exists...
aws ecr describe-repositories --repository-names !ECR_REPO! --region !AWS_REGION! >nul 2>&1
if !ERRORLEVEL! neq 0 (
    echo Creating ECR repository: !ECR_REPO!
    aws ecr create-repository --repository-name !ECR_REPO! --region !AWS_REGION!
    if !ERRORLEVEL! neq 0 (
        echo ERROR: Failed to create ECR repository
        exit /b 1
    )
    echo ECR repository created.
) else (
    echo ECR repository already exists.
)
echo.

REM Build the Docker image
echo 4. Building Docker image...
docker build -f Dockerfile -t !FULL_IMAGE_NAME! --platform linux/amd64 .
if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker build failed
    exit /b 1
)
echo Docker build successful.
echo.

REM Push the image
echo 5. Pushing Docker image to ECR...
docker push !FULL_IMAGE_NAME!
if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker push failed
    exit /b 1
)
echo.

echo === Success ===
echo Image pushed: !FULL_IMAGE_NAME!
echo Use this image URI when deploying to EKS.

endlocal
