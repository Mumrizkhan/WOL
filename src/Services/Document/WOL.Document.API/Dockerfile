FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5009

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Shared/WOL.Shared.Common/WOL.Shared.Common.csproj", "Shared/WOL.Shared.Common/"]

COPY ["src/Services/Document/WOL.Document.Domain/WOL.Document.Domain.csproj", "Services/Document/WOL.Document.Domain/"]
COPY ["src/Services/Document/WOL.Document.Application/WOL.Document.Application.csproj", "Services/Document/WOL.Document.Application/"]
COPY ["src/Services/Document/WOL.Document.Infrastructure/WOL.Document.Infrastructure.csproj", "Services/Document/WOL.Document.Infrastructure/"]
COPY ["src/Services/Document/WOL.Document.API/WOL.Document.API.csproj", "Services/Document/WOL.Document.API/"]

RUN dotnet restore "Services/Document/WOL.Document.API/WOL.Document.API.csproj"

COPY src/Shared/ Shared/
COPY src/Services/Document/ Services/Document/

WORKDIR "/src/Services/Document/WOL.Document.API"
RUN dotnet build "WOL.Document.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WOL.Document.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WOL.Document.API.dll"]
