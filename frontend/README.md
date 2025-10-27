# World of Logistics - Frontend Applications

This directory contains all frontend applications for the World of Logistics platform.

## Applications

### 1. Admin Web App (React)
**Location**: `admin-web/`

A comprehensive web-based admin dashboard for managing the entire WOL platform.

**Features**:
- Dashboard with analytics and KPIs
- User management (customers, drivers, suppliers)
- Booking management and tracking
- Vehicle fleet management
- Payment tracking and reporting
- Report generation and export

**Tech Stack**:
- React 18 with TypeScript
- Vite (build tool)
- React Router (navigation)
- TanStack Query (server state)
- Zustand (client state)
- Tailwind CSS (styling)
- Recharts (data visualization)
- Axios (API client)

**Getting Started**:
```bash
cd admin-web
npm install
npm run dev
```

Access at: http://localhost:3000

---

### 2. Customer/Supplier Mobile App (React Native)
**Location**: `customer-mobile/`

Unified mobile application for individual customers and commercial suppliers.

**Features**:
- User authentication (login/register)
- Home dashboard with quick actions
- Booking creation and management
- Real-time shipment tracking with GPS
- Profile and settings management
- Role-based features (Individual vs Commercial users)

**Tech Stack**:
- React Native (Expo)
- TypeScript
- React Navigation
- TanStack Query
- Zustand
- React Native Maps
- Expo Location
- React Native Paper

**Getting Started**:
```bash
cd customer-mobile
npm install
npm start
```

---

### 3. Driver Mobile App (React Native)
**Location**: `driver-mobile/`

Dedicated mobile application for service providers (drivers).

**Features**:
- Driver authentication
- Online/offline status toggle
- Browse and accept available jobs
- Active job tracking with real-time navigation
- Earnings dashboard (daily, weekly, monthly)
- Profile and vehicle management

**Tech Stack**:
- React Native (Expo)
- TypeScript
- React Navigation
- TanStack Query
- Zustand
- React Native Maps
- Expo Location
- React Native Chart Kit

**Getting Started**:
```bash
cd driver-mobile
npm install
npm start
```

---

## Configuration

All applications require API configuration. Update the API URL in each app:

**Admin Web**: `admin-web/src/lib/api.ts`
```typescript
const API_URL = 'YOUR_API_GATEWAY_URL';
```

**Customer Mobile**: `customer-mobile/src/lib/api.ts`
```typescript
const API_URL = 'YOUR_API_GATEWAY_URL';
```

**Driver Mobile**: `driver-mobile/src/lib/api.ts`
```typescript
const API_URL = 'YOUR_API_GATEWAY_URL';
```

Default API Gateway URL: `http://localhost:5000/api`

---

## Development Workflow

### Admin Web App
1. Start the backend services (API Gateway + Microservices)
2. Navigate to `admin-web/`
3. Run `npm install` (first time only)
4. Run `npm run dev`
5. Access at http://localhost:3000
6. Login with admin credentials

### Mobile Apps
1. Install Expo CLI: `npm install -g expo-cli`
2. Navigate to app directory
3. Run `npm install` (first time only)
4. Run `npm start`
5. Scan QR code with Expo Go app (iOS/Android)
6. Or press `i` for iOS Simulator / `a` for Android Emulator

---

## Building for Production

### Admin Web App
```bash
cd admin-web
npm run build
# Output in dist/ directory
```

### Mobile Apps
```bash
cd customer-mobile  # or driver-mobile
expo build:android
expo build:ios
```

---

## Architecture

### State Management
- **Server State**: TanStack Query (React Query) for API data fetching, caching, and synchronization
- **Client State**: Zustand for local state (authentication, UI state)

### API Communication
- Axios HTTP client with interceptors
- JWT token authentication
- Automatic token refresh
- Error handling and retry logic

### Navigation
- **Web**: React Router with protected routes
- **Mobile**: React Navigation with stack and tab navigators

### Styling
- **Web**: Tailwind CSS utility-first framework
- **Mobile**: React Native StyleSheet with custom theme

---

## Key Features

### Authentication
- JWT-based authentication
- Secure token storage (localStorage for web, AsyncStorage for mobile)
- Automatic token injection in API requests
- Session management and logout

### Real-time Features
- Live GPS tracking with React Native Maps
- Location updates every 10 seconds
- Real-time booking status updates
- Push notifications (mobile)

### Offline Support
- React Query caching for offline data access
- Optimistic updates for better UX
- Background sync when connection restored

---

## Testing

### Admin Web App
```bash
cd admin-web
npm run lint
npm run build  # Verify production build
```

### Mobile Apps
```bash
cd customer-mobile  # or driver-mobile
expo start
# Test on physical device or simulator
```

---

## Deployment

### Admin Web App
Deploy to any static hosting service:
- Vercel
- Netlify
- AWS S3 + CloudFront
- Azure Static Web Apps

### Mobile Apps
Publish to app stores:
- **iOS**: Apple App Store (requires Apple Developer account)
- **Android**: Google Play Store (requires Google Play Developer account)

Use Expo EAS Build for managed builds:
```bash
npm install -g eas-cli
eas build --platform android
eas build --platform ios
```

---

## Environment Variables

### Admin Web App
Create `.env` file:
```
VITE_API_URL=http://localhost:5000/api
```

### Mobile Apps
Update `app.json` and API configuration files as needed.

---

## Troubleshooting

### Admin Web App
- **Port 3000 already in use**: Change port in `vite.config.ts`
- **API connection failed**: Verify API Gateway is running
- **Build errors**: Clear node_modules and reinstall

### Mobile Apps
- **Expo connection issues**: Ensure device and computer are on same network
- **Maps not showing**: Verify location permissions are granted
- **Build failures**: Check Expo SDK version compatibility

---

## Support

For issues or questions:
1. Check application README files
2. Review API documentation
3. Contact development team

---

## License

Proprietary - World of Logistics KSA
