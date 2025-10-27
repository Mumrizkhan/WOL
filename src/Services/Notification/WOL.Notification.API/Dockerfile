FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5008

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Shared/WOL.Shared.Common/WOL.Shared.Common.csproj", "Shared/WOL.Shared.Common/"]

COPY ["src/Services/Notification/WOL.Notification.Domain/WOL.Notification.Domain.csproj", "Services/Notification/WOL.Notification.Domain/"]
COPY ["src/Services/Notification/WOL.Notification.Application/WOL.Notification.Application.csproj", "Services/Notification/WOL.Notification.Application/"]
COPY ["src/Services/Notification/WOL.Notification.Infrastructure/WOL.Notification.Infrastructure.csproj", "Services/Notification/WOL.Notification.Infrastructure/"]
COPY ["src/Services/Notification/WOL.Notification.API/WOL.Notification.API.csproj", "Services/Notification/WOL.Notification.API/"]

RUN dotnet restore "Services/Notification/WOL.Notification.API/WOL.Notification.API.csproj"

COPY src/Shared/ Shared/
COPY src/Services/Notification/ Services/Notification/

WORKDIR "/src/Services/Notification/WOL.Notification.API"
RUN dotnet build "WOL.Notification.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WOL.Notification.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WOL.Notification.API.dll"]
