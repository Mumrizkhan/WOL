FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy shared projects
COPY ["src/Shared/WOL.Shared.Common/WOL.Shared.Common.csproj", "Shared/WOL.Shared.Common/"]
COPY ["src/Shared/WOL.Shared.Messages/WOL.Shared.Messages.csproj", "Shared/WOL.Shared.Messages/"]

# Copy Booking service projects
COPY ["src/Services/Booking/WOL.Booking.Domain/WOL.Booking.Domain.csproj", "Services/Booking/WOL.Booking.Domain/"]
COPY ["src/Services/Booking/WOL.Booking.Application/WOL.Booking.Application.csproj", "Services/Booking/WOL.Booking.Application/"]
COPY ["src/Services/Booking/WOL.Booking.Infrastructure/WOL.Booking.Infrastructure.csproj", "Services/Booking/WOL.Booking.Infrastructure/"]
COPY ["src/Services/Booking/WOL.Booking.API/WOL.Booking.API.csproj", "Services/Booking/WOL.Booking.API/"]

# Restore dependencies
RUN dotnet restore "Services/Booking/WOL.Booking.API/WOL.Booking.API.csproj"

# Copy all source files
COPY src/Shared/ Shared/
COPY src/Services/Booking/ Services/Booking/

# Build
WORKDIR "/src/Services/Booking/WOL.Booking.API"
RUN dotnet build "WOL.Booking.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WOL.Booking.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WOL.Booking.API.dll"]
