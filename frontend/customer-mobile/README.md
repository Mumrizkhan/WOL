# WOL Customer/Supplier Mobile App

React Native mobile application for World of Logistics customers and suppliers.

## Features

- **User Authentication**: Login and registration with JWT
- **Home Dashboard**: Quick access to bookings and services
- **Booking Management**: Create and manage bookings
- **Live Tracking**: Real-time GPS tracking of shipments
- **Profile Management**: User profile and settings
- **Multi-role Support**: Unified app for Individual and Commercial users

## Tech Stack

- React Native (Expo)
- TypeScript
- React Navigation
- TanStack Query (React Query)
- Zustand (State Management)
- React Native Maps
- Expo Location
- React Native Paper (UI Components)

## Getting Started

### Prerequisites

- Node.js 18+ and npm
- Expo CLI: `npm install -g expo-cli`
- iOS Simulator (Mac) or Android Emulator

### Installation

```bash
# Install dependencies
npm install

# Start development server
npm start

# Run on iOS
npm run ios

# Run on Android
npm run android
```

## Configuration

Update the API URL in `src/lib/api.ts`:

```typescript
const API_URL = 'YOUR_API_URL';
```

## Project Structure

```
src/
├── navigation/     # Navigation configuration
├── screens/        # Screen components
│   ├── auth/       # Authentication screens
│   ├── home/       # Home dashboard
│   ├── bookings/   # Booking management
│   ├── tracking/   # Live tracking
│   └── profile/    # User profile
├── lib/            # API client and utilities
└── store/          # Zustand stores
```

## Features by User Type

### Individual Users
- Create one-way bookings
- Track shipments
- View booking history
- Manage profile

### Commercial Users (Suppliers)
- All individual features
- Bulk booking creation
- Fleet management
- Advanced analytics

## Permissions

The app requires the following permissions:
- **Location**: For real-time tracking
- **Notifications**: For booking updates

## License

Proprietary - World of Logistics KSA
