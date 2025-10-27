FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/ApiGateway/WOL.ApiGateway/WOL.ApiGateway.csproj", "ApiGateway/WOL.ApiGateway/"]

RUN dotnet restore "ApiGateway/WOL.ApiGateway/WOL.ApiGateway.csproj"

COPY src/ApiGateway/ ApiGateway/

WORKDIR "/src/ApiGateway/WOL.ApiGateway"
RUN dotnet build "WOL.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WOL.ApiGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WOL.ApiGateway.dll"]
