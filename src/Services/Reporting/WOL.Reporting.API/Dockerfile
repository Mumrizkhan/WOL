FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5014

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Shared/WOL.Shared.Common/WOL.Shared.Common.csproj", "Shared/WOL.Shared.Common/"]

COPY ["src/Services/Reporting/WOL.Reporting.Domain/WOL.Reporting.Domain.csproj", "Services/Reporting/WOL.Reporting.Domain/"]
COPY ["src/Services/Reporting/WOL.Reporting.Application/WOL.Reporting.Application.csproj", "Services/Reporting/WOL.Reporting.Application/"]
COPY ["src/Services/Reporting/WOL.Reporting.Infrastructure/WOL.Reporting.Infrastructure.csproj", "Services/Reporting/WOL.Reporting.Infrastructure/"]
COPY ["src/Services/Reporting/WOL.Reporting.API/WOL.Reporting.API.csproj", "Services/Reporting/WOL.Reporting.API/"]

RUN dotnet restore "Services/Reporting/WOL.Reporting.API/WOL.Reporting.API.csproj"

COPY src/Shared/ Shared/
COPY src/Services/Reporting/ Services/Reporting/

WORKDIR "/src/Services/Reporting/WOL.Reporting.API"
RUN dotnet build "WOL.Reporting.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WOL.Reporting.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WOL.Reporting.API.dll"]
