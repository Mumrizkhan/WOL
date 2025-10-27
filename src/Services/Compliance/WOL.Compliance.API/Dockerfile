FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5012

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Shared/WOL.Shared.Common/WOL.Shared.Common.csproj", "Shared/WOL.Shared.Common/"]

COPY ["src/Services/Compliance/WOL.Compliance.Domain/WOL.Compliance.Domain.csproj", "Services/Compliance/WOL.Compliance.Domain/"]
COPY ["src/Services/Compliance/WOL.Compliance.Application/WOL.Compliance.Application.csproj", "Services/Compliance/WOL.Compliance.Application/"]
COPY ["src/Services/Compliance/WOL.Compliance.Infrastructure/WOL.Compliance.Infrastructure.csproj", "Services/Compliance/WOL.Compliance.Infrastructure/"]
COPY ["src/Services/Compliance/WOL.Compliance.API/WOL.Compliance.API.csproj", "Services/Compliance/WOL.Compliance.API/"]

RUN dotnet restore "Services/Compliance/WOL.Compliance.API/WOL.Compliance.API.csproj"

COPY src/Shared/ Shared/
COPY src/Services/Compliance/ Services/Compliance/

WORKDIR "/src/Services/Compliance/WOL.Compliance.API"
RUN dotnet build "WOL.Compliance.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WOL.Compliance.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WOL.Compliance.API.dll"]
