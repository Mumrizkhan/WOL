import { useQuery } from '@tanstack/react-query';
import httpClient from './http';
import type {
  Vehicle,
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
