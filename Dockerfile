# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["OrbitAOS.V6/OrbitAOS.V6.csproj", "OrbitAOS.V6/"]
RUN dotnet restore "OrbitAOS.V6/OrbitAOS.V6.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/OrbitAOS.V6"
RUN dotnet publish "OrbitAOS.V6.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
RUN addgroup --gid 1000 appuser && \
    adduser --uid 1000 --gid 1000 --disabled-password --disabled-login appuser
COPY --from=build --chown=appuser:appuser /app/publish .
USER appuser

# Configure for container: listen on port 8080
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 8080

ENTRYPOINT ["dotnet", "OrbitAOS.V6.dll"]
