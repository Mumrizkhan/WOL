import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import httpClient from './http';
import type {
  ProcessPaymentRequest,
  ProcessPaymentResponse,
  Payment,
} from './types';

export const paymentsApi = {
  getAll: async (): Promise<Payment[]> => {
    const response = await httpClient.get<Payment[]>('/payments');
    return response.data;
  },

  getById: async (id: string): Promise<Payment> => {
    const response = await httpClient.get<Payment>(`/payments/${id}`);
    return response.data;
  },

  getByBooking: async (bookingId: string): Promise<Payment[]> => {
    const response = await httpClient.get<Payment[]>(`/payments/booking/${bookingId}`);
    return response.data;
  },

  processPayment: async (data: ProcessPaymentRequest): Promise<ProcessPaymentResponse> => {
    const response = await httpClient.post<ProcessPaymentResponse>('/payments', data);
    return response.data;
  },
};

export const usePayments = () => {
  return useQuery({
    queryKey: ['payments'],
    queryFn: paymentsApi.getAll,
  });
};

export const usePayment = (id: string) => {
  return useQuery({
    queryKey: ['payment', id],
    queryFn: () => paymentsApi.getById(id),
    enabled: !!id,
  });
};

export const useBookingPayments = (bookingId: string) => {
  return useQuery({
    queryKey: ['payments', 'booking', bookingId],
    queryFn: () => paymentsApi.getByBooking(bookingId),
    enabled: !!bookingId,
  });
};

export const useProcessPayment = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: paymentsApi.processPayment,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['payments'] });
      queryClient.invalidateQueries({ queryKey: ['bookings'] });
    },
  });
};
