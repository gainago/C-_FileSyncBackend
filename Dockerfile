# Используем .NET 10 runtime для финального образа
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Используем .NET 10 SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FileSyncBackend.csproj", "./"]
RUN dotnet restore "FileSyncBackend.csproj"
COPY . .
RUN dotnet build "FileSyncBackend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FileSyncBackend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Создаём директорию для blob-хранилища
RUN mkdir -p /data/blobs

ENTRYPOINT ["dotnet", "FileSyncBackend.dll"]