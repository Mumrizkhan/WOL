import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import httpClient from './http';
import type {
  CreateBookingRequest,
  CreateBookingResponse,
  Booking,
} from './types';

export const bookingsApi = {
  getAll: async (): Promise<Booking[]> => {
    const response = await httpClient.get<Booking[]>('/bookings');
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

  assignDriver: async (bookingId: string, driverId: string, vehicleId: string): Promise<void> => {
    await httpClient.post(`/bookings/${bookingId}/assign`, { driverId, vehicleId });
  },

  cancel: async (bookingId: string, reason: string): Promise<void> => {
    await httpClient.post(`/bookings/${bookingId}/cancel`, { reason });
  },

  complete: async (bookingId: string): Promise<void> => {
    await httpClient.post(`/bookings/${bookingId}/complete`);
  },

  updateStatus: async (bookingId: string, status: string): Promise<void> => {
    await httpClient.put(`/bookings/${bookingId}/status`, { status });
  },
};

export const useBookings = () => {
  return useQuery({
    queryKey: ['bookings'],
    queryFn: bookingsApi.getAll,
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
      queryClient.invalidateQueries({ queryKey: ['bookings'] });
    },
  });
};

export const useAssignDriver = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ bookingId, driverId, vehicleId }: { bookingId: string; driverId: string; vehicleId: string }) =>
      bookingsApi.assignDriver(bookingId, driverId, vehicleId),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['bookings'] });
      queryClient.invalidateQueries({ queryKey: ['booking', variables.bookingId] });
    },
  });
};

export const useCancelBooking = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ bookingId, reason }: { bookingId: string; reason: string }) =>
      bookingsApi.cancel(bookingId, reason),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['bookings'] });
      queryClient.invalidateQueries({ queryKey: ['booking', variables.bookingId] });
    },
  });
};

export const useCompleteBooking = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: bookingsApi.complete,
    onSuccess: (_, bookingId) => {
      queryClient.invalidateQueries({ queryKey: ['bookings'] });
      queryClient.invalidateQueries({ queryKey: ['booking', bookingId] });
    },
  });
};

export const useUpdateBookingStatus = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ bookingId, status }: { bookingId: string; status: string }) =>
      bookingsApi.updateStatus(bookingId, status),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['bookings'] });
      queryClient.invalidateQueries({ queryKey: ['booking', variables.bookingId] });
    },
  });
};
