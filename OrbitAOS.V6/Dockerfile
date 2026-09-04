# =============================================================================
# Stage 1: Build
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src

# Copy project file first for dependency caching
COPY ["OrbitAOS.V6/OrbitAOS.V6.csproj", "OrbitAOS.V6/"]

# Restore dependencies
RUN dotnet restore "OrbitAOS.V6/OrbitAOS.V6.csproj"

# Copy remaining source code
COPY . .

WORKDIR "/src/OrbitAOS.V6"

# Build in Release mode
RUN dotnet build "OrbitAOS.V6.csproj" -c Release -o /app/build

# =============================================================================
# Stage 2: Publish
# =============================================================================
FROM build AS publish

RUN dotnet publish "OrbitAOS.V6.csproj" -c Release -o /app/publish /p:UseAppHost=false

# =============================================================================
# Stage 3: Runtime
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS final

WORKDIR /app

# Create non-root user for security
RUN groupadd -r appgroup && useradd -r -g appgroup -s /bin/false appuser

# Copy published output
COPY --from=publish /app/publish .

# Set ownership
RUN chown -R appuser:appgroup /app

# Switch to non-root user
USER appuser

# Expose application port
EXPOSE 80

# Set .NET environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV TZ=UTC

# Entry point
ENTRYPOINT ["dotnet", "OrbitAOS.V6.dll"]
