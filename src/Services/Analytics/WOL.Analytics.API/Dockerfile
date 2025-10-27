FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5013

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Shared/WOL.Shared.Common/WOL.Shared.Common.csproj", "Shared/WOL.Shared.Common/"]

COPY ["src/Services/Analytics/WOL.Analytics.Domain/WOL.Analytics.Domain.csproj", "Services/Analytics/WOL.Analytics.Domain/"]
COPY ["src/Services/Analytics/WOL.Analytics.Application/WOL.Analytics.Application.csproj", "Services/Analytics/WOL.Analytics.Application/"]
COPY ["src/Services/Analytics/WOL.Analytics.Infrastructure/WOL.Analytics.Infrastructure.csproj", "Services/Analytics/WOL.Analytics.Infrastructure/"]
COPY ["src/Services/Analytics/WOL.Analytics.API/WOL.Analytics.API.csproj", "Services/Analytics/WOL.Analytics.API/"]

RUN dotnet restore "Services/Analytics/WOL.Analytics.API/WOL.Analytics.API.csproj"

COPY src/Shared/ Shared/
COPY src/Services/Analytics/ Services/Analytics/

WORKDIR "/src/Services/Analytics/WOL.Analytics.API"
RUN dotnet build "WOL.Analytics.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WOL.Analytics.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WOL.Analytics.API.dll"]
