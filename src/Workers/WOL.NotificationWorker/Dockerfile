FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Shared/WOL.Shared.Messages/WOL.Shared.Messages.csproj", "Shared/WOL.Shared.Messages/"]
COPY ["src/Workers/WOL.NotificationWorker/WOL.NotificationWorker.csproj", "Workers/WOL.NotificationWorker/"]

RUN dotnet restore "Workers/WOL.NotificationWorker/WOL.NotificationWorker.csproj"

COPY src/Shared/ Shared/
COPY src/Workers/WOL.NotificationWorker/ Workers/WOL.NotificationWorker/

WORKDIR "/src/Workers/WOL.NotificationWorker"
RUN dotnet build "WOL.NotificationWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WOL.NotificationWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WOL.NotificationWorker.dll"]
