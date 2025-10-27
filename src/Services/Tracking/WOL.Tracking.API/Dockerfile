FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5006

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Shared/WOL.Shared.Common/WOL.Shared.Common.csproj", "Shared/WOL.Shared.Common/"]

COPY ["src/Services/Tracking/WOL.Tracking.Domain/WOL.Tracking.Domain.csproj", "Services/Tracking/WOL.Tracking.Domain/"]
COPY ["src/Services/Tracking/WOL.Tracking.Application/WOL.Tracking.Application.csproj", "Services/Tracking/WOL.Tracking.Application/"]
COPY ["src/Services/Tracking/WOL.Tracking.Infrastructure/WOL.Tracking.Infrastructure.csproj", "Services/Tracking/WOL.Tracking.Infrastructure/"]
COPY ["src/Services/Tracking/WOL.Tracking.API/WOL.Tracking.API.csproj", "Services/Tracking/WOL.Tracking.API/"]

RUN dotnet restore "Services/Tracking/WOL.Tracking.API/WOL.Tracking.API.csproj"

COPY src/Shared/ Shared/
COPY src/Services/Tracking/ Services/Tracking/

WORKDIR "/src/Services/Tracking/WOL.Tracking.API"
RUN dotnet build "WOL.Tracking.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WOL.Tracking.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WOL.Tracking.API.dll"]
