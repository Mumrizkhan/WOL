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
  register: (data: any) =>
    api.post('/identity/auth/register', data),
};

export const bookingsApi = {
  getAll: () => api.get('/bookings'),
  getById: (id: string) => api.get(`/bookings/${id}`),
  create: (data: any) => api.post('/bookings', data),
  getMyBookings: () => api.get('/bookings/my'),
};

export const trackingApi = {
  getLocation: (bookingId: string) => api.get(`/tracking/${bookingId}`),
  updateLocation: (bookingId: string, latitude: number, longitude: number) =>
    api.post(`/tracking/${bookingId}/location`, { latitude, longitude }),
};

export const paymentsApi = {
  process: (data: any) => api.post('/payments/process', data),
  getByBooking: (bookingId: string) => api.get(`/payments/booking/${bookingId}`),
};

export const vehiclesApi = {
  getAll: () => api.get('/vehicles'),
  getById: (id: string) => api.get(`/vehicles/${id}`),
};
