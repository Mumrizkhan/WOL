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
import { jobsApi } from '../../lib/api';

export default function JobsScreen({ navigation }: any) {
  const [filter, setFilter] = useState('available');

  const { data: jobs, isLoading, refetch } = useQuery({
    queryKey: ['jobs', filter],
    queryFn: () => filter === 'available' ? jobsApi.getAvailable() : jobsApi.getMyJobs(),
  });

  const filters = [
    { id: 'available', label: 'Available', icon: 'search' },
    { id: 'my-jobs', label: 'My Jobs', icon: 'list' },
  ];

  const renderJobItem = ({ item }: any) => (
    <TouchableOpacity
      style={styles.jobCard}
      onPress={() => navigation.navigate('JobDetails', { jobId: item.id })}
    >
      <View style={styles.jobHeader}>
        <View>
          <Text style={styles.jobNumber}>{item.bookingNumber}</Text>
          <Text style={styles.jobType}>{item.bookingType}</Text>
        </View>
        <View style={styles.earningBadge}>
          <Text style={styles.earningText}>SAR {item.totalAmount?.toFixed(2)}</Text>
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

      <View style={styles.jobFooter}>
        <View style={styles.infoItem}>
          <Ionicons name="calendar-outline" size={16} color="#6b7280" />
          <Text style={styles.infoText}>
            {new Date(item.pickupDate).toLocaleDateString()}
          </Text>
        </View>
        <View style={styles.infoItem}>
          <Ionicons name="cube-outline" size={16} color="#6b7280" />
          <Text style={styles.infoText}>{item.vehicleType}</Text>
        </View>
        <View style={styles.infoItem}>
          <Ionicons name="navigate-outline" size={16} color="#6b7280" />
          <Text style={styles.infoText}>
            {item.distance ? `${item.distance} km` : 'N/A'}
          </Text>
        </View>
      </View>

      {filter === 'available' && (
        <TouchableOpacity style={styles.acceptButton}>
          <Text style={styles.acceptButtonText}>Accept Job</Text>
        </TouchableOpacity>
      )}

      {filter === 'my-jobs' && (
        <View style={[styles.statusBadge, { backgroundColor: getStatusColor(item.status) }]}>
          <Text style={styles.statusText}>{item.status}</Text>
        </View>
      )}
    </TouchableOpacity>
  );

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <Text style={styles.title}>Jobs</Text>
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
        data={jobs?.data || []}
        renderItem={renderJobItem}
        keyExtractor={(item) => item.id}
        contentContainerStyle={styles.listContent}
        refreshControl={
          <RefreshControl refreshing={isLoading} onRefresh={refetch} />
        }
        ListEmptyComponent={
          <View style={styles.emptyContainer}>
            <Ionicons name="briefcase-outline" size={64} color="#d1d5db" />
            <Text style={styles.emptyText}>
              {filter === 'available' ? 'No available jobs' : 'No jobs yet'}
            </Text>
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
    borderBottomWidth: 1,
    borderBottomColor: '#f3f4f6',
  },
  title: {
    fontSize: 28,
    fontWeight: 'bold',
    color: '#111827',
  },
  filterContainer: {
    flexDirection: 'row',
    padding: 16,
    gap: 12,
    backgroundColor: '#fff',
  },
  filterButton: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 12,
    paddingHorizontal: 16,
    borderRadius: 12,
    backgroundColor: '#f9fafb',
    gap: 8,
  },
  filterButtonActive: {
    backgroundColor: '#8b5cf6',
  },
  filterText: {
    fontSize: 14,
    fontWeight: '600',
    color: '#6b7280',
  },
  filterTextActive: {
    color: '#fff',
  },
  listContent: {
    padding: 16,
  },
  jobCard: {
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
  jobHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    marginBottom: 16,
  },
  jobNumber: {
    fontSize: 18,
    fontWeight: '600',
    color: '#111827',
  },
  jobType: {
    fontSize: 14,
    color: '#6b7280',
    marginTop: 2,
  },
  earningBadge: {
    backgroundColor: '#dcfce7',
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: 12,
  },
  earningText: {
    color: '#16a34a',
    fontSize: 16,
    fontWeight: 'bold',
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
  jobFooter: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    paddingTop: 12,
    borderTopWidth: 1,
    borderTopColor: '#f3f4f6',
    marginBottom: 12,
  },
  infoItem: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 4,
  },
  infoText: {
    fontSize: 12,
    color: '#6b7280',
  },
  acceptButton: {
    backgroundColor: '#8b5cf6',
    paddingVertical: 12,
    borderRadius: 12,
    alignItems: 'center',
  },
  acceptButtonText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: '600',
  },
  statusBadge: {
    paddingVertical: 8,
    borderRadius: 12,
    alignItems: 'center',
  },
  statusText: {
    color: '#fff',
    fontSize: 14,
    fontWeight: '600',
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
  },
});
