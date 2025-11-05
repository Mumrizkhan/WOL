import { useQuery } from '@tanstack/react-query';
import httpClient from './http';
import type {
  LocationHistory,
} from './types';

export const trackingApi = {
  getLocationHistory: async (bookingId: string): Promise<LocationHistory> => {
    const response = await httpClient.get<LocationHistory>(`/tracking/${bookingId}/history`);
    return response.data;
  },

  getCurrentLocation: async (bookingId: string): Promise<any> => {
    const response = await httpClient.get(`/tracking/${bookingId}/current`);
    return response.data;
  },
};

export const useLocationHistory = (bookingId: string) => {
  return useQuery({
    queryKey: ['locationHistory', bookingId],
    queryFn: () => trackingApi.getLocationHistory(bookingId),
    enabled: !!bookingId,
  });
};

export const useCurrentLocation = (bookingId: string) => {
  return useQuery({
    queryKey: ['currentLocation', bookingId],
    queryFn: () => trackingApi.getCurrentLocation(bookingId),
    enabled: !!bookingId,
    refetchInterval: 5000, // Refetch every 5 seconds for real-time tracking
  });
};
