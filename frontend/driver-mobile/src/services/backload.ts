import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import httpClient from './http';
import type {
  CreateBackloadOpportunityRequest,
  BackloadOpportunity,
} from './types';

export const backloadApi = {
  createOpportunity: async (data: CreateBackloadOpportunityRequest): Promise<{ opportunityId: string }> => {
    const response = await httpClient.post<{ opportunityId: string }>('/backload/opportunities', data);
    return response.data;
  },

  getMyOpportunities: async (): Promise<BackloadOpportunity[]> => {
    const response = await httpClient.get<BackloadOpportunity[]>('/backload/opportunities/my');
    return response.data;
  },

  getRecommendations: async (): Promise<BackloadOpportunity[]> => {
    const response = await httpClient.get<BackloadOpportunity[]>('/backload/recommendations');
    return response.data;
  },

  toggleAvailability: async (isAvailable: boolean): Promise<void> => {
    await httpClient.post('/backload/toggle-availability', { isAvailable });
  },
};

export const useCreateBackloadOpportunity = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: backloadApi.createOpportunity,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['myBackloadOpportunities'] });
    },
  });
};

export const useMyBackloadOpportunities = () => {
  return useQuery({
    queryKey: ['myBackloadOpportunities'],
    queryFn: backloadApi.getMyOpportunities,
  });
};

export const useBackloadRecommendations = () => {
  return useQuery({
    queryKey: ['backloadRecommendations'],
    queryFn: backloadApi.getRecommendations,
    refetchInterval: 60000, // Refetch every minute
  });
};

export const useToggleBackloadAvailability = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: backloadApi.toggleAvailability,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['myBackloadOpportunities'] });
    },
  });
};
