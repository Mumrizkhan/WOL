FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5003

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Shared/WOL.Shared.Common/WOL.Shared.Common.csproj", "Shared/WOL.Shared.Common/"]
COPY ["src/Shared/WOL.Shared.Messages/WOL.Shared.Messages.csproj", "Shared/WOL.Shared.Messages/"]

COPY ["src/Services/Vehicle/WOL.Vehicle.Domain/WOL.Vehicle.Domain.csproj", "Services/Vehicle/WOL.Vehicle.Domain/"]
COPY ["src/Services/Vehicle/WOL.Vehicle.Application/WOL.Vehicle.Application.csproj", "Services/Vehicle/WOL.Vehicle.Application/"]
COPY ["src/Services/Vehicle/WOL.Vehicle.Infrastructure/WOL.Vehicle.Infrastructure.csproj", "Services/Vehicle/WOL.Vehicle.Infrastructure/"]
COPY ["src/Services/Vehicle/WOL.Vehicle.API/WOL.Vehicle.API.csproj", "Services/Vehicle/WOL.Vehicle.API/"]

RUN dotnet restore "Services/Vehicle/WOL.Vehicle.API/WOL.Vehicle.API.csproj"

COPY src/Shared/ Shared/
COPY src/Services/Vehicle/ Services/Vehicle/

WORKDIR "/src/Services/Vehicle/WOL.Vehicle.API"
RUN dotnet build "WOL.Vehicle.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WOL.Vehicle.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WOL.Vehicle.API.dll"]
