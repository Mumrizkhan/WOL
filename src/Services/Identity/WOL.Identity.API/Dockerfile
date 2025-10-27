FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy shared projects
COPY ["src/Shared/WOL.Shared.Common/WOL.Shared.Common.csproj", "Shared/WOL.Shared.Common/"]
COPY ["src/Shared/WOL.Shared.Messages/WOL.Shared.Messages.csproj", "Shared/WOL.Shared.Messages/"]

# Copy Identity service projects
COPY ["src/Services/Identity/WOL.Identity.Domain/WOL.Identity.Domain.csproj", "Services/Identity/WOL.Identity.Domain/"]
COPY ["src/Services/Identity/WOL.Identity.Application/WOL.Identity.Application.csproj", "Services/Identity/WOL.Identity.Application/"]
COPY ["src/Services/Identity/WOL.Identity.Infrastructure/WOL.Identity.Infrastructure.csproj", "Services/Identity/WOL.Identity.Infrastructure/"]
COPY ["src/Services/Identity/WOL.Identity.API/WOL.Identity.API.csproj", "Services/Identity/WOL.Identity.API/"]

# Restore dependencies
RUN dotnet restore "Services/Identity/WOL.Identity.API/WOL.Identity.API.csproj"

# Copy all source files
COPY src/Shared/ Shared/
COPY src/Services/Identity/ Services/Identity/

# Build
WORKDIR "/src/Services/Identity/WOL.Identity.API"
RUN dotnet build "WOL.Identity.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WOL.Identity.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WOL.Identity.API.dll"]
