import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { useAuthStore } from './store/authStore'
import Login from './pages/Login'
import Dashboard from './pages/Dashboard'
import Users from './pages/Users'
import Bookings from './pages/Bookings'
import Vehicles from './pages/Vehicles'
import Payments from './pages/Payments'
import Reports from './pages/Reports'
import Layout from './components/Layout'

function App() {
  const { isAuthenticated } = useAuthStore()

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route
          path="/"
          element={
            isAuthenticated ? (
              <Layout />
            ) : (
              <Navigate to="/login" replace />
            )
          }
        >
          <Route index element={<Dashboard />} />
          <Route path="users" element={<Users />} />
          <Route path="bookings" element={<Bookings />} />
          <Route path="vehicles" element={<Vehicles />} />
          <Route path="payments" element={<Payments />} />
          <Route path="reports" element={<Reports />} />
        </Route>
      </Routes>
    </BrowserRouter>
  )
}

export default App
