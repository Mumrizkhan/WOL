import { useMutation, useQuery } from '@tanstack/react-query';
import httpClient from './http';
import type {
  CalculatePriceRequest,
  CalculatePriceResponse,
} from './types';

export const pricingApi = {
  calculatePrice: async (data: CalculatePriceRequest): Promise<CalculatePriceResponse> => {
    const response = await httpClient.post<CalculatePriceResponse>('/pricing/calculate', data);
    return response.data;
  },
};

export const useCalculatePrice = () => {
  return useMutation({
    mutationFn: pricingApi.calculatePrice,
  });
};
