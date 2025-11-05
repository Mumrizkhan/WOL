import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import httpClient from '../http';
import type {
  BANTiming,
  CreateBANTimingRequest,
} from '../types';

export const banTimingApi = {
  getAll: async (): Promise<BANTiming[]> => {
    const response = await httpClient.get<BANTiming[]>('/bookings/bantiming');
    return response.data;
  },

  getByCity: async (city: string): Promise<BANTiming[]> => {
    const response = await httpClient.get<BANTiming[]>(`/bookings/bantiming/${city}`);
    return response.data;
  },

  create: async (data: CreateBANTimingRequest): Promise<{ id: string }> => {
    const response = await httpClient.post<{ id: string }>('/bookings/bantiming', data);
    return response.data;
  },

  update: async (id: string, data: Partial<CreateBANTimingRequest>): Promise<void> => {
    await httpClient.put(`/bookings/bantiming/${id}`, data);
  },

  delete: async (id: string): Promise<void> => {
    await httpClient.delete(`/bookings/bantiming/${id}`);
  },

  activate: async (id: string): Promise<void> => {
    await httpClient.post(`/bookings/bantiming/${id}/activate`);
  },

  deactivate: async (id: string): Promise<void> => {
    await httpClient.post(`/bookings/bantiming/${id}/deactivate`);
  },
};

export const useBANTimings = () => {
  return useQuery({
    queryKey: ['banTimings'],
    queryFn: banTimingApi.getAll,
  });
};

export const useCityBANTimings = (city: string) => {
  return useQuery({
    queryKey: ['banTimings', city],
    queryFn: () => banTimingApi.getByCity(city),
    enabled: !!city,
  });
};

export const useCreateBANTiming = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: banTimingApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['banTimings'] });
    },
  });
};

export const useUpdateBANTiming = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: Partial<CreateBANTimingRequest> }) =>
      banTimingApi.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['banTimings'] });
    },
  });
};

export const useDeleteBANTiming = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: banTimingApi.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['banTimings'] });
    },
  });
};

export const useActivateBANTiming = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: banTimingApi.activate,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['banTimings'] });
    },
  });
};

export const useDeactivateBANTiming = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: banTimingApi.deactivate,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['banTimings'] });
    },
  });
};
