import { useMutation, useQueryClient } from '@tanstack/react-query';
import httpClient from './http';
import type {
  RecordLocationRequest,
} from './types';

export const trackingApi = {
  recordLocation: async (data: RecordLocationRequest): Promise<void> => {
    await httpClient.post('/tracking/location', data);
  },
};

export const useRecordLocation = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: trackingApi.recordLocation,
    onSuccess: () => {
    },
  });
};
