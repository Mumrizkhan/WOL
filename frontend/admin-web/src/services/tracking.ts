import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import httpClient from './http';
import type {
  RecordLocationRequest,
  LocationHistory,
} from './types';

export const trackingApi = {
  recordLocation: async (data: RecordLocationRequest): Promise<void> => {
    await httpClient.post('/tracking/location', data);
  },

  getLocationHistory: async (bookingId: string): Promise<LocationHistory> => {
    const response = await httpClient.get<LocationHistory>(`/tracking/${bookingId}/history`);
    return response.data;
  },

  getCurrentLocation: async (bookingId: string): Promise<any> => {
    const response = await httpClient.get(`/tracking/${bookingId}/current`);
    return response.data;
  },
};

export const useRecordLocation = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: trackingApi.recordLocation,
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['locationHistory', variables.bookingId] });
      queryClient.invalidateQueries({ queryKey: ['currentLocation', variables.bookingId] });
    },
  });
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
