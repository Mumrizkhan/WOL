import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { vehiclesApi } from '../lib/api'
import { Plus, Search, Edit, Trash2, Truck, CheckCircle, XCircle } from 'lucide-react'

export default function Vehicles() {
  const queryClient = useQueryClient()
  const [searchTerm, setSearchTerm] = useState('')
  const [showModal, setShowModal] = useState(false)
  const [selectedVehicle, setSelectedVehicle] = useState<any>(null)

  const { data: vehicles, isLoading } = useQuery({
    queryKey: ['vehicles'],
    queryFn: () => vehiclesApi.getAll(),
  })

  const deleteMutation = useMutation({
    mutationFn: (id: string) => vehiclesApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['vehicles'] })
    },
  })

  const handleDelete = (id: string) => {
    if (window.confirm('Are you sure you want to delete this vehicle?')) {
      deleteMutation.mutate(id)
    }
  }

  const filteredVehicles = vehicles?.data?.filter((vehicle: any) =>
    vehicle.plateNumber?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    vehicle.vehicleType?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    vehicle.make?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    vehicle.model?.toLowerCase().includes(searchTerm.toLowerCase())
  ) || []

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-96">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Vehicles</h1>
          <p className="text-gray-600 mt-1">Manage fleet vehicles</p>
        </div>
        <button
          onClick={() => {
            setSelectedVehicle(null)
            setShowModal(true)
          }}
          className="flex items-center gap-2 bg-primary-600 text-white px-4 py-2 rounded-lg hover:bg-primary-700 transition-colors"
        >
          <Plus className="h-5 w-5" />
          Add Vehicle
        </button>
      </div>

      {/* Search */}
      <div className="bg-white rounded-xl shadow-sm p-4 border border-gray-100">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-gray-400" />
          <input
            type="text"
            placeholder="Search vehicles by plate number, type, make, or model..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent"
          />
        </div>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div className="bg-white rounded-xl shadow-sm p-4 border border-gray-100">
          <p className="text-sm text-gray-600">Total Vehicles</p>
          <p className="text-2xl font-bold text-gray-900 mt-1">{vehicles?.data?.length || 0}</p>
        </div>
        <div className="bg-white rounded-xl shadow-sm p-4 border border-gray-100">
          <p className="text-sm text-gray-600">Active</p>
          <p className="text-2xl font-bold text-green-600 mt-1">
            {vehicles?.data?.filter((v: any) => v.status === 'Active').length || 0}
          </p>
        </div>
        <div className="bg-white rounded-xl shadow-sm p-4 border border-gray-100">
          <p className="text-sm text-gray-600">In Maintenance</p>
          <p className="text-2xl font-bold text-yellow-600 mt-1">
            {vehicles?.data?.filter((v: any) => v.status === 'Maintenance').length || 0}
          </p>
        </div>
        <div className="bg-white rounded-xl shadow-sm p-4 border border-gray-100">
          <p className="text-sm text-gray-600">Inactive</p>
          <p className="text-2xl font-bold text-red-600 mt-1">
            {vehicles?.data?.filter((v: any) => v.status === 'Inactive').length || 0}
          </p>
        </div>
      </div>

      {/* Vehicles Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {filteredVehicles.length === 0 ? (
          <div className="col-span-full bg-white rounded-xl shadow-sm p-12 text-center border border-gray-100">
            <p className="text-gray-500">No vehicles found</p>
          </div>
        ) : (
          filteredVehicles.map((vehicle: any) => (
            <div
              key={vehicle.id}
              className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden hover:shadow-md transition-shadow"
            >
              <div className="bg-gradient-to-br from-primary-500 to-primary-600 p-6 text-white">
                <div className="flex items-center justify-between mb-4">
                  <Truck className="h-12 w-12" />
                  <span className={`px-3 py-1 text-xs font-medium rounded-full ${
                    vehicle.status === 'Active' 
                      ? 'bg-green-500 text-white' 
                      : vehicle.status === 'Maintenance'
                      ? 'bg-yellow-500 text-white'
                      : 'bg-red-500 text-white'
                  }`}>
                    {vehicle.status}
                  </span>
                </div>
                <h3 className="text-2xl font-bold">{vehicle.plateNumber}</h3>
                <p className="text-primary-100 mt-1">{vehicle.vehicleType}</p>
              </div>

              <div className="p-6 space-y-3">
                <div className="flex justify-between text-sm">
                  <span className="text-gray-600">Make & Model</span>
                  <span className="font-medium text-gray-900">
                    {vehicle.make} {vehicle.model}
                  </span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-gray-600">Year</span>
                  <span className="font-medium text-gray-900">{vehicle.year}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-gray-600">Capacity</span>
                  <span className="font-medium text-gray-900">{vehicle.capacity} tons</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-gray-600">Owner</span>
                  <span className="font-medium text-gray-900">{vehicle.ownerId}</span>
                </div>

                <div className="pt-3 border-t border-gray-100">
                  <div className="flex items-center gap-2 text-sm mb-2">
                    {vehicle.isVerified ? (
                      <>
                        <CheckCircle className="h-4 w-4 text-green-600" />
                        <span className="text-green-600">Verified</span>
                      </>
                    ) : (
                      <>
                        <XCircle className="h-4 w-4 text-red-600" />
                        <span className="text-red-600">Not Verified</span>
                      </>
                    )}
                  </div>
                </div>

                <div className="flex gap-2 pt-3">
                  <button
                    onClick={() => {
                      setSelectedVehicle(vehicle)
                      setShowModal(true)
                    }}
                    className="flex-1 px-3 py-2 text-sm text-blue-600 border border-blue-600 rounded-lg hover:bg-blue-50 transition-colors"
                  >
                    <Edit className="h-4 w-4 inline mr-1" />
                    Edit
                  </button>
                  <button
                    onClick={() => handleDelete(vehicle.id)}
                    className="flex-1 px-3 py-2 text-sm text-red-600 border border-red-600 rounded-lg hover:bg-red-50 transition-colors"
                  >
                    <Trash2 className="h-4 w-4 inline mr-1" />
                    Delete
                  </button>
                </div>
              </div>
            </div>
          ))
        )}
      </div>

      {/* Vehicle Modal */}
      {showModal && (
        <VehicleModal
          vehicle={selectedVehicle}
          onClose={() => setShowModal(false)}
          onSuccess={() => {
            setShowModal(false)
            queryClient.invalidateQueries({ queryKey: ['vehicles'] })
          }}
        />
      )}
    </div>
  )
}

function VehicleModal({ vehicle, onClose, onSuccess }: any) {
  const [formData, setFormData] = useState({
    plateNumber: vehicle?.plateNumber || '',
    vehicleType: vehicle?.vehicleType || 'Truck',
    make: vehicle?.make || '',
    model: vehicle?.model || '',
    year: vehicle?.year || new Date().getFullYear(),
    capacity: vehicle?.capacity || 0,
    ownerId: vehicle?.ownerId || '',
    status: vehicle?.status || 'Active',
  })

  const mutation = useMutation({
    mutationFn: (data: any) =>
      vehicle ? vehiclesApi.update(vehicle.id, data) : vehiclesApi.create(data),
    onSuccess,
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    mutation.mutate(formData)
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50">
      <div className="bg-white rounded-xl shadow-xl w-full max-w-md p-6 max-h-screen overflow-y-auto">
        <h2 className="text-xl font-bold text-gray-900 mb-4">
          {vehicle ? 'Edit Vehicle' : 'Add New Vehicle'}
        </h2>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Plate Number
            </label>
            <input
              type="text"
              value={formData.plateNumber}
              onChange={(e) => setFormData({ ...formData, plateNumber: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500"
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Vehicle Type
            </label>
            <select
              value={formData.vehicleType}
              onChange={(e) => setFormData({ ...formData, vehicleType: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500"
            >
              <option value="Truck">Truck</option>
              <option value="Van">Van</option>
              <option value="Pickup">Pickup</option>
              <option value="Trailer">Trailer</option>
            </select>
          </div>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Make
              </label>
              <input
                type="text"
                value={formData.make}
                onChange={(e) => setFormData({ ...formData, make: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500"
                required
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Model
              </label>
              <input
                type="text"
                value={formData.model}
                onChange={(e) => setFormData({ ...formData, model: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500"
                required
              />
            </div>
          </div>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Year
              </label>
              <input
                type="number"
                value={formData.year}
                onChange={(e) => setFormData({ ...formData, year: parseInt(e.target.value) })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500"
                required
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Capacity (tons)
              </label>
              <input
                type="number"
                step="0.1"
                value={formData.capacity}
                onChange={(e) => setFormData({ ...formData, capacity: parseFloat(e.target.value) })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500"
                required
              />
            </div>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Owner ID
            </label>
            <input
              type="text"
              value={formData.ownerId}
              onChange={(e) => setFormData({ ...formData, ownerId: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500"
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Status
            </label>
            <select
              value={formData.status}
              onChange={(e) => setFormData({ ...formData, status: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500"
            >
              <option value="Active">Active</option>
              <option value="Maintenance">Maintenance</option>
              <option value="Inactive">Inactive</option>
            </select>
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
              disabled={mutation.isPending}
              className="flex-1 px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 disabled:opacity-50"
            >
              {mutation.isPending ? 'Saving...' : 'Save'}
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}
