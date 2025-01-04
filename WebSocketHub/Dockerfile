FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WebSocketHub/WebSocketHub.csproj", "WebSocketHub/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["Messages/Messages.csproj", "Messages/"]
COPY Directory.*.props .
RUN dotnet restore "WebSocketHub/WebSocketHub.csproj"
COPY . .
WORKDIR "/src/WebSocketHub"
RUN dotnet build "WebSocketHub.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WebSocketHub.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebSocketHub.dll"]
