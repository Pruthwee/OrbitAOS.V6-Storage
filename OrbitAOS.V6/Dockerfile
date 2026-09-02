# =============================================================================
# Stage 1: Build Stage
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS builder

WORKDIR /src

# Copy project file first for dependency caching
COPY ["OrbitAOS.V6/OrbitAOS.V6.csproj", "OrbitAOS.V6/"]

# Restore NuGet dependencies (cached layer)
RUN dotnet restore "OrbitAOS.V6/OrbitAOS.V6.csproj"

# Copy remaining source code
COPY . .

WORKDIR "/src/OrbitAOS.V6"

# Build and publish in Release configuration
RUN dotnet build "OrbitAOS.V6.csproj" -c Release -o /app/build
RUN dotnet publish "OrbitAOS.V6.csproj" -c Release -o /app/publish --no-restore

# =============================================================================
# Stage 2: Runtime Stage
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS runtime

# Set working directory
WORKDIR /app

# Create non-root user for security
RUN addgroup --system --gid 1001 appgroup && \
    adduser --system --uid 1001 --ingroup appgroup --no-create-home appuser

# Copy published artifacts from build stage
COPY --from=builder /app/publish .

# Set ownership to non-root user
RUN chown -R appuser:appgroup /app

# Switch to non-root user
USER appuser

# Configure .NET environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV TZ=UTC

# Expose application port
EXPOSE 80

# Entry point
ENTRYPOINT ["dotnet", "OrbitAOS.V6.dll"]
