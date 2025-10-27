import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { paymentsApi } from '../lib/api'
import { Search, DollarSign, CreditCard, CheckCircle, XCircle, Clock } from 'lucide-react'
import { format } from 'date-fns'

export default function Payments() {
  const [searchTerm, setSearchTerm] = useState('')
  const [statusFilter, setStatusFilter] = useState('all')

  const { data: payments, isLoading } = useQuery({
    queryKey: ['payments'],
    queryFn: () => paymentsApi.getAll(),
  })

  const filteredPayments = payments?.data?.filter((payment: any) => {
    const matchesSearch = 
      payment.transactionId?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      payment.bookingId?.toLowerCase().includes(searchTerm.toLowerCase())
    
    const matchesStatus = statusFilter === 'all' || payment.status === statusFilter

    return matchesSearch && matchesStatus
  }) || []

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-96">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    )
  }

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'Completed':
        return <CheckCircle className="h-5 w-5 text-green-600" />
      case 'Failed':
        return <XCircle className="h-5 w-5 text-red-600" />
      case 'Pending':
        return <Clock className="h-5 w-5 text-yellow-600" />
      default:
        return <Clock className="h-5 w-5 text-gray-600" />
    }
  }

  const getStatusColor = (status: string) => {
    const colors: any = {
      Completed: 'bg-green-100 text-green-700',
      Failed: 'bg-red-100 text-red-700',
      Pending: 'bg-yellow-100 text-yellow-700',
      Refunded: 'bg-blue-100 text-blue-700',
    }
    return colors[status] || 'bg-gray-100 text-gray-700'
  }

  const totalAmount = filteredPayments.reduce((sum: number, p: any) => 
    p.status === 'Completed' ? sum + (p.amount || 0) : sum, 0
  )

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Payments</h1>
        <p className="text-gray-600 mt-1">Track all payment transactions</p>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-gray-600">Total Revenue</p>
              <p className="text-2xl font-bold text-gray-900 mt-1">
                SAR {totalAmount.toLocaleString()}
              </p>
            </div>
            <div className="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
              <DollarSign className="h-6 w-6 text-green-600" />
            </div>
          </div>
        </div>
        <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-gray-600">Completed</p>
              <p className="text-2xl font-bold text-green-600 mt-1">
                {payments?.data?.filter((p: any) => p.status === 'Completed').length || 0}
              </p>
            </div>
            <CheckCircle className="h-8 w-8 text-green-600" />
          </div>
        </div>
        <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-gray-600">Pending</p>
              <p className="text-2xl font-bold text-yellow-600 mt-1">
                {payments?.data?.filter((p: any) => p.status === 'Pending').length || 0}
              </p>
            </div>
            <Clock className="h-8 w-8 text-yellow-600" />
          </div>
        </div>
        <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-gray-600">Failed</p>
              <p className="text-2xl font-bold text-red-600 mt-1">
                {payments?.data?.filter((p: any) => p.status === 'Failed').length || 0}
              </p>
            </div>
            <XCircle className="h-8 w-8 text-red-600" />
          </div>
        </div>
      </div>

      {/* Filters */}
      <div className="bg-white rounded-xl shadow-sm p-4 border border-gray-100">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-gray-400" />
            <input
              type="text"
              placeholder="Search by transaction ID or booking ID..."
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
            <option value="Completed">Completed</option>
            <option value="Pending">Pending</option>
            <option value="Failed">Failed</option>
            <option value="Refunded">Refunded</option>
          </select>
        </div>
      </div>

      {/* Payments Table */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50">
              <tr>
                <th className="text-left py-4 px-6 text-sm font-semibold text-gray-700">Transaction ID</th>
                <th className="text-left py-4 px-6 text-sm font-semibold text-gray-700">Booking ID</th>
                <th className="text-left py-4 px-6 text-sm font-semibold text-gray-700">Amount</th>
                <th className="text-left py-4 px-6 text-sm font-semibold text-gray-700">Method</th>
                <th className="text-left py-4 px-6 text-sm font-semibold text-gray-700">Status</th>
                <th className="text-left py-4 px-6 text-sm font-semibold text-gray-700">Date</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {filteredPayments.length === 0 ? (
                <tr>
                  <td colSpan={6} className="py-12 text-center text-gray-500">
                    No payments found
                  </td>
                </tr>
              ) : (
                filteredPayments.map((payment: any) => (
                  <tr key={payment.id} className="hover:bg-gray-50">
                    <td className="py-4 px-6">
                      <div className="flex items-center gap-2">
                        {getStatusIcon(payment.status)}
                        <span className="font-mono text-sm font-medium text-gray-900">
                          {payment.transactionId}
                        </span>
                      </div>
                    </td>
                    <td className="py-4 px-6 text-sm text-gray-700">
                      {payment.bookingId}
                    </td>
                    <td className="py-4 px-6">
                      <span className="text-lg font-bold text-gray-900">
                        SAR {payment.amount?.toFixed(2) || '0.00'}
                      </span>
                    </td>
                    <td className="py-4 px-6">
                      <div className="flex items-center gap-2">
                        <CreditCard className="h-4 w-4 text-gray-400" />
                        <span className="text-sm text-gray-700">
                          {payment.paymentMethod || 'Card'}
                        </span>
                      </div>
                    </td>
                    <td className="py-4 px-6">
                      <span className={`px-3 py-1 text-xs font-medium rounded-full ${getStatusColor(payment.status)}`}>
                        {payment.status}
                      </span>
                    </td>
                    <td className="py-4 px-6 text-sm text-gray-700">
                      {payment.createdAt 
                        ? format(new Date(payment.createdAt), 'MMM dd, yyyy HH:mm')
                        : 'N/A'
                      }
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  )
}
