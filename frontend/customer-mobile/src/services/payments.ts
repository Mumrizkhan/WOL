import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import httpClient from './http';
import type {
  ProcessPaymentRequest,
  ProcessPaymentResponse,
  Payment,
} from './types';

export const paymentsApi = {
  getByBooking: async (bookingId: string): Promise<Payment[]> => {
    const response = await httpClient.get<Payment[]>(`/payments/booking/${bookingId}`);
    return response.data;
  },

  processPayment: async (data: ProcessPaymentRequest): Promise<ProcessPaymentResponse> => {
    const response = await httpClient.post<ProcessPaymentResponse>('/payments', data);
    return response.data;
  },
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
      queryClient.invalidateQueries({ queryKey: ['myBookings'] });
    },
  });
};
