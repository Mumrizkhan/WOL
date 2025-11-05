import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import httpClient from './http';
import type {
  GenerateReportRequest,
  GenerateReportResponse,
  Report,
} from './types';

export const reportingApi = {
  generate: async (data: GenerateReportRequest): Promise<GenerateReportResponse> => {
    const response = await httpClient.post<GenerateReportResponse>('/reports', data);
    return response.data;
  },

  getAll: async (): Promise<Report[]> => {
    const response = await httpClient.get<Report[]>('/reports');
    return response.data;
  },

  getById: async (id: string): Promise<Report> => {
    const response = await httpClient.get<Report>(`/reports/${id}`);
    return response.data;
  },

  download: async (id: string): Promise<Blob> => {
    const response = await httpClient.get(`/reports/${id}/download`, {
      responseType: 'blob',
    });
    return response.data;
  },
};

export const useGenerateReport = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: reportingApi.generate,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['reports'] });
    },
  });
};

export const useReports = () => {
  return useQuery({
    queryKey: ['reports'],
    queryFn: reportingApi.getAll,
  });
};

export const useReport = (id: string) => {
  return useQuery({
    queryKey: ['report', id],
    queryFn: () => reportingApi.getById(id),
    enabled: !!id,
  });
};

export const useDownloadReport = () => {
  return useMutation({
    mutationFn: reportingApi.download,
  });
};
