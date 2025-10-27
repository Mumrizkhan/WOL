FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5011

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Shared/WOL.Shared.Common/WOL.Shared.Common.csproj", "Shared/WOL.Shared.Common/"]

COPY ["src/Services/Backload/WOL.Backload.Domain/WOL.Backload.Domain.csproj", "Services/Backload/WOL.Backload.Domain/"]
COPY ["src/Services/Backload/WOL.Backload.Application/WOL.Backload.Application.csproj", "Services/Backload/WOL.Backload.Application/"]
COPY ["src/Services/Backload/WOL.Backload.Infrastructure/WOL.Backload.Infrastructure.csproj", "Services/Backload/WOL.Backload.Infrastructure/"]
COPY ["src/Services/Backload/WOL.Backload.API/WOL.Backload.API.csproj", "Services/Backload/WOL.Backload.API/"]

RUN dotnet restore "Services/Backload/WOL.Backload.API/WOL.Backload.API.csproj"

COPY src/Shared/ Shared/
COPY src/Services/Backload/ Services/Backload/

WORKDIR "/src/Services/Backload/WOL.Backload.API"
RUN dotnet build "WOL.Backload.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WOL.Backload.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WOL.Backload.API.dll"]
