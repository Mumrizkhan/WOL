import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import httpClient from './http';
import type {
  Vehicle,
  RegisterVehicleRequest,
} from './types';

export const vehiclesApi = {
  getAll: async (): Promise<Vehicle[]> => {
    const response = await httpClient.get<Vehicle[]>('/vehicles');
    return response.data;
  },

  getById: async (id: string): Promise<Vehicle> => {
    const response = await httpClient.get<Vehicle>(`/vehicles/${id}`);
    return response.data;
  },

  register: async (data: RegisterVehicleRequest): Promise<{ vehicleId: string }> => {
    const response = await httpClient.post<{ vehicleId: string }>('/vehicles', data);
    return response.data;
  },

  update: async (id: string, data: Partial<RegisterVehicleRequest>): Promise<void> => {
    await httpClient.put(`/vehicles/${id}`, data);
  },

  delete: async (id: string): Promise<void> => {
    await httpClient.delete(`/vehicles/${id}`);
  },
};

export const useVehicles = () => {
  return useQuery({
    queryKey: ['vehicles'],
    queryFn: vehiclesApi.getAll,
  });
};

export const useVehicle = (id: string) => {
  return useQuery({
    queryKey: ['vehicle', id],
    queryFn: () => vehiclesApi.getById(id),
    enabled: !!id,
  });
};

export const useRegisterVehicle = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: vehiclesApi.register,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['vehicles'] });
    },
  });
};

export const useUpdateVehicle = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: Partial<RegisterVehicleRequest> }) =>
      vehiclesApi.update(id, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['vehicles'] });
      queryClient.invalidateQueries({ queryKey: ['vehicle', variables.id] });
    },
  });
};

export const useDeleteVehicle = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: vehiclesApi.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['vehicles'] });
    },
  });
};
