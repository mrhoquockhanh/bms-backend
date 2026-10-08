# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
# Install curl for healthcheck (as root, before switching user)
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
EXPOSE 7263
EXPOSE 7264


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
# Optimize NuGet and .NET SDK
ENV NUGET_XMLDOC_MODE=skip
ENV DOTNET_NOLOGO=true
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true
ENV DOTNET_CLI_TELEMETRY_OPTOUT=true
WORKDIR /src
# Copy only project files first for better layer caching
COPY ["Patient-service/Patient-service.csproj", "Patient-service/"]
# Restore dependencies (cached if csproj hasn't changed)
RUN dotnet restore "./Patient-service/Patient-service.csproj" --verbosity quiet
# Copy remaining files
COPY . .
WORKDIR "/src/Patient-service"
# Build without restore (already restored above)
RUN dotnet build "./Patient-service.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Patient-service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Patient-service.dll"]