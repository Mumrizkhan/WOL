import React from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  ActivityIndicator,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { useQuery } from '@tanstack/react-query';
import { bookingsApi } from '../../lib/api';

export default function BookingDetailsScreen({ route, navigation }: any) {
  const { bookingId } = route.params;

  const { data: booking, isLoading } = useQuery({
    queryKey: ['booking', bookingId],
    queryFn: () => bookingsApi.getById(bookingId),
  });

  if (isLoading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#0ea5e9" />
      </View>
    );
  }

  const bookingData = booking?.data;

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <TouchableOpacity onPress={() => navigation.goBack()} style={styles.backButton}>
          <Ionicons name="arrow-back" size={24} color="#111827" />
        </TouchableOpacity>
        <Text style={styles.title}>Booking Details</Text>
        <View style={{ width: 40 }} />
      </View>

      <ScrollView style={styles.content} showsVerticalScrollIndicator={false}>
        {/* Status Card */}
        <View style={styles.statusCard}>
          <View style={[styles.statusBadge, { backgroundColor: getStatusColor(bookingData?.status) }]}>
            <Text style={styles.statusText}>{bookingData?.status}</Text>
          </View>
          <Text style={styles.bookingNumber}>{bookingData?.bookingNumber}</Text>
          <Text style={styles.bookingType}>{bookingData?.bookingType} Booking</Text>
        </View>

        {/* Location Details */}
        <View style={styles.card}>
          <Text style={styles.cardTitle}>Route Details</Text>
          <View style={styles.locationContainer}>
            <View style={styles.locationRow}>
              <View style={styles.locationIconContainer}>
                <Ionicons name="location" size={20} color="#10b981" />
              </View>
              <View style={styles.locationInfo}>
                <Text style={styles.locationLabel}>Pickup Location</Text>
                <Text style={styles.locationText}>{bookingData?.pickupLocation}</Text>
              </View>
            </View>
            <View style={styles.locationLine} />
            <View style={styles.locationRow}>
              <View style={[styles.locationIconContainer, { backgroundColor: '#fee2e2' }]}>
                <Ionicons name="location" size={20} color="#ef4444" />
              </View>
              <View style={styles.locationInfo}>
                <Text style={styles.locationLabel}>Dropoff Location</Text>
                <Text style={styles.locationText}>{bookingData?.dropoffLocation}</Text>
              </View>
            </View>
          </View>
        </View>

        {/* Booking Information */}
        <View style={styles.card}>
          <Text style={styles.cardTitle}>Booking Information</Text>
          <View style={styles.infoRow}>
            <View style={styles.infoItem}>
              <Ionicons name="calendar-outline" size={20} color="#6b7280" />
              <View style={styles.infoContent}>
                <Text style={styles.infoLabel}>Pickup Date</Text>
                <Text style={styles.infoValue}>
                  {bookingData?.pickupDate ? new Date(bookingData.pickupDate).toLocaleDateString() : 'N/A'}
                </Text>
              </View>
            </View>
            <View style={styles.infoItem}>
              <Ionicons name="time-outline" size={20} color="#6b7280" />
              <View style={styles.infoContent}>
                <Text style={styles.infoLabel}>Pickup Time</Text>
                <Text style={styles.infoValue}>
                  {bookingData?.pickupTime || 'N/A'}
                </Text>
              </View>
            </View>
          </View>
          <View style={styles.infoRow}>
            <View style={styles.infoItem}>
              <Ionicons name="cube-outline" size={20} color="#6b7280" />
              <View style={styles.infoContent}>
                <Text style={styles.infoLabel}>Vehicle Type</Text>
                <Text style={styles.infoValue}>{bookingData?.vehicleType || 'N/A'}</Text>
              </View>
            </View>
            <View style={styles.infoItem}>
              <Ionicons name="scale-outline" size={20} color="#6b7280" />
              <View style={styles.infoContent}>
                <Text style={styles.infoLabel}>Load Weight</Text>
                <Text style={styles.infoValue}>{bookingData?.loadWeight || 'N/A'} kg</Text>
              </View>
            </View>
          </View>
        </View>

        {/* Payment Details */}
        <View style={styles.card}>
          <Text style={styles.cardTitle}>Payment Details</Text>
          <View style={styles.paymentRow}>
            <Text style={styles.paymentLabel}>Base Fare</Text>
            <Text style={styles.paymentValue}>SAR {bookingData?.baseFare?.toFixed(2) || '0.00'}</Text>
          </View>
          <View style={styles.paymentRow}>
            <Text style={styles.paymentLabel}>Distance Charge</Text>
            <Text style={styles.paymentValue}>SAR {bookingData?.distanceCharge?.toFixed(2) || '0.00'}</Text>
          </View>
          <View style={styles.paymentRow}>
            <Text style={styles.paymentLabel}>Additional Charges</Text>
            <Text style={styles.paymentValue}>SAR {bookingData?.additionalCharges?.toFixed(2) || '0.00'}</Text>
          </View>
          <View style={[styles.paymentRow, styles.totalRow]}>
            <Text style={styles.totalLabel}>Total Amount</Text>
            <Text style={styles.totalValue}>SAR {bookingData?.totalAmount?.toFixed(2) || '0.00'}</Text>
          </View>
          <View style={styles.paymentStatusContainer}>
            <Text style={styles.paymentStatusLabel}>Payment Status:</Text>
            <View style={[styles.paymentStatusBadge, { backgroundColor: getPaymentStatusColor(bookingData?.paymentStatus) }]}>
              <Text style={styles.paymentStatusText}>{bookingData?.paymentStatus || 'Pending'}</Text>
            </View>
          </View>
        </View>

        {/* Actions */}
        {bookingData?.status === 'InProgress' && (
          <TouchableOpacity
            style={styles.trackButton}
            onPress={() => navigation.navigate('Tracking', { bookingId })}
          >
            <Ionicons name="navigate" size={20} color="#fff" />
            <Text style={styles.trackButtonText}>Track Live</Text>
          </TouchableOpacity>
        )}

        {bookingData?.status === 'Pending' && (
          <TouchableOpacity style={styles.cancelButton}>
            <Ionicons name="close-circle-outline" size={20} color="#ef4444" />
            <Text style={styles.cancelButtonText}>Cancel Booking</Text>
          </TouchableOpacity>
        )}
      </ScrollView>
    </View>
  );
}

function getStatusColor(status: string) {
  const colors: any = {
    Pending: '#f59e0b',
    Confirmed: '#0ea5e9',
    InProgress: '#8b5cf6',
    Completed: '#10b981',
    Cancelled: '#ef4444',
  };
  return colors[status] || '#6b7280';
}

function getPaymentStatusColor(status: string) {
  const colors: any = {
    Paid: '#10b981',
    Pending: '#f59e0b',
    Failed: '#ef4444',
  };
  return colors[status] || '#6b7280';
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f9fafb',
  },
  loadingContainer: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
  },
  header: {
    backgroundColor: '#fff',
    padding: 16,
    paddingTop: 60,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    borderBottomWidth: 1,
    borderBottomColor: '#f3f4f6',
  },
  backButton: {
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: '#f9fafb',
    alignItems: 'center',
    justifyContent: 'center',
  },
  title: {
    fontSize: 18,
    fontWeight: '600',
    color: '#111827',
  },
  content: {
    flex: 1,
    padding: 16,
  },
  statusCard: {
    backgroundColor: '#fff',
    borderRadius: 16,
    padding: 24,
    alignItems: 'center',
    marginBottom: 16,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 2,
  },
  statusBadge: {
    paddingHorizontal: 16,
    paddingVertical: 8,
    borderRadius: 16,
    marginBottom: 12,
  },
  statusText: {
    color: '#fff',
    fontSize: 14,
    fontWeight: '600',
  },
  bookingNumber: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#111827',
    marginBottom: 4,
  },
  bookingType: {
    fontSize: 14,
    color: '#6b7280',
  },
  card: {
    backgroundColor: '#fff',
    borderRadius: 16,
    padding: 16,
    marginBottom: 16,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 2,
  },
  cardTitle: {
    fontSize: 18,
    fontWeight: '600',
    color: '#111827',
    marginBottom: 16,
  },
  locationContainer: {
    marginTop: 8,
  },
  locationRow: {
    flexDirection: 'row',
    alignItems: 'flex-start',
  },
  locationIconContainer: {
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: '#d1fae5',
    alignItems: 'center',
    justifyContent: 'center',
  },
  locationInfo: {
    flex: 1,
    marginLeft: 12,
  },
  locationLabel: {
    fontSize: 12,
    color: '#6b7280',
    marginBottom: 4,
  },
  locationText: {
    fontSize: 16,
    color: '#111827',
    fontWeight: '500',
  },
  locationLine: {
    width: 2,
    height: 24,
    backgroundColor: '#e5e7eb',
    marginLeft: 19,
    marginVertical: 8,
  },
  infoRow: {
    flexDirection: 'row',
    marginBottom: 16,
    gap: 12,
  },
  infoItem: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'flex-start',
  },
  infoContent: {
    flex: 1,
    marginLeft: 8,
  },
  infoLabel: {
    fontSize: 12,
    color: '#6b7280',
    marginBottom: 4,
  },
  infoValue: {
    fontSize: 14,
    color: '#111827',
    fontWeight: '500',
  },
  paymentRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginBottom: 12,
  },
  paymentLabel: {
    fontSize: 14,
    color: '#6b7280',
  },
  paymentValue: {
    fontSize: 14,
    color: '#111827',
    fontWeight: '500',
  },
  totalRow: {
    paddingTop: 12,
    borderTopWidth: 1,
    borderTopColor: '#f3f4f6',
    marginTop: 4,
  },
  totalLabel: {
    fontSize: 16,
    fontWeight: '600',
    color: '#111827',
  },
  totalValue: {
    fontSize: 18,
    fontWeight: 'bold',
    color: '#0ea5e9',
  },
  paymentStatusContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    marginTop: 12,
    paddingTop: 12,
    borderTopWidth: 1,
    borderTopColor: '#f3f4f6',
  },
  paymentStatusLabel: {
    fontSize: 14,
    color: '#6b7280',
  },
  paymentStatusBadge: {
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: 12,
  },
  paymentStatusText: {
    color: '#fff',
    fontSize: 12,
    fontWeight: '600',
  },
  trackButton: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: '#0ea5e9',
    padding: 16,
    borderRadius: 12,
    marginBottom: 12,
    gap: 8,
  },
  trackButtonText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: '600',
  },
  cancelButton: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: '#fff',
    padding: 16,
    borderRadius: 12,
    borderWidth: 1,
    borderColor: '#ef4444',
    gap: 8,
  },
  cancelButtonText: {
    color: '#ef4444',
    fontSize: 16,
    fontWeight: '600',
  },
});
