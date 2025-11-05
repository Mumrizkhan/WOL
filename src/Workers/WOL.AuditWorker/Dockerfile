FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Workers/WOL.AuditWorker/WOL.AuditWorker.csproj", "src/Workers/WOL.AuditWorker/"]
COPY ["src/Shared/WOL.Shared.Messages/WOL.Shared.Messages.csproj", "src/Shared/WOL.Shared.Messages/"]
RUN dotnet restore "src/Workers/WOL.AuditWorker/WOL.AuditWorker.csproj"
COPY . .
WORKDIR "/src/src/Workers/WOL.AuditWorker"
RUN dotnet build "WOL.AuditWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WOL.AuditWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WOL.AuditWorker.dll"]
