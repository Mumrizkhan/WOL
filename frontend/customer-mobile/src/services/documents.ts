import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import httpClient from './http';
import type {
  UploadDocumentRequest,
  Document,
} from './types';

export const documentsApi = {
  upload: async (data: UploadDocumentRequest): Promise<{ documentId: string }> => {
    const response = await httpClient.post<{ documentId: string }>('/documents', data);
    return response.data;
  },

  getMyDocuments: async (): Promise<Document[]> => {
    const response = await httpClient.get<Document[]>('/documents/my');
    return response.data;
  },

  getById: async (id: string): Promise<Document> => {
    const response = await httpClient.get<Document>(`/documents/${id}`);
    return response.data;
  },
};

export const useUploadDocument = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: documentsApi.upload,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['myDocuments'] });
    },
  });
};

export const useMyDocuments = () => {
  return useQuery({
    queryKey: ['myDocuments'],
    queryFn: documentsApi.getMyDocuments,
  });
};

export const useDocument = (id: string) => {
  return useQuery({
    queryKey: ['document', id],
    queryFn: () => documentsApi.getById(id),
    enabled: !!id,
  });
};
