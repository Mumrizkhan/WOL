# World of Logistics (WOL) - API Specifications

## Overview

This document provides detailed API specifications for all microservices in the WOL platform. All APIs follow RESTful principles and use JSON for request/response payloads.

## Base URLs

```
Development:  http://localhost:5000/api
Staging:      https://staging-api.wol.sa/api
Production:   https://api.wol.sa/api
```

## Authentication

All authenticated endpoints require a Bearer token in the Authorization header:

```
Authorization: Bearer {access_token}
```

## Common Response Format

### Success Response
```json
{
  "success": true,
  "data": { ... },
  "message": "Operation completed successfully",
  "timestamp": "2025-10-27T10:30:00Z"
}
```

### Error Response
```json
{
  "success": false,
  "error": {
    "code": "ERROR_CODE",
    "message": "Human readable error message",
    "details": { ... }
  },
  "timestamp": "2025-10-27T10:30:00Z"
}
```

## Common HTTP Status Codes

- `200 OK` - Request succeeded
- `201 Created` - Resource created successfully
- `400 Bad Request` - Invalid request data
- `401 Unauthorized` - Authentication required
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `409 Conflict` - Resource conflict
- `422 Unprocessable Entity` - Validation failed
- `429 Too Many Requests` - Rate limit exceeded
- `500 Internal Server Error` - Server error

---

## 1. Identity Service API

### 1.1 Register User

**Endpoint:** `POST /identity/register`

**Description:** Register a new user (Individual, Commercial, or Service Provider)

**Request Body:**
```json
{
  "userType": "Individual",
  "mobileNumber": "+966501234567",
  "email": "user@example.com",
  "iqamaNumber": "1234567890",
  "idNumber": "1234567890",
  "password": "SecurePassword123!",
  "preferredLanguage": "en",
  "profile": {
    "fullName": "Ahmed Ali",
    "commercialLicense": "1234567890",
    "commercialLicenseExpiry": "2026-12-31",
    "vatNumber": "300123456789003",
    "city": "Riyadh",
    "address": "King Fahd Road, Riyadh"
  }
}
```

**Response:** `201 Created`
```json
{
  "success": true,
  "data": {
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "mobileNumber": "+966501234567",
    "email": "user@example.com",
    "userType": "Individual",
    "status": "PendingVerification"
  },
  "message": "Registration successful. Please verify your mobile number."
}
```

---

### 1.2 Verify OTP

**Endpoint:** `POST /identity/verify-otp`

**Description:** Verify OTP sent to mobile number

**Request Body:**
```json
{
  "mobileNumber": "+966501234567",
  "otpCode": "123456",
  "purpose": "Registration"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "verified": true,
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4...",
    "expiresIn": 86400,
    "user": {
      "userId": "550e8400-e29b-41d4-a716-446655440000",
      "fullName": "Ahmed Ali",
      "mobileNumber": "+966501234567",
      "userType": "Individual",
      "roles": ["Customer"]
    }
  }
}
```

---

### 1.3 Login

**Endpoint:** `POST /identity/login`

**Description:** Authenticate user and get access token

**Request Body:**
```json
{
  "mobileNumber": "+966501234567",
  "password": "SecurePassword123!"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "requiresOtp": true,
    "otpSent": true,
    "message": "OTP sent to your mobile number"
  }
}
```

---

### 1.4 Refresh Token

**Endpoint:** `POST /identity/refresh-token`

**Description:** Get new access token using refresh token

**Request Body:**
```json
{
  "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4..."
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "bmV3IHJlZnJlc2ggdG9rZW4...",
    "expiresIn": 86400
  }
}
```

---

### 1.5 Get Profile

**Endpoint:** `GET /identity/profile`

**Authentication:** Required

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "userType": "Individual",
    "mobileNumber": "+966501234567",
    "email": "user@example.com",
    "profile": {
      "fullName": "Ahmed Ali",
      "city": "Riyadh",
      "address": "King Fahd Road, Riyadh",
      "profileImageUrl": "https://storage.wol.sa/profiles/user123.jpg"
    },
    "isEmailVerified": true,
    "isMobileVerified": true,
    "preferredLanguage": "en",
    "status": "Active",
    "createdAt": "2025-01-15T10:30:00Z"
  }
}
```

---

### 1.6 Update Profile

**Endpoint:** `PUT /identity/profile`

**Authentication:** Required

**Request Body:**
```json
{
  "fullName": "Ahmed Ali Updated",
  "email": "newemail@example.com",
  "city": "Jeddah",
  "address": "Corniche Road, Jeddah",
  "preferredLanguage": "ar"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "profile": { ... }
  },
  "message": "Profile updated successfully"
}
```

---

## 2. Booking Service API

### 2.1 Create Booking

**Endpoint:** `POST /bookings`

**Authentication:** Required

**Request Body:**
```json
{
  "vehicleTypeId": "660e8400-e29b-41d4-a716-446655440001",
  "origin": {
    "address": "King Fahd Road, Riyadh",
    "latitude": 24.7136,
    "longitude": 46.6753,
    "city": "Riyadh"
  },
  "destination": {
    "address": "Corniche Road, Jeddah",
    "latitude": 21.5433,
    "longitude": 39.1728,
    "city": "Jeddah"
  },
  "pickupDate": "2025-11-01",
  "pickupTime": "08:00:00",
  "isWholeDayBooking": false,
  "isFlexibleDate": false,
  "cargo": {
    "type": "Dry",
    "grossWeightKg": 5000,
    "dimensions": {
      "lengthCm": 400,
      "widthCm": 200,
      "heightCm": 200
    },
    "numberOfBoxes": 50,
    "imageUrl": "https://storage.wol.sa/cargo/img123.jpg"
  },
  "shipper": {
    "name": "Ahmed Ali",
    "mobile": "+966501234567",
    "alternateMobile": "+966509876543"
  },
  "receiver": {
    "name": "Mohammed Hassan",
    "mobile": "+966502345678",
    "alternateMobile": "+966508765432"
  },
  "bookingType": "OneWay",
  "isBackload": false,
  "isSharedLoad": false
}
```

**Response:** `201 Created`
```json
{
  "success": true,
  "data": {
    "bookingId": "770e8400-e29b-41d4-a716-446655440002",
    "bookingNumber": "WOL-2025-001234",
    "status": "Pending",
    "pricing": {
      "baseFare": 2800.00,
      "discountAmount": 0.00,
      "waitingCharges": 0.00,
      "totalFare": 2800.00,
      "vatAmount": 420.00,
      "finalAmount": 3220.00,
      "currency": "SAR"
    },
    "estimatedDistance": 950.5,
    "estimatedDuration": 540,
    "pickupDateTime": "2025-11-01T08:00:00Z",
    "createdAt": "2025-10-27T10:30:00Z"
  },
  "message": "Booking created successfully. Searching for available drivers..."
}
```

---

### 2.2 Get Booking Details

**Endpoint:** `GET /bookings/{bookingId}`

**Authentication:** Required

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "bookingId": "770e8400-e29b-41d4-a716-446655440002",
    "bookingNumber": "WOL-2025-001234",
    "customerId": "550e8400-e29b-41d4-a716-446655440000",
    "vehicleId": "880e8400-e29b-41d4-a716-446655440003",
    "driverId": "990e8400-e29b-41d4-a716-446655440004",
    "status": "DriverAssigned",
    "origin": {
      "address": "King Fahd Road, Riyadh",
      "latitude": 24.7136,
      "longitude": 46.6753,
      "city": "Riyadh"
    },
    "destination": {
      "address": "Corniche Road, Jeddah",
      "latitude": 21.5433,
      "longitude": 39.1728,
      "city": "Jeddah"
    },
    "pickupDateTime": "2025-11-01T08:00:00Z",
    "vehicle": {
      "vehicleId": "880e8400-e29b-41d4-a716-446655440003",
      "plateNumber": "ABC-1234",
      "type": "Box Truck",
      "imageUrl": "https://storage.wol.sa/vehicles/truck123.jpg"
    },
    "driver": {
      "driverId": "990e8400-e29b-41d4-a716-446655440004",
      "name": "Mohammed Hassan",
      "mobile": "+966503456789",
      "rating": 4.8,
      "totalTrips": 245
    },
    "cargo": {
      "type": "Dry",
      "grossWeightKg": 5000,
      "dimensions": {
        "lengthCm": 400,
        "widthCm": 200,
        "heightCm": 200
      },
      "numberOfBoxes": 50
    },
    "pricing": {
      "baseFare": 2800.00,
      "discountAmount": 0.00,
      "waitingCharges": 0.00,
      "totalFare": 2800.00,
      "vatAmount": 420.00,
      "finalAmount": 3220.00,
      "currency": "SAR"
    },
    "timeline": {
      "createdAt": "2025-10-27T10:30:00Z",
      "driverAssignedAt": "2025-10-27T10:35:00Z",
      "driverAcceptedAt": "2025-10-27T10:36:00Z",
      "driverReachedAt": null,
      "loadingStartedAt": null,
      "tripStartedAt": null,
      "deliveredAt": null,
      "completedAt": null
    },
    "freeTimeMinutes": 120,
    "bookingType": "OneWay"
  }
}
```

---

### 2.3 Get Customer Bookings

**Endpoint:** `GET /bookings/customer/{customerId}`

**Authentication:** Required

**Query Parameters:**
- `status` (optional): Filter by status (Pending, Active, Completed, Cancelled)
- `page` (optional): Page number (default: 1)
- `pageSize` (optional): Items per page (default: 20)
- `sortBy` (optional): Sort field (default: createdAt)
- `sortOrder` (optional): asc or desc (default: desc)

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "bookingId": "770e8400-e29b-41d4-a716-446655440002",
        "bookingNumber": "WOL-2025-001234",
        "status": "DriverAssigned",
        "origin": {
          "city": "Riyadh",
          "address": "King Fahd Road, Riyadh"
        },
        "destination": {
          "city": "Jeddah",
          "address": "Corniche Road, Jeddah"
        },
        "pickupDateTime": "2025-11-01T08:00:00Z",
        "vehicleType": "Box Truck",
        "totalFare": 3220.00,
        "createdAt": "2025-10-27T10:30:00Z"
      }
    ],
    "pagination": {
      "currentPage": 1,
      "pageSize": 20,
      "totalItems": 45,
      "totalPages": 3
    }
  }
}
```

---

### 2.4 Get Available Trips (Driver)

**Endpoint:** `GET /bookings/available`

**Authentication:** Required (Driver only)

**Query Parameters:**
- `vehicleTypeId` (optional): Filter by vehicle type
- `city` (optional): Filter by pickup city
- `date` (optional): Filter by pickup date
- `page` (optional): Page number
- `pageSize` (optional): Items per page

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "bookingId": "770e8400-e29b-41d4-a716-446655440002",
        "bookingNumber": "WOL-2025-001234",
        "origin": {
          "city": "Riyadh",
          "address": "King Fahd Road, Riyadh",
          "latitude": 24.7136,
          "longitude": 46.6753
        },
        "destination": {
          "city": "Jeddah",
          "address": "Corniche Road, Jeddah",
          "latitude": 21.5433,
          "longitude": 39.1728
        },
        "pickupDateTime": "2025-11-01T08:00:00Z",
        "vehicleType": "Box Truck",
        "cargoType": "Dry",
        "grossWeightKg": 5000,
        "distanceKm": 950.5,
        "estimatedDuration": 540,
        "fare": 2800.00,
        "isBackload": false,
        "distanceFromDriver": 5.2,
        "createdAt": "2025-10-27T10:30:00Z"
      }
    ],
    "pagination": {
      "currentPage": 1,
      "pageSize": 20,
      "totalItems": 12,
      "totalPages": 1
    }
  }
}
```

---

### 2.5 Accept Booking (Driver)

**Endpoint:** `POST /bookings/{bookingId}/accept`

**Authentication:** Required (Driver only)

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "bookingId": "770e8400-e29b-41d4-a716-446655440002",
    "status": "Accepted",
    "acceptedAt": "2025-10-27T10:36:00Z",
    "pickupLocation": {
      "address": "King Fahd Road, Riyadh",
      "latitude": 24.7136,
      "longitude": 46.6753
    },
    "customerContact": {
      "name": "Ahmed Ali",
      "mobile": "+966501234567"
    }
  },
  "message": "Booking accepted successfully. Please proceed to pickup location."
}
```

---

### 2.6 Mark as Reached

**Endpoint:** `PUT /bookings/{bookingId}/mark-reached`

**Authentication:** Required (Driver only)

**Request Body:**
```json
{
  "latitude": 24.7136,
  "longitude": 46.6753,
  "photoUrl": "https://storage.wol.sa/arrivals/photo123.jpg",
  "timestamp": "2025-11-01T07:55:00Z"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "bookingId": "770e8400-e29b-41d4-a716-446655440002",
    "status": "DriverReached",
    "reachedAt": "2025-11-01T07:55:00Z",
    "freeTimeEndsAt": "2025-11-01T09:55:00Z"
  },
  "message": "Arrival confirmed. Customer has been notified."
}
```

---

### 2.7 Mark as Loaded

**Endpoint:** `PUT /bookings/{bookingId}/mark-loaded`

**Authentication:** Required (Driver only)

**Request Body:**
```json
{
  "loadingCompletedAt": "2025-11-01T08:30:00Z",
  "photoUrl": "https://storage.wol.sa/loading/photo123.jpg"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "bookingId": "770e8400-e29b-41d4-a716-446655440002",
    "status": "InTransit",
    "loadingCompletedAt": "2025-11-01T08:30:00Z",
    "tripStartedAt": "2025-11-01T08:30:00Z",
    "waitingCharges": 0.00
  },
  "message": "Loading confirmed. Trip started."
}
```

---

### 2.8 Mark as Delivered

**Endpoint:** `PUT /bookings/{bookingId}/mark-delivered`

**Authentication:** Required (Driver only)

**Request Body:**
```json
{
  "deliveredAt": "2025-11-01T17:30:00Z",
  "latitude": 21.5433,
  "longitude": 39.1728,
  "photoUrl": "https://storage.wol.sa/delivery/photo123.jpg",
  "receiverSignature": "base64_encoded_signature",
  "notes": "Delivered successfully"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "bookingId": "770e8400-e29b-41d4-a716-446655440002",
    "status": "Delivered",
    "deliveredAt": "2025-11-01T17:30:00Z",
    "totalDuration": 570,
    "actualDistance": 955.2
  },
  "message": "Delivery confirmed. Trip completed."
}
```

---

### 2.9 Cancel Booking

**Endpoint:** `POST /bookings/{bookingId}/cancel`

**Authentication:** Required

**Request Body:**
```json
{
  "reason": "Customer not available",
  "cancelledBy": "Customer"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "bookingId": "770e8400-e29b-41d4-a716-446655440002",
    "status": "Cancelled",
    "cancelledAt": "2025-10-27T11:00:00Z",
    "cancellationFee": 100.00,
    "refundAmount": 3120.00,
    "refundStatus": "Pending"
  },
  "message": "Booking cancelled. Refund will be processed within 3-7 business days."
}
```

---

### 2.10 Calculate Charges

**Endpoint:** `GET /bookings/{bookingId}/calculate-charges`

**Authentication:** Required

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "bookingId": "770e8400-e29b-41d4-a716-446655440002",
    "baseFare": 2800.00,
    "discountAmount": 0.00,
    "waitingCharges": 200.00,
    "waitingDetails": {
      "freeTimeMinutes": 120,
      "waitingMinutes": 120,
      "chargePerHour": 100.00,
      "totalHours": 2
    },
    "totalFare": 3000.00,
    "vatAmount": 450.00,
    "finalAmount": 3450.00,
    "currency": "SAR"
  }
}
```

---

## 3. Vehicle Service API

### 3.1 Register Vehicle

**Endpoint:** `POST /vehicles`

**Authentication:** Required (Service Provider only)

**Request Body:**
```json
{
  "vehicleTypeId": "660e8400-e29b-41d4-a716-446655440001",
  "plateNumber": "ABC-1234",
  "istemaraNumber": "IST-123456",
  "istemaraExpiry": "2026-12-31",
  "mvpiNumber": "MVPI-123456",
  "mvpiExpiry": "2026-06-30",
  "insuranceNumber": "INS-123456",
  "insuranceExpiry": "2026-12-31",
  "vehicleImageUrl": "https://storage.wol.sa/vehicles/truck123.jpg",
  "currentCity": "Riyadh"
}
```

**Response:** `201 Created`
```json
{
  "success": true,
  "data": {
    "vehicleId": "880e8400-e29b-41d4-a716-446655440003",
    "plateNumber": "ABC-1234",
    "vehicleType": "Box Truck",
    "status": "PendingVerification",
    "createdAt": "2025-10-27T10:30:00Z"
  },
  "message": "Vehicle registered successfully. Pending document verification."
}
```

---

### 3.2 Get Vehicle Details

**Endpoint:** `GET /vehicles/{vehicleId}`

**Authentication:** Required

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "vehicleId": "880e8400-e29b-41d4-a716-446655440003",
    "ownerId": "550e8400-e29b-41d4-a716-446655440000",
    "plateNumber": "ABC-1234",
    "vehicleType": {
      "id": "660e8400-e29b-41d4-a716-446655440001",
      "name": "Box Truck",
      "category": "Box",
      "capacityTons": 10.0,
      "capacityCubicMeters": 40.0,
      "dimensions": {
        "lengthMeters": 6.0,
        "widthMeters": 2.4,
        "heightMeters": 2.6
      }
    },
    "documents": {
      "istemara": {
        "number": "IST-123456",
        "expiryDate": "2026-12-31",
        "status": "Verified",
        "daysUntilExpiry": 430
      },
      "mvpi": {
        "number": "MVPI-123456",
        "expiryDate": "2026-06-30",
        "status": "Verified",
        "daysUntilExpiry": 246
      },
      "insurance": {
        "number": "INS-123456",
        "expiryDate": "2026-12-31",
        "status": "Verified",
        "daysUntilExpiry": 430
      }
    },
    "status": "Active",
    "isAvailable": true,
    "currentLocation": {
      "city": "Riyadh",
      "latitude": 24.7136,
      "longitude": 46.6753
    },
    "vehicleImageUrl": "https://storage.wol.sa/vehicles/truck123.jpg",
    "complianceStatus": "Compliant",
    "createdAt": "2025-10-27T10:30:00Z"
  }
}
```

---

### 3.3 Update Vehicle Availability

**Endpoint:** `PUT /vehicles/{vehicleId}/availability`

**Authentication:** Required (Owner only)

**Request Body:**
```json
{
  "isAvailable": true,
  "currentCity": "Riyadh",
  "latitude": 24.7136,
  "longitude": 46.6753
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "vehicleId": "880e8400-e29b-41d4-a716-446655440003",
    "isAvailable": true,
    "currentLocation": {
      "city": "Riyadh",
      "latitude": 24.7136,
      "longitude": 46.6753
    },
    "updatedAt": "2025-10-27T10:30:00Z"
  }
}
```

---

### 3.4 Register Driver

**Endpoint:** `POST /drivers`

**Authentication:** Required (Service Provider only)

**Request Body:**
```json
{
  "fullName": "Mohammed Hassan",
  "mobileNumber": "+966503456789",
  "iqamaNumber": "2345678901",
  "iqamaExpiry": "2027-12-31",
  "licenseNumber": "LIC-123456",
  "licenseExpiry": "2028-12-31"
}
```

**Response:** `201 Created`
```json
{
  "success": true,
  "data": {
    "driverId": "990e8400-e29b-41d4-a716-446655440004",
    "fullName": "Mohammed Hassan",
    "mobileNumber": "+966503456789",
    "status": "PendingVerification",
    "createdAt": "2025-10-27T10:30:00Z"
  },
  "message": "Driver registered successfully. Pending document verification."
}
```

---

### 3.5 Get Vehicle Types

**Endpoint:** `GET /vehicles/types`

**Authentication:** Not required

**Response:** `200 OK`
```json
{
  "success": true,
  "data": [
    {
      "id": "660e8400-e29b-41d4-a716-446655440001",
      "name": "Light Truck (Dyna)",
      "category": "Light",
      "subTypes": ["Box", "Flatbed", "Reefer", "Canopy"],
      "capacityTons": 3.5,
      "capacityCubicMeters": 15.0,
      "dimensions": {
        "lengthMeters": 4.5,
        "widthMeters": 2.0,
        "heightMeters": 2.2
      },
      "baseRatePerKm": 2.5
    },
    {
      "id": "660e8400-e29b-41d4-a716-446655440002",
      "name": "Box Truck",
      "category": "Box",
      "subTypes": ["Rigid", "Semi-trailer"],
      "capacityTons": 10.0,
      "capacityCubicMeters": 40.0,
      "dimensions": {
        "lengthMeters": 6.0,
        "widthMeters": 2.4,
        "heightMeters": 2.6
      },
      "baseRatePerKm": 3.5
    }
  ]
}
```

---

## 4. Pricing Service API

### 4.1 Calculate Fare

**Endpoint:** `POST /pricing/calculate`

**Authentication:** Required

**Request Body:**
```json
{
  "vehicleTypeId": "660e8400-e29b-41d4-a716-446655440001",
  "origin": {
    "city": "Riyadh",
    "latitude": 24.7136,
    "longitude": 46.6753
  },
  "destination": {
    "city": "Jeddah",
    "latitude": 21.5433,
    "longitude": 39.1728
  },
  "pickupDateTime": "2025-11-01T08:00:00Z",
  "distance": 950.5,
  "estimatedDuration": 540,
  "isBackload": false,
  "isFlexibleDate": false,
  "isSharedLoad": false,
  "customerId": "550e8400-e29b-41d4-a716-446655440000"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "baseFare": 2800.00,
    "breakdown": {
      "distanceFare": 2375.00,
      "timeFare": 225.00,
      "baseFee": 200.00
    },
    "surgeMultiplier": 1.0,
    "discounts": [
      {
        "type": "Backload",
        "amount": 0.00,
        "percentage": 0
      },
      {
        "type": "FlexibleDate",
        "amount": 0.00,
        "percentage": 0
      }
    ],
    "totalDiscount": 0.00,
    "subtotal": 2800.00,
    "vatAmount": 420.00,
    "totalFare": 3220.00,
    "currency": "SAR",
    "estimatedDistance": 950.5,
    "estimatedDuration": 540
  }
}
```

---

### 4.2 Get Fare Estimate

**Endpoint:** `POST /pricing/estimate`

**Authentication:** Not required

**Request Body:**
```json
{
  "originCity": "Riyadh",
  "destinationCity": "Jeddah",
  "vehicleTypeId": "660e8400-e29b-41d4-a716-446655440001"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "estimatedFare": {
      "min": 2500.00,
      "max": 3200.00,
      "average": 2850.00
    },
    "distance": 950.5,
    "duration": 540,
    "currency": "SAR"
  }
}
```

---

## 5. Backload Service API

### 5.1 Post Backload Availability

**Endpoint:** `POST /backload/availability`

**Authentication:** Required (Driver only)

**Request Body:**
```json
{
  "vehicleId": "880e8400-e29b-41d4-a716-446655440003",
  "currentLocation": {
    "address": "Corniche Road, Jeddah",
    "city": "Jeddah",
    "latitude": 21.5433,
    "longitude": 39.1728
  },
  "returnCity": "Riyadh",
  "availableFrom": "2025-11-01T18:00:00Z",
  "availableTo": "2025-11-02T12:00:00Z",
  "capacityAvailableTons": 10.0,
  "capacityAvailableCubicMeters": 40.0,
  "preferredCargoTypes": ["Dry", "Perishable"],
  "minimumPriceExpected": 2000.00
}
```

**Response:** `201 Created`
```json
{
  "success": true,
  "data": {
    "availabilityId": "aa0e8400-e29b-41d4-a716-446655440005",
    "vehicleId": "880e8400-e29b-41d4-a716-446655440003",
    "currentCity": "Jeddah",
    "returnCity": "Riyadh",
    "availableFrom": "2025-11-01T18:00:00Z",
    "availableTo": "2025-11-02T12:00:00Z",
    "status": "Available",
    "createdAt": "2025-11-01T17:30:00Z"
  },
  "message": "Backload availability posted. Searching for matching loads..."
}
```

---

### 5.2 Search Backload Opportunities

**Endpoint:** `POST /backload/search`

**Authentication:** Required

**Request Body:**
```json
{
  "origin": {
    "city": "Jeddah",
    "latitude": 21.5433,
    "longitude": 39.1728
  },
  "destination": {
    "city": "Riyadh"
  },
  "pickupDate": "2025-11-01",
  "vehicleTypeId": "660e8400-e29b-41d4-a716-446655440001"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "backloadOpportunities": [
      {
        "availabilityId": "aa0e8400-e29b-41d4-a716-446655440005",
        "vehicleId": "880e8400-e29b-41d4-a716-446655440003",
        "vehicleType": "Box Truck",
        "plateNumber": "ABC-1234",
        "driver": {
          "name": "Mohammed Hassan",
          "rating": 4.8,
          "totalTrips": 245
        },
        "currentLocation": {
          "city": "Jeddah",
          "address": "Corniche Road, Jeddah"
        },
        "returnCity": "Riyadh",
        "availableFrom": "2025-11-01T18:00:00Z",
        "availableTo": "2025-11-02T12:00:00Z",
        "capacityAvailable": {
          "tons": 10.0,
          "cubicMeters": 40.0
        },
        "pricing": {
          "regularFare": 2800.00,
          "backloadFare": 2380.00,
          "discount": 420.00,
          "discountPercentage": 15,
          "savings": 420.00
        },
        "distance": 950.5,
        "matchScore": 95.5
      }
    ],
    "totalResults": 1
  }
}
```

---

### 5.3 Get Backload Recommendations (Driver)

**Endpoint:** `GET /backload/recommendations/{driverId}`

**Authentication:** Required (Driver only)

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "recommendations": [
      {
        "bookingId": "770e8400-e29b-41d4-a716-446655440002",
        "bookingNumber": "WOL-2025-001235",
        "origin": {
          "city": "Jeddah",
          "address": "King Abdullah Street, Jeddah",
          "latitude": 21.5433,
          "longitude": 39.1728
        },
        "destination": {
          "city": "Riyadh",
          "address": "King Fahd Road, Riyadh",
          "latitude": 24.7136,
          "longitude": 46.6753
        },
        "pickupDateTime": "2025-11-01T19:00:00Z",
        "cargoType": "Dry",
        "grossWeightKg": 4500,
        "fare": 2380.00,
        "distance": 950.5,
        "distanceFromYou": 3.2,
        "matchScore": 95.5,
        "matchReasons": [
          "Perfect route match",
          "High capacity utilization (90%)",
          "Pickup time matches your availability",
          "Close to your current location"
        ]
      }
    ],
    "totalRecommendations": 1
  }
}
```

---

### 5.4 Get Route Heatmap

**Endpoint:** `GET /backload/heatmap`

**Authentication:** Required (Admin only)

**Query Parameters:**
- `startDate` (optional): Start date for analysis
- `endDate` (optional): End date for analysis

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "routes": [
      {
        "origin": "Riyadh",
        "destination": "Jeddah",
        "outboundTrips": 450,
        "returnTrips": 280,
        "balance": -170,
        "utilizationRate": 62.2,
        "emptyReturnRate": 37.8,
        "status": "Imbalanced"
      },
      {
        "origin": "Riyadh",
        "destination": "Dammam",
        "outboundTrips": 320,
        "returnTrips": 305,
        "balance": -15,
        "utilizationRate": 95.3,
        "emptyReturnRate": 4.7,
        "status": "Balanced"
      }
    ],
    "summary": {
      "totalRoutes": 25,
      "averageUtilization": 78.5,
      "imbalancedRoutes": 8,
      "balancedRoutes": 17
    }
  }
}
```

---

## 6. Tracking Service API

### 6.1 Update Location

**Endpoint:** `POST /tracking/location`

**Authentication:** Required (Driver only)

**Request Body:**
```json
{
  "tripId": "770e8400-e29b-41d4-a716-446655440002",
  "latitude": 24.7136,
  "longitude": 46.6753,
  "accuracy": 10.5,
  "speed": 85.0,
  "heading": 180.0,
  "timestamp": "2025-11-01T10:30:00Z"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "locationUpdated": true,
    "timestamp": "2025-11-01T10:30:00Z"
  }
}
```

---

### 6.2 Get Trip Tracking

**Endpoint:** `GET /tracking/trip/{tripId}`

**Authentication:** Required

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "tripId": "770e8400-e29b-41d4-a716-446655440002",
    "status": "InTransit",
    "currentLocation": {
      "latitude": 24.7136,
      "longitude": 46.6753,
      "address": "King Fahd Road, Riyadh",
      "timestamp": "2025-11-01T10:30:00Z"
    },
    "origin": {
      "latitude": 24.7136,
      "longitude": 46.6753,
      "address": "King Fahd Road, Riyadh"
    },
    "destination": {
      "latitude": 21.5433,
      "longitude": 39.1728,
      "address": "Corniche Road, Jeddah"
    },
    "eta": "2025-11-01T17:30:00Z",
    "distanceCovered": 245.5,
    "distanceRemaining": 705.0,
    "progress": 25.8,
    "speed": 85.0,
    "heading": 180.0
  }
}
```

---

### 6.3 Get Location History

**Endpoint:** `GET /tracking/trip/{tripId}/history`

**Authentication:** Required

**Query Parameters:**
- `startTime` (optional): Start timestamp
- `endTime` (optional): End timestamp
- `limit` (optional): Maximum number of points (default: 100)

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "tripId": "770e8400-e29b-41d4-a716-446655440002",
    "locations": [
      {
        "latitude": 24.7136,
        "longitude": 46.6753,
        "accuracy": 10.5,
        "speed": 85.0,
        "heading": 180.0,
        "timestamp": "2025-11-01T10:30:00Z"
      },
      {
        "latitude": 24.6950,
        "longitude": 46.6850,
        "accuracy": 12.0,
        "speed": 90.0,
        "heading": 185.0,
        "timestamp": "2025-11-01T10:35:00Z"
      }
    ],
    "totalPoints": 2
  }
}
```

---

## 7. Payment Service API

### 7.1 Initialize Payment

**Endpoint:** `POST /payments/initialize`

**Authentication:** Required

**Request Body:**
```json
{
  "bookingId": "770e8400-e29b-41d4-a716-446655440002",
  "amount": 3220.00,
  "currency": "SAR",
  "paymentMethod": "Card"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "paymentId": "bb0e8400-e29b-41d4-a716-446655440006",
    "paymentIntentId": "pi_1234567890",
    "clientSecret": "pi_1234567890_secret_abcdef",
    "amount": 3220.00,
    "currency": "SAR",
    "status": "Pending",
    "expiresAt": "2025-10-27T11:00:00Z"
  }
}
```

---

### 7.2 Confirm Payment

**Endpoint:** `POST /payments/confirm`

**Authentication:** Required

**Request Body:**
```json
{
  "paymentId": "bb0e8400-e29b-41d4-a716-446655440006",
  "paymentIntentId": "pi_1234567890",
  "paymentMethodId": "pm_1234567890"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "paymentId": "bb0e8400-e29b-41d4-a716-446655440006",
    "status": "Captured",
    "amount": 3220.00,
    "currency": "SAR",
    "transactionId": "txn_1234567890",
    "capturedAt": "2025-10-27T10:45:00Z"
  },
  "message": "Payment successful"
}
```

---

### 7.3 Request Refund

**Endpoint:** `POST /payments/refund`

**Authentication:** Required

**Request Body:**
```json
{
  "paymentId": "bb0e8400-e29b-41d4-a716-446655440006",
  "bookingId": "770e8400-e29b-41d4-a716-446655440002",
  "refundAmount": 3120.00,
  "reason": "Booking cancelled by customer"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "refundId": "cc0e8400-e29b-41d4-a716-446655440007",
    "paymentId": "bb0e8400-e29b-41d4-a716-446655440006",
    "refundAmount": 3120.00,
    "status": "Pending",
    "estimatedProcessingDays": 7,
    "createdAt": "2025-10-27T11:00:00Z"
  },
  "message": "Refund initiated. Amount will be credited within 3-7 business days."
}
```

---

### 7.4 Get Payment History

**Endpoint:** `GET /payments/customer/{customerId}/history`

**Authentication:** Required

**Query Parameters:**
- `page` (optional): Page number
- `pageSize` (optional): Items per page
- `status` (optional): Filter by status

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "paymentId": "bb0e8400-e29b-41d4-a716-446655440006",
        "bookingNumber": "WOL-2025-001234",
        "amount": 3220.00,
        "currency": "SAR",
        "paymentMethod": "Card",
        "status": "Captured",
        "transactionId": "txn_1234567890",
        "createdAt": "2025-10-27T10:30:00Z",
        "capturedAt": "2025-10-27T10:45:00Z"
      }
    ],
    "pagination": {
      "currentPage": 1,
      "pageSize": 20,
      "totalItems": 15,
      "totalPages": 1
    }
  }
}
```

---

## 8. Notification Service API

### 8.1 Send Notification

**Endpoint:** `POST /notifications/send`

**Authentication:** Required (Internal service only)

**Request Body:**
```json
{
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "type": "BookingConfirmed",
  "channels": ["Push", "SMS"],
  "title": "Booking Confirmed",
  "body": "Your booking WOL-2025-001234 has been confirmed.",
  "data": {
    "bookingId": "770e8400-e29b-41d4-a716-446655440002",
    "bookingNumber": "WOL-2025-001234"
  },
  "priority": "High"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "notificationId": "dd0e8400-e29b-41d4-a716-446655440008",
    "status": "Sent",
    "sentAt": "2025-10-27T10:30:00Z"
  }
}
```

---

### 8.2 Get User Notifications

**Endpoint:** `GET /notifications/user/{userId}`

**Authentication:** Required

**Query Parameters:**
- `page` (optional): Page number
- `pageSize` (optional): Items per page
- `unreadOnly` (optional): Filter unread notifications

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "notificationId": "dd0e8400-e29b-41d4-a716-446655440008",
        "type": "BookingConfirmed",
        "title": "Booking Confirmed",
        "body": "Your booking WOL-2025-001234 has been confirmed.",
        "data": {
          "bookingId": "770e8400-e29b-41d4-a716-446655440002",
          "bookingNumber": "WOL-2025-001234"
        },
        "isRead": false,
        "createdAt": "2025-10-27T10:30:00Z"
      }
    ],
    "pagination": {
      "currentPage": 1,
      "pageSize": 20,
      "totalItems": 45,
      "totalPages": 3
    },
    "unreadCount": 12
  }
}
```

---

### 8.3 Mark Notification as Read

**Endpoint:** `PUT /notifications/{notificationId}/read`

**Authentication:** Required

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "notificationId": "dd0e8400-e29b-41d4-a716-446655440008",
    "isRead": true,
    "readAt": "2025-10-27T10:35:00Z"
  }
}
```

---

## 9. Analytics Service API

### 9.1 Get Dashboard Stats

**Endpoint:** `GET /analytics/dashboard`

**Authentication:** Required (Admin only)

**Query Parameters:**
- `startDate` (optional): Start date for analysis
- `endDate` (optional): End date for analysis

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "totalBookings": 1250,
    "bookingsChange": 12.5,
    "activeTrips": 45,
    "totalRevenue": 3850000.00,
    "revenueChange": 8.3,
    "utilizationRate": 78.5,
    "utilizationChange": 5.2,
    "revenueData": [
      {
        "date": "2025-10-20",
        "revenue": 125000.00,
        "bookings": 42
      },
      {
        "date": "2025-10-21",
        "revenue": 135000.00,
        "bookings": 48
      }
    ],
    "routeData": [
      {
        "route": "Riyadh-Jeddah",
        "trips": 450,
        "utilization": 62.2
      },
      {
        "route": "Riyadh-Dammam",
        "trips": 320,
        "utilization": 95.3
      }
    ]
  }
}
```

---

### 9.2 Get Trip Analytics

**Endpoint:** `GET /analytics/trips/summary`

**Authentication:** Required (Admin only)

**Query Parameters:**
- `startDate` (optional): Start date
- `endDate` (optional): End date
- `groupBy` (optional): day, week, month

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "totalTrips": 1250,
    "completedTrips": 1180,
    "cancelledTrips": 70,
    "averageTripDuration": 485,
    "averageDistance": 425.5,
    "onTimeDeliveryRate": 92.5,
    "customerSatisfactionRate": 4.6,
    "tripsByStatus": {
      "Completed": 1180,
      "Cancelled": 70,
      "InProgress": 45
    },
    "tripsByType": {
      "OneWay": 850,
      "Backload": 320,
      "Shared": 80
    }
  }
}
```

---

## 10. Document Service API

### 10.1 Upload Document

**Endpoint:** `POST /documents/upload`

**Authentication:** Required

**Request:** Multipart form data
- `file`: Document file
- `documentTypeId`: UUID
- `entityType`: Vehicle, Driver, User
- `entityId`: UUID
- `documentNumber`: String (optional)
- `issueDate`: Date (optional)
- `expiryDate`: Date (optional)

**Response:** `201 Created`
```json
{
  "success": true,
  "data": {
    "documentId": "ee0e8400-e29b-41d4-a716-446655440009",
    "documentType": "Istemara",
    "entityType": "Vehicle",
    "entityId": "880e8400-e29b-41d4-a716-446655440003",
    "fileUrl": "https://storage.wol.sa/documents/istemara_123.pdf",
    "fileName": "istemara_123.pdf",
    "fileSize": 245678,
    "verificationStatus": "Pending",
    "uploadedAt": "2025-10-27T10:30:00Z"
  },
  "message": "Document uploaded successfully. Pending verification."
}
```

---

### 10.2 Get Entity Documents

**Endpoint:** `GET /documents/entity/{entityType}/{entityId}`

**Authentication:** Required

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "documents": [
      {
        "documentId": "ee0e8400-e29b-41d4-a716-446655440009",
        "documentType": "Istemara",
        "documentNumber": "IST-123456",
        "issueDate": "2024-01-01",
        "expiryDate": "2026-12-31",
        "fileUrl": "https://storage.wol.sa/documents/istemara_123.pdf",
        "verificationStatus": "Verified",
        "verifiedAt": "2025-10-27T11:00:00Z",
        "daysUntilExpiry": 430,
        "uploadedAt": "2025-10-27T10:30:00Z"
      }
    ],
    "totalDocuments": 1
  }
}
```

---

## 11. Compliance Service API

### 11.1 Check Compliance

**Endpoint:** `GET /compliance/check/{entityType}/{entityId}`

**Authentication:** Required

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "entityType": "Vehicle",
    "entityId": "880e8400-e29b-41d4-a716-446655440003",
    "isCompliant": true,
    "complianceStatus": "Compliant",
    "checks": [
      {
        "checkType": "DocumentExpiry",
        "documentType": "Istemara",
        "status": "Pass",
        "expiryDate": "2026-12-31",
        "daysUntilExpiry": 430
      },
      {
        "checkType": "DocumentExpiry",
        "documentType": "MVPI",
        "status": "Pass",
        "expiryDate": "2026-06-30",
        "daysUntilExpiry": 246
      },
      {
        "checkType": "DocumentExpiry",
        "documentType": "Insurance",
        "status": "Pass",
        "expiryDate": "2026-12-31",
        "daysUntilExpiry": 430
      }
    ],
    "violations": [],
    "lastCheckedAt": "2025-10-27T10:30:00Z"
  }
}
```

---

### 11.2 Check BAN Time

**Endpoint:** `GET /compliance/is-booking-allowed`

**Authentication:** Required

**Query Parameters:**
- `city`: City name
- `vehicleTypeId`: Vehicle type UUID
- `pickupDateTime`: Pickup date and time

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "isAllowed": true,
    "city": "Riyadh",
    "pickupDateTime": "2025-11-01T08:00:00Z",
    "banTimeActive": false,
    "message": "Booking is allowed at this time"
  }
}
```

**Response (BAN time active):** `200 OK`
```json
{
  "success": true,
  "data": {
    "isAllowed": false,
    "city": "Riyadh",
    "pickupDateTime": "2025-11-01T12:00:00Z",
    "banTimeActive": true,
    "banStartTime": "11:00:00",
    "banEndTime": "16:00:00",
    "message": "Booking not allowed during BAN hours (11:00 AM - 4:00 PM)"
  }
}
```

---

## WebSocket / SignalR Endpoints

### Tracking Hub

**Connection:** `wss://api.wol.sa/hubs/tracking`

**Authentication:** Bearer token in query string or header

**Methods:**

#### JoinTripTracking
```javascript
connection.invoke("JoinTripTracking", tripId);
```

#### UpdateLocation
```javascript
connection.invoke("UpdateLocation", {
  tripId: "770e8400-e29b-41d4-a716-446655440002",
  latitude: 24.7136,
  longitude: 46.6753,
  accuracy: 10.5,
  speed: 85.0,
  heading: 180.0,
  timestamp: "2025-11-01T10:30:00Z"
});
```

**Events:**

#### LocationUpdated
```javascript
connection.on("LocationUpdated", (location) => {
  console.log("Location updated:", location);
});
```

#### ETAUpdated
```javascript
connection.on("ETAUpdated", (eta) => {
  console.log("ETA updated:", eta);
});
```

#### TripStatusChanged
```javascript
connection.on("TripStatusChanged", (status) => {
  console.log("Trip status changed:", status);
});
```

---

## Error Codes

### Authentication Errors
- `AUTH_001` - Invalid credentials
- `AUTH_002` - Token expired
- `AUTH_003` - Invalid token
- `AUTH_004` - OTP expired
- `AUTH_005` - Invalid OTP
- `AUTH_006` - Account locked
- `AUTH_007` - Account not verified

### Booking Errors
- `BOOK_001` - Invalid booking data
- `BOOK_002` - No drivers available
- `BOOK_003` - Booking not found
- `BOOK_004` - Cannot cancel booking
- `BOOK_005` - BAN time violation
- `BOOK_006` - Driver already assigned
- `BOOK_007` - Invalid status transition

### Payment Errors
- `PAY_001` - Payment failed
- `PAY_002` - Insufficient funds
- `PAY_003` - Payment method not supported
- `PAY_004` - Payment already processed
- `PAY_005` - Refund failed
- `PAY_006` - Invalid payment amount

### Compliance Errors
- `COMP_001` - Document expired
- `COMP_002` - Document not verified
- `COMP_003` - Compliance violation
- `COMP_004` - Vehicle blocked
- `COMP_005` - Driver blocked

---

## Rate Limiting

All API endpoints are rate-limited to prevent abuse:

- **Authenticated users**: 1000 requests per hour
- **Unauthenticated users**: 100 requests per hour
- **Admin users**: 5000 requests per hour

Rate limit headers are included in all responses:
```
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 995
X-RateLimit-Reset: 1635340800
```

---

## Pagination

All list endpoints support pagination with the following query parameters:

- `page`: Page number (default: 1)
- `pageSize`: Items per page (default: 20, max: 100)
- `sortBy`: Field to sort by
- `sortOrder`: `asc` or `desc` (default: `desc`)

Pagination metadata is included in all list responses:
```json
{
  "pagination": {
    "currentPage": 1,
    "pageSize": 20,
    "totalItems": 150,
    "totalPages": 8,
    "hasNextPage": true,
    "hasPreviousPage": false
  }
}
```

---

## Versioning

The API uses URL versioning:

```
/api/v1/bookings
/api/v2/bookings
```

Current version: `v1`

---

## Support

For API support and questions:
- Email: api-support@wol.sa
- Documentation: https://docs.wol.sa
- Status Page: https://status.wol.sa
