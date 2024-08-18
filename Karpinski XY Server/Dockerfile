#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Karpinski XY Server/Karpinski XY Server.csproj", "Karpinski XY Server/"]
RUN dotnet restore "Karpinski XY Server/Karpinski XY Server.csproj"
COPY . .
WORKDIR "/src/Karpinski XY Server"
RUN dotnet build "Karpinski XY Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Karpinski XY Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Karpinski XY Server.dll"]