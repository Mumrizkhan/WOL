FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5010

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Shared/WOL.Shared.Common/WOL.Shared.Common.csproj", "Shared/WOL.Shared.Common/"]

COPY ["src/Services/Pricing/WOL.Pricing.Domain/WOL.Pricing.Domain.csproj", "Services/Pricing/WOL.Pricing.Domain/"]
COPY ["src/Services/Pricing/WOL.Pricing.Application/WOL.Pricing.Application.csproj", "Services/Pricing/WOL.Pricing.Application/"]
COPY ["src/Services/Pricing/WOL.Pricing.Infrastructure/WOL.Pricing.Infrastructure.csproj", "Services/Pricing/WOL.Pricing.Infrastructure/"]
COPY ["src/Services/Pricing/WOL.Pricing.API/WOL.Pricing.API.csproj", "Services/Pricing/WOL.Pricing.API/"]

RUN dotnet restore "Services/Pricing/WOL.Pricing.API/WOL.Pricing.API.csproj"

COPY src/Shared/ Shared/
COPY src/Services/Pricing/ Services/Pricing/

WORKDIR "/src/Services/Pricing/WOL.Pricing.API"
RUN dotnet build "WOL.Pricing.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WOL.Pricing.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WOL.Pricing.API.dll"]
