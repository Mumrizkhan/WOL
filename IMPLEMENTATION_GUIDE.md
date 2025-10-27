# World of Logistics (WOL) - Implementation Guide

## Table of Contents

1. [Getting Started](#getting-started)
2. [Development Environment Setup](#development-environment-setup)
3. [Project Structure](#project-structure)
4. [Clean Architecture Implementation](#clean-architecture-implementation)
5. [Microservice Template](#microservice-template)
6. [Database Setup](#database-setup)
7. [Testing Strategy](#testing-strategy)
8. [Deployment Guide](#deployment-guide)
9. [Best Practices](#best-practices)
10. [Troubleshooting](#troubleshooting)

---

## 1. Getting Started

### Prerequisites

**Required Software:**
- .NET 8.0 SDK or later
- Node.js 20.x or later
- Docker Desktop
- PostgreSQL 16
- MongoDB 7.0
- Redis 7.x
- RabbitMQ 3.12
- Visual Studio 2022 / VS Code / Rider
- Git

**Recommended Tools:**
- Postman or Insomnia (API testing)
- pgAdmin (PostgreSQL management)
- MongoDB Compass (MongoDB management)
- Redis Insight (Redis management)
- RabbitMQ Management UI
- Azure Data Studio / DBeaver

### Clone Repository

```bash
git clone https://github.com/Mumrizkhan/WOL.git
cd WOL
```

---

## 2. Development Environment Setup

### 2.1 Backend Setup

#### Install .NET 8.0 SDK

```bash
# Windows (using winget)
winget install Microsoft.DotNet.SDK.8

# macOS (using Homebrew)
brew install dotnet@8

# Linux (Ubuntu)
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0
```

#### Install Required Tools

```bash
# Install Entity Framework Core tools
dotnet tool install --global dotnet-ef

# Install code generators
dotnet tool install --global dotnet-aspnet-codegenerator

# Install user secrets tool
dotnet tool install --global dotnet-user-secrets
```

### 2.2 Database Setup

#### PostgreSQL

```bash
# Using Docker
docker run --name wol-postgres \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=wol_dev \
  -p 5432:5432 \
  -d postgres:16-alpine

# Create databases for each service
docker exec -it wol-postgres psql -U postgres -c "CREATE DATABASE wol_identity;"
docker exec -it wol-postgres psql -U postgres -c "CREATE DATABASE wol_booking;"
docker exec -it wol-postgres psql -U postgres -c "CREATE DATABASE wol_vehicle;"
docker exec -it wol-postgres psql -U postgres -c "CREATE DATABASE wol_pricing;"
docker exec -it wol-postgres psql -U postgres -c "CREATE DATABASE wol_backload;"
docker exec -it wol-postgres psql -U postgres -c "CREATE DATABASE wol_tracking;"
docker exec -it wol-postgres psql -U postgres -c "CREATE DATABASE wol_payment;"
```

#### MongoDB

```bash
# Using Docker
docker run --name wol-mongo \
  -p 27017:27017 \
  -d mongo:7.0
```

#### Redis

```bash
# Using Docker
docker run --name wol-redis \
  -p 6379:6379 \
  -d redis:7-alpine
```

#### RabbitMQ

```bash
# Using Docker
docker run --name wol-rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  -e RABBITMQ_DEFAULT_USER=guest \
  -e RABBITMQ_DEFAULT_PASS=guest \
  -d rabbitmq:3.12-management-alpine
```

### 2.3 Frontend Setup

#### React Native

```bash
# Install React Native CLI
npm install -g react-native-cli

# Install dependencies
cd src/Mobile/WOL.Mobile
npm install

# iOS setup (macOS only)
cd ios
pod install
cd ..

# Run on iOS
npm run ios

# Run on Android
npm run android
```

#### React Web Admin

```bash
# Install dependencies
cd src/Web/WOL.Admin
npm install

# Run development server
npm run dev
```

---

## 3. Project Structure

### 3.1 Solution Structure

```
WOL/
├── src/
│   ├── ApiGateway/
│   │   └── WOL.ApiGateway/
│   ├── Services/
│   │   ├── Identity/
│   │   │   ├── WOL.Identity.API/
│   │   │   ├── WOL.Identity.Application/
│   │   │   ├── WOL.Identity.Domain/
│   │   │   └── WOL.Identity.Infrastructure/
│   │   ├── Booking/
│   │   │   ├── WOL.Booking.API/
│   │   │   ├── WOL.Booking.Application/
│   │   │   ├── WOL.Booking.Domain/
│   │   │   └── WOL.Booking.Infrastructure/
│   │   ├── Vehicle/
│   │   ├── Pricing/
│   │   ├── Backload/
│   │   ├── Tracking/
│   │   ├── Payment/
│   │   ├── Notification/
│   │   ├── Document/
│   │   ├── Compliance/
│   │   ├── Analytics/
│   │   └── Reporting/
│   ├── BuildingBlocks/
│   │   ├── WOL.Common/
│   │   ├── WOL.EventBus/
│   │   ├── WOL.EventBus.RabbitMQ/
│   │   └── WOL.Logging/
│   ├── Mobile/
│   │   └── WOL.Mobile/
│   └── Web/
│       └── WOL.Admin/
├── tests/
│   ├── UnitTests/
│   ├── IntegrationTests/
│   └── E2ETests/
├── docs/
├── scripts/
└── docker-compose.yml
```

### 3.2 Microservice Structure (Example: Booking Service)

```
WOL.Booking/
├── WOL.Booking.API/
│   ├── Controllers/
│   │   └── BookingsController.cs
│   ├── Filters/
│   │   ├── ValidationFilter.cs
│   │   └── ExceptionFilter.cs
│   ├── Middleware/
│   │   └── RequestLoggingMiddleware.cs
│   ├── Extensions/
│   │   └── ServiceCollectionExtensions.cs
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
├── WOL.Booking.Application/
│   ├── Commands/
│   │   ├── CreateBooking/
│   │   │   ├── CreateBookingCommand.cs
│   │   │   ├── CreateBookingCommandHandler.cs
│   │   │   └── CreateBookingCommandValidator.cs
│   │   ├── CancelBooking/
│   │   └── UpdateBookingStatus/
│   ├── Queries/
│   │   ├── GetBookingById/
│   │   │   ├── GetBookingByIdQuery.cs
│   │   │   └── GetBookingByIdQueryHandler.cs
│   │   ├── GetCustomerBookings/
│   │   └── GetAvailableTrips/
│   ├── DTOs/
│   │   ├── BookingDto.cs
│   │   └── CreateBookingDto.cs
│   ├── Mappings/
│   │   └── BookingMappingProfile.cs
│   ├── Interfaces/
│   │   └── IBookingService.cs
│   ├── Services/
│   │   └── BookingService.cs
│   └── DependencyInjection.cs
├── WOL.Booking.Domain/
│   ├── Entities/
│   │   ├── Booking.cs
│   │   ├── BookingStatusHistory.cs
│   │   └── BookingCancellation.cs
│   ├── ValueObjects/
│   │   ├── Location.cs
│   │   ├── CargoDetails.cs
│   │   └── ContactInfo.cs
│   ├── Enums/
│   │   ├── BookingStatus.cs
│   │   └── BookingType.cs
│   ├── Events/
│   │   ├── BookingCreatedEvent.cs
│   │   ├── BookingAssignedEvent.cs
│   │   └── BookingCompletedEvent.cs
│   ├── Exceptions/
│   │   ├── BookingNotFoundException.cs
│   │   └── InvalidBookingStateException.cs
│   └── Interfaces/
│       └── IBookingRepository.cs
└── WOL.Booking.Infrastructure/
    ├── Data/
    │   ├── BookingDbContext.cs
    │   ├── Configurations/
    │   │   └── BookingConfiguration.cs
    │   └── Migrations/
    ├── Repositories/
    │   └── BookingRepository.cs
    ├── Services/
    │   └── ExternalServices/
    └── DependencyInjection.cs
```

---

## 4. Clean Architecture Implementation

### 4.1 Domain Layer

The Domain layer contains the business logic and is the core of the application.

#### Entity Example

```csharp
// WOL.Booking.Domain/Entities/Booking.cs
using WOL.Booking.Domain.ValueObjects;
using WOL.Booking.Domain.Enums;
using WOL.Booking.Domain.Events;

namespace WOL.Booking.Domain.Entities
{
    public class Booking : BaseEntity
    {
        public Guid Id { get; private set; }
        public string BookingNumber { get; private set; }
        public Guid CustomerId { get; private set; }
        public Guid? VehicleId { get; private set; }
        public Guid? DriverId { get; private set; }
        public Guid VehicleTypeId { get; private set; }
        
        public Location Origin { get; private set; }
        public Location Destination { get; private set; }
        
        public DateTime PickupDate { get; private set; }
        public TimeSpan PickupTime { get; private set; }
        public bool IsWholeDayBooking { get; private set; }
        public bool IsFlexibleDate { get; private set; }
        
        public CargoDetails Cargo { get; private set; }
        
        public ContactInfo Shipper { get; private set; }
        public ContactInfo Receiver { get; private set; }
        
        public BookingType BookingType { get; private set; }
        public bool IsBackload { get; private set; }
        public bool IsSharedLoad { get; private set; }
        
        public decimal BaseFare { get; private set; }
        public decimal DiscountAmount { get; private set; }
        public decimal WaitingCharges { get; private set; }
        public decimal TotalFare { get; private set; }
        public decimal VatAmount { get; private set; }
        public decimal FinalAmount { get; private set; }
        
        public BookingStatus Status { get; private set; }
        
        public DateTime? DriverAssignedAt { get; private set; }
        public DateTime? DriverAcceptedAt { get; private set; }
        public DateTime? DriverReachedAt { get; private set; }
        public DateTime? LoadingStartedAt { get; private set; }
        public DateTime? LoadingCompletedAt { get; private set; }
        public DateTime? TripStartedAt { get; private set; }
        public DateTime? DeliveredAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public DateTime? CancelledAt { get; private set; }
        
        public int FreeTimeMinutes { get; private set; }
        public DateTime? FreeTimeEndAt { get; private set; }
        
        private readonly List<DomainEvent> _domainEvents = new();
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();
        
        // Private constructor for EF Core
        private Booking() { }
        
        // Factory method
        public static Booking Create(
            Guid customerId,
            Guid vehicleTypeId,
            Location origin,
            Location destination,
            DateTime pickupDate,
            TimeSpan pickupTime,
            CargoDetails cargo,
            ContactInfo shipper,
            ContactInfo receiver,
            BookingType bookingType,
            decimal totalFare)
        {
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                BookingNumber = GenerateBookingNumber(),
                CustomerId = customerId,
                VehicleTypeId = vehicleTypeId,
                Origin = origin,
                Destination = destination,
                PickupDate = pickupDate,
                PickupTime = pickupTime,
                Cargo = cargo,
                Shipper = shipper,
                Receiver = receiver,
                BookingType = bookingType,
                TotalFare = totalFare,
                Status = BookingStatus.Pending,
                FreeTimeMinutes = 120,
                CreatedAt = DateTime.UtcNow
            };
            
            booking.AddDomainEvent(new BookingCreatedEvent(booking));
            
            return booking;
        }
        
        public void AssignDriver(Guid vehicleId, Guid driverId)
        {
            if (Status != BookingStatus.Pending)
                throw new InvalidBookingStateException("Cannot assign driver to non-pending booking");
            
            VehicleId = vehicleId;
            DriverId = driverId;
            Status = BookingStatus.DriverAssigned;
            DriverAssignedAt = DateTime.UtcNow;
            
            AddDomainEvent(new BookingAssignedEvent(Id, vehicleId, driverId));
        }
        
        public void Accept()
        {
            if (Status != BookingStatus.DriverAssigned)
                throw new InvalidBookingStateException("Cannot accept booking in current state");
            
            Status = BookingStatus.Accepted;
            DriverAcceptedAt = DateTime.UtcNow;
            
            AddDomainEvent(new BookingAcceptedEvent(Id));
        }
        
        public void MarkAsReached()
        {
            if (Status != BookingStatus.Accepted)
                throw new InvalidBookingStateException("Cannot mark as reached in current state");
            
            Status = BookingStatus.DriverReached;
            DriverReachedAt = DateTime.UtcNow;
            FreeTimeEndAt = DateTime.UtcNow.AddMinutes(FreeTimeMinutes);
            
            AddDomainEvent(new DriverReachedEvent(Id, DriverReachedAt.Value));
        }
        
        public void MarkAsLoaded()
        {
            if (Status != BookingStatus.DriverReached)
                throw new InvalidBookingStateException("Cannot mark as loaded in current state");
            
            LoadingCompletedAt = DateTime.UtcNow;
            TripStartedAt = DateTime.UtcNow;
            Status = BookingStatus.InTransit;
            
            // Calculate waiting charges if applicable
            if (FreeTimeEndAt.HasValue && LoadingCompletedAt > FreeTimeEndAt)
            {
                var waitingTime = LoadingCompletedAt.Value - FreeTimeEndAt.Value;
                var waitingHours = Math.Ceiling(waitingTime.TotalHours);
                WaitingCharges = (decimal)waitingHours * 100; // SAR 100 per hour
                FinalAmount = TotalFare + WaitingCharges + VatAmount;
            }
            
            AddDomainEvent(new LoadingCompletedEvent(Id, TripStartedAt.Value));
        }
        
        public void MarkAsDelivered()
        {
            if (Status != BookingStatus.InTransit)
                throw new InvalidBookingStateException("Cannot mark as delivered in current state");
            
            DeliveredAt = DateTime.UtcNow;
            Status = BookingStatus.Delivered;
            
            AddDomainEvent(new DeliveryCompletedEvent(Id, DeliveredAt.Value));
        }
        
        public void Complete()
        {
            if (Status != BookingStatus.Delivered)
                throw new InvalidBookingStateException("Cannot complete booking in current state");
            
            CompletedAt = DateTime.UtcNow;
            Status = BookingStatus.Completed;
            
            AddDomainEvent(new BookingCompletedEvent(Id, CompletedAt.Value));
        }
        
        public void Cancel(string reason)
        {
            if (Status == BookingStatus.Completed || Status == BookingStatus.Cancelled)
                throw new InvalidBookingStateException("Cannot cancel completed or already cancelled booking");
            
            CancelledAt = DateTime.UtcNow;
            Status = BookingStatus.Cancelled;
            
            AddDomainEvent(new BookingCancelledEvent(Id, reason, CancelledAt.Value));
        }
        
        private void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }
        
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
        
        private static string GenerateBookingNumber()
        {
            return $"WOL-{DateTime.UtcNow.Year}-{Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()}";
        }
    }
}
```

#### Value Object Example

```csharp
// WOL.Booking.Domain/ValueObjects/Location.cs
namespace WOL.Booking.Domain.ValueObjects
{
    public class Location : ValueObject
    {
        public string Address { get; private set; }
        public decimal Latitude { get; private set; }
        public decimal Longitude { get; private set; }
        public string City { get; private set; }
        
        private Location() { }
        
        public Location(string address, decimal latitude, decimal longitude, string city)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Address cannot be empty", nameof(address));
            
            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("Latitude must be between -90 and 90", nameof(latitude));
            
            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("Longitude must be between -180 and 180", nameof(longitude));
            
            Address = address;
            Latitude = latitude;
            Longitude = longitude;
            City = city;
        }
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Latitude;
            yield return Longitude;
        }
    }
}
```

### 4.2 Application Layer

The Application layer contains use cases and orchestrates the domain logic.

#### Command Example

```csharp
// WOL.Booking.Application/Commands/CreateBooking/CreateBookingCommand.cs
using MediatR;

namespace WOL.Booking.Application.Commands.CreateBooking
{
    public class CreateBookingCommand : IRequest<CreateBookingResponse>
    {
        public Guid CustomerId { get; set; }
        public Guid VehicleTypeId { get; set; }
        public LocationDto Origin { get; set; }
        public LocationDto Destination { get; set; }
        public DateTime PickupDate { get; set; }
        public TimeSpan PickupTime { get; set; }
        public bool IsWholeDayBooking { get; set; }
        public bool IsFlexibleDate { get; set; }
        public CargoDetailsDto Cargo { get; set; }
        public ContactInfoDto Shipper { get; set; }
        public ContactInfoDto Receiver { get; set; }
        public BookingType BookingType { get; set; }
        public bool IsBackload { get; set; }
        public bool IsSharedLoad { get; set; }
    }
    
    public class CreateBookingResponse
    {
        public Guid BookingId { get; set; }
        public string BookingNumber { get; set; }
        public BookingStatus Status { get; set; }
        public PricingDto Pricing { get; set; }
        public decimal EstimatedDistance { get; set; }
        public int EstimatedDuration { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
```

#### Command Handler Example

```csharp
// WOL.Booking.Application/Commands/CreateBooking/CreateBookingCommandHandler.cs
using MediatR;
using WOL.Booking.Domain.Entities;
using WOL.Booking.Domain.Interfaces;
using WOL.Booking.Application.Interfaces;

namespace WOL.Booking.Application.Commands.CreateBooking
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, CreateBookingResponse>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IPricingService _pricingService;
        private readonly IComplianceService _complianceService;
        private readonly IEventBus _eventBus;
        private readonly ILogger<CreateBookingCommandHandler> _logger;
        
        public CreateBookingCommandHandler(
            IBookingRepository bookingRepository,
            IPricingService pricingService,
            IComplianceService complianceService,
            IEventBus eventBus,
            ILogger<CreateBookingCommandHandler> logger)
        {
            _bookingRepository = bookingRepository;
            _pricingService = pricingService;
            _complianceService = complianceService;
            _eventBus = eventBus;
            _logger = logger;
        }
        
        public async Task<CreateBookingResponse> Handle(
            CreateBookingCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating booking for customer {CustomerId}", request.CustomerId);
            
            // Check BAN time compliance
            var isAllowed = await _complianceService.IsBookingAllowedAsync(
                request.Origin.City,
                request.VehicleTypeId,
                request.PickupDate.Add(request.PickupTime),
                cancellationToken);
            
            if (!isAllowed)
            {
                throw new BookingNotAllowedException("Booking not allowed during BAN hours");
            }
            
            // Calculate pricing
            var pricing = await _pricingService.CalculateFareAsync(new PricingRequest
            {
                VehicleTypeId = request.VehicleTypeId,
                Origin = request.Origin,
                Destination = request.Destination,
                PickupDateTime = request.PickupDate.Add(request.PickupTime),
                IsBackload = request.IsBackload,
                IsFlexibleDate = request.IsFlexibleDate,
                IsSharedLoad = request.IsSharedLoad,
                CustomerId = request.CustomerId
            }, cancellationToken);
            
            // Create booking entity
            var booking = Domain.Entities.Booking.Create(
                request.CustomerId,
                request.VehicleTypeId,
                new Location(
                    request.Origin.Address,
                    request.Origin.Latitude,
                    request.Origin.Longitude,
                    request.Origin.City),
                new Location(
                    request.Destination.Address,
                    request.Destination.Latitude,
                    request.Destination.Longitude,
                    request.Destination.City),
                request.PickupDate,
                request.PickupTime,
                new CargoDetails(
                    request.Cargo.Type,
                    request.Cargo.GrossWeightKg,
                    request.Cargo.Dimensions,
                    request.Cargo.NumberOfBoxes,
                    request.Cargo.ImageUrl),
                new ContactInfo(
                    request.Shipper.Name,
                    request.Shipper.Mobile,
                    request.Shipper.AlternateMobile),
                new ContactInfo(
                    request.Receiver.Name,
                    request.Receiver.Mobile,
                    request.Receiver.AlternateMobile),
                request.BookingType,
                pricing.TotalFare);
            
            // Save to repository
            await _bookingRepository.AddAsync(booking, cancellationToken);
            await _bookingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            
            // Publish domain events
            foreach (var domainEvent in booking.DomainEvents)
            {
                await _eventBus.PublishAsync(domainEvent, cancellationToken);
            }
            
            booking.ClearDomainEvents();
            
            _logger.LogInformation("Booking created successfully: {BookingId}", booking.Id);
            
            return new CreateBookingResponse
            {
                BookingId = booking.Id,
                BookingNumber = booking.BookingNumber,
                Status = booking.Status,
                Pricing = new PricingDto
                {
                    BaseFare = booking.BaseFare,
                    DiscountAmount = booking.DiscountAmount,
                    TotalFare = booking.TotalFare,
                    VatAmount = booking.VatAmount,
                    FinalAmount = booking.FinalAmount
                },
                EstimatedDistance = pricing.EstimatedDistance,
                EstimatedDuration = pricing.EstimatedDuration,
                CreatedAt = booking.CreatedAt
            };
        }
    }
}
```

#### Command Validator Example

```csharp
// WOL.Booking.Application/Commands/CreateBooking/CreateBookingCommandValidator.cs
using FluentValidation;

namespace WOL.Booking.Application.Commands.CreateBooking
{
    public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
    {
        public CreateBookingCommandValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty()
                .WithMessage("Customer ID is required");
            
            RuleFor(x => x.VehicleTypeId)
                .NotEmpty()
                .WithMessage("Vehicle type is required");
            
            RuleFor(x => x.Origin)
                .NotNull()
                .WithMessage("Origin location is required");
            
            RuleFor(x => x.Origin.Address)
                .NotEmpty()
                .MaximumLength(500)
                .When(x => x.Origin != null);
            
            RuleFor(x => x.Origin.Latitude)
                .InclusiveBetween(-90, 90)
                .When(x => x.Origin != null);
            
            RuleFor(x => x.Origin.Longitude)
                .InclusiveBetween(-180, 180)
                .When(x => x.Origin != null);
            
            RuleFor(x => x.Destination)
                .NotNull()
                .WithMessage("Destination location is required");
            
            RuleFor(x => x.PickupDate)
                .GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage("Pickup date must be today or in the future");
            
            RuleFor(x => x.Shipper)
                .NotNull()
                .WithMessage("Shipper information is required");
            
            RuleFor(x => x.Shipper.Mobile)
                .Matches(@"^(05|5)[0-9]{8}$")
                .WithMessage("Invalid Saudi mobile number")
                .When(x => x.Shipper != null);
            
            RuleFor(x => x.Receiver)
                .NotNull()
                .WithMessage("Receiver information is required");
            
            RuleFor(x => x.Cargo)
                .NotNull()
                .WithMessage("Cargo details are required");
            
            RuleFor(x => x.Cargo.GrossWeightKg)
                .GreaterThan(0)
                .WithMessage("Cargo weight must be greater than 0")
                .When(x => x.Cargo != null);
        }
    }
}
```

#### Query Example

```csharp
// WOL.Booking.Application/Queries/GetBookingById/GetBookingByIdQuery.cs
using MediatR;

namespace WOL.Booking.Application.Queries.GetBookingById
{
    public class GetBookingByIdQuery : IRequest<BookingDetailDto>
    {
        public Guid BookingId { get; set; }
        public Guid RequesterId { get; set; }
        
        public GetBookingByIdQuery(Guid bookingId, Guid requesterId)
        {
            BookingId = bookingId;
            RequesterId = requesterId;
        }
    }
}
```

#### Query Handler Example

```csharp
// WOL.Booking.Application/Queries/GetBookingById/GetBookingByIdQueryHandler.cs
using MediatR;
using AutoMapper;
using WOL.Booking.Domain.Interfaces;

namespace WOL.Booking.Application.Queries.GetBookingById
{
    public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, BookingDetailDto>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetBookingByIdQueryHandler> _logger;
        
        public GetBookingByIdQueryHandler(
            IBookingRepository bookingRepository,
            IMapper mapper,
            ILogger<GetBookingByIdQueryHandler> logger)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
            _logger = logger;
        }
        
        public async Task<BookingDetailDto> Handle(
            GetBookingByIdQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting booking {BookingId}", request.BookingId);
            
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
            
            if (booking == null)
            {
                throw new BookingNotFoundException(request.BookingId);
            }
            
            // Check authorization
            if (booking.CustomerId != request.RequesterId && booking.DriverId != request.RequesterId)
            {
                throw new UnauthorizedAccessException("You don't have permission to view this booking");
            }
            
            return _mapper.Map<BookingDetailDto>(booking);
        }
    }
}
```

### 4.3 Infrastructure Layer

The Infrastructure layer contains implementations of interfaces defined in the Domain and Application layers.

#### DbContext Example

```csharp
// WOL.Booking.Infrastructure/Data/BookingDbContext.cs
using Microsoft.EntityFrameworkCore;
using WOL.Booking.Domain.Entities;

namespace WOL.Booking.Infrastructure.Data
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<Domain.Entities.Booking> Bookings { get; set; }
        public DbSet<BookingStatusHistory> BookingStatusHistories { get; set; }
        public DbSet<BookingCancellation> BookingCancellations { get; set; }
        public DbSet<WaitingChargeLog> WaitingChargeLogs { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookingDbContext).Assembly);
        }
        
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Set audit fields
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && 
                    (e.State == EntityState.Added || e.State == EntityState.Modified));
            
            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;
                
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }
                
                entity.UpdatedAt = DateTime.UtcNow;
            }
            
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
```

#### Entity Configuration Example

```csharp
// WOL.Booking.Infrastructure/Data/Configurations/BookingConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WOL.Booking.Domain.Entities;

namespace WOL.Booking.Infrastructure.Data.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Domain.Entities.Booking>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Booking> builder)
        {
            builder.ToTable("bookings");
            
            builder.HasKey(b => b.Id);
            
            builder.Property(b => b.BookingNumber)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.HasIndex(b => b.BookingNumber)
                .IsUnique();
            
            builder.Property(b => b.CustomerId)
                .IsRequired();
            
            builder.HasIndex(b => b.CustomerId);
            
            // Value object - Location (Origin)
            builder.OwnsOne(b => b.Origin, origin =>
            {
                origin.Property(l => l.Address)
                    .HasColumnName("origin_address")
                    .HasMaxLength(500)
                    .IsRequired();
                
                origin.Property(l => l.Latitude)
                    .HasColumnName("origin_lat")
                    .HasPrecision(10, 8)
                    .IsRequired();
                
                origin.Property(l => l.Longitude)
                    .HasColumnName("origin_lng")
                    .HasPrecision(11, 8)
                    .IsRequired();
                
                origin.Property(l => l.City)
                    .HasColumnName("origin_city")
                    .HasMaxLength(100)
                    .IsRequired();
            });
            
            // Value object - Location (Destination)
            builder.OwnsOne(b => b.Destination, destination =>
            {
                destination.Property(l => l.Address)
                    .HasColumnName("destination_address")
                    .HasMaxLength(500)
                    .IsRequired();
                
                destination.Property(l => l.Latitude)
                    .HasColumnName("destination_lat")
                    .HasPrecision(10, 8)
                    .IsRequired();
                
                destination.Property(l => l.Longitude)
                    .HasColumnName("destination_lng")
                    .HasPrecision(11, 8)
                    .IsRequired();
                
                destination.Property(l => l.City)
                    .HasColumnName("destination_city")
                    .HasMaxLength(100)
                    .IsRequired();
            });
            
            // Value object - CargoDetails
            builder.OwnsOne(b => b.Cargo, cargo =>
            {
                cargo.Property(c => c.Type)
                    .HasColumnName("cargo_type")
                    .HasMaxLength(50);
                
                cargo.Property(c => c.GrossWeightKg)
                    .HasColumnName("gross_weight_kg")
                    .HasPrecision(10, 2);
                
                cargo.Property(c => c.NumberOfBoxes)
                    .HasColumnName("number_of_boxes");
                
                cargo.Property(c => c.ImageUrl)
                    .HasColumnName("cargo_image_url");
            });
            
            // Value object - ContactInfo (Shipper)
            builder.OwnsOne(b => b.Shipper, shipper =>
            {
                shipper.Property(c => c.Name)
                    .HasColumnName("shipper_name")
                    .HasMaxLength(255);
                
                shipper.Property(c => c.Mobile)
                    .HasColumnName("shipper_mobile")
                    .HasMaxLength(20);
                
                shipper.Property(c => c.AlternateMobile)
                    .HasColumnName("shipper_alternate_mobile")
                    .HasMaxLength(20);
            });
            
            // Value object - ContactInfo (Receiver)
            builder.OwnsOne(b => b.Receiver, receiver =>
            {
                receiver.Property(c => c.Name)
                    .HasColumnName("receiver_name")
                    .HasMaxLength(255);
                
                receiver.Property(c => c.Mobile)
                    .HasColumnName("receiver_mobile")
                    .HasMaxLength(20);
                
                receiver.Property(c => c.AlternateMobile)
                    .HasColumnName("receiver_alternate_mobile")
                    .HasMaxLength(20);
            });
            
            // Enums
            builder.Property(b => b.BookingType)
                .HasConversion<string>()
                .HasMaxLength(20);
            
            builder.Property(b => b.Status)
                .HasConversion<string>()
                .HasMaxLength(50);
            
            builder.HasIndex(b => b.Status);
            
            // Pricing
            builder.Property(b => b.BaseFare)
                .HasPrecision(10, 2);
            
            builder.Property(b => b.DiscountAmount)
                .HasPrecision(10, 2);
            
            builder.Property(b => b.WaitingCharges)
                .HasPrecision(10, 2);
            
            builder.Property(b => b.TotalFare)
                .HasPrecision(10, 2);
            
            builder.Property(b => b.VatAmount)
                .HasPrecision(10, 2);
            
            builder.Property(b => b.FinalAmount)
                .HasPrecision(10, 2);
            
            // Timestamps
            builder.Property(b => b.CreatedAt)
                .IsRequired();
            
            builder.Property(b => b.UpdatedAt)
                .IsRequired();
            
            // Ignore domain events
            builder.Ignore(b => b.DomainEvents);
        }
    }
}
```

#### Repository Example

```csharp
// WOL.Booking.Infrastructure/Repositories/BookingRepository.cs
using Microsoft.EntityFrameworkCore;
using WOL.Booking.Domain.Entities;
using WOL.Booking.Domain.Interfaces;
using WOL.Booking.Infrastructure.Data;

namespace WOL.Booking.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingDbContext _context;
        
        public IUnitOfWork UnitOfWork => _context;
        
        public BookingRepository(BookingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public async Task<Domain.Entities.Booking> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }
        
        public async Task<Domain.Entities.Booking> GetByBookingNumberAsync(
            string bookingNumber,
            CancellationToken cancellationToken = default)
        {
            return await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingNumber == bookingNumber, cancellationToken);
        }
        
        public async Task<List<Domain.Entities.Booking>> GetCustomerBookingsAsync(
            Guid customerId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            return await _context.Bookings
                .Where(b => b.CustomerId == customerId)
                .OrderByDescending(b => b.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }
        
        public async Task<List<Domain.Entities.Booking>> GetDriverBookingsAsync(
            Guid driverId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            return await _context.Bookings
                .Where(b => b.DriverId == driverId)
                .OrderByDescending(b => b.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }
        
        public async Task<List<Domain.Entities.Booking>> GetAvailableTripsAsync(
            Guid vehicleTypeId,
            string city,
            DateTime? date,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Bookings
                .Where(b => b.Status == BookingStatus.Pending);
            
            if (vehicleTypeId != Guid.Empty)
            {
                query = query.Where(b => b.VehicleTypeId == vehicleTypeId);
            }
            
            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(b => b.Origin.City == city);
            }
            
            if (date.HasValue)
            {
                query = query.Where(b => b.PickupDate.Date == date.Value.Date);
            }
            
            return await query
                .OrderBy(b => b.PickupDate)
                .ThenBy(b => b.PickupTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }
        
        public async Task<Domain.Entities.Booking> AddAsync(
            Domain.Entities.Booking booking,
            CancellationToken cancellationToken = default)
        {
            return (await _context.Bookings.AddAsync(booking, cancellationToken)).Entity;
        }
        
        public void Update(Domain.Entities.Booking booking)
        {
            _context.Entry(booking).State = EntityState.Modified;
        }
        
        public void Delete(Domain.Entities.Booking booking)
        {
            _context.Bookings.Remove(booking);
        }
    }
}
```

### 4.4 API Layer

The API layer exposes the application functionality through HTTP endpoints.

#### Controller Example

```csharp
// WOL.Booking.API/Controllers/BookingsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using WOL.Booking.Application.Commands.CreateBooking;
using WOL.Booking.Application.Queries.GetBookingById;

namespace WOL.Booking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BookingsController> _logger;
        
        public BookingsController(
            IMediator mediator,
            ILogger<BookingsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        
        /// <summary>
        /// Create a new booking
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CreateBookingResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateBooking(
            [FromBody] CreateBookingCommand command,
            CancellationToken cancellationToken)
        {
            try
            {
                // Get customer ID from claims
                var customerId = Guid.Parse(User.FindFirst("sub")?.Value 
                    ?? throw new UnauthorizedAccessException());
                
                command.CustomerId = customerId;
                
                var result = await _mediator.Send(command, cancellationToken);
                
                return CreatedAtAction(
                    nameof(GetBooking),
                    new { id = result.BookingId },
                    ApiResponse<CreateBookingResponse>.Success(
                        result,
                        "Booking created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                return BadRequest(ApiResponse.Error("BOOK_001", ex.Message));
            }
        }
        
        /// <summary>
        /// Get booking by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<BookingDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBooking(
            Guid id,
            CancellationToken cancellationToken)
        {
            try
            {
                var requesterId = Guid.Parse(User.FindFirst("sub")?.Value 
                    ?? throw new UnauthorizedAccessException());
                
                var query = new GetBookingByIdQuery(id, requesterId);
                var result = await _mediator.Send(query, cancellationToken);
                
                return Ok(ApiResponse<BookingDetailDto>.Success(result));
            }
            catch (BookingNotFoundException ex)
            {
                return NotFound(ApiResponse.Error("BOOK_003", ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting booking {BookingId}", id);
                return BadRequest(ApiResponse.Error("BOOK_001", ex.Message));
            }
        }
        
        /// <summary>
        /// Get customer bookings
        /// </summary>
        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<BookingDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCustomerBookings(
            Guid customerId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var query = new GetCustomerBookingsQuery(customerId, page, pageSize);
                var result = await _mediator.Send(query, cancellationToken);
                
                return Ok(ApiResponse<PagedResult<BookingDto>>.Success(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer bookings");
                return BadRequest(ApiResponse.Error("BOOK_001", ex.Message));
            }
        }
        
        /// <summary>
        /// Accept booking (Driver only)
        /// </summary>
        [HttpPost("{id}/accept")]
        [Authorize(Policy = "DriverOnly")]
        [ProducesResponseType(typeof(ApiResponse<AcceptBookingResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> AcceptBooking(
            Guid id,
            CancellationToken cancellationToken)
        {
            try
            {
                var driverId = Guid.Parse(User.FindFirst("sub")?.Value 
                    ?? throw new UnauthorizedAccessException());
                
                var command = new AcceptBookingCommand(id, driverId);
                var result = await _mediator.Send(command, cancellationToken);
                
                return Ok(ApiResponse<AcceptBookingResponse>.Success(
                    result,
                    "Booking accepted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting booking {BookingId}", id);
                return BadRequest(ApiResponse.Error("BOOK_001", ex.Message));
            }
        }
        
        /// <summary>
        /// Cancel booking
        /// </summary>
        [HttpPost("{id}/cancel")]
        [ProducesResponseType(typeof(ApiResponse<CancelBookingResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CancelBooking(
            Guid id,
            [FromBody] CancelBookingRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("sub")?.Value 
                    ?? throw new UnauthorizedAccessException());
                
                var command = new CancelBookingCommand(id, userId, request.Reason);
                var result = await _mediator.Send(command, cancellationToken);
                
                return Ok(ApiResponse<CancelBookingResponse>.Success(
                    result,
                    "Booking cancelled successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling booking {BookingId}", id);
                return BadRequest(ApiResponse.Error("BOOK_004", ex.Message));
            }
        }
    }
}
```

#### Program.cs Example

```csharp
// WOL.Booking.API/Program.cs
using WOL.Booking.API.Extensions;
using WOL.Booking.Application;
using WOL.Booking.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add application services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add authentication
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddAuthorization(builder.Configuration);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add health checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection"))
    .AddRabbitMQ(builder.Configuration["RabbitMQ:ConnectionString"]);

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();
```

---

## 5. Microservice Template

To create a new microservice, follow this template structure:

### Step 1: Create Solution Structure

```bash
# Create solution
dotnet new sln -n WOL.NewService

# Create projects
dotnet new webapi -n WOL.NewService.API
dotnet new classlib -n WOL.NewService.Application
dotnet new classlib -n WOL.NewService.Domain
dotnet new classlib -n WOL.NewService.Infrastructure

# Add projects to solution
dotnet sln add WOL.NewService.API
dotnet sln add WOL.NewService.Application
dotnet sln add WOL.NewService.Domain
dotnet sln add WOL.NewService.Infrastructure

# Add project references
cd WOL.NewService.API
dotnet add reference ../WOL.NewService.Application

cd ../WOL.NewService.Application
dotnet add reference ../WOL.NewService.Domain

cd ../WOL.NewService.Infrastructure
dotnet add reference ../WOL.NewService.Domain
dotnet add reference ../WOL.NewService.Application
```

### Step 2: Install NuGet Packages

```bash
# Domain project
cd WOL.NewService.Domain
# No external dependencies - pure domain logic

# Application project
cd ../WOL.NewService.Application
dotnet add package MediatR
dotnet add package FluentValidation
dotnet add package AutoMapper
dotnet add package Microsoft.Extensions.Logging.Abstractions

# Infrastructure project
cd ../WOL.NewService.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package MassTransit.RabbitMQ
dotnet add package StackExchange.Redis

# API project
cd ../WOL.NewService.API
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Swashbuckle.AspNetCore
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Elasticsearch
```

### Step 3: Implement Domain Layer

Create entities, value objects, enums, and domain events following the examples above.

### Step 4: Implement Application Layer

Create commands, queries, handlers, validators, and DTOs following the CQRS pattern.

### Step 5: Implement Infrastructure Layer

Create DbContext, repositories, and external service integrations.

### Step 6: Implement API Layer

Create controllers, middleware, and configure services.

---

## 6. Worker Services Implementation

Worker Services are .NET background services that handle asynchronous tasks, RabbitMQ message processing, scheduled jobs, and long-running operations. This section provides a complete implementation guide for creating Worker Services.

### 6.1 Worker Service Project Structure

```
src/Workers/Notification/
├── WOL.Notification.Worker.csproj
├── Program.cs
├── NotificationWorker.cs
├── Consumers/
│   ├── SendNotificationConsumer.cs
│   ├── BookingCreatedConsumer.cs
│   └── DocumentExpiringConsumer.cs
├── Services/
│   ├── INotificationService.cs
│   ├── NotificationService.cs
│   ├── IPushNotificationService.cs
│   ├── PushNotificationService.cs
│   ├── ISmsService.cs
│   ├── SmsService.cs
│   ├── IEmailService.cs
│   └── EmailService.cs
├── Configuration/
│   ├── NotificationWorkerSettings.cs
│   ├── FirebaseSettings.cs
│   ├── TwilioSettings.cs
│   └── SendGridSettings.cs
├── appsettings.json
├── appsettings.Development.json
└── Dockerfile
```

### 6.2 Create Worker Service Project

```bash
# Navigate to Workers directory
cd src/Workers

# Create new Worker Service project
dotnet new worker -n WOL.Notification.Worker

cd WOL.Notification.Worker

# Add required packages
dotnet add package MassTransit.RabbitMQ
dotnet add package Microsoft.Extensions.Hosting
dotnet add package Serilog.Extensions.Hosting
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
dotnet add package MongoDB.Driver
dotnet add package StackExchange.Redis
dotnet add package FirebaseAdmin
dotnet add package Twilio
dotnet add package SendGrid
```

### 6.3 Implement Worker Service

**Program.cs:**

```csharp
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Serilog;
using StackExchange.Redis;
using WOL.Notification.Worker;
using WOL.Notification.Worker.Configuration;
using WOL.Notification.Worker.Consumers;
using WOL.Notification.Worker.Services;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/notification-worker-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting Notification Worker Service");

    var host = Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureServices((context, services) =>
        {
            var configuration = context.Configuration;

            // Add configuration settings
            services.Configure<NotificationWorkerSettings>(
                configuration.GetSection("NotificationWorker"));
            services.Configure<FirebaseSettings>(
                configuration.GetSection("Firebase"));
            services.Configure<TwilioSettings>(
                configuration.GetSection("Twilio"));
            services.Configure<SendGridSettings>(
                configuration.GetSection("SendGrid"));

            // Add MongoDB
            services.AddSingleton<IMongoClient>(sp =>
            {
                var connectionString = configuration["MongoDB:ConnectionString"];
                return new MongoClient(connectionString);
            });
            services.AddScoped(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                var databaseName = configuration["MongoDB:DatabaseName"];
                return client.GetDatabase(databaseName);
            });

            // Add Redis
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var connectionString = configuration["Redis:ConnectionString"];
                return ConnectionMultiplexer.Connect(connectionString);
            });

            // Add notification services
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IPushNotificationService, PushNotificationService>();
            services.AddScoped<ISmsService, SmsService>();
            services.AddScoped<IEmailService, EmailService>();

            // Add MassTransit with RabbitMQ
            services.AddMassTransit(x =>
            {
                // Add consumers
                x.AddConsumer<SendNotificationConsumer>();
                x.AddConsumer<BookingCreatedConsumer>();
                x.AddConsumer<DocumentExpiringConsumer>();

                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(configuration["RabbitMQ:Host"], h =>
                    {
                        h.Username(configuration["RabbitMQ:Username"]);
                        h.Password(configuration["RabbitMQ:Password"]);
                    });

                    // Configure notification queue
                    cfg.ReceiveEndpoint("notification-queue", e =>
                    {
                        e.ConfigureConsumer<SendNotificationConsumer>(ctx);
                        e.PrefetchCount = 10;
                        e.UseMessageRetry(r => r.Exponential(
                            3,
                            TimeSpan.FromSeconds(5),
                            TimeSpan.FromMinutes(5),
                            TimeSpan.FromSeconds(5)));
                    });

                    // Configure booking events queue
                    cfg.ReceiveEndpoint("notification-booking-events", e =>
                    {
                        e.ConfigureConsumer<BookingCreatedConsumer>(ctx);
                        e.PrefetchCount = 10;
                    });

                    // Configure document events queue
                    cfg.ReceiveEndpoint("notification-document-events", e =>
                    {
                        e.ConfigureConsumer<DocumentExpiringConsumer>(ctx);
                        e.PrefetchCount = 5;
                    });
                });
            });

            // Add hosted service
            services.AddHostedService<NotificationWorker>();
        })
        .Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Notification Worker Service terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
```

**NotificationWorker.cs:**

```csharp
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WOL.Notification.Worker;

public class NotificationWorker : BackgroundService
{
    private readonly IBusControl _busControl;
    private readonly ILogger<NotificationWorker> _logger;

    public NotificationWorker(
        IBusControl busControl,
        ILogger<NotificationWorker> logger)
    {
        _busControl = busControl;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Notification Worker starting at: {time}", DateTimeOffset.Now);

        try
        {
            await _busControl.StartAsync(stoppingToken);
            _logger.LogInformation("Notification Worker started successfully");

            // Keep the worker running
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Notification Worker is stopping due to cancellation");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error in Notification Worker");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Notification Worker stopping at: {time}", DateTimeOffset.Now);

        try
        {
            await _busControl.StopAsync(cancellationToken);
            _logger.LogInformation("Notification Worker stopped successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping Notification Worker");
        }

        await base.StopAsync(cancellationToken);
    }
}
```

### 6.4 Implement Message Consumers

**SendNotificationConsumer.cs:**

```csharp
using MassTransit;
using Microsoft.Extensions.Logging;
using WOL.Notification.Worker.Services;
using WOL.Shared.Messages;

namespace WOL.Notification.Worker.Consumers;

public class SendNotificationConsumer : IConsumer<SendNotificationCommand>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<SendNotificationConsumer> _logger;

    public SendNotificationConsumer(
        INotificationService notificationService,
        ILogger<SendNotificationConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendNotificationCommand> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Processing notification for user {UserId}, Type: {Type}",
            message.UserId,
            message.Type);

        try
        {
            await _notificationService.SendAsync(
                message.UserId,
                message.Title,
                message.Body,
                message.Data,
                message.Channels,
                context.CancellationToken);

            _logger.LogInformation(
                "Notification sent successfully for user {UserId}",
                message.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to send notification for user {UserId}",
                message.UserId);

            // Throw to trigger retry
            throw;
        }
    }
}
```

**BookingCreatedConsumer.cs:**

```csharp
using MassTransit;
using Microsoft.Extensions.Logging;
using WOL.Notification.Worker.Services;
using WOL.Shared.Events;

namespace WOL.Notification.Worker.Consumers;

public class BookingCreatedConsumer : IConsumer<BookingCreatedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<BookingCreatedConsumer> _logger;

    public BookingCreatedConsumer(
        INotificationService notificationService,
        ILogger<BookingCreatedConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BookingCreatedEvent> context)
    {
        var @event = context.Message;

        _logger.LogInformation(
            "Processing BookingCreatedEvent for booking {BookingId}",
            @event.BookingId);

        try
        {
            // Send notification to customer
            await _notificationService.SendAsync(
                @event.CustomerId,
                "Booking Confirmed",
                $"Your booking #{@event.BookingNumber} has been confirmed.",
                new Dictionary<string, string>
                {
                    ["bookingId"] = @event.BookingId.ToString(),
                    ["bookingNumber"] = @event.BookingNumber,
                    ["type"] = "booking_created"
                },
                new[] { "push", "sms" },
                context.CancellationToken);

            _logger.LogInformation(
                "Notification sent for booking {BookingId}",
                @event.BookingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to send notification for booking {BookingId}",
                @event.BookingId);

            throw;
        }
    }
}
```

### 6.5 Implement Notification Services

**INotificationService.cs:**

```csharp
namespace WOL.Notification.Worker.Services;

public interface INotificationService
{
    Task SendAsync(
        Guid userId,
        string title,
        string body,
        Dictionary<string, string>? data = null,
        string[]? channels = null,
        CancellationToken cancellationToken = default);
}
```

**NotificationService.cs:**

```csharp
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace WOL.Notification.Worker.Services;

public class NotificationService : INotificationService
{
    private readonly IPushNotificationService _pushService;
    private readonly ISmsService _smsService;
    private readonly IEmailService _emailService;
    private readonly IMongoDatabase _database;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IPushNotificationService pushService,
        ISmsService smsService,
        IEmailService emailService,
        IMongoDatabase database,
        ILogger<NotificationService> logger)
    {
        _pushService = pushService;
        _smsService = smsService;
        _emailService = emailService;
        _database = database;
        _logger = logger;
    }

    public async Task SendAsync(
        Guid userId,
        string title,
        string body,
        Dictionary<string, string>? data = null,
        string[]? channels = null,
        CancellationToken cancellationToken = default)
    {
        channels ??= new[] { "push" };

        var tasks = new List<Task>();

        // Send push notification
        if (channels.Contains("push"))
        {
            tasks.Add(SendPushNotificationAsync(userId, title, body, data, cancellationToken));
        }

        // Send SMS
        if (channels.Contains("sms"))
        {
            tasks.Add(SendSmsAsync(userId, body, cancellationToken));
        }

        // Send email
        if (channels.Contains("email"))
        {
            tasks.Add(SendEmailAsync(userId, title, body, cancellationToken));
        }

        await Task.WhenAll(tasks);

        // Log notification to MongoDB
        await LogNotificationAsync(userId, title, body, channels, cancellationToken);
    }

    private async Task SendPushNotificationAsync(
        Guid userId,
        string title,
        string body,
        Dictionary<string, string>? data,
        CancellationToken cancellationToken)
    {
        try
        {
            await _pushService.SendAsync(userId, title, body, data, cancellationToken);
            _logger.LogInformation("Push notification sent to user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send push notification to user {UserId}", userId);
        }
    }

    private async Task SendSmsAsync(
        Guid userId,
        string message,
        CancellationToken cancellationToken)
    {
        try
        {
            await _smsService.SendAsync(userId, message, cancellationToken);
            _logger.LogInformation("SMS sent to user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send SMS to user {UserId}", userId);
        }
    }

    private async Task SendEmailAsync(
        Guid userId,
        string subject,
        string body,
        CancellationToken cancellationToken)
    {
        try
        {
            await _emailService.SendAsync(userId, subject, body, cancellationToken);
            _logger.LogInformation("Email sent to user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to user {UserId}", userId);
        }
    }

    private async Task LogNotificationAsync(
        Guid userId,
        string title,
        string body,
        string[] channels,
        CancellationToken cancellationToken)
    {
        var collection = _database.GetCollection<NotificationLog>("notification_logs");

        var log = new NotificationLog
        {
            UserId = userId,
            Title = title,
            Body = body,
            Channels = channels,
            SentAt = DateTime.UtcNow
        };

        await collection.InsertOneAsync(log, cancellationToken: cancellationToken);
    }
}

public class NotificationLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string[] Channels { get; set; } = Array.Empty<string>();
    public DateTime SentAt { get; set; }
}
```

### 6.6 Configuration Files

**appsettings.json:**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "NotificationWorker": {
    "MaxConcurrentMessages": 10,
    "RetryAttempts": 3,
    "RetryDelaySeconds": 5,
    "BatchSize": 100
  },
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "wol_notifications"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  },
  "Firebase": {
    "ProjectId": "wol-app",
    "CredentialsPath": "/secrets/firebase-credentials.json"
  },
  "Twilio": {
    "AccountSid": "",
    "AuthToken": "",
    "FromNumber": ""
  },
  "SendGrid": {
    "ApiKey": ""
  }
}
```

### 6.7 Dockerfile for Worker Service

```dockerfile
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["WOL.Notification.Worker.csproj", "./"]
RUN dotnet restore "WOL.Notification.Worker.csproj"
COPY . .
RUN dotnet build "WOL.Notification.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WOL.Notification.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WOL.Notification.Worker.dll"]
```

### 6.8 Running Worker Service

**Local Development:**

```bash
# Run worker service
cd src/Workers/Notification
dotnet run

# Run with specific environment
dotnet run --environment Development
```

**Docker:**

```bash
# Build Docker image
docker build -t wol/notification-worker:latest .

# Run container
docker run -d \
  --name notification-worker \
  -e DOTNET_ENVIRONMENT=Development \
  -e RabbitMQ__Host=rabbitmq \
  -e MongoDB__ConnectionString=mongodb://mongodb:27017 \
  wol/notification-worker:latest
```

**Docker Compose:**

```bash
# Start all services including workers
docker-compose up -d

# View worker logs
docker-compose logs -f notification-worker

# Scale workers
docker-compose up -d --scale notification-worker=3
```

### 6.9 Testing Worker Services

**Unit Test Example:**

```csharp
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using WOL.Notification.Worker.Services;

namespace WOL.Notification.Worker.Tests;

public class NotificationServiceTests
{
    [Fact]
    public async Task SendAsync_WithPushChannel_ShouldCallPushService()
    {
        // Arrange
        var mockPushService = new Mock<IPushNotificationService>();
        var mockSmsService = new Mock<ISmsService>();
        var mockEmailService = new Mock<IEmailService>();
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockLogger = new Mock<ILogger<NotificationService>>();

        var service = new NotificationService(
            mockPushService.Object,
            mockSmsService.Object,
            mockEmailService.Object,
            mockDatabase.Object,
            mockLogger.Object);

        var userId = Guid.NewGuid();
        var title = "Test Title";
        var body = "Test Body";
        var channels = new[] { "push" };

        // Act
        await service.SendAsync(userId, title, body, null, channels);

        // Assert
        mockPushService.Verify(
            x => x.SendAsync(userId, title, body, null, It.IsAny<CancellationToken>()),
            Times.Once);
        mockSmsService.Verify(
            x => x.SendAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
```

### 6.10 Monitoring Worker Services

**Health Checks:**

Add health checks to monitor worker service status:

```csharp
// In Program.cs
services.AddHealthChecks()
    .AddRabbitMQ(configuration["RabbitMQ:Host"])
    .AddMongoDb(configuration["MongoDB:ConnectionString"])
    .AddRedis(configuration["Redis:ConnectionString"]);
```

**Metrics:**

Use Prometheus metrics to monitor worker performance:

```csharp
services.AddSingleton<IMetricsService, MetricsService>();

// Track metrics
_metricsService.IncrementNotificationsSent();
_metricsService.RecordProcessingTime(duration);
```

---

## 7. Database Setup

### 6.1 Create Migration

```bash
cd src/Services/Booking/WOL.Booking.Infrastructure

dotnet ef migrations add InitialCreate \
  --project WOL.Booking.Infrastructure.csproj \
  --startup-project ../WOL.Booking.API/WOL.Booking.API.csproj \
  --context BookingDbContext \
  --output-dir Data/Migrations
```

### 6.2 Apply Migration

```bash
dotnet ef database update \
  --project WOL.Booking.Infrastructure.csproj \
  --startup-project ../WOL.Booking.API/WOL.Booking.API.csproj \
  --context BookingDbContext
```

### 6.3 Generate SQL Script

```bash
dotnet ef migrations script \
  --project WOL.Booking.Infrastructure.csproj \
  --startup-project ../WOL.Booking.API/WOL.Booking.API.csproj \
  --context BookingDbContext \
  --output migration.sql
```

---

## 7. Testing Strategy

### 7.1 Unit Tests

```csharp
// tests/UnitTests/WOL.Booking.UnitTests/Domain/BookingTests.cs
using Xunit;
using WOL.Booking.Domain.Entities;
using WOL.Booking.Domain.ValueObjects;
using WOL.Booking.Domain.Enums;

namespace WOL.Booking.UnitTests.Domain
{
    public class BookingTests
    {
        [Fact]
        public void Create_WithValidData_ShouldCreateBooking()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var vehicleTypeId = Guid.NewGuid();
            var origin = new Location("Address 1", 24.7136m, 46.6753m, "Riyadh");
            var destination = new Location("Address 2", 21.5433m, 39.1728m, "Jeddah");
            var pickupDate = DateTime.Today.AddDays(1);
            var pickupTime = new TimeSpan(8, 0, 0);
            var cargo = new CargoDetails("Dry", 5000, null, 50, null);
            var shipper = new ContactInfo("Ahmed", "+966501234567", null);
            var receiver = new ContactInfo("Mohammed", "+966502345678", null);
            var totalFare = 2800m;
            
            // Act
            var booking = Domain.Entities.Booking.Create(
                customerId,
                vehicleTypeId,
                origin,
                destination,
                pickupDate,
                pickupTime,
                cargo,
                shipper,
                receiver,
                BookingType.OneWay,
                totalFare);
            
            // Assert
            Assert.NotNull(booking);
            Assert.NotEqual(Guid.Empty, booking.Id);
            Assert.Equal(customerId, booking.CustomerId);
            Assert.Equal(BookingStatus.Pending, booking.Status);
            Assert.NotEmpty(booking.BookingNumber);
            Assert.Single(booking.DomainEvents);
        }
        
        [Fact]
        public void AssignDriver_WhenPending_ShouldUpdateStatus()
        {
            // Arrange
            var booking = CreateTestBooking();
            var vehicleId = Guid.NewGuid();
            var driverId = Guid.NewGuid();
            
            // Act
            booking.AssignDriver(vehicleId, driverId);
            
            // Assert
            Assert.Equal(BookingStatus.DriverAssigned, booking.Status);
            Assert.Equal(vehicleId, booking.VehicleId);
            Assert.Equal(driverId, booking.DriverId);
            Assert.NotNull(booking.DriverAssignedAt);
        }
        
        [Fact]
        public void AssignDriver_WhenNotPending_ShouldThrowException()
        {
            // Arrange
            var booking = CreateTestBooking();
            booking.AssignDriver(Guid.NewGuid(), Guid.NewGuid());
            
            // Act & Assert
            Assert.Throws<InvalidBookingStateException>(() =>
                booking.AssignDriver(Guid.NewGuid(), Guid.NewGuid()));
        }
        
        private Domain.Entities.Booking CreateTestBooking()
        {
            return Domain.Entities.Booking.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                new Location("Address 1", 24.7136m, 46.6753m, "Riyadh"),
                new Location("Address 2", 21.5433m, 39.1728m, "Jeddah"),
                DateTime.Today.AddDays(1),
                new TimeSpan(8, 0, 0),
                new CargoDetails("Dry", 5000, null, 50, null),
                new ContactInfo("Ahmed", "+966501234567", null),
                new ContactInfo("Mohammed", "+966502345678", null),
                BookingType.OneWay,
                2800m);
        }
    }
}
```

### 7.2 Integration Tests

```csharp
// tests/IntegrationTests/WOL.Booking.IntegrationTests/API/BookingsControllerTests.cs
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace WOL.Booking.IntegrationTests.API
{
    public class BookingsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        
        public BookingsControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }
        
        [Fact]
        public async Task CreateBooking_WithValidData_ShouldReturn201()
        {
            // Arrange
            var command = new CreateBookingCommand
            {
                VehicleTypeId = Guid.NewGuid(),
                Origin = new LocationDto
                {
                    Address = "King Fahd Road, Riyadh",
                    Latitude = 24.7136m,
                    Longitude = 46.6753m,
                    City = "Riyadh"
                },
                Destination = new LocationDto
                {
                    Address = "Corniche Road, Jeddah",
                    Latitude = 21.5433m,
                    Longitude = 39.1728m,
                    City = "Jeddah"
                },
                PickupDate = DateTime.Today.AddDays(1),
                PickupTime = new TimeSpan(8, 0, 0),
                Cargo = new CargoDetailsDto
                {
                    Type = "Dry",
                    GrossWeightKg = 5000,
                    NumberOfBoxes = 50
                },
                Shipper = new ContactInfoDto
                {
                    Name = "Ahmed Ali",
                    Mobile = "+966501234567"
                },
                Receiver = new ContactInfoDto
                {
                    Name = "Mohammed Hassan",
                    Mobile = "+966502345678"
                },
                BookingType = BookingType.OneWay
            };
            
            // Act
            var response = await _client.PostAsJsonAsync("/api/bookings", command);
            
            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<CreateBookingResponse>>();
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotEqual(Guid.Empty, result.Data.BookingId);
        }
    }
}
```

---

## 8. Deployment Guide

### 8.1 Docker Build

```bash
# Build Docker image
docker build -t wol/booking-service:latest \
  -f src/Services/Booking/WOL.Booking.API/Dockerfile .

# Run container
docker run -d \
  --name booking-service \
  -p 5002:80 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="Host=postgres;Database=wol_booking;Username=postgres;Password=postgres" \
  wol/booking-service:latest
```

### 8.2 Kubernetes Deployment

```bash
# Apply deployment
kubectl apply -f k8s/booking-service-deployment.yaml

# Check status
kubectl get pods -n wol

# View logs
kubectl logs -f deployment/booking-service -n wol

# Scale deployment
kubectl scale deployment booking-service --replicas=5 -n wol
```

---

## 9. Best Practices

### 9.1 Code Organization
- Follow Clean Architecture principles
- Keep domain logic in the Domain layer
- Use CQRS for complex operations
- Implement repository pattern for data access
- Use dependency injection

### 9.2 Error Handling
- Use custom exceptions for domain errors
- Implement global exception handling middleware
- Log all errors with context
- Return meaningful error messages to clients

### 9.3 Performance
- Use async/await for I/O operations
- Implement caching where appropriate
- Use pagination for list endpoints
- Optimize database queries
- Use connection pooling

### 9.4 Security
- Always validate input
- Use parameterized queries
- Implement authentication and authorization
- Never log sensitive data
- Use HTTPS in production

### 9.5 Testing
- Write unit tests for domain logic
- Write integration tests for API endpoints
- Aim for high code coverage
- Use test-driven development (TDD)
- Mock external dependencies

---

## 10. Troubleshooting

### Common Issues

#### Database Connection Failed
```bash
# Check connection string
# Verify PostgreSQL is running
docker ps | grep postgres

# Test connection
psql -h localhost -U postgres -d wol_booking
```

#### Migration Failed
```bash
# Drop database and recreate
dotnet ef database drop --force
dotnet ef database update
```

#### RabbitMQ Connection Failed
```bash
# Check RabbitMQ is running
docker ps | grep rabbitmq

# Access management UI
# http://localhost:15672 (guest/guest)
```

#### Service Not Starting
```bash
# Check logs
docker logs booking-service

# Check environment variables
docker inspect booking-service
```

---

## Additional Resources

- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)

---

## Support

For implementation support:
- Email: dev-support@wol.sa
- Slack: #wol-development
- Wiki: https://wiki.wol.sa
