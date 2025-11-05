import { useQuery } from '@tanstack/react-query';
import httpClient from './http';
import type {
  ComplianceCheck,
} from './types';

export const complianceApi = {
  getMy: async (): Promise<ComplianceCheck[]> => {
    const response = await httpClient.get<ComplianceCheck[]>('/compliance/my');
    return response.data;
  },

  getStatus: async (): Promise<any> => {
    const response = await httpClient.get('/compliance/status');
    return response.data;
  },
};

export const useMyCompliance = () => {
  return useQuery({
    queryKey: ['myCompliance'],
    queryFn: complianceApi.getMy,
  });
};

export const useComplianceStatus = () => {
  return useQuery({
    queryKey: ['complianceStatus'],
    queryFn: complianceApi.getStatus,
  });
};
