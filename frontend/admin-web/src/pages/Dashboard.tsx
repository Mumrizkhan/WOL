import { 
  TrendingUp, 
  Users, 
  Package, 
  DollarSign,
  Truck,
  Clock
} from 'lucide-react'
import { LineChart, Line, BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts'
import { useDashboardStats, useBookingTrends, useRevenueTrends } from '../services/analytics'

export default function Dashboard() {
  const { data: stats, isLoading } = useDashboardStats()
  const { data: bookingTrends } = useBookingTrends()
  const { data: revenueTrends } = useRevenueTrends()

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-96">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    )
  }

  const statsData = stats || {
    totalBookings: 1234,
    activeBookings: 89,
    completedBookings: 1100,
    totalRevenue: 456789,
    totalDrivers: 150,
    activeDrivers: 89,
    totalVehicles: 234,
    complianceViolations: 5,
  }

  const statCards = [
    {
      name: 'Total Bookings',
      value: statsData.totalBookings,
      icon: Package,
      color: 'bg-blue-500',
      change: '+12.5%',
    },
    {
      name: 'Active Bookings',
      value: statsData.activeBookings,
      icon: Clock,
      color: 'bg-green-500',
      change: '+8.2%',
    },
    {
      name: 'Total Revenue',
      value: `SAR ${statsData.totalRevenue.toLocaleString()}`,
      icon: DollarSign,
      color: 'bg-purple-500',
      change: '+23.1%',
    },
    {
      name: 'Active Drivers',
      value: statsData.activeDrivers,
      icon: Users,
      color: 'bg-orange-500',
      change: '+5.4%',
    },
    {
      name: 'Total Vehicles',
      value: statsData.totalVehicles,
      icon: Truck,
      color: 'bg-indigo-500',
      change: '+3.2%',
    },
    {
      name: 'Compliance Issues',
      value: statsData.complianceViolations,
      icon: TrendingUp,
      color: 'bg-red-500',
      change: '-2.1%',
    },
  ]

  const bookingData = bookingTrends || [
    { date: '2024-01', count: 65 },
    { date: '2024-02', count: 78 },
    { date: '2024-03', count: 90 },
    { date: '2024-04', count: 81 },
    { date: '2024-05', count: 95 },
    { date: '2024-06', count: 110 },
  ]

  const revenueData = revenueTrends || [
    { date: 'Jan', amount: 45000 },
    { date: 'Feb', amount: 52000 },
    { date: 'Mar', amount: 48000 },
    { date: 'Apr', amount: 61000 },
    { date: 'May', amount: 55000 },
    { date: 'Jun', amount: 67000 },
  ]

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Dashboard</h1>
        <p className="text-gray-600 mt-1">Welcome to World of Logistics Admin Panel</p>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {statCards.map((stat) => {
          const Icon = stat.icon
          return (
            <div key={stat.name} className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm text-gray-600">{stat.name}</p>
                  <p className="text-2xl font-bold text-gray-900 mt-2">{stat.value}</p>
                  <p className={`text-sm mt-2 ${stat.change.startsWith('+') ? 'text-green-600' : 'text-red-600'}`}>
                    {stat.change} from last month
                  </p>
                </div>
                <div className={`${stat.color} w-12 h-12 rounded-lg flex items-center justify-center`}>
                  <Icon className="h-6 w-6 text-white" />
                </div>
              </div>
            </div>
          )
        })}
      </div>

      {/* Charts */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Booking Trends */}
        <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">Booking Trends</h2>
          <ResponsiveContainer width="100%" height={300}>
            <LineChart data={bookingData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="date" />
              <YAxis />
              <Tooltip />
              <Line type="monotone" dataKey="count" stroke="#0ea5e9" strokeWidth={2} />
            </LineChart>
          </ResponsiveContainer>
        </div>

        {/* Revenue Trends */}
        <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">Revenue Trends</h2>
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={revenueData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="date" />
              <YAxis />
              <Tooltip />
              <Bar dataKey="amount" fill="#8b5cf6" />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>

      {/* Recent Activity */}
      <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
        <h2 className="text-lg font-semibold text-gray-900 mb-4">Recent Bookings</h2>
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-gray-200">
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-600">Booking ID</th>
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-600">Customer</th>
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-600">Route</th>
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-600">Status</th>
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-600">Amount</th>
              </tr>
            </thead>
            <tbody>
              {[1, 2, 3, 4, 5].map((i) => (
                <tr key={i} className="border-b border-gray-100 hover:bg-gray-50">
                  <td className="py-3 px-4 text-sm">BK-{1000 + i}</td>
                  <td className="py-3 px-4 text-sm">Customer {i}</td>
                  <td className="py-3 px-4 text-sm">Riyadh → Jeddah</td>
                  <td className="py-3 px-4">
                    <span className="px-2 py-1 text-xs rounded-full bg-green-100 text-green-700">
                      Active
                    </span>
                  </td>
                  <td className="py-3 px-4 text-sm font-medium">SAR {(Math.random() * 5000 + 1000).toFixed(2)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  )
}
