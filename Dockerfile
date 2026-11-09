# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["OrbitAOS.V6/OrbitAOS.V6.csproj", "OrbitAOS.V6/"]
RUN dotnet restore "OrbitAOS.V6/OrbitAOS.V6.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/OrbitAOS.V6"
RUN dotnet publish "OrbitAOS.V6.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
EXPOSE 8080

# Set environment variables for production
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    TZ=UTC

# Configure .NET garbage collector for container memory limits
ENV DOTNET_gcServer=true \
    DOTNET_gcConcurrent=true \
    DOTNET_GCHeapHardLimit=0x18000000

# Create non-root user
RUN groupadd -r appgroup && useradd -r -g appgroup -d /app -s /sbin/nologin appuser

# Copy published output
COPY --from=build /app/publish .

# Set ownership and permissions
RUN chown -R appuser:appgroup /app
USER appuser

# Health check is handled by the platform via the /health endpoint
ENTRYPOINT ["dotnet", "OrbitAOS.V6.dll"]
