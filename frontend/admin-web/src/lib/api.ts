import axios from 'axios'
import { useAuthStore } from '../store/authStore'

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api'

export const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

api.interceptors.request.use((config) => {
  const token = useAuthStore.getState().token
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      useAuthStore.getState().logout()
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

export const authApi = {
  login: (email: string, password: string) =>
    api.post('/identity/auth/login', { email, password }),
  register: (data: any) =>
    api.post('/identity/auth/register', data),
}

export const usersApi = {
  getAll: () => api.get('/identity/users'),
  getById: (id: string) => api.get(`/identity/users/${id}`),
  create: (data: any) => api.post('/identity/users', data),
  update: (id: string, data: any) => api.put(`/identity/users/${id}`, data),
  delete: (id: string) => api.delete(`/identity/users/${id}`),
}

export const bookingsApi = {
  getAll: () => api.get('/bookings'),
  getById: (id: string) => api.get(`/bookings/${id}`),
  create: (data: any) => api.post('/bookings', data),
  update: (id: string, data: any) => api.put(`/bookings/${id}`, data),
  delete: (id: string) => api.delete(`/bookings/${id}`),
  assign: (id: string, driverId: string) =>
    api.post(`/bookings/${id}/assign`, { driverId }),
}

export const vehiclesApi = {
  getAll: () => api.get('/vehicles'),
  getById: (id: string) => api.get(`/vehicles/${id}`),
  create: (data: any) => api.post('/vehicles', data),
  update: (id: string, data: any) => api.put(`/vehicles/${id}`, data),
  delete: (id: string) => api.delete(`/vehicles/${id}`),
}

export const paymentsApi = {
  getAll: () => api.get('/payments'),
  getById: (id: string) => api.get(`/payments/${id}`),
  process: (data: any) => api.post('/payments/process', data),
}

export const analyticsApi = {
  getDashboardStats: () => api.get('/analytics/dashboard'),
  getBookingTrends: () => api.get('/analytics/bookings/trends'),
  getRevenueTrends: () => api.get('/analytics/revenue/trends'),
}

export const reportsApi = {
  generate: (data: any) => api.post('/reports', data),
  getAll: () => api.get('/reports'),
  download: (id: string) => api.get(`/reports/${id}/download`, { responseType: 'blob' }),
}
