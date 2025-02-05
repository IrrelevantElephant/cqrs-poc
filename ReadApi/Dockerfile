FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ARG BUILDPLATFORM

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH
WORKDIR /src
COPY ["ReadApi/ReadApi.csproj", "ReadApi/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["Messages/Messages.csproj", "Messages/"]
COPY Directory.*.props .
RUN dotnet restore "ReadApi/ReadApi.csproj" --arch $TARGETARCH
COPY . .
WORKDIR "/src/ReadApi"
RUN dotnet build "ReadApi.csproj" --arch $TARGETARCH -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ReadApi.csproj" --arch $TARGETARCH -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReadApi.dll"]
