# World of Logistics (WOL) - Enterprise System Architecture

## Executive Summary

This document outlines the comprehensive enterprise architecture for the World of Logistics (WOL) platform - a logistics marketplace connecting shippers with carriers in Saudi Arabia. The system implements a microservices architecture using .NET Core APIs, React Native mobile applications, React web admin portal, PostgreSQL for transactional data, MongoDB for analytics and logs, and RabbitMQ for asynchronous messaging.

## Table of Contents

1. [System Overview](#system-overview)
2. [Architecture Principles](#architecture-principles)
3. [Microservices Architecture](#microservices-architecture)
4. [Database Design](#database-design)
5. [Messaging Architecture](#messaging-architecture)
6. [API Gateway & Authentication](#api-gateway--authentication)
7. [Mobile Application Architecture](#mobile-application-architecture)
8. [Web Admin Architecture](#web-admin-architecture)
9. [Infrastructure & Deployment](#infrastructure--deployment)
10. [Security Architecture](#security-architecture)
11. [Monitoring & Observability](#monitoring--observability)

---

## 1. System Overview

### 1.1 Business Context

WOL is a digital logistics platform designed to optimize freight transportation in Saudi Arabia by:
- Connecting shippers (customers) with carriers (service providers)
- Implementing intelligent backload matching to reduce empty return trips
- Providing real-time tracking and notifications
- Automating pricing, billing, and compliance management
- Supporting multiple vehicle types and cargo categories

### 1.2 Key Features

**For Shippers:**
- Multi-language support (English, Arabic, Urdu)
- Real-time booking with dynamic pricing
- Backload discount options (up to 15% savings)
- Shared/LTL (Less Than Truckload) options
- Live tracking and notifications
- Document management
- Payment integration

**For Carriers/Drivers:**
- Fleet and driver management
- Trip acceptance and management
- Backload availability posting
- Earnings tracking
- Document compliance monitoring
- Route optimization suggestions

**For Administrators:**
- Comprehensive dashboard with analytics
- User and fleet management
- Pricing and discount management
- Compliance monitoring
- Route heatmap and utilization analytics
- Financial reporting

### 1.3 User Types

1. **Individual Shipper** - End customers booking shipments
2. **Commercial Shipper** - Companies/establishments with regular shipping needs
3. **Individual Service Provider** - Independent vehicle owners/drivers
4. **Fleet Owner/Company** - Companies with multiple vehicles
5. **WOL Administrator** - Platform operators and support staff

---

## 2. Architecture Principles

### 2.1 Clean Architecture

The system follows Clean Architecture principles with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────────┐
│                     Presentation Layer                       │
│  (API Controllers, SignalR Hubs, gRPC Services)             │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                     Application Layer                        │
│  (Use Cases, DTOs, Interfaces, Validators)                  │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                      Domain Layer                            │
│  (Entities, Value Objects, Domain Events, Business Logic)   │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                   Infrastructure Layer                       │
│  (Data Access, External Services, Messaging, Caching)       │
└─────────────────────────────────────────────────────────────┘
```

**Key Principles:**
- **Dependency Inversion**: Inner layers don't depend on outer layers
- **Single Responsibility**: Each microservice has a single, well-defined purpose
- **Interface Segregation**: Clients depend on minimal interfaces
- **Domain-Driven Design**: Business logic encapsulated in domain entities
- **CQRS Pattern**: Separation of read and write operations where beneficial

### 2.2 Design Patterns

- **Repository Pattern**: Data access abstraction
- **Unit of Work Pattern**: Transaction management
- **Mediator Pattern**: Request/response handling (MediatR)
- **Specification Pattern**: Business rule encapsulation
- **Factory Pattern**: Complex object creation
- **Strategy Pattern**: Dynamic pricing and matching algorithms
- **Observer Pattern**: Event-driven notifications
- **Circuit Breaker Pattern**: Resilient service communication

### 2.3 Technology Stack

**Backend:**
- .NET 8.0 (LTS)
- ASP.NET Core Web API
- Entity Framework Core 8.0
- MediatR for CQRS
- FluentValidation
- AutoMapper
- Serilog for logging
- Polly for resilience

**Databases:**
- PostgreSQL 16 (Primary transactional database)
- MongoDB 7.0 (Analytics, logs, document storage)
- Redis (Caching and session management)

**Messaging:**
- RabbitMQ 3.12 (Message broker)
- MassTransit (Messaging abstraction)

**Frontend:**
- React Native 0.73 (Mobile apps - iOS & Android)
- React 18 (Web admin portal)
- TypeScript
- Redux Toolkit (State management)
- React Query (Server state management)
- Tailwind CSS (Styling)

**Infrastructure:**
- Docker & Kubernetes
- Azure/AWS cloud services
- Nginx (Reverse proxy & load balancer)
- Elasticsearch (Search and analytics)
- Grafana & Prometheus (Monitoring)

---

## 3. Microservices Architecture

### 3.1 Service Catalog

```
┌─────────────────────────────────────────────────────────────────┐
│                         API Gateway                              │
│              (Ocelot - Routing & Auth)                          │
└─────────────────────────────────────────────────────────────────┘
                              ↓
        ┌─────────────────────┴─────────────────────┐
        ↓                     ↓                      ↓
┌──────────────┐    ┌──────────────┐      ┌──────────────┐
│   Identity   │    │   Booking    │      │   Vehicle    │
│   Service    │    │   Service    │      │   Service    │
└──────────────┘    └──────────────┘      └──────────────┘
        ↓                     ↓                      ↓
┌──────────────┐    ┌──────────────┐      ┌──────────────┐
│   Payment    │    │   Tracking   │      │   Pricing    │
│   Service    │    │   Service    │      │   Service    │
└──────────────┘    └──────────────┘      └──────────────┘
        ↓                     ↓                      ↓
┌──────────────┐    ┌──────────────┐      ┌──────────────┐
│Notification  │    │  Backload    │      │  Document    │
│   Service    │    │   Service    │      │   Service    │
└──────────────┘    └──────────────┘      └──────────────┘
        ↓                     ↓                      ↓
┌──────────────┐    ┌──────────────┐      ┌──────────────┐
│  Analytics   │    │  Compliance  │      │   Reporting  │
│   Service    │    │   Service    │      │   Service    │
└──────────────┘    └──────────────┘      └──────────────┘
```

### 3.2 Service Descriptions

#### 3.2.1 Identity Service

**Responsibility**: User authentication, authorization, and profile management

**Key Features:**
- Multi-factor authentication (OTP via SMS)
- JWT token generation and validation
- Role-based access control (RBAC)
- User profile management
- Session management
- Password reset and account recovery
- Multi-language support

**Technology:**
- ASP.NET Core Identity
- IdentityServer4 / Duende IdentityServer
- PostgreSQL for user data
- Redis for session storage

**API Endpoints:**
```
POST   /api/identity/register
POST   /api/identity/login
POST   /api/identity/verify-otp
POST   /api/identity/refresh-token
POST   /api/identity/logout
GET    /api/identity/profile
PUT    /api/identity/profile
POST   /api/identity/change-password
POST   /api/identity/forgot-password
POST   /api/identity/reset-password
```

**Database Tables:**
- Users
- Roles
- UserRoles
- UserClaims
- RefreshTokens
- OTPVerifications

---

#### 3.2.2 Booking Service

**Responsibility**: Shipment booking, trip management, and order lifecycle

**Key Features:**
- Create and manage bookings
- Trip assignment to drivers
- Booking status management
- Cancellation and refund logic
- Free time and waiting charge calculation
- BAN time validation
- Flexible booking options (one-way, backload, shared)

**Technology:**
- ASP.NET Core Web API
- PostgreSQL for transactional data
- MongoDB for booking history and audit logs
- RabbitMQ for event publishing

**API Endpoints:**
```
POST   /api/bookings
GET    /api/bookings/{id}
GET    /api/bookings/customer/{customerId}
GET    /api/bookings/driver/{driverId}
PUT    /api/bookings/{id}/assign-driver
PUT    /api/bookings/{id}/status
POST   /api/bookings/{id}/cancel
GET    /api/bookings/available
POST   /api/bookings/{id}/accept
POST   /api/bookings/{id}/reject
PUT    /api/bookings/{id}/mark-reached
PUT    /api/bookings/{id}/mark-loaded
PUT    /api/bookings/{id}/mark-delivered
GET    /api/bookings/{id}/calculate-charges
```

**Database Tables:**
- Bookings
- BookingItems
- BookingStatusHistory
- BookingCancellations
- WaitingCharges
- BookingDocuments

**Domain Events:**
```
BookingCreatedEvent
BookingAssignedEvent
BookingAcceptedEvent
BookingRejectedEvent
BookingCancelledEvent
BookingCompletedEvent
DriverReachedEvent
LoadingCompletedEvent
DeliveryCompletedEvent
```

---

#### 3.2.3 Vehicle Service

**Responsibility**: Fleet and driver management, vehicle registration, compliance tracking

**Key Features:**
- Vehicle registration and management
- Driver management
- Document upload and verification
- Compliance monitoring (Istemara, MVPI, Insurance, License, Iqama)
- Vehicle availability management
- Vehicle type and capacity management

**Technology:**
- ASP.NET Core Web API
- PostgreSQL for vehicle data
- Azure Blob Storage / AWS S3 for document storage
- RabbitMQ for compliance notifications

**API Endpoints:**
```
POST   /api/vehicles
GET    /api/vehicles/{id}
GET    /api/vehicles/owner/{ownerId}
PUT    /api/vehicles/{id}
DELETE /api/vehicles/{id}
POST   /api/vehicles/{id}/documents
GET    /api/vehicles/{id}/compliance-status
PUT    /api/vehicles/{id}/availability
GET    /api/vehicles/available
POST   /api/drivers
GET    /api/drivers/{id}
PUT    /api/drivers/{id}
POST   /api/drivers/{id}/documents
GET    /api/drivers/{id}/compliance-status
GET    /api/vehicles/types
```

**Database Tables:**
- Vehicles
- VehicleTypes
- VehicleDocuments
- Drivers
- DriverDocuments
- VehicleAvailability
- ComplianceChecks

**Domain Events:**
```
VehicleRegisteredEvent
VehicleUpdatedEvent
DocumentUploadedEvent
DocumentExpiringSoonEvent
DocumentExpiredEvent
ComplianceViolationEvent
VehicleAvailabilityChangedEvent
```

---

#### 3.2.4 Pricing Service

**Responsibility**: Dynamic pricing, discount calculation, fare estimation

**Key Features:**
- Base fare calculation by route and vehicle type
- Dynamic pricing based on demand and supply
- Backload discount calculation (up to 15%)
- Shared load pricing
- Waiting charge calculation
- Cancellation fee calculation
- Promotional pricing
- Surge pricing during peak hours

**Technology:**
- ASP.NET Core Web API
- PostgreSQL for pricing rules
- Redis for caching pricing data
- Machine learning models for dynamic pricing

**API Endpoints:**
```
POST   /api/pricing/calculate
GET    /api/pricing/routes
POST   /api/pricing/routes
PUT    /api/pricing/routes/{id}
GET    /api/pricing/vehicle-types
POST   /api/pricing/discounts
GET    /api/pricing/discounts
PUT    /api/pricing/discounts/{id}
POST   /api/pricing/estimate
GET    /api/pricing/surge-multiplier
```

**Database Tables:**
- PricingRules
- RouteBaseFares
- VehicleTypePricing
- DiscountRules
- SurgePricingRules
- PricingHistory

**Pricing Algorithm:**
```csharp
public class PricingCalculator
{
    public decimal CalculateFare(PricingRequest request)
    {
        // Base fare calculation
        var baseFare = GetBaseFare(request.Origin, request.Destination, request.VehicleType);
        
        // Distance-based calculation
        var distanceFare = request.Distance * GetRatePerKm(request.VehicleType);
        
        // Time-based component
        var timeFare = request.EstimatedDuration * GetRatePerHour(request.VehicleType);
        
        // Demand-based surge pricing
        var surgeMultiplier = GetSurgeMultiplier(request.Origin, request.PickupTime);
        
        // Calculate subtotal
        var subtotal = (baseFare + distanceFare + timeFare) * surgeMultiplier;
        
        // Apply discounts
        var discount = CalculateDiscount(request);
        
        // Final fare
        var finalFare = subtotal - discount;
        
        return Math.Max(finalFare, GetMinimumFare(request.VehicleType));
    }
    
    private decimal CalculateDiscount(PricingRequest request)
    {
        decimal discount = 0;
        
        // Backload discount (15%)
        if (request.IsBackload)
        {
            discount += subtotal * 0.15m;
        }
        
        // Flexible date discount (5%)
        if (request.IsFlexibleDate)
        {
            discount += subtotal * 0.05m;
        }
        
        // Shared load discount (10-20%)
        if (request.IsSharedLoad)
        {
            discount += subtotal * 0.15m;
        }
        
        // Loyalty discount
        if (request.CustomerTier == CustomerTier.Gold)
        {
            discount += subtotal * 0.05m;
        }
        
        return discount;
    }
}
```

---

#### 3.2.5 Backload Service

**Responsibility**: Intelligent backload matching, return trip optimization

**Key Features:**
- Backload availability posting
- Smart matching algorithm
- Route optimization
- Discount recommendation
- Utilization analytics
- Heatmap generation
- AI-based load prediction

**Technology:**
- ASP.NET Core Web API
- PostgreSQL for backload data
- MongoDB for analytics
- Redis for real-time matching
- ML.NET for predictive matching

**API Endpoints:**
```
POST   /api/backload/availability
GET    /api/backload/available
POST   /api/backload/search
POST   /api/backload/match
GET    /api/backload/recommendations/{driverId}
GET    /api/backload/heatmap
GET    /api/backload/utilization-stats
PUT    /api/backload/availability/{id}
DELETE /api/backload/availability/{id}
```

**Database Tables:**
- BackloadAvailability
- BackloadMatches
- MatchingScores
- RouteUtilization
- BackloadAnalytics

**Matching Algorithm:**
```csharp
public class BackloadMatchingEngine
{
    public List<BackloadMatch> FindMatches(BackloadAvailability availability)
    {
        // Get pending loads in destination area
        var pendingLoads = GetPendingLoads(
            availability.ReturnCity, 
            availability.AvailableFrom, 
            availability.AvailableTo
        );
        
        var matches = new List<BackloadMatch>();
        
        foreach (var load in pendingLoads)
        {
            // Calculate match score
            var score = CalculateMatchScore(availability, load);
            
            if (score > MinimumMatchThreshold)
            {
                matches.Add(new BackloadMatch
                {
                    AvailabilityId = availability.Id,
                    LoadId = load.Id,
                    MatchScore = score,
                    EstimatedDiscount = CalculateDiscount(availability, load),
                    Distance = CalculateDistance(availability.CurrentLocation, load.PickupLocation)
                });
            }
        }
        
        return matches.OrderByDescending(m => m.MatchScore).ToList();
    }
    
    private double CalculateMatchScore(BackloadAvailability availability, Load load)
    {
        double score = 100.0;
        
        // Distance factor (closer is better)
        var distance = CalculateDistance(availability.CurrentLocation, load.PickupLocation);
        score -= distance * 0.5; // Penalty for distance
        
        // Time window compatibility
        var timeDiff = Math.Abs((load.PickupTime - availability.AvailableFrom).TotalHours);
        score -= timeDiff * 2; // Penalty for time mismatch
        
        // Capacity match
        if (load.Weight > availability.Capacity)
        {
            return 0; // Cannot handle load
        }
        var capacityUtilization = (load.Weight / availability.Capacity) * 100;
        score += capacityUtilization * 0.3; // Bonus for high utilization
        
        // Vehicle type match
        if (load.RequiredVehicleType == availability.VehicleType)
        {
            score += 20;
        }
        
        // Route popularity
        var routePopularity = GetRoutePopularity(availability.ReturnCity, load.Destination);
        score += routePopularity * 0.2;
        
        return Math.Max(0, score);
    }
}
```

---

#### 3.2.6 Tracking Service

**Responsibility**: Real-time location tracking, trip monitoring, ETA calculation

**Key Features:**
- Real-time GPS tracking
- Geofencing and arrival detection
- ETA calculation and updates
- Route deviation alerts
- Trip status updates
- Location history
- Geo-tagged photo verification

**Technology:**
- ASP.NET Core Web API
- SignalR for real-time updates
- PostgreSQL for trip data
- MongoDB for location history
- Redis for real-time location cache

**API Endpoints:**
```
POST   /api/tracking/location
GET    /api/tracking/trip/{tripId}
GET    /api/tracking/trip/{tripId}/history
GET    /api/tracking/trip/{tripId}/eta
POST   /api/tracking/trip/{tripId}/mark-reached
POST   /api/tracking/trip/{tripId}/upload-photo
GET    /api/tracking/driver/{driverId}/current-location
WS     /hubs/tracking (SignalR hub)
```

**Database Tables:**
- TripTracking
- LocationHistory
- GeofenceEvents
- ArrivalProofs

**SignalR Hub:**
```csharp
public class TrackingHub : Hub
{
    public async Task UpdateLocation(LocationUpdate update)
    {
        // Store location
        await _trackingService.UpdateLocation(update);
        
        // Broadcast to trip participants
        await Clients.Group($"trip_{update.TripId}")
            .SendAsync("LocationUpdated", update);
        
        // Check geofence events
        await CheckGeofenceEvents(update);
    }
    
    public async Task JoinTripTracking(string tripId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"trip_{tripId}");
    }
}
```

---

#### 3.2.7 Payment Service

**Responsibility**: Payment processing, invoicing, settlement, refunds

**Key Features:**
- Multiple payment methods (card, wallet, bank transfer)
- Payment gateway integration (Stripe, PayTabs, HyperPay)
- Invoice generation
- Automatic payment holds and captures
- Refund processing
- Settlement to drivers/carriers
- Payment history and receipts
- VAT calculation

**Technology:**
- ASP.NET Core Web API
- PostgreSQL for payment data
- Stripe/PayTabs SDK
- RabbitMQ for payment events

**API Endpoints:**
```
POST   /api/payments/initialize
POST   /api/payments/confirm
POST   /api/payments/capture
POST   /api/payments/refund
GET    /api/payments/{id}
GET    /api/payments/booking/{bookingId}
POST   /api/payments/invoice
GET    /api/payments/customer/{customerId}/history
GET    /api/payments/driver/{driverId}/earnings
POST   /api/payments/settle
GET    /api/payments/methods
POST   /api/payments/methods
```

**Database Tables:**
- Payments
- PaymentMethods
- Invoices
- Refunds
- DriverSettlements
- PaymentTransactions

**Payment Flow:**
```
1. Booking Created → Payment Initialized (Hold)
2. Driver Assigned → Payment Confirmed (15 min window)
3. Trip Completed → Payment Captured
4. Settlement Scheduled → Driver Payout
```

---

#### 3.2.8 Notification Service

**Responsibility**: Multi-channel notifications, push notifications, SMS, email

**Key Features:**
- Push notifications (Firebase Cloud Messaging)
- SMS notifications (Twilio, AWS SNS)
- Email notifications (SendGrid, AWS SES)
- In-app notifications
- Notification preferences management
- Template management
- Multi-language support
- Notification history

**Technology:**
- ASP.NET Core Web API
- MongoDB for notification logs
- RabbitMQ for async notification processing
- Firebase Admin SDK
- Twilio SDK
- SendGrid SDK

**API Endpoints:**
```
POST   /api/notifications/send
GET    /api/notifications/user/{userId}
PUT    /api/notifications/{id}/read
GET    /api/notifications/preferences
PUT    /api/notifications/preferences
POST   /api/notifications/templates
GET    /api/notifications/templates
```

**Database Collections (MongoDB):**
- Notifications
- NotificationTemplates
- NotificationPreferences
- NotificationLogs

**Notification Types:**
```
- BookingConfirmed
- DriverAssigned
- DriverReached
- LoadingStarted
- TripStarted
- TripCompleted
- PaymentReceived
- DocumentExpiring
- ComplianceViolation
- BackloadAvailable
- PriceAlert
```

---

#### 3.2.9 Document Service

**Responsibility**: Document upload, storage, verification, OCR processing

**Key Features:**
- Document upload and storage
- Document verification workflow
- OCR for automatic data extraction
- Expiry date tracking
- Document categories (Istemara, MVPI, Insurance, License, Iqama)
- Secure document access
- Document versioning

**Technology:**
- ASP.NET Core Web API
- Azure Blob Storage / AWS S3
- PostgreSQL for metadata
- Azure Computer Vision / AWS Textract for OCR
- RabbitMQ for async processing

**API Endpoints:**
```
POST   /api/documents/upload
GET    /api/documents/{id}
GET    /api/documents/entity/{entityType}/{entityId}
PUT    /api/documents/{id}/verify
DELETE /api/documents/{id}
GET    /api/documents/{id}/download
POST   /api/documents/{id}/extract-data
GET    /api/documents/expiring
```

**Database Tables:**
- Documents
- DocumentTypes
- DocumentVerifications
- DocumentExtractionResults

---

#### 3.2.10 Compliance Service

**Responsibility**: Regulatory compliance, document expiry monitoring, BAN time management

**Key Features:**
- Document expiry monitoring
- Automatic compliance checks
- BAN time enforcement
- Compliance violation tracking
- Automated notifications for expiring documents
- Vehicle/driver blocking for non-compliance
- Compliance reporting

**Technology:**
- ASP.NET Core Web API
- PostgreSQL for compliance data
- RabbitMQ for scheduled checks
- Hangfire for background jobs

**API Endpoints:**
```
GET    /api/compliance/check/{entityType}/{entityId}
GET    /api/compliance/violations
GET    /api/compliance/expiring-documents
POST   /api/compliance/ban-times
GET    /api/compliance/ban-times
PUT    /api/compliance/ban-times/{id}
GET    /api/compliance/is-booking-allowed
```

**Database Tables:**
- ComplianceRules
- ComplianceChecks
- ComplianceViolations
- BANTimes
- DocumentExpiryAlerts

---

#### 3.2.11 Analytics Service

**Responsibility**: Business intelligence, reporting, data analytics, ML insights

**Key Features:**
- Trip analytics
- Revenue analytics
- Utilization metrics
- Route heatmaps
- Driver performance analytics
- Customer behavior analytics
- Predictive analytics (demand forecasting)
- Carbon footprint calculation

**Technology:**
- ASP.NET Core Web API
- MongoDB for analytics data
- Elasticsearch for search and aggregations
- ML.NET for machine learning
- Apache Spark for big data processing

**API Endpoints:**
```
GET    /api/analytics/dashboard
GET    /api/analytics/trips/summary
GET    /api/analytics/revenue
GET    /api/analytics/utilization
GET    /api/analytics/routes/heatmap
GET    /api/analytics/drivers/performance
GET    /api/analytics/customers/behavior
GET    /api/analytics/predictions/demand
GET    /api/analytics/carbon-footprint
POST   /api/analytics/reports/generate
```

**Database Collections (MongoDB):**
- TripAnalytics
- RevenueAnalytics
- UtilizationMetrics
- RouteAnalytics
- DriverPerformance
- CustomerBehavior

---

#### 3.2.12 Reporting Service

**Responsibility**: Report generation, export, scheduled reports

**Key Features:**
- Custom report builder
- Scheduled reports
- Export to PDF, Excel, CSV
- Financial reports
- Operational reports
- Compliance reports
- Email delivery of reports

**Technology:**
- ASP.NET Core Web API
- PostgreSQL for report definitions
- MongoDB for report data
- Hangfire for scheduled jobs
- FastReport / Telerik Reporting

**API Endpoints:**
```
POST   /api/reports/generate
GET    /api/reports/{id}
GET    /api/reports/templates
POST   /api/reports/templates
POST   /api/reports/schedule
GET    /api/reports/scheduled
GET    /api/reports/{id}/download
```

---

### 3.3 Worker Services (Background Processing)

Worker Services are .NET background services that handle asynchronous tasks, RabbitMQ message processing, scheduled jobs, and long-running operations. They run independently from the API services and provide better scalability and resilience for background processing.

#### 3.3.1 Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                         RabbitMQ Broker                          │
│                    (Message Exchange & Queues)                   │
└─────────────────────────────────────────────────────────────────┘
                              ↓
        ┌─────────────────────┴─────────────────────┐
        ↓                     ↓                      ↓
┌──────────────┐    ┌──────────────┐      ┌──────────────┐
│ Notification │    │  Analytics   │      │  Document    │
│    Worker    │    │    Worker    │      │   Worker     │
└──────────────┘    └──────────────┘      └──────────────┘
        ↓                     ↓                      ↓
┌──────────────┐    ┌──────────────┐      ┌──────────────┐
│  Compliance  │    │   Reporting  │      │   Backload   │
│    Worker    │    │    Worker    │      │   Worker     │
└──────────────┘    └──────────────┘      └──────────────┘
```

#### 3.3.2 Notification Worker Service

**Responsibility**: Process notification requests and send via multiple channels (Push, SMS, Email)

**Key Features:**
- Consume notification messages from RabbitMQ
- Send push notifications via Firebase/APNs
- Send SMS via Twilio/AWS SNS
- Send emails via SendGrid/AWS SES
- Retry failed notifications with exponential backoff
- Track notification delivery status
- Handle notification templates and localization

**Message Consumers:**
```csharp
// Consumes from RabbitMQ queues
- SendNotificationCommand
- BookingCreatedEvent
- BookingAssignedEvent
- DriverReachedEvent
- TripCompletedEvent
- PaymentReceivedEvent
- DocumentExpiringEvent
- ComplianceViolationEvent
```

**Technology:**
- .NET 8.0 Worker Service
- MassTransit for RabbitMQ integration
- Firebase Admin SDK (Push notifications)
- Twilio SDK (SMS)
- SendGrid SDK (Email)
- MongoDB for notification logs
- Redis for rate limiting

**Configuration:**
```json
{
  "NotificationWorker": {
    "MaxConcurrentMessages": 10,
    "RetryAttempts": 3,
    "RetryDelaySeconds": 5,
    "BatchSize": 100
  },
  "Firebase": {
    "ProjectId": "wol-app",
    "CredentialsPath": "/secrets/firebase-credentials.json"
  },
  "Twilio": {
    "AccountSid": "...",
    "AuthToken": "...",
    "FromNumber": "+966..."
  }
}
```

---

#### 3.3.3 Analytics Worker Service

**Responsibility**: Process analytics events, aggregate data, and generate insights

**Key Features:**
- Consume booking, trip, and user behavior events
- Aggregate data for dashboards
- Calculate KPIs and metrics
- Generate route utilization analytics
- Process driver performance metrics
- Update real-time analytics dashboards
- Archive historical data

**Message Consumers:**
```csharp
- BookingCompletedEvent
- TripCompletedEvent
- PaymentProcessedEvent
- UserBehaviorEvent
- LocationUpdateEvent
- BackloadMatchedEvent
```

**Technology:**
- .NET 8.0 Worker Service
- MassTransit for RabbitMQ integration
- MongoDB for analytics data
- Redis for real-time metrics
- Time-series data processing

**Processing Pipeline:**
```
Event → Validation → Transformation → Aggregation → Storage → Cache Update
```

**Configuration:**
```json
{
  "AnalyticsWorker": {
    "MaxConcurrentMessages": 20,
    "AggregationIntervalMinutes": 5,
    "ArchiveAfterDays": 90
  }
}
```

---

#### 3.3.4 Document Processing Worker Service

**Responsibility**: Process uploaded documents, perform OCR, extract data, and verify compliance

**Key Features:**
- Consume document upload events
- Perform OCR using Azure Computer Vision/AWS Textract
- Extract key information (dates, numbers, text)
- Validate document authenticity
- Check expiry dates
- Generate thumbnails
- Virus scanning
- Archive processed documents

**Message Consumers:**
```csharp
- DocumentUploadedEvent
- DocumentVerificationRequestedEvent
```

**Technology:**
- .NET 8.0 Worker Service
- MassTransit for RabbitMQ integration
- Azure Computer Vision / AWS Textract (OCR)
- Azure Blob Storage / AWS S3
- PostgreSQL for metadata
- ImageSharp for image processing

**Processing Pipeline:**
```
Upload → Virus Scan → OCR → Data Extraction → Validation → Thumbnail → Storage → Notification
```

**Configuration:**
```json
{
  "DocumentWorker": {
    "MaxConcurrentMessages": 5,
    "MaxFileSizeMB": 10,
    "SupportedFormats": ["pdf", "jpg", "png"],
    "OCRProvider": "AzureComputerVision"
  }
}
```

---

#### 3.3.5 Compliance Worker Service

**Responsibility**: Monitor compliance, check document expiry, enforce BAN times, and send alerts

**Key Features:**
- Daily compliance checks for all vehicles and drivers
- Document expiry monitoring (30, 15, 7, 1 day alerts)
- BAN time enforcement
- Automatic vehicle/driver blocking for non-compliance
- Compliance violation tracking
- Generate compliance reports
- Send compliance notifications

**Message Consumers:**
```csharp
- DailyComplianceCheckScheduled
- DocumentExpiryCheckScheduled
- VehicleRegisteredEvent
- DriverRegisteredEvent
- DocumentUploadedEvent
```

**Technology:**
- .NET 8.0 Worker Service
- MassTransit for RabbitMQ integration
- Hangfire for scheduled jobs
- PostgreSQL for compliance data
- Redis for caching BAN time rules

**Scheduled Jobs:**
```csharp
- Daily compliance check (2:00 AM)
- Document expiry check (3:00 AM)
- BAN time validation (every hour)
- Compliance report generation (weekly)
```

**Configuration:**
```json
{
  "ComplianceWorker": {
    "DailyCheckTime": "02:00",
    "ExpiryAlertDays": [30, 15, 7, 1],
    "AutoBlockOnExpiry": true
  }
}
```

---

#### 3.3.6 Reporting Worker Service

**Responsibility**: Generate scheduled reports, export data, and deliver reports

**Key Features:**
- Generate scheduled reports (daily, weekly, monthly)
- Export to PDF, Excel, CSV
- Email report delivery
- Archive generated reports
- Custom report generation
- Data aggregation for reports

**Message Consumers:**
```csharp
- GenerateReportCommand
- ScheduledReportDueEvent
```

**Technology:**
- .NET 8.0 Worker Service
- MassTransit for RabbitMQ integration
- Hangfire for scheduled jobs
- FastReport / QuestPDF for PDF generation
- EPPlus for Excel generation
- MongoDB for report storage

**Report Types:**
```
- Daily booking summary
- Weekly revenue report
- Monthly driver performance
- Quarterly compliance report
- Annual financial report
- Custom ad-hoc reports
```

**Configuration:**
```json
{
  "ReportingWorker": {
    "MaxConcurrentReports": 3,
    "ReportRetentionDays": 90,
    "EmailDelivery": true
  }
}
```

---

#### 3.3.7 Backload Matching Worker Service

**Responsibility**: Process backload availability, match with bookings, and optimize routes

**Key Features:**
- Consume backload availability posts
- Match with pending bookings
- Calculate match scores using ML algorithm
- Optimize route suggestions
- Send match notifications to drivers
- Update route utilization analytics

**Message Consumers:**
```csharp
- BackloadAvailabilityPostedEvent
- BookingCreatedEvent
- TripCompletedEvent
```

**Technology:**
- .NET 8.0 Worker Service
- MassTransit for RabbitMQ integration
- ML.NET for matching algorithm
- PostgreSQL for backload data
- Redis for caching routes

**Matching Algorithm:**
```
Score = (RouteMatch × 0.4) + (TimeMatch × 0.3) + (CapacityMatch × 0.2) + (PriceMatch × 0.1)
```

**Configuration:**
```json
{
  "BackloadWorker": {
    "MaxConcurrentMatches": 10,
    "MinMatchScore": 0.7,
    "MaxMatchRadius": 50,
    "MatchExpiryHours": 24
  }
}
```

---

#### 3.3.8 Worker Service Implementation Pattern

**Base Worker Service Template:**

```csharp
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
        _logger.LogInformation("Notification Worker starting...");
        
        await _busControl.StartAsync(stoppingToken);
        
        _logger.LogInformation("Notification Worker started successfully");
        
        // Keep the worker running
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
    
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Notification Worker stopping...");
        
        await _busControl.StopAsync(cancellationToken);
        
        _logger.LogInformation("Notification Worker stopped");
        
        await base.StopAsync(cancellationToken);
    }
}

// Message Consumer
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
        try
        {
            _logger.LogInformation(
                "Processing notification for user {UserId}", 
                context.Message.UserId);
            
            await _notificationService.SendAsync(
                context.Message.UserId,
                context.Message.Title,
                context.Message.Message,
                context.Message.Channels);
            
            _logger.LogInformation(
                "Notification sent successfully for user {UserId}", 
                context.Message.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Failed to send notification for user {UserId}", 
                context.Message.UserId);
            
            // Retry with exponential backoff
            throw;
        }
    }
}

// Program.cs
public class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Add MassTransit with RabbitMQ
                services.AddMassTransit(x =>
                {
                    x.AddConsumer<SendNotificationConsumer>();
                    
                    x.UsingRabbitMq((ctx, cfg) =>
                    {
                        cfg.Host(context.Configuration["RabbitMQ:Host"], h =>
                        {
                            h.Username(context.Configuration["RabbitMQ:Username"]);
                            h.Password(context.Configuration["RabbitMQ:Password"]);
                        });
                        
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
                    });
                });
                
                // Add services
                services.AddScoped<INotificationService, NotificationService>();
                
                // Add hosted service
                services.AddHostedService<NotificationWorker>();
            })
            .Build();
        
        await host.RunAsync();
    }
}
```

---

#### 3.3.9 Worker Service Deployment

**Docker Deployment:**

Each worker service runs as a separate container with its own scaling configuration:

```yaml
# docker-compose.yml
notification-worker:
  build:
    context: ./src/Workers/Notification
    dockerfile: Dockerfile
  environment:
    - RabbitMQ__Host=rabbitmq
    - MongoDB__ConnectionString=mongodb://mongodb:27017
  depends_on:
    - rabbitmq
    - mongodb
  restart: unless-stopped
  deploy:
    replicas: 2
    resources:
      limits:
        cpus: '0.5'
        memory: 512M
```

**Kubernetes Deployment:**

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: notification-worker
spec:
  replicas: 2
  selector:
    matchLabels:
      app: notification-worker
  template:
    metadata:
      labels:
        app: notification-worker
    spec:
      containers:
      - name: notification-worker
        image: wol/notification-worker:latest
        env:
        - name: RabbitMQ__Host
          value: "rabbitmq-service"
        resources:
          limits:
            cpu: "500m"
            memory: "512Mi"
          requests:
            cpu: "250m"
            memory: "256Mi"
```

---

#### 3.3.10 Worker Service Benefits

**Advantages of Using Worker Services:**

1. **Separation of Concerns**: API services handle synchronous requests, workers handle async tasks
2. **Better Scalability**: Scale workers independently based on queue depth
3. **Improved Resilience**: Failed workers don't affect API availability
4. **Resource Optimization**: Workers can use different resource profiles
5. **Better Monitoring**: Separate metrics and logs for background processing
6. **Easier Debugging**: Isolated worker processes are easier to troubleshoot
7. **Flexible Deployment**: Deploy workers on different infrastructure (spot instances, etc.)

**When to Use Worker Services vs API Services:**

| Use Worker Service | Use API Service |
|-------------------|-----------------|
| Sending notifications | User registration |
| Processing analytics | Booking creation |
| Generating reports | Payment processing |
| OCR processing | Real-time tracking |
| Compliance checks | Search queries |
| Backload matching | Profile updates |

---

## 4. Database Design

### 4.1 PostgreSQL Schema

PostgreSQL is used for all transactional data requiring ACID properties.

#### 4.1.1 Identity Schema

```sql
-- Users table
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_type VARCHAR(50) NOT NULL, -- Individual, Commercial, ServiceProvider
    mobile_number VARCHAR(20) NOT NULL UNIQUE,
    email VARCHAR(255),
    iqama_number VARCHAR(50),
    id_number VARCHAR(50),
    password_hash VARCHAR(255) NOT NULL,
    is_email_verified BOOLEAN DEFAULT FALSE,
    is_mobile_verified BOOLEAN DEFAULT FALSE,
    preferred_language VARCHAR(10) DEFAULT 'en',
    status VARCHAR(20) DEFAULT 'Active',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    last_login_at TIMESTAMP
);

CREATE INDEX idx_users_mobile ON users(mobile_number);
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_iqama ON users(iqama_number);

-- Roles table
CREATE TABLE roles (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(50) NOT NULL UNIQUE,
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- User roles mapping
CREATE TABLE user_roles (
    user_id UUID REFERENCES users(id) ON DELETE CASCADE,
    role_id UUID REFERENCES roles(id) ON DELETE CASCADE,
    assigned_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id, role_id)
);

-- User profiles
CREATE TABLE user_profiles (
    user_id UUID PRIMARY KEY REFERENCES users(id) ON DELETE CASCADE,
    full_name VARCHAR(255) NOT NULL,
    commercial_license VARCHAR(100),
    commercial_license_expiry DATE,
    vat_number VARCHAR(50),
    city VARCHAR(100),
    address TEXT,
    profile_image_url TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- OTP verifications
CREATE TABLE otp_verifications (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    mobile_number VARCHAR(20) NOT NULL,
    otp_code VARCHAR(6) NOT NULL,
    purpose VARCHAR(50) NOT NULL, -- Registration, Login, PasswordReset
    is_verified BOOLEAN DEFAULT FALSE,
    expires_at TIMESTAMP NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_otp_mobile ON otp_verifications(mobile_number);
CREATE INDEX idx_otp_expires ON otp_verifications(expires_at);

-- Refresh tokens
CREATE TABLE refresh_tokens (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID REFERENCES users(id) ON DELETE CASCADE,
    token VARCHAR(500) NOT NULL UNIQUE,
    expires_at TIMESTAMP NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    revoked_at TIMESTAMP
);

CREATE INDEX idx_refresh_tokens_user ON refresh_tokens(user_id);
CREATE INDEX idx_refresh_tokens_token ON refresh_tokens(token);
```

#### 4.1.2 Vehicle Schema

```sql
-- Vehicle types
CREATE TABLE vehicle_types (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL,
    category VARCHAR(50) NOT NULL, -- Light, Box, Curtain, Flatbed, etc.
    capacity_tons DECIMAL(10,2),
    capacity_cubic_meters DECIMAL(10,2),
    length_meters DECIMAL(10,2),
    width_meters DECIMAL(10,2),
    height_meters DECIMAL(10,2),
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Vehicles
CREATE TABLE vehicles (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    owner_id UUID REFERENCES users(id) ON DELETE CASCADE,
    vehicle_type_id UUID REFERENCES vehicle_types(id),
    plate_number VARCHAR(50) NOT NULL UNIQUE,
    istemara_number VARCHAR(100),
    istemara_expiry DATE,
    mvpi_number VARCHAR(100),
    mvpi_expiry DATE,
    insurance_number VARCHAR(100),
    insurance_expiry DATE,
    vehicle_image_url TEXT,
    status VARCHAR(20) DEFAULT 'Active', -- Active, Inactive, Blocked
    is_available BOOLEAN DEFAULT TRUE,
    current_location_lat DECIMAL(10,8),
    current_location_lng DECIMAL(11,8),
    current_city VARCHAR(100),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_vehicles_owner ON vehicles(owner_id);
CREATE INDEX idx_vehicles_type ON vehicles(vehicle_type_id);
CREATE INDEX idx_vehicles_status ON vehicles(status);
CREATE INDEX idx_vehicles_available ON vehicles(is_available);

-- Drivers
CREATE TABLE drivers (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID REFERENCES users(id) ON DELETE CASCADE,
    owner_id UUID REFERENCES users(id),
    full_name VARCHAR(255) NOT NULL,
    mobile_number VARCHAR(20) NOT NULL,
    iqama_number VARCHAR(50),
    iqama_expiry DATE,
    license_number VARCHAR(100),
    license_expiry DATE,
    status VARCHAR(20) DEFAULT 'Active',
    rating DECIMAL(3,2) DEFAULT 0.00,
    total_trips INTEGER DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_drivers_user ON drivers(user_id);
CREATE INDEX idx_drivers_owner ON drivers(owner_id);
CREATE INDEX idx_drivers_status ON drivers(status);

-- Vehicle-Driver assignments
CREATE TABLE vehicle_driver_assignments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    vehicle_id UUID REFERENCES vehicles(id) ON DELETE CASCADE,
    driver_id UUID REFERENCES drivers(id) ON DELETE CASCADE,
    is_primary BOOLEAN DEFAULT FALSE,
    assigned_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    unassigned_at TIMESTAMP
);

CREATE INDEX idx_assignments_vehicle ON vehicle_driver_assignments(vehicle_id);
CREATE INDEX idx_assignments_driver ON vehicle_driver_assignments(driver_id);
```

#### 4.1.3 Booking Schema

```sql
-- Bookings
CREATE TABLE bookings (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    booking_number VARCHAR(50) NOT NULL UNIQUE,
    customer_id UUID REFERENCES users(id),
    vehicle_id UUID REFERENCES vehicles(id),
    driver_id UUID REFERENCES drivers(id),
    vehicle_type_id UUID REFERENCES vehicle_types(id) NOT NULL,
    
    -- Trip details
    origin_address TEXT NOT NULL,
    origin_lat DECIMAL(10,8) NOT NULL,
    origin_lng DECIMAL(11,8) NOT NULL,
    origin_city VARCHAR(100) NOT NULL,
    
    destination_address TEXT NOT NULL,
    destination_lat DECIMAL(10,8) NOT NULL,
    destination_lng DECIMAL(11,8) NOT NULL,
    destination_city VARCHAR(100) NOT NULL,
    
    distance_km DECIMAL(10,2),
    estimated_duration_minutes INTEGER,
    
    -- Scheduling
    pickup_date DATE NOT NULL,
    pickup_time TIME NOT NULL,
    is_whole_day BOOLEAN DEFAULT FALSE,
    is_flexible_date BOOLEAN DEFAULT FALSE,
    
    -- Cargo details
    cargo_type VARCHAR(50), -- Dry, Perishable, DG, HighValue
    gross_weight_kg DECIMAL(10,2),
    dimensions_length_cm DECIMAL(10,2),
    dimensions_width_cm DECIMAL(10,2),
    dimensions_height_cm DECIMAL(10,2),
    number_of_boxes INTEGER,
    cargo_image_url TEXT,
    
    -- Contact details
    shipper_name VARCHAR(255),
    shipper_mobile VARCHAR(20),
    shipper_alternate_mobile VARCHAR(20),
    receiver_name VARCHAR(255),
    receiver_mobile VARCHAR(20),
    receiver_alternate_mobile VARCHAR(20),
    
    -- Booking type
    booking_type VARCHAR(20) DEFAULT 'OneWay', -- OneWay, Backload, Shared
    is_backload BOOLEAN DEFAULT FALSE,
    is_shared_load BOOLEAN DEFAULT FALSE,
    
    -- Pricing
    base_fare DECIMAL(10,2) NOT NULL,
    discount_amount DECIMAL(10,2) DEFAULT 0,
    waiting_charges DECIMAL(10,2) DEFAULT 0,
    total_fare DECIMAL(10,2) NOT NULL,
    vat_amount DECIMAL(10,2) DEFAULT 0,
    final_amount DECIMAL(10,2) NOT NULL,
    
    -- Status
    status VARCHAR(50) DEFAULT 'Pending', 
    -- Pending, DriverAssigned, Accepted, DriverReached, Loading, InTransit, Delivered, Completed, Cancelled
    
    -- Timestamps
    driver_assigned_at TIMESTAMP,
    driver_accepted_at TIMESTAMP,
    driver_reached_at TIMESTAMP,
    loading_started_at TIMESTAMP,
    loading_completed_at TIMESTAMP,
    trip_started_at TIMESTAMP,
    delivered_at TIMESTAMP,
    completed_at TIMESTAMP,
    cancelled_at TIMESTAMP,
    
    -- Free time and waiting
    free_time_minutes INTEGER DEFAULT 120,
    free_time_end_at TIMESTAMP,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_bookings_customer ON bookings(customer_id);
CREATE INDEX idx_bookings_driver ON bookings(driver_id);
CREATE INDEX idx_bookings_vehicle ON bookings(vehicle_id);
CREATE INDEX idx_bookings_status ON bookings(status);
CREATE INDEX idx_bookings_pickup_date ON bookings(pickup_date);
CREATE INDEX idx_bookings_number ON bookings(booking_number);

-- Booking status history
CREATE TABLE booking_status_history (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    booking_id UUID REFERENCES bookings(id) ON DELETE CASCADE,
    from_status VARCHAR(50),
    to_status VARCHAR(50) NOT NULL,
    changed_by UUID REFERENCES users(id),
    reason TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_status_history_booking ON booking_status_history(booking_id);

-- Booking cancellations
CREATE TABLE booking_cancellations (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    booking_id UUID REFERENCES bookings(id) ON DELETE CASCADE,
    cancelled_by UUID REFERENCES users(id),
    cancellation_reason TEXT,
    cancellation_fee DECIMAL(10,2) DEFAULT 0,
    refund_amount DECIMAL(10,2) DEFAULT 0,
    cancelled_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Waiting charges
CREATE TABLE waiting_charge_logs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    booking_id UUID REFERENCES bookings(id) ON DELETE CASCADE,
    free_time_end_at TIMESTAMP NOT NULL,
    waiting_started_at TIMESTAMP NOT NULL,
    waiting_ended_at TIMESTAMP,
    hours_waited DECIMAL(10,2),
    charge_per_hour DECIMAL(10,2) DEFAULT 100,
    total_charge DECIMAL(10,2),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

#### 4.1.4 Pricing Schema

```sql
-- Pricing rules
CREATE TABLE pricing_rules (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    rule_type VARCHAR(50) NOT NULL, -- BaseFare, PerKm, PerHour, Surge
    vehicle_type_id UUID REFERENCES vehicle_types(id),
    origin_city VARCHAR(100),
    destination_city VARCHAR(100),
    base_amount DECIMAL(10,2),
    rate_per_km DECIMAL(10,2),
    rate_per_hour DECIMAL(10,2),
    minimum_fare DECIMAL(10,2),
    is_active BOOLEAN DEFAULT TRUE,
    effective_from DATE,
    effective_to DATE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_pricing_vehicle_type ON pricing_rules(vehicle_type_id);
CREATE INDEX idx_pricing_cities ON pricing_rules(origin_city, destination_city);

-- Discount rules
CREATE TABLE discount_rules (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    discount_type VARCHAR(50) NOT NULL, -- Backload, FlexibleDate, SharedLoad, Loyalty, Promotional
    discount_percentage DECIMAL(5,2),
    discount_amount DECIMAL(10,2),
    max_discount_amount DECIMAL(10,2),
    conditions JSONB,
    is_active BOOLEAN DEFAULT TRUE,
    valid_from DATE,
    valid_to DATE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Surge pricing
CREATE TABLE surge_pricing_rules (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    city VARCHAR(100) NOT NULL,
    day_of_week INTEGER, -- 0=Sunday, 6=Saturday
    time_from TIME,
    time_to TIME,
    surge_multiplier DECIMAL(5,2) NOT NULL,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

#### 4.1.5 Backload Schema

```sql
-- Backload availability
CREATE TABLE backload_availability (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    vehicle_id UUID REFERENCES vehicles(id) ON DELETE CASCADE,
    driver_id UUID REFERENCES drivers(id) ON DELETE CASCADE,
    
    current_location_address TEXT,
    current_location_lat DECIMAL(10,8),
    current_location_lng DECIMAL(11,8),
    current_city VARCHAR(100) NOT NULL,
    
    return_city VARCHAR(100) NOT NULL,
    return_location_lat DECIMAL(10,8),
    return_location_lng DECIMAL(11,8),
    
    available_from TIMESTAMP NOT NULL,
    available_to TIMESTAMP NOT NULL,
    
    capacity_available_tons DECIMAL(10,2),
    capacity_available_cubic_meters DECIMAL(10,2),
    
    preferred_cargo_types VARCHAR(255),
    minimum_price_expected DECIMAL(10,2),
    
    status VARCHAR(20) DEFAULT 'Available', -- Available, Matched, Expired
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_backload_vehicle ON backload_availability(vehicle_id);
CREATE INDEX idx_backload_driver ON backload_availability(driver_id);
CREATE INDEX idx_backload_cities ON backload_availability(current_city, return_city);
CREATE INDEX idx_backload_dates ON backload_availability(available_from, available_to);
CREATE INDEX idx_backload_status ON backload_availability(status);

-- Backload matches
CREATE TABLE backload_matches (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    availability_id UUID REFERENCES backload_availability(id) ON DELETE CASCADE,
    booking_id UUID REFERENCES bookings(id) ON DELETE CASCADE,
    match_score DECIMAL(5,2) NOT NULL,
    estimated_discount DECIMAL(10,2),
    distance_km DECIMAL(10,2),
    status VARCHAR(20) DEFAULT 'Suggested', -- Suggested, Accepted, Rejected
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_matches_availability ON backload_matches(availability_id);
CREATE INDEX idx_matches_booking ON backload_matches(booking_id);
```

#### 4.1.6 Payment Schema

```sql
-- Payments
CREATE TABLE payments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    booking_id UUID REFERENCES bookings(id) ON DELETE CASCADE,
    customer_id UUID REFERENCES users(id),
    
    payment_method VARCHAR(50) NOT NULL, -- Card, Wallet, BankTransfer, Cash
    payment_provider VARCHAR(50), -- Stripe, PayTabs, HyperPay
    
    amount DECIMAL(10,2) NOT NULL,
    currency VARCHAR(3) DEFAULT 'SAR',
    
    status VARCHAR(50) DEFAULT 'Pending', 
    -- Pending, Authorized, Captured, Failed, Refunded, PartiallyRefunded
    
    payment_intent_id VARCHAR(255),
    transaction_id VARCHAR(255),
    
    authorized_at TIMESTAMP,
    captured_at TIMESTAMP,
    failed_at TIMESTAMP,
    failure_reason TEXT,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_payments_booking ON payments(booking_id);
CREATE INDEX idx_payments_customer ON payments(customer_id);
CREATE INDEX idx_payments_status ON payments(status);

-- Refunds
CREATE TABLE refunds (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    payment_id UUID REFERENCES payments(id) ON DELETE CASCADE,
    booking_id UUID REFERENCES bookings(id),
    
    refund_amount DECIMAL(10,2) NOT NULL,
    refund_reason TEXT,
    
    status VARCHAR(50) DEFAULT 'Pending', -- Pending, Processed, Failed
    
    refund_transaction_id VARCHAR(255),
    processed_at TIMESTAMP,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Driver settlements
CREATE TABLE driver_settlements (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    driver_id UUID REFERENCES drivers(id) ON DELETE CASCADE,
    
    period_start DATE NOT NULL,
    period_end DATE NOT NULL,
    
    total_trips INTEGER DEFAULT 0,
    total_earnings DECIMAL(10,2) DEFAULT 0,
    platform_commission DECIMAL(10,2) DEFAULT 0,
    penalties DECIMAL(10,2) DEFAULT 0,
    net_amount DECIMAL(10,2) NOT NULL,
    
    status VARCHAR(50) DEFAULT 'Pending', -- Pending, Processed, Paid
    
    payment_method VARCHAR(50),
    payment_reference VARCHAR(255),
    paid_at TIMESTAMP,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_settlements_driver ON driver_settlements(driver_id);
CREATE INDEX idx_settlements_period ON driver_settlements(period_start, period_end);
```

#### 4.1.7 Document Schema

```sql
-- Document types
CREATE TABLE document_types (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL UNIQUE,
    category VARCHAR(50) NOT NULL, -- Vehicle, Driver, User
    is_required BOOLEAN DEFAULT TRUE,
    has_expiry BOOLEAN DEFAULT TRUE,
    expiry_alert_days INTEGER DEFAULT 30,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Documents
CREATE TABLE documents (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    document_type_id UUID REFERENCES document_types(id),
    entity_type VARCHAR(50) NOT NULL, -- Vehicle, Driver, User
    entity_id UUID NOT NULL,
    
    document_number VARCHAR(100),
    issue_date DATE,
    expiry_date DATE,
    
    file_url TEXT NOT NULL,
    file_name VARCHAR(255),
    file_size_bytes BIGINT,
    mime_type VARCHAR(100),
    
    verification_status VARCHAR(50) DEFAULT 'Pending', -- Pending, Verified, Rejected
    verified_by UUID REFERENCES users(id),
    verified_at TIMESTAMP,
    rejection_reason TEXT,
    
    extracted_data JSONB,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_documents_entity ON documents(entity_type, entity_id);
CREATE INDEX idx_documents_type ON documents(document_type_id);
CREATE INDEX idx_documents_expiry ON documents(expiry_date);
CREATE INDEX idx_documents_verification ON documents(verification_status);
```

#### 4.1.8 Compliance Schema

```sql
-- BAN times
CREATE TABLE ban_times (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    city VARCHAR(100) NOT NULL,
    vehicle_type_id UUID REFERENCES vehicle_types(id),
    
    ban_start_time TIME NOT NULL,
    ban_end_time TIME NOT NULL,
    
    days_of_week INTEGER[], -- Array of days: 0=Sunday, 6=Saturday
    
    is_active BOOLEAN DEFAULT TRUE,
    reason TEXT,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_ban_times_city ON ban_times(city);

-- Compliance violations
CREATE TABLE compliance_violations (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    entity_type VARCHAR(50) NOT NULL, -- Vehicle, Driver
    entity_id UUID NOT NULL,
    
    violation_type VARCHAR(100) NOT NULL, -- ExpiredDocument, MissingDocument, BANViolation
    severity VARCHAR(20) NOT NULL, -- Low, Medium, High, Critical
    
    description TEXT,
    
    is_blocking BOOLEAN DEFAULT FALSE,
    resolved_at TIMESTAMP,
    resolved_by UUID REFERENCES users(id),
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_violations_entity ON compliance_violations(entity_type, entity_id);
CREATE INDEX idx_violations_resolved ON compliance_violations(resolved_at);
```

### 4.2 MongoDB Collections

MongoDB is used for analytics, logs, and document-oriented data.

#### 4.2.1 Notification Logs

```javascript
{
  _id: ObjectId,
  userId: UUID,
  notificationType: String, // Push, SMS, Email, InApp
  channel: String,
  template: String,
  subject: String,
  body: String,
  data: Object,
  status: String, // Sent, Failed, Delivered, Read
  sentAt: ISODate,
  deliveredAt: ISODate,
  readAt: ISODate,
  error: String,
  metadata: Object
}
```

#### 4.2.2 Location History

```javascript
{
  _id: ObjectId,
  tripId: UUID,
  driverId: UUID,
  vehicleId: UUID,
  location: {
    type: "Point",
    coordinates: [longitude, latitude]
  },
  accuracy: Number,
  speed: Number,
  heading: Number,
  timestamp: ISODate,
  metadata: Object
}

// Geospatial index
db.locationHistory.createIndex({ location: "2dsphere" })
db.locationHistory.createIndex({ tripId: 1, timestamp: 1 })
```

#### 4.2.3 Analytics Events

```javascript
{
  _id: ObjectId,
  eventType: String,
  eventCategory: String,
  userId: UUID,
  sessionId: String,
  properties: Object,
  timestamp: ISODate,
  deviceInfo: {
    platform: String,
    osVersion: String,
    appVersion: String,
    deviceId: String
  },
  location: {
    city: String,
    country: String,
    coordinates: [longitude, latitude]
  }
}

db.analyticsEvents.createIndex({ eventType: 1, timestamp: -1 })
db.analyticsEvents.createIndex({ userId: 1, timestamp: -1 })
```

#### 4.2.4 Audit Logs

```javascript
{
  _id: ObjectId,
  action: String,
  entityType: String,
  entityId: UUID,
  userId: UUID,
  userName: String,
  changes: {
    before: Object,
    after: Object
  },
  ipAddress: String,
  userAgent: String,
  timestamp: ISODate
}

db.auditLogs.createIndex({ entityType: 1, entityId: 1, timestamp: -1 })
db.auditLogs.createIndex({ userId: 1, timestamp: -1 })
```

#### 4.2.5 Trip Analytics

```javascript
{
  _id: ObjectId,
  tripId: UUID,
  bookingNumber: String,
  customerId: UUID,
  driverId: UUID,
  vehicleId: UUID,
  
  route: {
    origin: { city: String, coordinates: [Number, Number] },
    destination: { city: String, coordinates: [Number, Number] },
    distanceKm: Number,
    actualDistanceKm: Number
  },
  
  timing: {
    bookedAt: ISODate,
    scheduledPickup: ISODate,
    driverAssignedAt: ISODate,
    driverReachedAt: ISODate,
    loadingStartedAt: ISODate,
    tripStartedAt: ISODate,
    deliveredAt: ISODate,
    completedAt: ISODate,
    
    assignmentDelayMinutes: Number,
    loadingDurationMinutes: Number,
    tripDurationMinutes: Number,
    totalDurationMinutes: Number
  },
  
  financial: {
    baseFare: Number,
    discountAmount: Number,
    waitingCharges: Number,
    totalFare: Number,
    platformCommission: Number,
    driverEarnings: Number
  },
  
  performance: {
    onTimePickup: Boolean,
    onTimeDelivery: Boolean,
    customerRating: Number,
    driverRating: Number
  },
  
  isBackload: Boolean,
  isSharedLoad: Boolean,
  
  createdAt: ISODate
}

db.tripAnalytics.createIndex({ tripId: 1 })
db.tripAnalytics.createIndex({ customerId: 1, createdAt: -1 })
db.tripAnalytics.createIndex({ driverId: 1, createdAt: -1 })
db.tripAnalytics.createIndex({ "route.origin.city": 1, "route.destination.city": 1 })
```

---

## 5. Messaging Architecture (RabbitMQ)

### 5.1 Exchange and Queue Design

```
┌─────────────────────────────────────────────────────────────┐
│                    RabbitMQ Exchanges                        │
└─────────────────────────────────────────────────────────────┘

Exchange: wol.booking.events (Topic)
├── Queue: booking.created → Notification Service
├── Queue: booking.created → Analytics Service
├── Queue: booking.assigned → Notification Service
├── Queue: booking.assigned → Tracking Service
├── Queue: booking.completed → Payment Service
├── Queue: booking.completed → Analytics Service
└── Queue: booking.cancelled → Payment Service

Exchange: wol.payment.events (Topic)
├── Queue: payment.completed → Booking Service
├── Queue: payment.completed → Notification Service
├── Queue: payment.failed → Booking Service
└── Queue: refund.processed → Notification Service

Exchange: wol.tracking.events (Topic)
├── Queue: driver.reached → Booking Service
├── Queue: driver.reached → Notification Service
├── Queue: trip.started → Analytics Service
└── Queue: trip.completed → Booking Service

Exchange: wol.compliance.events (Topic)
├── Queue: document.expiring → Notification Service
├── Queue: document.expired → Compliance Service
└── Queue: compliance.violation → Notification Service

Exchange: wol.backload.events (Topic)
├── Queue: backload.available → Matching Service
├── Queue: backload.matched → Notification Service
└── Queue: backload.matched → Pricing Service

Exchange: wol.notifications (Fanout)
├── Queue: notifications.push → Firebase Service
├── Queue: notifications.sms → SMS Service
└── Queue: notifications.email → Email Service
```

### 5.2 Message Contracts

#### Booking Events

```csharp
public class BookingCreatedEvent
{
    public Guid BookingId { get; set; }
    public string BookingNumber { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerMobile { get; set; }
    public string OriginCity { get; set; }
    public string DestinationCity { get; set; }
    public DateTime PickupDateTime { get; set; }
    public Guid VehicleTypeId { get; set; }
    public decimal TotalFare { get; set; }
    public bool IsBackload { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BookingAssignedEvent
{
    public Guid BookingId { get; set; }
    public string BookingNumber { get; set; }
    public Guid CustomerId { get; set; }
    public Guid DriverId { get; set; }
    public string DriverName { get; set; }
    public string DriverMobile { get; set; }
    public Guid VehicleId { get; set; }
    public string VehiclePlateNumber { get; set; }
    public DateTime AssignedAt { get; set; }
}

public class BookingCompletedEvent
{
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid DriverId { get; set; }
    public decimal TotalFare { get; set; }
    public decimal WaitingCharges { get; set; }
    public DateTime CompletedAt { get; set; }
}
```

#### Payment Events

```csharp
public class PaymentCompletedEvent
{
    public Guid PaymentId { get; set; }
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    public string TransactionId { get; set; }
    public DateTime CompletedAt { get; set; }
}

public class RefundProcessedEvent
{
    public Guid RefundId { get; set; }
    public Guid PaymentId { get; set; }
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal RefundAmount { get; set; }
    public string Reason { get; set; }
    public DateTime ProcessedAt { get; set; }
}
```

#### Compliance Events

```csharp
public class DocumentExpiringEvent
{
    public Guid DocumentId { get; set; }
    public string DocumentType { get; set; }
    public string EntityType { get; set; }
    public Guid EntityId { get; set; }
    public Guid OwnerId { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int DaysUntilExpiry { get; set; }
}

public class ComplianceViolationEvent
{
    public Guid ViolationId { get; set; }
    public string EntityType { get; set; }
    public Guid EntityId { get; set; }
    public string ViolationType { get; set; }
    public string Severity { get; set; }
    public bool IsBlocking { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### 5.3 MassTransit Configuration

```csharp
public static class MassTransitConfiguration
{
    public static IServiceCollection AddMassTransitWithRabbitMq(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            // Add consumers
            x.AddConsumers(Assembly.GetExecutingAssembly());
            
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"], h =>
                {
                    h.Username(configuration["RabbitMQ:Username"]);
                    h.Password(configuration["RabbitMQ:Password"]);
                });
                
                // Configure retry policy
                cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                
                // Configure circuit breaker
                cfg.UseCircuitBreaker(cb =>
                {
                    cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                    cb.TripThreshold = 15;
                    cb.ActiveThreshold = 10;
                    cb.ResetInterval = TimeSpan.FromMinutes(5);
                });
                
                cfg.ConfigureEndpoints(context);
            });
        });
        
        return services;
    }
}
```

---

## 6. API Gateway & Authentication

### 6.1 API Gateway (Ocelot)

**Ocelot Configuration (ocelot.json):**

```json
{
  "Routes": [
    {
      "DownstreamPathTemplate": "/api/{everything}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "identity-service",
          "Port": 5001
        }
      ],
      "UpstreamPathTemplate": "/api/identity/{everything}",
      "UpstreamHttpMethod": [ "GET", "POST", "PUT", "DELETE" ],
      "RouteIsCaseSensitive": false,
      "Key": "identity-service"
    },
    {
      "DownstreamPathTemplate": "/api/{everything}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "booking-service",
          "Port": 5002
        }
      ],
      "UpstreamPathTemplate": "/api/bookings/{everything}",
      "UpstreamHttpMethod": [ "GET", "POST", "PUT", "DELETE" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      },
      "RouteIsCaseSensitive": false,
      "Key": "booking-service"
    },
    {
      "DownstreamPathTemplate": "/api/{everything}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "vehicle-service",
          "Port": 5003
        }
      ],
      "UpstreamPathTemplate": "/api/vehicles/{everything}",
      "UpstreamHttpMethod": [ "GET", "POST", "PUT", "DELETE" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      },
      "RouteIsCaseSensitive": false,
      "Key": "vehicle-service"
    },
    {
      "DownstreamPathTemplate": "/api/{everything}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "pricing-service",
          "Port": 5004
        }
      ],
      "UpstreamPathTemplate": "/api/pricing/{everything}",
      "UpstreamHttpMethod": [ "GET", "POST" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      },
      "RouteIsCaseSensitive": false,
      "Key": "pricing-service"
    },
    {
      "DownstreamPathTemplate": "/api/{everything}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "backload-service",
          "Port": 5005
        }
      ],
      "UpstreamPathTemplate": "/api/backload/{everything}",
      "UpstreamHttpMethod": [ "GET", "POST", "PUT", "DELETE" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      },
      "RouteIsCaseSensitive": false,
      "Key": "backload-service"
    },
    {
      "DownstreamPathTemplate": "/api/{everything}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "tracking-service",
          "Port": 5006
        }
      ],
      "UpstreamPathTemplate": "/api/tracking/{everything}",
      "UpstreamHttpMethod": [ "GET", "POST", "PUT" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      },
      "RouteIsCaseSensitive": false,
      "Key": "tracking-service"
    },
    {
      "DownstreamPathTemplate": "/api/{everything}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "payment-service",
          "Port": 5007
        }
      ],
      "UpstreamPathTemplate": "/api/payments/{everything}",
      "UpstreamHttpMethod": [ "GET", "POST", "PUT" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      },
      "RouteIsCaseSensitive": false,
      "Key": "payment-service"
    },
    {
      "DownstreamPathTemplate": "/api/{everything}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "notification-service",
          "Port": 5008
        }
      ],
      "UpstreamPathTemplate": "/api/notifications/{everything}",
      "UpstreamHttpMethod": [ "GET", "POST", "PUT" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      },
      "RouteIsCaseSensitive": false,
      "Key": "notification-service"
    },
    {
      "DownstreamPathTemplate": "/api/{everything}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "document-service",
          "Port": 5009
        }
      ],
      "UpstreamPathTemplate": "/api/documents/{everything}",
      "UpstreamHttpMethod": [ "GET", "POST", "PUT", "DELETE" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      },
      "RouteIsCaseSensitive": false,
      "Key": "document-service"
    },
    {
      "DownstreamPathTemplate": "/api/{everything}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "compliance-service",
          "Port": 5010
        }
      ],
      "UpstreamPathTemplate": "/api/compliance/{everything}",
      "UpstreamHttpMethod": [ "GET", "POST" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      },
      "RouteIsCaseSensitive": false,
      "Key": "compliance-service"
    },
    {
      "DownstreamPathTemplate": "/api/{everything}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "analytics-service",
          "Port": 5011
        }
      ],
      "UpstreamPathTemplate": "/api/analytics/{everything}",
      "UpstreamHttpMethod": [ "GET" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      },
      "RouteIsCaseSensitive": false,
      "Key": "analytics-service"
    }
  ],
  "GlobalConfiguration": {
    "BaseUrl": "http://localhost:5000",
    "RateLimitOptions": {
      "DisableRateLimitHeaders": false,
      "QuotaExceededMessage": "Rate limit exceeded. Please try again later.",
      "HttpStatusCode": 429
    },
    "QoSOptions": {
      "ExceptionsAllowedBeforeBreaking": 3,
      "DurationOfBreak": 10000,
      "TimeoutValue": 30000
    },
    "LoadBalancerOptions": {
      "Type": "RoundRobin"
    },
    "ServiceDiscoveryProvider": {
      "Type": "Docker",
      "Host": "localhost",
      "Port": 2375
    }
  }
}
```

**Program.cs for Ocelot API Gateway:**

```csharp
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add Ocelot configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["Jwt:Issuer"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };
    });

// Add Ocelot
builder.Services.AddOcelot();

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

var app = builder.Build();

app.UseCors("AllowAll");

// Use Ocelot middleware
await app.UseOcelot();

app.Run();
```

### 6.2 JWT Authentication

```csharp
public class JwtTokenService
{
    private readonly IConfiguration _configuration;
    
    public string GenerateAccessToken(User user, List<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.MobilePhone, user.MobileNumber),
            new Claim("user_type", user.UserType),
            new Claim("preferred_language", user.PreferredLanguage)
        };
        
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
```

### 6.3 Authorization Policies

```csharp
public static class AuthorizationPolicies
{
    public const string CustomerOnly = "CustomerOnly";
    public const string DriverOnly = "DriverOnly";
    public const string AdminOnly = "AdminOnly";
    public const string CustomerOrDriver = "CustomerOrDriver";
    
    public static void AddWolAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(CustomerOnly, policy =>
                policy.RequireClaim("user_type", "Individual", "Commercial"));
            
            options.AddPolicy(DriverOnly, policy =>
                policy.RequireClaim("user_type", "ServiceProvider"));
            
            options.AddPolicy(AdminOnly, policy =>
                policy.RequireRole("Admin", "SuperAdmin"));
            
            options.AddPolicy(CustomerOrDriver, policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "user_type" &&
                        (c.Value == "Individual" || c.Value == "Commercial" || c.Value == "ServiceProvider")
                    )
                ));
        });
    }
}
```

---

## 7. Mobile Application Architecture (React Native)

### 7.1 Project Structure

```
wol-mobile/
├── src/
│   ├── api/
│   │   ├── client.ts
│   │   ├── endpoints/
│   │   │   ├── auth.ts
│   │   │   ├── bookings.ts
│   │   │   ├── vehicles.ts
│   │   │   └── tracking.ts
│   │   └── interceptors.ts
│   ├── components/
│   │   ├── common/
│   │   │   ├── Button.tsx
│   │   │   ├── Input.tsx
│   │   │   ├── Card.tsx
│   │   │   └── Loading.tsx
│   │   ├── booking/
│   │   │   ├── BookingCard.tsx
│   │   │   ├── BookingForm.tsx
│   │   │   └── BookingDetails.tsx
│   │   ├── tracking/
│   │   │   ├── MapView.tsx
│   │   │   ├── LiveTracking.tsx
│   │   │   └── ETADisplay.tsx
│   │   └── vehicle/
│   │       ├── VehicleCard.tsx
│   │       └── VehicleForm.tsx
│   ├── screens/
│   │   ├── auth/
│   │   │   ├── LoginScreen.tsx
│   │   │   ├── RegisterScreen.tsx
│   │   │   └── OTPVerificationScreen.tsx
│   │   ├── customer/
│   │   │   ├── HomeScreen.tsx
│   │   │   ├── BookingScreen.tsx
│   │   │   ├── BookingHistoryScreen.tsx
│   │   │   └── TrackingScreen.tsx
│   │   ├── driver/
│   │   │   ├── DashboardScreen.tsx
│   │   │   ├── AvailableTripsScreen.tsx
│   │   │   ├── ActiveTripScreen.tsx
│   │   │   └── EarningsScreen.tsx
│   │   └── common/
│   │       ├── ProfileScreen.tsx
│   │       └── SettingsScreen.tsx
│   ├── navigation/
│   │   ├── AppNavigator.tsx
│   │   ├── AuthNavigator.tsx
│   │   ├── CustomerNavigator.tsx
│   │   └── DriverNavigator.tsx
│   ├── store/
│   │   ├── index.ts
│   │   ├── slices/
│   │   │   ├── authSlice.ts
│   │   │   ├── bookingSlice.ts
│   │   │   ├── vehicleSlice.ts
│   │   │   └── trackingSlice.ts
│   │   └── middleware/
│   │       └── apiMiddleware.ts
│   ├── hooks/
│   │   ├── useAuth.ts
│   │   ├── useBooking.ts
│   │   ├── useLocation.ts
│   │   └── useNotifications.ts
│   ├── services/
│   │   ├── LocationService.ts
│   │   ├── NotificationService.ts
│   │   ├── StorageService.ts
│   │   └── PermissionService.ts
│   ├── utils/
│   │   ├── validation.ts
│   │   ├── formatting.ts
│   │   ├── constants.ts
│   │   └── helpers.ts
│   ├── localization/
│   │   ├── i18n.ts
│   │   ├── en.json
│   │   ├── ar.json
│   │   └── ur.json
│   ├── theme/
│   │   ├── colors.ts
│   │   ├── typography.ts
│   │   └── spacing.ts
│   └── types/
│       ├── api.ts
│       ├── models.ts
│       └── navigation.ts
├── android/
├── ios/
├── package.json
└── tsconfig.json
```

### 7.2 State Management (Redux Toolkit)

```typescript
// store/slices/bookingSlice.ts
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { bookingApi } from '@/api/endpoints/bookings';

export const createBooking = createAsyncThunk(
  'booking/create',
  async (bookingData: CreateBookingRequest) => {
    const response = await bookingApi.createBooking(bookingData);
    return response.data;
  }
);

export const fetchBookings = createAsyncThunk(
  'booking/fetchAll',
  async (userId: string) => {
    const response = await bookingApi.getCustomerBookings(userId);
    return response.data;
  }
);

interface BookingState {
  bookings: Booking[];
  currentBooking: Booking | null;
  loading: boolean;
  error: string | null;
}

const initialState: BookingState = {
  bookings: [],
  currentBooking: null,
  loading: false,
  error: null,
};

const bookingSlice = createSlice({
  name: 'booking',
  initialState,
  reducers: {
    setCurrentBooking: (state, action) => {
      state.currentBooking = action.payload;
    },
    clearError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(createBooking.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(createBooking.fulfilled, (state, action) => {
        state.loading = false;
        state.bookings.unshift(action.payload);
        state.currentBooking = action.payload;
      })
      .addCase(createBooking.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to create booking';
      })
      .addCase(fetchBookings.fulfilled, (state, action) => {
        state.bookings = action.payload;
      });
  },
});

export const { setCurrentBooking, clearError } = bookingSlice.actions;
export default bookingSlice.reducer;
```

### 7.3 Real-time Tracking

```typescript
// services/LocationService.ts
import Geolocation from '@react-native-community/geolocation';
import { io, Socket } from 'socket.io-client';

class LocationService {
  private socket: Socket | null = null;
  private watchId: number | null = null;
  
  connect(tripId: string, token: string) {
    this.socket = io(API_BASE_URL, {
      auth: { token },
      transports: ['websocket'],
    });
    
    this.socket.on('connect', () => {
      console.log('Connected to tracking server');
      this.socket?.emit('joinTripTracking', tripId);
    });
    
    this.socket.on('locationUpdated', (location) => {
      // Handle location updates from other participants
      console.log('Location updated:', location);
    });
  }
  
  startTracking(tripId: string) {
    this.watchId = Geolocation.watchPosition(
      (position) => {
        const location = {
          tripId,
          latitude: position.coords.latitude,
          longitude: position.coords.longitude,
          accuracy: position.coords.accuracy,
          speed: position.coords.speed,
          heading: position.coords.heading,
          timestamp: new Date(position.timestamp).toISOString(),
        };
        
        // Send location to server
        this.socket?.emit('updateLocation', location);
      },
      (error) => {
        console.error('Location error:', error);
      },
      {
        enableHighAccuracy: true,
        distanceFilter: 10, // Update every 10 meters
        interval: 5000, // Update every 5 seconds
        fastestInterval: 3000,
      }
    );
  }
  
  stopTracking() {
    if (this.watchId !== null) {
      Geolocation.clearWatch(this.watchId);
      this.watchId = null;
    }
    
    if (this.socket) {
      this.socket.disconnect();
      this.socket = null;
    }
  }
}

export default new LocationService();
```

### 7.4 Push Notifications

```typescript
// services/NotificationService.ts
import messaging from '@react-native-firebase/messaging';
import notifee from '@notifee/react-native';

class NotificationService {
  async initialize() {
    // Request permission
    const authStatus = await messaging().requestPermission();
    const enabled =
      authStatus === messaging.AuthorizationStatus.AUTHORIZED ||
      authStatus === messaging.AuthorizationStatus.PROVISIONAL;
    
    if (enabled) {
      // Get FCM token
      const token = await messaging().getToken();
      console.log('FCM Token:', token);
      
      // Send token to backend
      await this.registerToken(token);
    }
    
    // Handle foreground messages
    messaging().onMessage(async (remoteMessage) => {
      await this.displayNotification(remoteMessage);
    });
    
    // Handle background messages
    messaging().setBackgroundMessageHandler(async (remoteMessage) => {
      console.log('Background message:', remoteMessage);
    });
  }
  
  async displayNotification(message: any) {
    const channelId = await notifee.createChannel({
      id: 'default',
      name: 'Default Channel',
      importance: AndroidImportance.HIGH,
    });
    
    await notifee.displayNotification({
      title: message.notification?.title,
      body: message.notification?.body,
      android: {
        channelId,
        smallIcon: 'ic_launcher',
        pressAction: {
          id: 'default',
        },
      },
      ios: {
        sound: 'default',
      },
      data: message.data,
    });
  }
  
  async registerToken(token: string) {
    // Send token to backend API
    // await api.post('/api/notifications/register-device', { token });
  }
}

export default new NotificationService();
```

---

## 8. Web Admin Architecture (React)

### 8.1 Project Structure

```
wol-admin/
├── src/
│   ├── api/
│   │   ├── client.ts
│   │   └── endpoints/
│   ├── components/
│   │   ├── layout/
│   │   │   ├── Sidebar.tsx
│   │   │   ├── Header.tsx
│   │   │   └── Layout.tsx
│   │   ├── dashboard/
│   │   │   ├── StatsCard.tsx
│   │   │   ├── RevenueChart.tsx
│   │   │   └── RouteHeatmap.tsx
│   │   ├── bookings/
│   │   │   ├── BookingTable.tsx
│   │   │   ├── BookingDetails.tsx
│   │   │   └── BookingFilters.tsx
│   │   ├── users/
│   │   │   ├── UserTable.tsx
│   │   │   └── UserForm.tsx
│   │   └── common/
│   │       ├── DataTable.tsx
│   │       ├── Modal.tsx
│   │       └── Charts.tsx
│   ├── pages/
│   │   ├── Dashboard.tsx
│   │   ├── Bookings.tsx
│   │   ├── Users.tsx
│   │   ├── Vehicles.tsx
│   │   ├── Drivers.tsx
│   │   ├── Pricing.tsx
│   │   ├── Analytics.tsx
│   │   ├── Reports.tsx
│   │   └── Settings.tsx
│   ├── hooks/
│   │   ├── useAuth.ts
│   │   ├── useQuery.ts
│   │   └── useMutation.ts
│   ├── store/
│   │   └── index.ts
│   ├── utils/
│   │   ├── validation.ts
│   │   └── formatting.ts
│   ├── types/
│   │   └── index.ts
│   └── App.tsx
├── package.json
└── tsconfig.json
```

### 8.2 Dashboard Components

```typescript
// pages/Dashboard.tsx
import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { dashboardApi } from '@/api/endpoints/dashboard';
import StatsCard from '@/components/dashboard/StatsCard';
import RevenueChart from '@/components/dashboard/RevenueChart';
import RouteHeatmap from '@/components/dashboard/RouteHeatmap';

const Dashboard: React.FC = () => {
  const { data: stats, isLoading } = useQuery({
    queryKey: ['dashboard-stats'],
    queryFn: dashboardApi.getStats,
  });
  
  if (isLoading) return <div>Loading...</div>;
  
  return (
    <div className="p-6">
      <h1 className="text-3xl font-bold mb-6">Dashboard</h1>
      
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <StatsCard
          title="Total Bookings"
          value={stats?.totalBookings}
          change={stats?.bookingsChange}
          icon="📦"
        />
        <StatsCard
          title="Active Trips"
          value={stats?.activeTrips}
          icon="🚚"
        />
        <StatsCard
          title="Total Revenue"
          value={`SAR ${stats?.totalRevenue?.toLocaleString()}`}
          change={stats?.revenueChange}
          icon="💰"
        />
        <StatsCard
          title="Utilization Rate"
          value={`${stats?.utilizationRate}%`}
          change={stats?.utilizationChange}
          icon="📊"
        />
      </div>
      
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        <RevenueChart data={stats?.revenueData} />
        <RouteHeatmap data={stats?.routeData} />
      </div>
    </div>
  );
};

export default Dashboard;
```

---

## 9. Infrastructure & Deployment

### 9.1 Docker Compose (Development)

```yaml
version: '3.8'

services:
  # API Gateway
  api-gateway:
    build:
      context: ./src/ApiGateway
      dockerfile: Dockerfile
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
    depends_on:
      - identity-service
      - booking-service
    networks:
      - wol-network

  # Identity Service
  identity-service:
    build:
      context: ./src/Services/Identity
      dockerfile: Dockerfile
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=wol_identity;Username=postgres;Password=postgres
      - Redis__ConnectionString=redis:6379
    depends_on:
      - postgres
      - redis
    networks:
      - wol-network

  # Booking Service
  booking-service:
    build:
      context: ./src/Services/Booking
      dockerfile: Dockerfile
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=wol_booking;Username=postgres;Password=postgres
      - RabbitMQ__Host=rabbitmq
    depends_on:
      - postgres
      - rabbitmq
    networks:
      - wol-network

  # Vehicle Service
  vehicle-service:
    build:
      context: ./src/Services/Vehicle
      dockerfile: Dockerfile
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=wol_vehicle;Username=postgres;Password=postgres
    depends_on:
      - postgres
    networks:
      - wol-network

  # Pricing Service
  pricing-service:
    build:
      context: ./src/Services/Pricing
      dockerfile: Dockerfile
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=wol_pricing;Username=postgres;Password=postgres
      - Redis__ConnectionString=redis:6379
    depends_on:
      - postgres
      - redis
    networks:
      - wol-network

  # Backload Service
  backload-service:
    build:
      context: ./src/Services/Backload
      dockerfile: Dockerfile
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=wol_backload;Username=postgres;Password=postgres
      - MongoDB__ConnectionString=mongodb://mongo:27017
      - MongoDB__DatabaseName=wol_analytics
    depends_on:
      - postgres
      - mongo
    networks:
      - wol-network

  # Tracking Service
  tracking-service:
    build:
      context: ./src/Services/Tracking
      dockerfile: Dockerfile
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=wol_tracking;Username=postgres;Password=postgres
      - MongoDB__ConnectionString=mongodb://mongo:27017
      - MongoDB__DatabaseName=wol_tracking
      - Redis__ConnectionString=redis:6379
    depends_on:
      - postgres
      - mongo
      - redis
    networks:
      - wol-network

  # Payment Service
  payment-service:
    build:
      context: ./src/Services/Payment
      dockerfile: Dockerfile
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=wol_payment;Username=postgres;Password=postgres
      - RabbitMQ__Host=rabbitmq
    depends_on:
      - postgres
      - rabbitmq
    networks:
      - wol-network

  # Notification Service
  notification-service:
    build:
      context: ./src/Services/Notification
      dockerfile: Dockerfile
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - MongoDB__ConnectionString=mongodb://mongo:27017
      - MongoDB__DatabaseName=wol_notifications
      - RabbitMQ__Host=rabbitmq
    depends_on:
      - mongo
      - rabbitmq
    networks:
      - wol-network

  # PostgreSQL
  postgres:
    image: postgres:16-alpine
    environment:
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=postgres
    ports:
      - "5432:5432"
    volumes:
      - postgres-data:/var/lib/postgresql/data
    networks:
      - wol-network

  # MongoDB
  mongo:
    image: mongo:7.0
    ports:
      - "27017:27017"
    volumes:
      - mongo-data:/data/db
    networks:
      - wol-network

  # Redis
  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    volumes:
      - redis-data:/data
    networks:
      - wol-network

  # RabbitMQ
  rabbitmq:
    image: rabbitmq:3.12-management-alpine
    ports:
      - "5672:5672"
      - "15672:15672"
    environment:
      - RABBITMQ_DEFAULT_USER=guest
      - RABBITMQ_DEFAULT_PASS=guest
    volumes:
      - rabbitmq-data:/var/lib/rabbitmq
    networks:
      - wol-network

  # Elasticsearch
  elasticsearch:
    image: docker.elastic.co/elasticsearch/elasticsearch:8.11.0
    environment:
      - discovery.type=single-node
      - xpack.security.enabled=false
    ports:
      - "9200:9200"
    volumes:
      - elasticsearch-data:/usr/share/elasticsearch/data
    networks:
      - wol-network

volumes:
  postgres-data:
  mongo-data:
  redis-data:
  rabbitmq-data:
  elasticsearch-data:

networks:
  wol-network:
    driver: bridge
```

### 9.2 Kubernetes Deployment

```yaml
# k8s/booking-service-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: booking-service
  namespace: wol
spec:
  replicas: 3
  selector:
    matchLabels:
      app: booking-service
  template:
    metadata:
      labels:
        app: booking-service
    spec:
      containers:
      - name: booking-service
        image: wol/booking-service:latest
        ports:
        - containerPort: 80
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: database-secrets
              key: booking-connection-string
        - name: RabbitMQ__Host
          value: "rabbitmq-service"
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health
            port: 80
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 80
          initialDelaySeconds: 10
          periodSeconds: 5
---
apiVersion: v1
kind: Service
metadata:
  name: booking-service
  namespace: wol
spec:
  selector:
    app: booking-service
  ports:
  - protocol: TCP
    port: 80
    targetPort: 80
  type: ClusterIP
---
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: booking-service-hpa
  namespace: wol
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: booking-service
  minReplicas: 3
  maxReplicas: 10
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
  - type: Resource
    resource:
      name: memory
      target:
        type: Utilization
        averageUtilization: 80
```

---

## 10. Security Architecture

### 10.1 Security Layers

1. **Network Security**
   - TLS/SSL encryption for all communications
   - API Gateway with rate limiting
   - DDoS protection
   - Firewall rules

2. **Authentication & Authorization**
   - JWT-based authentication
   - Multi-factor authentication (OTP)
   - Role-based access control (RBAC)
   - Token refresh mechanism

3. **Data Security**
   - Encryption at rest (database encryption)
   - Encryption in transit (TLS 1.3)
   - Sensitive data masking in logs
   - PII data protection

4. **Application Security**
   - Input validation
   - SQL injection prevention (parameterized queries)
   - XSS protection
   - CSRF protection
   - Secure file upload validation

5. **API Security**
   - API key management
   - Request signing
   - Rate limiting per user/IP
   - API versioning

### 10.2 Security Best Practices

```csharp
// Secure password hashing
public class PasswordHasher
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }
    
    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}

// Input validation
public class BookingValidator : AbstractValidator<CreateBookingRequest>
{
    public BookingValidator()
    {
        RuleFor(x => x.OriginAddress)
            .NotEmpty()
            .MaximumLength(500);
        
        RuleFor(x => x.DestinationAddress)
            .NotEmpty()
            .MaximumLength(500);
        
        RuleFor(x => x.PickupDate)
            .GreaterThanOrEqualTo(DateTime.Today);
        
        RuleFor(x => x.ShipperMobile)
            .Matches(@"^(05|5)[0-9]{8}$")
            .WithMessage("Invalid Saudi mobile number");
    }
}

// Rate limiting
public class RateLimitingMiddleware
{
    public async Task InvokeAsync(HttpContext context, IDistributedCache cache)
    {
        var key = $"rate_limit:{context.User.Identity?.Name ?? context.Connection.RemoteIpAddress}";
        var count = await cache.GetStringAsync(key);
        
        if (int.TryParse(count, out var requestCount) && requestCount >= 100)
        {
            context.Response.StatusCode = 429; // Too Many Requests
            await context.Response.WriteAsync("Rate limit exceeded");
            return;
        }
        
        await cache.SetStringAsync(key, (requestCount + 1).ToString(), 
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            });
        
        await _next(context);
    }
}
```

---

## 11. Monitoring & Observability

### 11.1 Logging (Serilog)

```csharp
public static class LoggingConfiguration
{
    public static IHostBuilder ConfigureLogging(this IHostBuilder builder)
    {
        return builder.UseSerilog((context, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .Enrich.WithProperty("Application", "WOL")
                .WriteTo.Console()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(
                    new Uri(context.Configuration["Elasticsearch:Uri"]))
                {
                    IndexFormat = "wol-logs-{0:yyyy.MM.dd}",
                    AutoRegisterTemplate = true,
                    NumberOfShards = 2,
                    NumberOfReplicas = 1
                })
                .WriteTo.File(
                    path: "logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30);
        });
    }
}
```

### 11.2 Metrics (Prometheus)

```csharp
public static class MetricsConfiguration
{
    public static IServiceCollection AddMetrics(this IServiceCollection services)
    {
        services.AddSingleton<IMetrics>(sp =>
        {
            var metrics = new MetricsBuilder()
                .Configuration.Configure(options =>
                {
                    options.DefaultContextLabel = "wol";
                })
                .OutputMetrics.AsPrometheusPlainText()
                .Build();
            
            return metrics;
        });
        
        return services;
    }
}

// Custom metrics
public class BookingMetrics
{
    private readonly IMetrics _metrics;
    
    public BookingMetrics(IMetrics metrics)
    {
        _metrics = metrics;
    }
    
    public void RecordBookingCreated()
    {
        _metrics.Measure.Counter.Increment(
            new CounterOptions { Name = "bookings_created_total" });
    }
    
    public void RecordBookingDuration(double durationSeconds)
    {
        _metrics.Measure.Histogram.Update(
            new HistogramOptions { Name = "booking_duration_seconds" },
            durationSeconds);
    }
}
```

### 11.3 Distributed Tracing (OpenTelemetry)

```csharp
public static class TracingConfiguration
{
    public static IServiceCollection AddTracing(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddSource("WOL.*")
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService("WOL", serviceVersion: "1.0.0"))
                    .AddJaegerExporter(options =>
                    {
                        options.AgentHost = configuration["Jaeger:Host"];
                        options.AgentPort = int.Parse(configuration["Jaeger:Port"]);
                    });
            });
        
        return services;
    }
}
```

---

## 12. Implementation Roadmap

### Phase 1: Foundation (Weeks 1-4)
- Set up development environment
- Implement Identity Service
- Implement API Gateway
- Set up databases (PostgreSQL, MongoDB, Redis)
- Set up RabbitMQ
- Implement basic authentication and authorization

### Phase 2: Core Services (Weeks 5-10)
- Implement Booking Service
- Implement Vehicle Service
- Implement Pricing Service
- Implement Payment Service (basic)
- Implement Notification Service
- Implement Document Service

### Phase 3: Advanced Features (Weeks 11-14)
- Implement Backload Service with matching algorithm
- Implement Tracking Service with real-time updates
- Implement Compliance Service
- Enhance Payment Service with multiple gateways

### Phase 4: Mobile Applications (Weeks 15-20)
- Develop React Native mobile app
- Implement customer flows
- Implement driver flows
- Integrate real-time tracking
- Implement push notifications
- Multi-language support

### Phase 5: Web Admin (Weeks 21-24)
- Develop React web admin portal
- Implement dashboard and analytics
- Implement user management
- Implement pricing management
- Implement reporting

### Phase 6: Analytics & Reporting (Weeks 25-28)
- Implement Analytics Service
- Implement Reporting Service
- Build ML models for demand prediction
- Implement route optimization

### Phase 7: Testing & QA (Weeks 29-32)
- Unit testing
- Integration testing
- End-to-end testing
- Performance testing
- Security testing
- User acceptance testing

### Phase 8: Deployment & Launch (Weeks 33-36)
- Set up production infrastructure
- Deploy to Kubernetes
- Configure monitoring and alerting
- Load testing
- Soft launch
- Full launch

---

## Conclusion

This enterprise architecture provides a comprehensive, scalable, and maintainable solution for the World of Logistics platform. The microservices architecture ensures that each component can be developed, deployed, and scaled independently. The use of modern technologies like .NET Core, React Native, React, PostgreSQL, MongoDB, and RabbitMQ ensures high performance, reliability, and developer productivity.

The clean architecture approach ensures that business logic is decoupled from infrastructure concerns, making the system easier to test and maintain. The intelligent backload matching system provides a unique competitive advantage that can significantly reduce costs for customers while increasing utilization for carriers.

The system is designed with security, scalability, and observability in mind, ensuring that it can handle growth and provide insights into system behavior. The comprehensive monitoring and logging infrastructure ensures that issues can be quickly identified and resolved.

This architecture serves as a solid foundation for building a world-class logistics platform that can transform the freight transportation industry in Saudi Arabia.
