# Use the official .NET 8 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Use the .NET 8 SDK for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution file and project files first for better layer caching
COPY ["SoftwareDeveloperCase.sln", "./"]
COPY ["src/SoftwareDeveloperCase.Api/SoftwareDeveloperCase.Api.csproj", "src/SoftwareDeveloperCase.Api/"]
COPY ["src/SoftwareDeveloperCase.Application/SoftwareDeveloperCase.Application.csproj", "src/SoftwareDeveloperCase.Application/"]
COPY ["src/SoftwareDeveloperCase.Domain/SoftwareDeveloperCase.Domain.csproj", "src/SoftwareDeveloperCase.Domain/"]
COPY ["src/SoftwareDeveloperCase.Infrastructure/SoftwareDeveloperCase.Infrastructure.csproj", "src/SoftwareDeveloperCase.Infrastructure/"]

# Restore dependencies (this layer will be cached if project files don't change)
RUN dotnet restore "src/SoftwareDeveloperCase.Api/SoftwareDeveloperCase.Api.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/src/SoftwareDeveloperCase.Api"
RUN dotnet build "SoftwareDeveloperCase.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SoftwareDeveloperCase.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage - create the runtime image
FROM base AS final
WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Create a non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Health check for the application
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "SoftwareDeveloperCase.Api.dll"]
