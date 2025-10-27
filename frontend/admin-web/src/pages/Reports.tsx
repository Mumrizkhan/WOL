import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { reportsApi } from '../lib/api'
import { FileText, Download, Calendar, Filter, TrendingUp } from 'lucide-react'
import { format } from 'date-fns'

export default function Reports() {
  const queryClient = useQueryClient()
  const [showModal, setShowModal] = useState(false)

  const { data: reports, isLoading } = useQuery({
    queryKey: ['reports'],
    queryFn: () => reportsApi.getAll(),
  })

  const generateMutation = useMutation({
    mutationFn: (data: any) => reportsApi.generate(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['reports'] })
      setShowModal(false)
    },
  })

  const handleDownload = async (reportId: string, reportName: string) => {
    try {
      const response = await reportsApi.download(reportId)
      const url = window.URL.createObjectURL(new Blob([response.data]))
      const link = document.createElement('a')
      link.href = url
      link.setAttribute('download', `${reportName}.pdf`)
      document.body.appendChild(link)
      link.click()
      link.remove()
    } catch (error) {
      console.error('Download failed:', error)
    }
  }

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-96">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    )
  }

  const reportTypes = [
    {
      name: 'Booking Report',
      description: 'Comprehensive booking statistics and trends',
      icon: FileText,
      color: 'bg-blue-500',
    },
    {
      name: 'Revenue Report',
      description: 'Financial performance and revenue analysis',
      icon: TrendingUp,
      color: 'bg-green-500',
    },
    {
      name: 'Vehicle Report',
      description: 'Fleet utilization and maintenance records',
      icon: FileText,
      color: 'bg-purple-500',
    },
    {
      name: 'User Report',
      description: 'User activity and engagement metrics',
      icon: FileText,
      color: 'bg-orange-500',
    },
  ]

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Reports & Analytics</h1>
          <p className="text-gray-600 mt-1">Generate and download system reports</p>
        </div>
        <button
          onClick={() => setShowModal(true)}
          className="flex items-center gap-2 bg-primary-600 text-white px-4 py-2 rounded-lg hover:bg-primary-700 transition-colors"
        >
          <FileText className="h-5 w-5" />
          Generate Report
        </button>
      </div>

      {/* Report Types */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {reportTypes.map((type) => {
          const Icon = type.icon
          return (
            <div
              key={type.name}
              className="bg-white rounded-xl shadow-sm p-6 border border-gray-100 hover:shadow-md transition-shadow cursor-pointer"
              onClick={() => setShowModal(true)}
            >
              <div className={`${type.color} w-12 h-12 rounded-lg flex items-center justify-center mb-4`}>
                <Icon className="h-6 w-6 text-white" />
              </div>
              <h3 className="text-lg font-semibold text-gray-900">{type.name}</h3>
              <p className="text-sm text-gray-600 mt-2">{type.description}</p>
            </div>
          )
        })}
      </div>

      {/* Recent Reports */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-100">
        <div className="p-6 border-b border-gray-100">
          <h2 className="text-lg font-semibold text-gray-900">Recent Reports</h2>
        </div>
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50">
              <tr>
                <th className="text-left py-4 px-6 text-sm font-semibold text-gray-700">Report Name</th>
                <th className="text-left py-4 px-6 text-sm font-semibold text-gray-700">Type</th>
                <th className="text-left py-4 px-6 text-sm font-semibold text-gray-700">Period</th>
                <th className="text-left py-4 px-6 text-sm font-semibold text-gray-700">Status</th>
                <th className="text-left py-4 px-6 text-sm font-semibold text-gray-700">Generated</th>
                <th className="text-right py-4 px-6 text-sm font-semibold text-gray-700">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {!reports?.data || reports.data.length === 0 ? (
                <tr>
                  <td colSpan={6} className="py-12 text-center text-gray-500">
                    No reports generated yet
                  </td>
                </tr>
              ) : (
                reports.data.map((report: any) => (
                  <tr key={report.id} className="hover:bg-gray-50">
                    <td className="py-4 px-6">
                      <div className="flex items-center gap-3">
                        <FileText className="h-5 w-5 text-gray-400" />
                        <span className="font-medium text-gray-900">{report.reportName}</span>
                      </div>
                    </td>
                    <td className="py-4 px-6">
                      <span className="px-3 py-1 text-xs font-medium rounded-full bg-blue-100 text-blue-700">
                        {report.reportType}
                      </span>
                    </td>
                    <td className="py-4 px-6 text-sm text-gray-700">
                      {report.startDate && report.endDate
                        ? `${format(new Date(report.startDate), 'MMM dd')} - ${format(new Date(report.endDate), 'MMM dd, yyyy')}`
                        : 'All time'
                      }
                    </td>
                    <td className="py-4 px-6">
                      <span className={`px-3 py-1 text-xs font-medium rounded-full ${
                        report.status === 'Completed'
                          ? 'bg-green-100 text-green-700'
                          : report.status === 'Failed'
                          ? 'bg-red-100 text-red-700'
                          : 'bg-yellow-100 text-yellow-700'
                      }`}>
                        {report.status}
                      </span>
                    </td>
                    <td className="py-4 px-6 text-sm text-gray-700">
                      {report.generatedAt
                        ? format(new Date(report.generatedAt), 'MMM dd, yyyy HH:mm')
                        : 'Pending'
                      }
                    </td>
                    <td className="py-4 px-6">
                      <div className="flex items-center justify-end">
                        {report.status === 'Completed' && (
                          <button
                            onClick={() => handleDownload(report.id, report.reportName)}
                            className="flex items-center gap-2 px-3 py-1 text-sm text-primary-600 hover:bg-primary-50 rounded-lg transition-colors"
                          >
                            <Download className="h-4 w-4" />
                            Download
                          </button>
                        )}
                      </div>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* Generate Report Modal */}
      {showModal && (
        <GenerateReportModal
          onClose={() => setShowModal(false)}
          onGenerate={(data) => generateMutation.mutate(data)}
          isLoading={generateMutation.isPending}
        />
      )}
    </div>
  )
}

function GenerateReportModal({ onClose, onGenerate, isLoading }: any) {
  const [formData, setFormData] = useState({
    reportType: 'Booking',
    reportName: '',
    startDate: '',
    endDate: '',
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    onGenerate({
      ...formData,
      parameters: JSON.stringify({}),
    })
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50">
      <div className="bg-white rounded-xl shadow-xl w-full max-w-md p-6">
        <h2 className="text-xl font-bold text-gray-900 mb-4">Generate New Report</h2>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Report Type
            </label>
            <select
              value={formData.reportType}
              onChange={(e) => setFormData({ ...formData, reportType: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500"
            >
              <option value="Booking">Booking Report</option>
              <option value="Revenue">Revenue Report</option>
              <option value="Vehicle">Vehicle Report</option>
              <option value="User">User Report</option>
            </select>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Report Name
            </label>
            <input
              type="text"
              value={formData.reportName}
              onChange={(e) => setFormData({ ...formData, reportName: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500"
              placeholder="e.g., Monthly Booking Report"
              required
            />
          </div>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                <Calendar className="h-4 w-4 inline mr-1" />
                Start Date
              </label>
              <input
                type="date"
                value={formData.startDate}
                onChange={(e) => setFormData({ ...formData, startDate: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                <Calendar className="h-4 w-4 inline mr-1" />
                End Date
              </label>
              <input
                type="date"
                value={formData.endDate}
                onChange={(e) => setFormData({ ...formData, endDate: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500"
              />
            </div>
          </div>
          <div className="flex gap-3 pt-4">
            <button
              type="button"
              onClick={onClose}
              className="flex-1 px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={isLoading}
              className="flex-1 px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 disabled:opacity-50"
            >
              {isLoading ? 'Generating...' : 'Generate'}
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}
