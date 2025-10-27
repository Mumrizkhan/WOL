# World of Logistics (WOL) - Enterprise System

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-18-61DAFB?logo=react)](https://reactjs.org/)
[![React Native](https://img.shields.io/badge/React_Native-Latest-61DAFB?logo=react)](https://reactnative.dev/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql)](https://www.postgresql.org/)
[![MongoDB](https://img.shields.io/badge/MongoDB-7.0-47A248?logo=mongodb)](https://www.mongodb.com/)
[![RabbitMQ](https://img.shields.io/badge/RabbitMQ-3.12-FF6600?logo=rabbitmq)](https://www.rabbitmq.com/)

## Overview

World of Logistics (WOL) is a comprehensive enterprise logistics platform designed for the Saudi Arabian market. The platform connects shippers with transport service providers, enabling efficient freight booking, real-time tracking, backload optimization, and complete logistics management.

### Key Features

- **Multi-User Support**: Individual customers, commercial entities, and service providers
- **Real-Time Tracking**: GPS-based shipment tracking with live ETA updates
- **Smart Backload Matching**: AI-powered algorithm to optimize return trips and reduce empty miles
- **Flexible Booking**: Support for one-way, backload, and shared load bookings
- **Compliance Management**: Automated BAN time checking and document verification
- **Multi-Language Support**: English, Arabic, and Urdu
- **Payment Integration**: Multiple payment methods including cards, wallets, and invoicing
- **Document Management**: OCR-based document verification and expiry tracking
- **Analytics & Reporting**: Comprehensive dashboards and automated reports

## Architecture

WOL is built using a **microservices architecture** with **clean architecture** principles, ensuring scalability, maintainability, and testability.

### Technology Stack

#### Backend
- **.NET 8.0**: Microservices backend
- **ASP.NET Core**: Web API framework
- **Entity Framework Core 8.0**: ORM for PostgreSQL
- **MediatR**: CQRS pattern implementation
- **FluentValidation**: Input validation
- **AutoMapper**: Object mapping
- **Polly**: Resilience and transient fault handling
- **Serilog**: Structured logging
- **MassTransit**: Message bus abstraction
- **SignalR**: Real-time communication

#### Frontend
- **React 18**: Web admin dashboard
- **React Native**: Cross-platform mobile apps (iOS & Android)
- **TypeScript**: Type-safe development
- **Redux Toolkit**: State management
- **React Query**: Server state management
- **Tailwind CSS**: Utility-first CSS framework

#### Databases
- **PostgreSQL 16**: Transactional data storage
- **MongoDB 7.0**: Analytics, logs, and document-oriented data
- **Redis 7.x**: Caching and session management

#### Messaging & Events
- **RabbitMQ 3.12**: Message broker for asynchronous communication
- **Event-Driven Architecture**: Domain events and integration events

#### Infrastructure
- **Docker**: Containerization
- **Kubernetes**: Container orchestration
- **Ocelot**: API Gateway and reverse proxy
- **Nginx**: Load balancing
- **Prometheus**: Metrics collection
- **Grafana**: Metrics visualization
- **ELK Stack**: Centralized logging (Elasticsearch, Logstash, Kibana)
- **OpenTelemetry**: Distributed tracing

### Microservices

The platform consists of 12 core microservices and 6 worker services:

**API Services:**
1. **Identity Service**: User authentication, authorization, and profile management
2. **Booking Service**: Trip booking, scheduling, and lifecycle management
3. **Vehicle Service**: Fleet and driver management
4. **Pricing Service**: Dynamic fare calculation and discount management
5. **Backload Service**: Return trip optimization and matching
6. **Tracking Service**: Real-time GPS tracking and geofencing
7. **Payment Service**: Payment processing, refunds, and settlements
8. **Notification Service**: Multi-channel notifications (Push, SMS, Email)
9. **Document Service**: Document upload, verification, and OCR processing
10. **Compliance Service**: BAN time checking and regulatory compliance
11. **Analytics Service**: Business intelligence and reporting
12. **Reporting Service**: Scheduled and on-demand report generation

**Worker Services (Background Processing):**
1. **Notification Worker**: Process RabbitMQ messages and send notifications via Push/SMS/Email
2. **Analytics Worker**: Aggregate analytics data and calculate KPIs
3. **Document Worker**: Perform OCR processing and document verification
4. **Compliance Worker**: Monitor compliance and check document expiry
5. **Reporting Worker**: Generate scheduled reports and exports
6. **Backload Worker**: Match backload opportunities with bookings

### Architecture Diagrams

For detailed architecture diagrams and design decisions, see [ENTERPRISE_ARCHITECTURE.md](./ENTERPRISE_ARCHITECTURE.md).

## Documentation

### Core Documentation

- **[Enterprise Architecture](./ENTERPRISE_ARCHITECTURE.md)**: Complete system architecture, design patterns, and technical decisions
- **[API Specifications](./API_SPECIFICATIONS.md)**: Comprehensive REST API documentation for all microservices
- **[Implementation Guide](./IMPLEMENTATION_GUIDE.md)**: Step-by-step guide for developers to implement the system

### Database Documentation

- **[PostgreSQL Schema](./database/init-postgres.sql)**: Complete database schemas for all microservices
- **[MongoDB Schema](./database/init-mongodb.js)**: MongoDB collections and indexes

## Getting Started

### Prerequisites

Before you begin, ensure you have the following installed:

- **.NET 8.0 SDK** or later
- **Node.js 20.x** or later
- **Docker Desktop**
- **PostgreSQL 16**
- **MongoDB 7.0**
- **Redis 7.x**
- **RabbitMQ 3.12**
- **Git**

### Quick Start with Docker Compose

The fastest way to get started is using Docker Compose:

```bash
# Clone the repository
git clone https://github.com/Mumrizkhan/WOL.git
cd WOL

# Start all services
docker-compose up -d

# Check service status
docker-compose ps

# View logs
docker-compose logs -f
```

This will start:
- PostgreSQL (port 5432)
- MongoDB (port 27017)
- Redis (port 6379)
- RabbitMQ (port 5672, management UI on 15672)
- All microservices
- API Gateway (port 5000)

### Manual Setup

For detailed manual setup instructions, see the [Implementation Guide](./IMPLEMENTATION_GUIDE.md).

#### 1. Database Setup

**PostgreSQL:**
```bash
# Start PostgreSQL
docker run --name wol-postgres \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  -d postgres:16-alpine

# Initialize databases
psql -h localhost -U postgres -f database/init-postgres.sql
```

**MongoDB:**
```bash
# Start MongoDB
docker run --name wol-mongo \
  -p 27017:27017 \
  -d mongo:7.0

# Initialize collections
mongosh < database/init-mongodb.js
```

#### 2. Backend Services

```bash
# Navigate to a service (example: Booking Service)
cd src/Services/Booking/WOL.Booking.API

# Restore dependencies
dotnet restore

# Apply migrations
dotnet ef database update

# Run the service
dotnet run
```

#### 3. Frontend Applications

**Web Admin:**
```bash
cd src/Web/WOL.Admin
npm install
npm run dev
```

**Mobile App:**
```bash
cd src/Mobile/WOL.Mobile
npm install

# iOS
npm run ios

# Android
npm run android
```

## Development

### Project Structure

```
WOL/
├── src/
│   ├── ApiGateway/              # API Gateway (Ocelot)
│   ├── Services/                # Microservices
│   │   ├── Identity/
│   │   ├── Booking/
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
│   ├── BuildingBlocks/          # Shared libraries
│   ├── Mobile/                  # React Native app
│   └── Web/                     # React admin dashboard
├── tests/                       # Test projects
├── docs/                        # Additional documentation
├── database/                    # Database scripts
├── k8s/                         # Kubernetes manifests
├── docker-compose.yml           # Docker Compose configuration
└── README.md                    # This file
```

### Clean Architecture Layers

Each microservice follows Clean Architecture with four layers:

1. **Domain Layer**: Business entities, value objects, domain events
2. **Application Layer**: Use cases, commands, queries, DTOs
3. **Infrastructure Layer**: Data access, external services, messaging
4. **API Layer**: Controllers, middleware, API configuration

### Development Workflow

1. **Create Feature Branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Implement Changes**
   - Follow clean architecture principles
   - Write unit tests for domain logic
   - Write integration tests for API endpoints
   - Update documentation

3. **Run Tests**
   ```bash
   dotnet test
   ```

4. **Commit Changes**
   ```bash
   git add .
   git commit -m "feat: your feature description"
   ```

5. **Push and Create PR**
   ```bash
   git push origin feature/your-feature-name
   ```

### Coding Standards

- **C# Style**: Follow Microsoft C# coding conventions
- **Naming**: Use PascalCase for classes/methods, camelCase for variables
- **Comments**: Avoid unnecessary comments; write self-documenting code
- **SOLID Principles**: Follow SOLID design principles
- **DRY**: Don't Repeat Yourself
- **Testing**: Aim for >80% code coverage

## API Documentation

### Base URLs

- **Development**: `http://localhost:5000/api`
- **Staging**: `https://staging-api.wol.sa/api`
- **Production**: `https://api.wol.sa/api`

### Authentication

All API requests require authentication using JWT Bearer tokens:

```bash
curl -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  https://api.wol.sa/api/bookings
```

### API Endpoints

For complete API documentation, see [API_SPECIFICATIONS.md](./API_SPECIFICATIONS.md).

#### Quick Reference

| Service | Endpoint | Description |
|---------|----------|-------------|
| Identity | `/api/auth/register` | User registration |
| Identity | `/api/auth/login` | User login |
| Booking | `/api/bookings` | Create/manage bookings |
| Vehicle | `/api/vehicles` | Fleet management |
| Tracking | `/api/tracking/{tripId}` | Real-time tracking |
| Payment | `/api/payments` | Payment processing |

### Swagger UI

Interactive API documentation is available at:
- Development: `http://localhost:5000/swagger`
- Staging: `https://staging-api.wol.sa/swagger`

## Testing

### Unit Tests

```bash
# Run all unit tests
dotnet test tests/UnitTests/

# Run specific service tests
dotnet test tests/UnitTests/WOL.Booking.UnitTests/
```

### Integration Tests

```bash
# Run all integration tests
dotnet test tests/IntegrationTests/

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### E2E Tests

```bash
# Run end-to-end tests
dotnet test tests/E2ETests/
```

## Deployment

### Docker Deployment

```bash
# Build all services
docker-compose build

# Deploy to production
docker-compose -f docker-compose.prod.yml up -d
```

### Kubernetes Deployment

```bash
# Apply all manifests
kubectl apply -f k8s/

# Check deployment status
kubectl get pods -n wol

# View service logs
kubectl logs -f deployment/booking-service -n wol
```

For detailed deployment instructions, see the [Implementation Guide](./IMPLEMENTATION_GUIDE.md#deployment-guide).

## Monitoring

### Metrics

- **Prometheus**: `http://localhost:9090`
- **Grafana**: `http://localhost:3000` (admin/admin)

### Logging

- **Kibana**: `http://localhost:5601`
- **Elasticsearch**: `http://localhost:9200`

### Tracing

- **Jaeger UI**: `http://localhost:16686`

## Security

### Authentication & Authorization

- **JWT Tokens**: 15-minute access tokens, 7-day refresh tokens
- **Role-Based Access Control (RBAC)**: Admin, Customer, Driver, Support roles
- **Multi-Factor Authentication**: OTP verification via SMS

### Data Protection

- **Encryption at Rest**: Database encryption enabled
- **Encryption in Transit**: TLS 1.3 for all communications
- **PII Protection**: Personal data encrypted and access-controlled
- **GDPR Compliance**: Data retention and deletion policies

### Security Best Practices

- Never commit secrets to version control
- Use environment variables for configuration
- Rotate credentials regularly
- Enable audit logging for sensitive operations
- Implement rate limiting on all endpoints

## Performance

### Optimization Strategies

- **Caching**: Redis for frequently accessed data
- **Database Indexing**: Optimized indexes on all tables
- **Connection Pooling**: Efficient database connections
- **Async Operations**: Non-blocking I/O throughout
- **CDN**: Static assets served via CDN
- **Load Balancing**: Horizontal scaling with Kubernetes

### Performance Targets

- **API Response Time**: < 200ms (p95)
- **Database Query Time**: < 50ms (p95)
- **Real-Time Updates**: < 1s latency
- **Uptime**: 99.9% availability
- **Concurrent Users**: 10,000+ simultaneous users

## Troubleshooting

### Common Issues

#### Database Connection Failed
```bash
# Check PostgreSQL is running
docker ps | grep postgres

# Test connection
psql -h localhost -U postgres -d wol_booking
```

#### Service Not Starting
```bash
# Check logs
docker logs booking-service

# Check environment variables
docker inspect booking-service
```

#### RabbitMQ Connection Failed
```bash
# Check RabbitMQ status
docker ps | grep rabbitmq

# Access management UI
# http://localhost:15672 (guest/guest)
```

For more troubleshooting tips, see the [Implementation Guide](./IMPLEMENTATION_GUIDE.md#troubleshooting).

## Contributing

We welcome contributions! Please follow these guidelines:

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Write/update tests
5. Update documentation
6. Submit a pull request

### Pull Request Process

1. Ensure all tests pass
2. Update README if needed
3. Follow commit message conventions
4. Request review from maintainers
5. Address review feedback

## Roadmap

### Phase 1: Foundation (Weeks 1-8)
- ✅ Architecture design
- ✅ Database schema design
- ✅ API specifications
- 🔄 Core microservices implementation
- 🔄 Authentication & authorization

### Phase 2: Core Features (Weeks 9-16)
- ⏳ Booking management
- ⏳ Vehicle & driver management
- ⏳ Real-time tracking
- ⏳ Payment integration

### Phase 3: Advanced Features (Weeks 17-24)
- ⏳ Backload optimization
- ⏳ Analytics & reporting
- ⏳ Document verification
- ⏳ Compliance management

### Phase 4: Mobile & Web (Weeks 25-32)
- ⏳ React Native mobile app
- ⏳ React web admin dashboard
- ⏳ User testing & feedback

### Phase 5: Production (Weeks 33-36)
- ⏳ Performance optimization
- ⏳ Security hardening
- ⏳ Production deployment
- ⏳ Monitoring & alerting

## License

Copyright © 2024 World of Logistics (WOL). All rights reserved.

## Support

For support and questions:

- **Email**: support@wol.sa
- **Documentation**: [https://docs.wol.sa](https://docs.wol.sa)
- **Issues**: [GitHub Issues](https://github.com/Mumrizkhan/WOL/issues)

## Acknowledgments

- Built with ❤️ for the Saudi Arabian logistics industry
- Powered by .NET, React, and open-source technologies
- Designed following industry best practices and clean architecture principles

---

**Version**: 1.0.0  
**Last Updated**: January 2024  
**Maintained by**: WOL Development Team
