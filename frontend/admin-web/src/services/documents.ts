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

  getByEntity: async (entityId: string, entityType: string): Promise<Document[]> => {
    const response = await httpClient.get<Document[]>(`/documents/${entityType}/${entityId}`);
    return response.data;
  },

  getById: async (id: string): Promise<Document> => {
    const response = await httpClient.get<Document>(`/documents/${id}`);
    return response.data;
  },

  delete: async (id: string): Promise<void> => {
    await httpClient.delete(`/documents/${id}`);
  },
};

export const useUploadDocument = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: documentsApi.upload,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['documents'] });
    },
  });
};

export const useEntityDocuments = (entityId: string, entityType: string) => {
  return useQuery({
    queryKey: ['documents', entityType, entityId],
    queryFn: () => documentsApi.getByEntity(entityId, entityType),
    enabled: !!entityId && !!entityType,
  });
};

export const useDocument = (id: string) => {
  return useQuery({
    queryKey: ['document', id],
    queryFn: () => documentsApi.getById(id),
    enabled: !!id,
  });
};

export const useDeleteDocument = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: documentsApi.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['documents'] });
    },
  });
};
