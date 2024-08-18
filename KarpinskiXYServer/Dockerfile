# Base image for running the application in production
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage using the .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the .csproj file and restore dependencies
COPY "KarpinskiXYServer.csproj" "./"
RUN dotnet restore "./KarpinskiXYServer.csproj"

# Copy the remaining source code and build the application
COPY . .
WORKDIR "/src"
RUN dotnet build "./KarpinskiXYServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./KarpinskiXYServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage: setting up the runtime environment
FROM base AS final
WORKDIR /app

# Copy the published application from the previous stage
COPY --from=publish /app/publish .

# Ensure the Resources/Images directory is copied
COPY Resources/Images /app/Resources/Images

ENTRYPOINT ["dotnet", "KarpinskiXYServer.dll"]
