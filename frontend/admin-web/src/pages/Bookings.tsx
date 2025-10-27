import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { bookingsApi } from '../lib/api'
import { Search, MapPin, Calendar, User, Truck } from 'lucide-react'
import { format } from 'date-fns'

export default function Bookings() {
  const queryClient = useQueryClient()
  const [searchTerm, setSearchTerm] = useState('')
  const [statusFilter, setStatusFilter] = useState('all')

  const { data: bookings, isLoading } = useQuery({
    queryKey: ['bookings'],
    queryFn: () => bookingsApi.getAll(),
  })

  const assignMutation = useMutation({
    mutationFn: ({ bookingId, driverId }: any) =>
      bookingsApi.assign(bookingId, driverId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['bookings'] })
    },
  })

  const filteredBookings = bookings?.data?.filter((booking: any) => {
    const matchesSearch = 
      booking.bookingNumber?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      booking.pickupLocation?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      booking.dropoffLocation?.toLowerCase().includes(searchTerm.toLowerCase())
    
    const matchesStatus = statusFilter === 'all' || booking.status === statusFilter

    return matchesSearch && matchesStatus
  }) || []

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-96">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    )
  }

  const getStatusColor = (status: string) => {
    const colors: any = {
      Pending: 'bg-yellow-100 text-yellow-700',
      Confirmed: 'bg-blue-100 text-blue-700',
      InProgress: 'bg-purple-100 text-purple-700',
      Completed: 'bg-green-100 text-green-700',
      Cancelled: 'bg-red-100 text-red-700',
    }
    return colors[status] || 'bg-gray-100 text-gray-700'
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Bookings</h1>
        <p className="text-gray-600 mt-1">Manage all transportation bookings</p>
      </div>

      {/* Filters */}
      <div className="bg-white rounded-xl shadow-sm p-4 border border-gray-100">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-gray-400" />
            <input
              type="text"
              placeholder="Search by booking number or location..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent"
            />
          </div>
          <select
            value={statusFilter}
            onChange={(e) => setStatusFilter(e.target.value)}
            className="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent"
          >
            <option value="all">All Status</option>
            <option value="Pending">Pending</option>
            <option value="Confirmed">Confirmed</option>
            <option value="InProgress">In Progress</option>
            <option value="Completed">Completed</option>
            <option value="Cancelled">Cancelled</option>
          </select>
        </div>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        {['Pending', 'Confirmed', 'InProgress', 'Completed'].map((status) => {
          const count = bookings?.data?.filter((b: any) => b.status === status).length || 0
          return (
            <div key={status} className="bg-white rounded-xl shadow-sm p-4 border border-gray-100">
              <p className="text-sm text-gray-600">{status}</p>
              <p className="text-2xl font-bold text-gray-900 mt-1">{count}</p>
            </div>
          )
        })}
      </div>

      {/* Bookings List */}
      <div className="space-y-4">
        {filteredBookings.length === 0 ? (
          <div className="bg-white rounded-xl shadow-sm p-12 text-center border border-gray-100">
            <p className="text-gray-500">No bookings found</p>
          </div>
        ) : (
          filteredBookings.map((booking: any) => (
            <div
              key={booking.id}
              className="bg-white rounded-xl shadow-sm p-6 border border-gray-100 hover:shadow-md transition-shadow"
            >
              <div className="flex items-start justify-between mb-4">
                <div>
                  <div className="flex items-center gap-3">
                    <h3 className="text-lg font-semibold text-gray-900">
                      {booking.bookingNumber}
                    </h3>
                    <span className={`px-3 py-1 text-xs font-medium rounded-full ${getStatusColor(booking.status)}`}>
                      {booking.status}
                    </span>
                  </div>
                  <p className="text-sm text-gray-600 mt-1">
                    Booking Type: {booking.bookingType}
                  </p>
                </div>
                <div className="text-right">
                  <p className="text-2xl font-bold text-primary-600">
                    SAR {booking.totalAmount?.toFixed(2) || '0.00'}
                  </p>
                  <p className="text-sm text-gray-600 mt-1">
                    {booking.paymentStatus}
                  </p>
                </div>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4">
                <div className="flex items-start gap-3">
                  <MapPin className="h-5 w-5 text-green-600 mt-0.5" />
                  <div>
                    <p className="text-sm font-medium text-gray-700">Pickup</p>
                    <p className="text-sm text-gray-600">{booking.pickupLocation}</p>
                  </div>
                </div>
                <div className="flex items-start gap-3">
                  <MapPin className="h-5 w-5 text-red-600 mt-0.5" />
                  <div>
                    <p className="text-sm font-medium text-gray-700">Dropoff</p>
                    <p className="text-sm text-gray-600">{booking.dropoffLocation}</p>
                  </div>
                </div>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-3 gap-4 pt-4 border-t border-gray-100">
                <div className="flex items-center gap-2">
                  <Calendar className="h-4 w-4 text-gray-400" />
                  <span className="text-sm text-gray-600">
                    {booking.pickupDate ? format(new Date(booking.pickupDate), 'MMM dd, yyyy') : 'N/A'}
                  </span>
                </div>
                <div className="flex items-center gap-2">
                  <User className="h-4 w-4 text-gray-400" />
                  <span className="text-sm text-gray-600">
                    Customer: {booking.customerId}
                  </span>
                </div>
                <div className="flex items-center gap-2">
                  <Truck className="h-4 w-4 text-gray-400" />
                  <span className="text-sm text-gray-600">
                    {booking.vehicleType || 'Not assigned'}
                  </span>
                </div>
              </div>

              {booking.status === 'Pending' && (
                <div className="mt-4 pt-4 border-t border-gray-100">
                  <button
                    onClick={() => {
                      const driverId = prompt('Enter Driver ID to assign:')
                      if (driverId) {
                        assignMutation.mutate({ bookingId: booking.id, driverId })
                      }
                    }}
                    className="px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors text-sm"
                  >
                    Assign Driver
                  </button>
                </div>
              )}
            </div>
          ))
        )}
      </div>
    </div>
  )
}
