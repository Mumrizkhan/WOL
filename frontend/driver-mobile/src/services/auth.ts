import { useMutation } from '@tanstack/react-query';
import httpClient from './http';
import type {
  LoginRequest,
  LoginResponse,
  GenerateOtpRequest,
  VerifyOtpRequest,
  OtpLoginRequest,
} from './types';

export const authApi = {
  login: async (data: LoginRequest): Promise<LoginResponse> => {
    const response = await httpClient.post<LoginResponse>('/identity/login', data);
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
};

export const useLogin = () => {
  return useMutation({
    mutationFn: authApi.login,
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
