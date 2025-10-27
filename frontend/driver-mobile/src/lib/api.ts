import axios from 'axios';
import { useAuthStore } from '../store/authStore';

const API_URL = 'http://localhost:5000/api'; // Update with your API URL

export const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use((config) => {
  const token = useAuthStore.getState().token;
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      await useAuthStore.getState().logout();
    }
    return Promise.reject(error);
  }
);

export const authApi = {
  login: (email: string, password: string) =>
    api.post('/identity/auth/login', { email, password }),
};

export const jobsApi = {
  getAvailable: () => api.get('/bookings/available'),
  getMyJobs: () => api.get('/bookings/driver/my-jobs'),
  getById: (id: string) => api.get(`/bookings/${id}`),
  accept: (id: string) => api.post(`/bookings/${id}/accept`),
  start: (id: string) => api.post(`/bookings/${id}/start`),
  complete: (id: string) => api.post(`/bookings/${id}/complete`),
  updateStatus: (id: string, status: string) => 
    api.put(`/bookings/${id}/status`, { status }),
};

export const trackingApi = {
  updateLocation: (bookingId: string, latitude: number, longitude: number) =>
    api.post(`/tracking/${bookingId}/location`, { latitude, longitude }),
};

export const earningsApi = {
  getStats: () => api.get('/payments/driver/earnings'),
  getHistory: () => api.get('/payments/driver/history'),
};

export const vehiclesApi = {
  getMy: () => api.get('/vehicles/my'),
  getById: (id: string) => api.get(`/vehicles/${id}`),
};
