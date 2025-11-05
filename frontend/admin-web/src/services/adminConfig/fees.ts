import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import httpClient from '../http';
import type {
  FeeConfiguration,
  UpdateFeeConfigurationRequest,
} from '../types';

export const feesApi = {
  getAll: async (): Promise<FeeConfiguration[]> => {
    const response = await httpClient.get<FeeConfiguration[]>('/pricing/fees');
    return response.data;
  },

  getByType: async (feeType: string): Promise<FeeConfiguration> => {
    const response = await httpClient.get<FeeConfiguration>(`/pricing/fees/${feeType}`);
    return response.data;
  },

  update: async (id: string, data: UpdateFeeConfigurationRequest): Promise<void> => {
    await httpClient.put(`/pricing/fees/${id}`, data);
  },

  activate: async (id: string): Promise<void> => {
    await httpClient.post(`/pricing/fees/${id}/activate`);
  },

  deactivate: async (id: string): Promise<void> => {
    await httpClient.post(`/pricing/fees/${id}/deactivate`);
  },
};

export const useFees = () => {
  return useQuery({
    queryKey: ['fees'],
    queryFn: feesApi.getAll,
  });
};

export const useFeeByType = (feeType: string) => {
  return useQuery({
    queryKey: ['fee', feeType],
    queryFn: () => feesApi.getByType(feeType),
    enabled: !!feeType,
  });
};

export const useUpdateFee = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateFeeConfigurationRequest }) =>
      feesApi.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['fees'] });
    },
  });
};

export const useActivateFee = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: feesApi.activate,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['fees'] });
    },
  });
};

export const useDeactivateFee = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: feesApi.deactivate,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['fees'] });
    },
  });
};
