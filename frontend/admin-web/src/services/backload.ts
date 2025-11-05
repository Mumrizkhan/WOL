import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import httpClient from './http';
import type {
  CreateBackloadOpportunityRequest,
  BackloadOpportunity,
  RouteHeatmapData,
} from './types';

export const backloadApi = {
  createOpportunity: async (data: CreateBackloadOpportunityRequest): Promise<{ opportunityId: string }> => {
    const response = await httpClient.post<{ opportunityId: string }>('/backload/opportunities', data);
    return response.data;
  },

  getOpportunities: async (): Promise<BackloadOpportunity[]> => {
    const response = await httpClient.get<BackloadOpportunity[]>('/backload/opportunities');
    return response.data;
  },

  getRouteHeatmap: async (startDate?: string, endDate?: string): Promise<RouteHeatmapData[]> => {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);
    
    const response = await httpClient.get<RouteHeatmapData[]>(`/backload/heatmap?${params}`);
    return response.data;
  },

  getRouteUtilization: async (originCity?: string, destinationCity?: string): Promise<any> => {
    const params = new URLSearchParams();
    if (originCity) params.append('originCity', originCity);
    if (destinationCity) params.append('destinationCity', destinationCity);
    
    const response = await httpClient.get(`/backload/heatmap/utilization?${params}`);
    return response.data;
  },

  getImbalancedRoutes: async (minImbalancePercentage: number = 30): Promise<RouteHeatmapData[]> => {
    const response = await httpClient.get<RouteHeatmapData[]>(
      `/backload/heatmap/imbalanced?minImbalancePercentage=${minImbalancePercentage}`
    );
    return response.data;
  },

  toggleDriverAvailability: async (driverId: string, isAvailable: boolean): Promise<void> => {
    await httpClient.post('/backload/toggle-availability', { driverId, isAvailable });
  },
};

export const useCreateBackloadOpportunity = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: backloadApi.createOpportunity,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['backloadOpportunities'] });
    },
  });
};

export const useBackloadOpportunities = () => {
  return useQuery({
    queryKey: ['backloadOpportunities'],
    queryFn: backloadApi.getOpportunities,
  });
};

export const useRouteHeatmap = (startDate?: string, endDate?: string) => {
  return useQuery({
    queryKey: ['routeHeatmap', startDate, endDate],
    queryFn: () => backloadApi.getRouteHeatmap(startDate, endDate),
  });
};

export const useRouteUtilization = (originCity?: string, destinationCity?: string) => {
  return useQuery({
    queryKey: ['routeUtilization', originCity, destinationCity],
    queryFn: () => backloadApi.getRouteUtilization(originCity, destinationCity),
  });
};

export const useImbalancedRoutes = (minImbalancePercentage: number = 30) => {
  return useQuery({
    queryKey: ['imbalancedRoutes', minImbalancePercentage],
    queryFn: () => backloadApi.getImbalancedRoutes(minImbalancePercentage),
  });
};

export const useToggleDriverAvailability = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ driverId, isAvailable }: { driverId: string; isAvailable: boolean }) =>
      backloadApi.toggleDriverAvailability(driverId, isAvailable),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['backloadOpportunities'] });
    },
  });
};
