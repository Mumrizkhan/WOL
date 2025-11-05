import { useQuery } from '@tanstack/react-query';
import httpClient from './http';

export interface EarningsStats {
  today: number;
  week: number;
  month: number;
  totalEarnings: number;
  pendingPayouts: number;
  completedTrips: number;
}

export interface EarningsHistory {
  id: string;
  bookingId: string;
  amount: number;
  date: string;
  status: string;
}

export const earningsApi = {
  getStats: async (): Promise<EarningsStats> => {
    const response = await httpClient.get<EarningsStats>('/payments/driver/earnings');
    return response.data;
  },

  getHistory: async (): Promise<EarningsHistory[]> => {
    const response = await httpClient.get<EarningsHistory[]>('/payments/driver/history');
    return response.data;
  },
};

export const useEarningsStats = () => {
  return useQuery({
    queryKey: ['earningsStats'],
    queryFn: earningsApi.getStats,
    refetchInterval: 60000, // Refetch every minute
  });
};

export const useEarningsHistory = () => {
  return useQuery({
    queryKey: ['earningsHistory'],
    queryFn: earningsApi.getHistory,
  });
};
