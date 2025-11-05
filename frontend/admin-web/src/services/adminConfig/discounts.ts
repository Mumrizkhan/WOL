import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import httpClient from '../http';
import type {
  DiscountConfiguration,
  UpdateDiscountConfigurationRequest,
} from '../types';

export const discountsApi = {
  getAll: async (): Promise<DiscountConfiguration[]> => {
    const response = await httpClient.get<DiscountConfiguration[]>('/pricing/discounts');
    return response.data;
  },

  getByType: async (discountType: string): Promise<DiscountConfiguration> => {
    const response = await httpClient.get<DiscountConfiguration>(`/pricing/discounts/${discountType}`);
    return response.data;
  },

  update: async (id: string, data: UpdateDiscountConfigurationRequest): Promise<void> => {
    await httpClient.put(`/pricing/discounts/${id}`, data);
  },

  activate: async (id: string): Promise<void> => {
    await httpClient.post(`/pricing/discounts/${id}/activate`);
  },

  deactivate: async (id: string): Promise<void> => {
    await httpClient.post(`/pricing/discounts/${id}/deactivate`);
  },
};

export const useDiscounts = () => {
  return useQuery({
    queryKey: ['discounts'],
    queryFn: discountsApi.getAll,
  });
};

export const useDiscountByType = (discountType: string) => {
  return useQuery({
    queryKey: ['discount', discountType],
    queryFn: () => discountsApi.getByType(discountType),
    enabled: !!discountType,
  });
};

export const useUpdateDiscount = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateDiscountConfigurationRequest }) =>
      discountsApi.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['discounts'] });
    },
  });
};

export const useActivateDiscount = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: discountsApi.activate,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['discounts'] });
    },
  });
};

export const useDeactivateDiscount = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: discountsApi.deactivate,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['discounts'] });
    },
  });
};
