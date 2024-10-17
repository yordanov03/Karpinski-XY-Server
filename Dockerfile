## Base image for running the application in production
#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#WORKDIR /app
#EXPOSE 49746
#
## Build stage using the .NET SDK
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#
## Copy the solution file and restore dependencies for all projects
#COPY *.sln ./
#COPY KarpinskiXYServer/KarpinskiXYServer.csproj KarpinskiXYServer/
#RUN dotnet restore "KarpinskiXYServer/KarpinskiXYServer.csproj"
#
## Copy the remaining source code and build the application
#COPY . .
#WORKDIR "/src/KarpinskiXYServer"
#RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build
#
## Publish the application
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
## Final stage: setting up the runtime environment
#FROM base AS final
#WORKDIR /app
#
## Copy the published application from the previous stage
#COPY --from=publish /app/publish .
#
## Ensure the Resources/Images directory is copied
#COPY KarpinskiXYServer/Resources/Images /app/Resources/Images
#
#ENTRYPOINT ["dotnet", "KarpinskiXYServer.dll"]

# Stage 1: Base image for running the application in production
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000 

# Stage 2: Build stage using the .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the solution file and restore dependencies for all projects
COPY *.sln ./
COPY KarpinskiXYServer/KarpinskiXYServer.csproj KarpinskiXYServer/
RUN dotnet restore "KarpinskiXYServer/KarpinskiXYServer.csproj"

# Copy the remaining source code and build the application
COPY . .
WORKDIR "/src/KarpinskiXYServer"
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Final stage - setting up the runtime environment
FROM base AS final
WORKDIR /app

# Copy the published application from the publish stage
COPY --from=publish /app/publish .

# Ensure the Resources/Images directory is copied to the correct location
COPY KarpinskiXYServer/Resources/Images /app/Resources/Images

# Define the entry point for the application
ENTRYPOINT ["dotnet", "KarpinskiXYServer.dll"]

#