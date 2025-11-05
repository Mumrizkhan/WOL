import { useQuery } from '@tanstack/react-query';
import httpClient from './http';
import type {
  ComplianceCheck,
} from './types';

export const complianceApi = {
  getAll: async (): Promise<ComplianceCheck[]> => {
    const response = await httpClient.get<ComplianceCheck[]>('/compliance/checks');
    return response.data;
  },

  getByDriver: async (driverId: string): Promise<ComplianceCheck[]> => {
    const response = await httpClient.get<ComplianceCheck[]>(`/compliance/driver/${driverId}`);
    return response.data;
  },

  getByVehicle: async (vehicleId: string): Promise<ComplianceCheck[]> => {
    const response = await httpClient.get<ComplianceCheck[]>(`/compliance/vehicle/${vehicleId}`);
    return response.data;
  },

  getViolations: async (): Promise<any[]> => {
    const response = await httpClient.get('/compliance/violations');
    return response.data;
  },
};

export const useComplianceChecks = () => {
  return useQuery({
    queryKey: ['complianceChecks'],
    queryFn: complianceApi.getAll,
  });
};

export const useDriverCompliance = (driverId: string) => {
  return useQuery({
    queryKey: ['driverCompliance', driverId],
    queryFn: () => complianceApi.getByDriver(driverId),
    enabled: !!driverId,
  });
};

export const useVehicleCompliance = (vehicleId: string) => {
  return useQuery({
    queryKey: ['vehicleCompliance', vehicleId],
    queryFn: () => complianceApi.getByVehicle(vehicleId),
    enabled: !!vehicleId,
  });
};

export const useComplianceViolations = () => {
  return useQuery({
    queryKey: ['complianceViolations'],
    queryFn: complianceApi.getViolations,
  });
};
