FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

ARG BUILDPLATFORM

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH
WORKDIR /src
COPY ["CacheUpdater/CacheUpdater.csproj", "CacheUpdater/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["Messages/Messages.csproj", "Messages/"]
COPY Directory.*.props .
RUN dotnet restore "CacheUpdater/CacheUpdater.csproj" --arch $TARGETARCH
COPY . .
WORKDIR "/src/CacheUpdater"
RUN dotnet build "CacheUpdater.csproj" --arch $TARGETARCH -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CacheUpdater.csproj" --arch $TARGETARCH -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CacheUpdater.dll"]
