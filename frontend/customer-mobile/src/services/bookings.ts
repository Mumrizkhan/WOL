import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import httpClient from './http';
import type {
  CreateBookingRequest,
  CreateBookingResponse,
  Booking,
} from './types';

export const bookingsApi = {
  getMyBookings: async (): Promise<Booking[]> => {
    const response = await httpClient.get<Booking[]>('/bookings/my');
    return response.data;
  },

  getById: async (id: string): Promise<Booking> => {
    const response = await httpClient.get<Booking>(`/bookings/${id}`);
    return response.data;
  },

  create: async (data: CreateBookingRequest): Promise<CreateBookingResponse> => {
    const response = await httpClient.post<CreateBookingResponse>('/bookings', data);
    return response.data;
  },

  cancel: async (bookingId: string, reason: string): Promise<void> => {
    await httpClient.post(`/bookings/${bookingId}/cancel`, { reason });
  },
};

export const useMyBookings = () => {
  return useQuery({
    queryKey: ['myBookings'],
    queryFn: bookingsApi.getMyBookings,
  });
};

export const useBooking = (id: string) => {
  return useQuery({
    queryKey: ['booking', id],
    queryFn: () => bookingsApi.getById(id),
    enabled: !!id,
  });
};

export const useCreateBooking = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: bookingsApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['myBookings'] });
    },
  });
};

export const useCancelBooking = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ bookingId, reason }: { bookingId: string; reason: string }) =>
      bookingsApi.cancel(bookingId, reason),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['myBookings'] });
      queryClient.invalidateQueries({ queryKey: ['booking', variables.bookingId] });
    },
  });
};
