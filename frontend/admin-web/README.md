# WOL Admin Web Application

React-based admin dashboard for World of Logistics management system.

## Features

- **Dashboard**: Real-time analytics and KPIs
- **User Management**: Manage customers, drivers, and service providers
- **Booking Management**: Track and manage all bookings
- **Vehicle Fleet Management**: Monitor and manage vehicles
- **Payment Tracking**: View all payment transactions
- **Reports & Analytics**: Generate and download reports

## Tech Stack

- React 18
- TypeScript
- Vite
- TailwindCSS
- React Router
- TanStack Query (React Query)
- Zustand (State Management)
- Recharts (Charts)
- Axios

## Getting Started

### Prerequisites

- Node.js 18+ and npm

### Installation

```bash
# Install dependencies
npm install

# Start development server
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview
```

## Configuration

Update the `.env` file with your API endpoint:

```
VITE_API_URL=http://localhost:5000/api
```

## Project Structure

```
src/
├── components/     # Reusable components
├── pages/          # Page components
├── lib/            # API client and utilities
├── store/          # Zustand stores
├── App.tsx         # Main app component
└── main.tsx        # Entry point
```

## Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build
- `npm run lint` - Run ESLint

## Authentication

The app uses JWT token-based authentication. Login credentials are stored in localStorage and automatically attached to API requests.

## License

Proprietary - World of Logistics KSA
