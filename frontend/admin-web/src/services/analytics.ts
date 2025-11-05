import { useQuery } from '@tanstack/react-query';
import httpClient from './http';
import type {
  DashboardStats,
  BookingTrend,
  RevenueTrend,
} from './types';

export const analyticsApi = {
  getDashboardStats: async (): Promise<DashboardStats> => {
    const response = await httpClient.get<DashboardStats>('/analytics/dashboard');
    return response.data;
  },

  getBookingTrends: async (startDate?: string, endDate?: string): Promise<BookingTrend[]> => {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);
    
    const response = await httpClient.get<BookingTrend[]>(`/analytics/bookings/trends?${params}`);
    return response.data;
  },

  getRevenueTrends: async (startDate?: string, endDate?: string): Promise<RevenueTrend[]> => {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);
    
    const response = await httpClient.get<RevenueTrend[]>(`/analytics/revenue/trends?${params}`);
    return response.data;
  },
};

export const useDashboardStats = () => {
  return useQuery({
    queryKey: ['dashboardStats'],
    queryFn: analyticsApi.getDashboardStats,
    refetchInterval: 60000, // Refetch every minute
  });
};

export const useBookingTrends = (startDate?: string, endDate?: string) => {
  return useQuery({
    queryKey: ['bookingTrends', startDate, endDate],
    queryFn: () => analyticsApi.getBookingTrends(startDate, endDate),
  });
};

export const useRevenueTrends = (startDate?: string, endDate?: string) => {
  return useQuery({
    queryKey: ['revenueTrends', startDate, endDate],
    queryFn: () => analyticsApi.getRevenueTrends(startDate, endDate),
  });
};
