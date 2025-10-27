# WOL Driver Mobile App

React Native mobile application for World of Logistics service providers (drivers).

## Features

- **Driver Authentication**: Secure login for drivers
- **Online/Offline Toggle**: Control availability status
- **Job Management**: Browse and accept available jobs
- **Active Job Tracking**: Real-time navigation and job completion
- **Earnings Dashboard**: Track daily, weekly, and monthly earnings
- **Profile Management**: Manage driver profile and vehicles

## Tech Stack

- React Native (Expo)
- TypeScript
- React Navigation
- TanStack Query (React Query)
- Zustand (State Management)
- React Native Maps
- Expo Location
- React Native Paper (UI Components)
- React Native Chart Kit

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
│   ├── jobs/       # Job management
│   ├── earnings/   # Earnings tracking
│   └── profile/    # Driver profile
├── lib/            # API client and utilities
└── store/          # Zustand stores
```

## Key Features

### Home Dashboard
- Online/offline status toggle
- Active job display
- Today's earnings summary
- Quick stats (completed jobs, distance driven)

### Job Management
- Browse available jobs
- View job details (route, earnings, requirements)
- Accept jobs
- View my jobs history

### Active Job
- Real-time GPS tracking
- Route navigation
- Customer contact
- Job completion

### Earnings
- Total earnings overview
- Weekly/monthly breakdown
- Earnings chart
- Transaction history

## Permissions

The app requires the following permissions:
- **Location**: For real-time tracking and navigation
- **Background Location**: For continuous tracking during active jobs
- **Notifications**: For job alerts and updates

## License

Proprietary - World of Logistics KSA
