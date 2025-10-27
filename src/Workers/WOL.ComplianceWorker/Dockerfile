FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Shared/WOL.Shared.Messages/WOL.Shared.Messages.csproj", "Shared/WOL.Shared.Messages/"]
COPY ["src/Workers/WOL.ComplianceWorker/WOL.ComplianceWorker.csproj", "Workers/WOL.ComplianceWorker/"]

RUN dotnet restore "Workers/WOL.ComplianceWorker/WOL.ComplianceWorker.csproj"

COPY src/Shared/ Shared/
COPY src/Workers/WOL.ComplianceWorker/ Workers/WOL.ComplianceWorker/

WORKDIR "/src/Workers/WOL.ComplianceWorker"
RUN dotnet build "WOL.ComplianceWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WOL.ComplianceWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WOL.ComplianceWorker.dll"]
