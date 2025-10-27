FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5007

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Shared/WOL.Shared.Common/WOL.Shared.Common.csproj", "Shared/WOL.Shared.Common/"]
COPY ["src/Shared/WOL.Shared.Messages/WOL.Shared.Messages.csproj", "Shared/WOL.Shared.Messages/"]

COPY ["src/Services/Payment/WOL.Payment.Domain/WOL.Payment.Domain.csproj", "Services/Payment/WOL.Payment.Domain/"]
COPY ["src/Services/Payment/WOL.Payment.Application/WOL.Payment.Application.csproj", "Services/Payment/WOL.Payment.Application/"]
COPY ["src/Services/Payment/WOL.Payment.Infrastructure/WOL.Payment.Infrastructure.csproj", "Services/Payment/WOL.Payment.Infrastructure/"]
COPY ["src/Services/Payment/WOL.Payment.API/WOL.Payment.API.csproj", "Services/Payment/WOL.Payment.API/"]

RUN dotnet restore "Services/Payment/WOL.Payment.API/WOL.Payment.API.csproj"

COPY src/Shared/ Shared/
COPY src/Services/Payment/ Services/Payment/

WORKDIR "/src/Services/Payment/WOL.Payment.API"
RUN dotnet build "WOL.Payment.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WOL.Payment.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WOL.Payment.API.dll"]
