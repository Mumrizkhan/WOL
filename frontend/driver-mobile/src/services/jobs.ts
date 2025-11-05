import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import httpClient from './http';
import type {
  Booking,
} from './types';

export const jobsApi = {
  getAvailable: async (): Promise<Booking[]> => {
    const response = await httpClient.get<Booking[]>('/bookings/available');
    return response.data;
  },

  getMyJobs: async (): Promise<Booking[]> => {
    const response = await httpClient.get<Booking[]>('/bookings/driver/my-jobs');
    return response.data;
  },

  getById: async (id: string): Promise<Booking> => {
    const response = await httpClient.get<Booking>(`/bookings/${id}`);
    return response.data;
  },

  accept: async (id: string): Promise<void> => {
    await httpClient.post(`/bookings/${id}/accept`);
  },

  start: async (id: string): Promise<void> => {
    await httpClient.post(`/bookings/${id}/start`);
  },

  complete: async (id: string): Promise<void> => {
    await httpClient.post(`/bookings/${id}/complete`);
  },

  updateStatus: async (id: string, status: string): Promise<void> => {
    await httpClient.put(`/bookings/${id}/status`, { status });
  },

  markReached: async (id: string, latitude: number, longitude: number, photoPath: string): Promise<void> => {
    await httpClient.post(`/bookings/${id}/reached`, { latitude, longitude, photoPath });
  },
};

export const useAvailableJobs = () => {
  return useQuery({
    queryKey: ['availableJobs'],
    queryFn: jobsApi.getAvailable,
    refetchInterval: 30000, // Refetch every 30 seconds
  });
};

export const useMyJobs = () => {
  return useQuery({
    queryKey: ['myJobs'],
    queryFn: jobsApi.getMyJobs,
  });
};

export const useJob = (id: string) => {
  return useQuery({
    queryKey: ['job', id],
    queryFn: () => jobsApi.getById(id),
    enabled: !!id,
  });
};

export const useAcceptJob = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: jobsApi.accept,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['availableJobs'] });
      queryClient.invalidateQueries({ queryKey: ['myJobs'] });
    },
  });
};

export const useStartJob = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: jobsApi.start,
    onSuccess: (_, jobId) => {
      queryClient.invalidateQueries({ queryKey: ['myJobs'] });
      queryClient.invalidateQueries({ queryKey: ['job', jobId] });
    },
  });
};

export const useCompleteJob = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: jobsApi.complete,
    onSuccess: (_, jobId) => {
      queryClient.invalidateQueries({ queryKey: ['myJobs'] });
      queryClient.invalidateQueries({ queryKey: ['job', jobId] });
    },
  });
};

export const useUpdateJobStatus = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, status }: { id: string; status: string }) =>
      jobsApi.updateStatus(id, status),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['myJobs'] });
      queryClient.invalidateQueries({ queryKey: ['job', variables.id] });
    },
  });
};

export const useMarkReached = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, latitude, longitude, photoPath }: { id: string; latitude: number; longitude: number; photoPath: string }) =>
      jobsApi.markReached(id, latitude, longitude, photoPath),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['myJobs'] });
      queryClient.invalidateQueries({ queryKey: ['job', variables.id] });
    },
  });
};
