import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import httpClient from './http';
import type { User } from './types';

export const usersApi = {
  getAll: async (): Promise<User[]> => {
    const response = await httpClient.get<User[]>('/identity/users');
    return response.data;
  },

  getById: async (id: string): Promise<User> => {
    const response = await httpClient.get<User>(`/identity/users/${id}`);
    return response.data;
  },

  update: async (id: string, data: Partial<User>): Promise<void> => {
    await httpClient.put(`/identity/users/${id}`, data);
  },

  delete: async (id: string): Promise<void> => {
    await httpClient.delete(`/identity/users/${id}`);
  },

  activate: async (id: string): Promise<void> => {
    await httpClient.post(`/identity/users/${id}/activate`);
  },

  deactivate: async (id: string): Promise<void> => {
    await httpClient.post(`/identity/users/${id}/deactivate`);
  },
};

export const useUsers = () => {
  return useQuery({
    queryKey: ['users'],
    queryFn: usersApi.getAll,
  });
};

export const useUser = (id: string) => {
  return useQuery({
    queryKey: ['user', id],
    queryFn: () => usersApi.getById(id),
    enabled: !!id,
  });
};

export const useUpdateUser = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: Partial<User> }) =>
      usersApi.update(id, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
      queryClient.invalidateQueries({ queryKey: ['user', variables.id] });
    },
  });
};

export const useDeleteUser = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: usersApi.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
    },
  });
};

export const useActivateUser = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: usersApi.activate,
    onSuccess: (_, userId) => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
      queryClient.invalidateQueries({ queryKey: ['user', userId] });
    },
  });
};

export const useDeactivateUser = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: usersApi.deactivate,
    onSuccess: (_, userId) => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
      queryClient.invalidateQueries({ queryKey: ['user', userId] });
    },
  });
};
