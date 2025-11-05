import { useMutation, useQuery } from '@tanstack/react-query';
import httpClient from './http';
import type {
  LoginRequest,
  LoginResponse,
  RegisterUserRequest,
  RegisterUserResponse,
  User,
  GenerateOtpRequest,
  VerifyOtpRequest,
  OtpLoginRequest,
} from './types';

export const authApi = {
  login: async (data: LoginRequest): Promise<LoginResponse> => {
    const response = await httpClient.post<LoginResponse>('/identity/login', data);
    return response.data;
  },

  register: async (data: RegisterUserRequest): Promise<RegisterUserResponse> => {
    const response = await httpClient.post<RegisterUserResponse>('/identity/register', data);
    return response.data;
  },

  generateOtp: async (data: GenerateOtpRequest): Promise<{ message: string; code?: string }> => {
    const response = await httpClient.post('/identity/otp/generate', data);
    return response.data;
  },

  verifyOtp: async (data: VerifyOtpRequest): Promise<{ success: boolean; message: string }> => {
    const response = await httpClient.post('/identity/otp/verify', data);
    return response.data;
  },

  loginWithOtp: async (data: OtpLoginRequest): Promise<LoginResponse> => {
    const response = await httpClient.post<LoginResponse>('/identity/otp/login', data);
    return response.data;
  },

  getUserRoles: async (userId: string): Promise<string[]> => {
    const response = await httpClient.get<string[]>(`/identity/roles/user/${userId}`);
    return response.data;
  },

  assignRole: async (userId: string, role: string): Promise<void> => {
    await httpClient.post('/identity/roles/assign', { userId, role });
  },

  removeRole: async (userId: string, role: string): Promise<void> => {
    await httpClient.post('/identity/roles/remove', { userId, role });
  },

  getUserClaims: async (userId: string): Promise<any[]> => {
    const response = await httpClient.get(`/identity/claims/user/${userId}`);
    return response.data;
  },

  addClaim: async (userId: string, claimType: string, claimValue: string): Promise<void> => {
    await httpClient.post('/identity/claims/add', { userId, claimType, claimValue });
  },
};

export const useLogin = () => {
  return useMutation({
    mutationFn: authApi.login,
  });
};

export const useRegister = () => {
  return useMutation({
    mutationFn: authApi.register,
  });
};

export const useGenerateOtp = () => {
  return useMutation({
    mutationFn: authApi.generateOtp,
  });
};

export const useVerifyOtp = () => {
  return useMutation({
    mutationFn: authApi.verifyOtp,
  });
};

export const useLoginWithOtp = () => {
  return useMutation({
    mutationFn: authApi.loginWithOtp,
  });
};

export const useUserRoles = (userId: string) => {
  return useQuery({
    queryKey: ['userRoles', userId],
    queryFn: () => authApi.getUserRoles(userId),
    enabled: !!userId,
  });
};

export const useAssignRole = () => {
  return useMutation({
    mutationFn: ({ userId, role }: { userId: string; role: string }) =>
      authApi.assignRole(userId, role),
  });
};

export const useRemoveRole = () => {
  return useMutation({
    mutationFn: ({ userId, role }: { userId: string; role: string }) =>
      authApi.removeRole(userId, role),
  });
};

export const useUserClaims = (userId: string) => {
  return useQuery({
    queryKey: ['userClaims', userId],
    queryFn: () => authApi.getUserClaims(userId),
    enabled: !!userId,
  });
};

export const useAddClaim = () => {
  return useMutation({
    mutationFn: ({ userId, claimType, claimValue }: { userId: string; claimType: string; claimValue: string }) =>
      authApi.addClaim(userId, claimType, claimValue),
  });
};
