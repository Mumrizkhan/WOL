FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Shared/WOL.Shared.Messages/WOL.Shared.Messages.csproj", "Shared/WOL.Shared.Messages/"]
COPY ["src/Workers/WOL.BackloadWorker/WOL.BackloadWorker.csproj", "Workers/WOL.BackloadWorker/"]

RUN dotnet restore "Workers/WOL.BackloadWorker/WOL.BackloadWorker.csproj"

COPY src/Shared/ Shared/
COPY src/Workers/WOL.BackloadWorker/ Workers/WOL.BackloadWorker/

WORKDIR "/src/Workers/WOL.BackloadWorker"
RUN dotnet build "WOL.BackloadWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WOL.BackloadWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WOL.BackloadWorker.dll"]
