import React, { useState } from 'react';
import {
  View,
  Text,
  StyleSheet,
  FlatList,
  TouchableOpacity,
  RefreshControl,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { useQuery } from '@tanstack/react-query';
import { bookingsApi } from '../../lib/api';

export default function BookingsScreen({ navigation }: any) {
  const [filter, setFilter] = useState('all');

  const { data: bookings, isLoading, refetch } = useQuery({
    queryKey: ['my-bookings'],
    queryFn: () => bookingsApi.getMyBookings(),
  });

  const filteredBookings = bookings?.data?.filter((booking: any) => {
    if (filter === 'all') return true;
    if (filter === 'active') {
      return ['Pending', 'Confirmed', 'InProgress'].includes(booking.status);
    }
    if (filter === 'completed') return booking.status === 'Completed';
    if (filter === 'cancelled') return booking.status === 'Cancelled';
    return true;
  }) || [];

  const filters = [
    { id: 'all', label: 'All', icon: 'list' },
    { id: 'active', label: 'Active', icon: 'time' },
    { id: 'completed', label: 'Completed', icon: 'checkmark-circle' },
    { id: 'cancelled', label: 'Cancelled', icon: 'close-circle' },
  ];

  const renderBookingItem = ({ item }: any) => (
    <TouchableOpacity
      style={styles.bookingCard}
      onPress={() => navigation.navigate('BookingDetails', { bookingId: item.id })}
    >
      <View style={styles.bookingHeader}>
        <View>
          <Text style={styles.bookingNumber}>{item.bookingNumber}</Text>
          <Text style={styles.bookingType}>{item.bookingType}</Text>
        </View>
        <View style={[styles.statusBadge, { backgroundColor: getStatusColor(item.status) }]}>
          <Text style={styles.statusText}>{item.status}</Text>
        </View>
      </View>

      <View style={styles.locationContainer}>
        <View style={styles.locationRow}>
          <View style={styles.locationDot} />
          <Text style={styles.locationText} numberOfLines={1}>
            {item.pickupLocation}
          </Text>
        </View>
        <View style={styles.locationLine} />
        <View style={styles.locationRow}>
          <View style={[styles.locationDot, { backgroundColor: '#ef4444' }]} />
          <Text style={styles.locationText} numberOfLines={1}>
            {item.dropoffLocation}
          </Text>
        </View>
      </View>

      <View style={styles.bookingFooter}>
        <View style={styles.dateContainer}>
          <Ionicons name="calendar-outline" size={16} color="#6b7280" />
          <Text style={styles.dateText}>
            {new Date(item.pickupDate).toLocaleDateString()}
          </Text>
        </View>
        <Text style={styles.amount}>SAR {item.totalAmount?.toFixed(2)}</Text>
      </View>
    </TouchableOpacity>
  );

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <Text style={styles.title}>My Bookings</Text>
        <TouchableOpacity
          style={styles.addButton}
          onPress={() => navigation.navigate('CreateBooking')}
        >
          <Ionicons name="add" size={24} color="#fff" />
        </TouchableOpacity>
      </View>

      <View style={styles.filterContainer}>
        {filters.map((f) => (
          <TouchableOpacity
            key={f.id}
            style={[styles.filterButton, filter === f.id && styles.filterButtonActive]}
            onPress={() => setFilter(f.id)}
          >
            <Ionicons
              name={f.icon as any}
              size={20}
              color={filter === f.id ? '#fff' : '#6b7280'}
            />
            <Text
              style={[styles.filterText, filter === f.id && styles.filterTextActive]}
            >
              {f.label}
            </Text>
          </TouchableOpacity>
        ))}
      </View>

      <FlatList
        data={filteredBookings}
        renderItem={renderBookingItem}
        keyExtractor={(item) => item.id}
        contentContainerStyle={styles.listContent}
        refreshControl={
          <RefreshControl refreshing={isLoading} onRefresh={refetch} />
        }
        ListEmptyComponent={
          <View style={styles.emptyContainer}>
            <Ionicons name="cube-outline" size={64} color="#d1d5db" />
            <Text style={styles.emptyText}>No bookings found</Text>
            <TouchableOpacity
              style={styles.createButton}
              onPress={() => navigation.navigate('CreateBooking')}
            >
              <Text style={styles.createButtonText}>Create Booking</Text>
            </TouchableOpacity>
          </View>
        }
      />
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

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f9fafb',
  },
  header: {
    backgroundColor: '#fff',
    padding: 24,
    paddingTop: 60,
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    borderBottomWidth: 1,
    borderBottomColor: '#f3f4f6',
  },
  title: {
    fontSize: 28,
    fontWeight: 'bold',
    color: '#111827',
  },
  addButton: {
    width: 48,
    height: 48,
    borderRadius: 24,
    backgroundColor: '#0ea5e9',
    alignItems: 'center',
    justifyContent: 'center',
  },
  filterContainer: {
    flexDirection: 'row',
    padding: 16,
    gap: 8,
    backgroundColor: '#fff',
  },
  filterButton: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 8,
    paddingHorizontal: 12,
    borderRadius: 12,
    backgroundColor: '#f9fafb',
    gap: 4,
  },
  filterButtonActive: {
    backgroundColor: '#0ea5e9',
  },
  filterText: {
    fontSize: 12,
    fontWeight: '600',
    color: '#6b7280',
  },
  filterTextActive: {
    color: '#fff',
  },
  listContent: {
    padding: 16,
  },
  bookingCard: {
    backgroundColor: '#fff',
    borderRadius: 16,
    padding: 16,
    marginBottom: 12,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 2,
  },
  bookingHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    marginBottom: 16,
  },
  bookingNumber: {
    fontSize: 18,
    fontWeight: '600',
    color: '#111827',
  },
  bookingType: {
    fontSize: 14,
    color: '#6b7280',
    marginTop: 2,
  },
  statusBadge: {
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: 12,
  },
  statusText: {
    color: '#fff',
    fontSize: 12,
    fontWeight: '600',
  },
  locationContainer: {
    marginBottom: 16,
  },
  locationRow: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  locationDot: {
    width: 12,
    height: 12,
    borderRadius: 6,
    backgroundColor: '#10b981',
  },
  locationLine: {
    width: 2,
    height: 20,
    backgroundColor: '#e5e7eb',
    marginLeft: 5,
    marginVertical: 4,
  },
  locationText: {
    flex: 1,
    marginLeft: 12,
    fontSize: 14,
    color: '#374151',
  },
  bookingFooter: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingTop: 16,
    borderTopWidth: 1,
    borderTopColor: '#f3f4f6',
  },
  dateContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 6,
  },
  dateText: {
    fontSize: 14,
    color: '#6b7280',
  },
  amount: {
    fontSize: 18,
    fontWeight: 'bold',
    color: '#0ea5e9',
  },
  emptyContainer: {
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 64,
  },
  emptyText: {
    fontSize: 16,
    color: '#9ca3af',
    marginTop: 16,
    marginBottom: 24,
  },
  createButton: {
    backgroundColor: '#0ea5e9',
    paddingHorizontal: 24,
    paddingVertical: 12,
    borderRadius: 12,
  },
  createButtonText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: '600',
  },
});
