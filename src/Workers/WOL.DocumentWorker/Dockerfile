FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Shared/WOL.Shared.Messages/WOL.Shared.Messages.csproj", "Shared/WOL.Shared.Messages/"]
COPY ["src/Workers/WOL.DocumentWorker/WOL.DocumentWorker.csproj", "Workers/WOL.DocumentWorker/"]

RUN dotnet restore "Workers/WOL.DocumentWorker/WOL.DocumentWorker.csproj"

COPY src/Shared/ Shared/
COPY src/Workers/WOL.DocumentWorker/ Workers/WOL.DocumentWorker/

WORKDIR "/src/Workers/WOL.DocumentWorker"
RUN dotnet build "WOL.DocumentWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WOL.DocumentWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WOL.DocumentWorker.dll"]
