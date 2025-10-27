FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Shared/WOL.Shared.Messages/WOL.Shared.Messages.csproj", "Shared/WOL.Shared.Messages/"]
COPY ["src/Workers/WOL.AnalyticsWorker/WOL.AnalyticsWorker.csproj", "Workers/WOL.AnalyticsWorker/"]

RUN dotnet restore "Workers/WOL.AnalyticsWorker/WOL.AnalyticsWorker.csproj"

COPY src/Shared/ Shared/
COPY src/Workers/WOL.AnalyticsWorker/ Workers/WOL.AnalyticsWorker/

WORKDIR "/src/Workers/WOL.AnalyticsWorker"
RUN dotnet build "WOL.AnalyticsWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WOL.AnalyticsWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WOL.AnalyticsWorker.dll"]
