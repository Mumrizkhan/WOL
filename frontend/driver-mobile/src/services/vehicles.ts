import { useQuery } from '@tanstack/react-query';
import httpClient from './http';
import type {
  Vehicle,
} from './types';

export const vehiclesApi = {
  getMy: async (): Promise<Vehicle[]> => {
    const response = await httpClient.get<Vehicle[]>('/vehicles/my');
    return response.data;
  },

  getById: async (id: string): Promise<Vehicle> => {
    const response = await httpClient.get<Vehicle>(`/vehicles/${id}`);
    return response.data;
  },
};

export const useMyVehicles = () => {
  return useQuery({
    queryKey: ['myVehicles'],
    queryFn: vehiclesApi.getMy,
  });
};

export const useVehicle = (id: string) => {
  return useQuery({
    queryKey: ['vehicle', id],
    queryFn: () => vehiclesApi.getById(id),
    enabled: !!id,
  });
};
