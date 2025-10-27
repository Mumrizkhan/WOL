import React from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  ActivityIndicator,
  Alert,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { jobsApi } from '../../lib/api';

export default function JobDetailsScreen({ route, navigation }: any) {
  const { jobId } = route.params;
  const queryClient = useQueryClient();

  const { data: job, isLoading } = useQuery({
    queryKey: ['job', jobId],
    queryFn: () => jobsApi.getById(jobId),
  });

  const acceptMutation = useMutation({
    mutationFn: () => jobsApi.accept(jobId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['jobs'] });
      Alert.alert('Success', 'Job accepted successfully!', [
        { text: 'OK', onPress: () => navigation.goBack() },
      ]);
    },
    onError: (error: any) => {
      Alert.alert('Error', error.response?.data?.message || 'Failed to accept job');
    },
  });

  if (isLoading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#8b5cf6" />
      </View>
    );
  }

  const jobData = job?.data;

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <TouchableOpacity onPress={() => navigation.goBack()} style={styles.backButton}>
          <Ionicons name="arrow-back" size={24} color="#111827" />
        </TouchableOpacity>
        <Text style={styles.title}>Job Details</Text>
        <View style={{ width: 40 }} />
      </View>

      <ScrollView style={styles.content} showsVerticalScrollIndicator={false}>
        {/* Earning Card */}
        <View style={styles.earningCard}>
          <Text style={styles.earningLabel}>You'll Earn</Text>
          <Text style={styles.earningAmount}>SAR {jobData?.totalAmount?.toFixed(2)}</Text>
          <Text style={styles.earningSubtext}>{jobData?.bookingType} Booking</Text>
        </View>

        {/* Route Details */}
        <View style={styles.card}>
          <Text style={styles.cardTitle}>Route Details</Text>
          <View style={styles.locationContainer}>
            <View style={styles.locationRow}>
              <View style={styles.locationIconContainer}>
                <Ionicons name="location" size={20} color="#10b981" />
              </View>
              <View style={styles.locationInfo}>
                <Text style={styles.locationLabel}>Pickup Location</Text>
                <Text style={styles.locationText}>{jobData?.pickupLocation}</Text>
              </View>
            </View>
            <View style={styles.locationLine} />
            <View style={styles.locationRow}>
              <View style={[styles.locationIconContainer, { backgroundColor: '#fee2e2' }]}>
                <Ionicons name="location" size={20} color="#ef4444" />
              </View>
              <View style={styles.locationInfo}>
                <Text style={styles.locationLabel}>Dropoff Location</Text>
                <Text style={styles.locationText}>{jobData?.dropoffLocation}</Text>
              </View>
            </View>
          </View>
        </View>

        {/* Job Information */}
        <View style={styles.card}>
          <Text style={styles.cardTitle}>Job Information</Text>
          <View style={styles.infoRow}>
            <View style={styles.infoItem}>
              <Ionicons name="calendar-outline" size={20} color="#6b7280" />
              <View style={styles.infoContent}>
                <Text style={styles.infoLabel}>Pickup Date</Text>
                <Text style={styles.infoValue}>
                  {jobData?.pickupDate ? new Date(jobData.pickupDate).toLocaleDateString() : 'N/A'}
                </Text>
              </View>
            </View>
            <View style={styles.infoItem}>
              <Ionicons name="time-outline" size={20} color="#6b7280" />
              <View style={styles.infoContent}>
                <Text style={styles.infoLabel}>Pickup Time</Text>
                <Text style={styles.infoValue}>{jobData?.pickupTime || 'N/A'}</Text>
              </View>
            </View>
          </View>
          <View style={styles.infoRow}>
            <View style={styles.infoItem}>
              <Ionicons name="cube-outline" size={20} color="#6b7280" />
              <View style={styles.infoContent}>
                <Text style={styles.infoLabel}>Vehicle Type</Text>
                <Text style={styles.infoValue}>{jobData?.vehicleType || 'N/A'}</Text>
              </View>
            </View>
            <View style={styles.infoItem}>
              <Ionicons name="scale-outline" size={20} color="#6b7280" />
              <View style={styles.infoContent}>
                <Text style={styles.infoLabel}>Load Weight</Text>
                <Text style={styles.infoValue}>{jobData?.loadWeight || 'N/A'} kg</Text>
              </View>
            </View>
          </View>
          <View style={styles.infoRow}>
            <View style={styles.infoItem}>
              <Ionicons name="navigate-outline" size={20} color="#6b7280" />
              <View style={styles.infoContent}>
                <Text style={styles.infoLabel}>Distance</Text>
                <Text style={styles.infoValue}>
                  {jobData?.distance ? `${jobData.distance} km` : 'N/A'}
                </Text>
              </View>
            </View>
            <View style={styles.infoItem}>
              <Ionicons name="time-outline" size={20} color="#6b7280" />
              <View style={styles.infoContent}>
                <Text style={styles.infoLabel}>Est. Duration</Text>
                <Text style={styles.infoValue}>
                  {jobData?.estimatedDuration || 'N/A'}
                </Text>
              </View>
            </View>
          </View>
        </View>

        {/* Load Description */}
        {jobData?.loadDescription && (
          <View style={styles.card}>
            <Text style={styles.cardTitle}>Load Description</Text>
            <Text style={styles.description}>{jobData.loadDescription}</Text>
          </View>
        )}

        {/* Special Instructions */}
        {jobData?.specialInstructions && (
          <View style={styles.card}>
            <Text style={styles.cardTitle}>Special Instructions</Text>
            <Text style={styles.description}>{jobData.specialInstructions}</Text>
          </View>
        )}

        {/* Customer Info */}
        <View style={styles.card}>
          <Text style={styles.cardTitle}>Customer Information</Text>
          <View style={styles.customerInfo}>
            <View style={styles.customerAvatar}>
              <Ionicons name="person" size={24} color="#8b5cf6" />
            </View>
            <View style={styles.customerDetails}>
              <Text style={styles.customerName}>Customer #{jobData?.customerId?.slice(0, 8)}</Text>
              <Text style={styles.customerType}>{jobData?.customerType || 'Individual'}</Text>
            </View>
          </View>
        </View>

        {/* Accept Button */}
        {jobData?.status === 'Pending' && (
          <TouchableOpacity
            style={[styles.acceptButton, acceptMutation.isPending && styles.acceptButtonDisabled]}
            onPress={() => acceptMutation.mutate()}
            disabled={acceptMutation.isPending}
          >
            <Ionicons name="checkmark-circle" size={24} color="#fff" />
            <Text style={styles.acceptButtonText}>
              {acceptMutation.isPending ? 'Accepting...' : 'Accept This Job'}
            </Text>
          </TouchableOpacity>
        )}
      </ScrollView>
    </View>
  );
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
  earningCard: {
    backgroundColor: '#8b5cf6',
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
  earningLabel: {
    fontSize: 14,
    color: 'rgba(255, 255, 255, 0.8)',
    marginBottom: 8,
  },
  earningAmount: {
    fontSize: 36,
    fontWeight: 'bold',
    color: '#fff',
    marginBottom: 4,
  },
  earningSubtext: {
    fontSize: 14,
    color: 'rgba(255, 255, 255, 0.8)',
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
  description: {
    fontSize: 14,
    color: '#6b7280',
    lineHeight: 20,
  },
  customerInfo: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  customerAvatar: {
    width: 48,
    height: 48,
    borderRadius: 24,
    backgroundColor: '#f3e8ff',
    alignItems: 'center',
    justifyContent: 'center',
  },
  customerDetails: {
    marginLeft: 12,
  },
  customerName: {
    fontSize: 16,
    fontWeight: '600',
    color: '#111827',
  },
  customerType: {
    fontSize: 14,
    color: '#6b7280',
    marginTop: 2,
  },
  acceptButton: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: '#8b5cf6',
    padding: 16,
    borderRadius: 12,
    marginBottom: 32,
    gap: 8,
  },
  acceptButtonDisabled: {
    opacity: 0.6,
  },
  acceptButtonText: {
    color: '#fff',
    fontSize: 18,
    fontWeight: '600',
  },
});
