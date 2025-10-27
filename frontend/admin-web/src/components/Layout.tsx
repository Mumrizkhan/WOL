import { Outlet, Link, useLocation } from 'react-router-dom'
import { 
  LayoutDashboard, 
  Users, 
  Package, 
  Truck, 
  CreditCard, 
  FileText,
  LogOut,
  Menu,
  X
} from 'lucide-react'
import { useAuthStore } from '../store/authStore'
import { useState } from 'react'

const navigation = [
  { name: 'Dashboard', href: '/', icon: LayoutDashboard },
  { name: 'Users', href: '/users', icon: Users },
  { name: 'Bookings', href: '/bookings', icon: Package },
  { name: 'Vehicles', href: '/vehicles', icon: Truck },
  { name: 'Payments', href: '/payments', icon: CreditCard },
  { name: 'Reports', href: '/reports', icon: FileText },
]

export default function Layout() {
  const location = useLocation()
  const { user, logout } = useAuthStore()
  const [sidebarOpen, setSidebarOpen] = useState(false)

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Mobile sidebar */}
      {sidebarOpen && (
        <div className="fixed inset-0 z-50 lg:hidden">
          <div className="fixed inset-0 bg-gray-600 bg-opacity-75" onClick={() => setSidebarOpen(false)} />
          <div className="fixed inset-y-0 left-0 flex w-64 flex-col bg-white">
            <div className="flex h-16 items-center justify-between px-4 border-b">
              <span className="text-xl font-bold text-primary-600">WOL Admin</span>
              <button onClick={() => setSidebarOpen(false)}>
                <X className="h-6 w-6" />
              </button>
            </div>
            <nav className="flex-1 space-y-1 px-2 py-4">
              {navigation.map((item) => {
                const Icon = item.icon
                const isActive = location.pathname === item.href
                return (
                  <Link
                    key={item.name}
                    to={item.href}
                    onClick={() => setSidebarOpen(false)}
                    className={`flex items-center gap-3 px-4 py-3 rounded-lg transition-colors ${
                      isActive
                        ? 'bg-primary-50 text-primary-600'
                        : 'text-gray-700 hover:bg-gray-50'
                    }`}
                  >
                    <Icon className="h-5 w-5" />
                    <span className="font-medium">{item.name}</span>
                  </Link>
                )
              })}
            </nav>
          </div>
        </div>
      )}

      {/* Desktop sidebar */}
      <div className="hidden lg:fixed lg:inset-y-0 lg:flex lg:w-64 lg:flex-col">
        <div className="flex flex-col flex-grow bg-white border-r">
          <div className="flex h-16 items-center px-6 border-b">
            <span className="text-xl font-bold text-primary-600">WOL Admin</span>
          </div>
          <nav className="flex-1 space-y-1 px-3 py-4">
            {navigation.map((item) => {
              const Icon = item.icon
              const isActive = location.pathname === item.href
              return (
                <Link
                  key={item.name}
                  to={item.href}
                  className={`flex items-center gap-3 px-4 py-3 rounded-lg transition-colors ${
                    isActive
                      ? 'bg-primary-50 text-primary-600'
                      : 'text-gray-700 hover:bg-gray-50'
                  }`}
                >
                  <Icon className="h-5 w-5" />
                  <span className="font-medium">{item.name}</span>
                </Link>
              )
            })}
          </nav>
        </div>
      </div>

      {/* Main content */}
      <div className="lg:pl-64">
        {/* Top bar */}
        <div className="sticky top-0 z-10 flex h-16 bg-white border-b">
          <button
            className="px-4 text-gray-500 lg:hidden"
            onClick={() => setSidebarOpen(true)}
          >
            <Menu className="h-6 w-6" />
          </button>
          <div className="flex flex-1 justify-end items-center px-4 gap-4">
            <span className="text-sm text-gray-700">{user?.email}</span>
            <button
              onClick={logout}
              className="flex items-center gap-2 px-4 py-2 text-sm text-gray-700 hover:text-gray-900"
            >
              <LogOut className="h-4 w-4" />
              <span>Logout</span>
            </button>
          </div>
        </div>

        {/* Page content */}
        <main className="p-6">
          <Outlet />
        </main>
      </div>
    </div>
  )
}
