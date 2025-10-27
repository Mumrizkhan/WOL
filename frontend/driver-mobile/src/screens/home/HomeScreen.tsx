import React from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  Switch,
  Dimensions,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { useAuthStore } from '../../store/authStore';
import { useQuery } from '@tanstack/react-query';
import { jobsApi, earningsApi } from '../../lib/api';

const { width } = Dimensions.get('window');

export default function HomeScreen({ navigation }: any) {
  const { user, isOnline, setOnlineStatus } = useAuthStore();

  const { data: jobs } = useQuery({
    queryKey: ['my-jobs'],
    queryFn: () => jobsApi.getMyJobs(),
  });

  const { data: earnings } = useQuery({
    queryKey: ['earnings'],
    queryFn: () => earningsApi.getStats(),
  });

  const activeJob = jobs?.data?.find((j: any) => j.status === 'InProgress');
  const todayEarnings = earnings?.data?.today || 0;
  const weekEarnings = earnings?.data?.week || 0;
  const monthEarnings = earnings?.data?.month || 0;

  return (
    <ScrollView style={styles.container} showsVerticalScrollIndicator={false}>
      {/* Header */}
      <View style={styles.header}>
        <View>
          <Text style={styles.greeting}>Welcome back,</Text>
          <Text style={styles.userName}>{user?.firstName}</Text>
        </View>
        <TouchableOpacity style={styles.notificationButton}>
          <Ionicons name="notifications-outline" size={24} color="#fff" />
          <View style={styles.badge}>
            <Text style={styles.badgeText}>2</Text>
          </View>
        </TouchableOpacity>
      </View>

      {/* Online Status */}
      <View style={styles.statusCard}>
        <View style={styles.statusLeft}>
          <View style={[styles.statusIndicator, isOnline && styles.statusIndicatorOnline]} />
          <View>
            <Text style={styles.statusTitle}>
              {isOnline ? 'You are Online' : 'You are Offline'}
            </Text>
            <Text style={styles.statusSubtitle}>
              {isOnline ? 'Ready to accept jobs' : 'Go online to receive jobs'}
            </Text>
          </View>
        </View>
        <Switch
          value={isOnline}
          onValueChange={setOnlineStatus}
          trackColor={{ false: '#d1d5db', true: '#a78bfa' }}
          thumbColor={isOnline ? '#8b5cf6' : '#f3f4f6'}
        />
      </View>

      {/* Active Job */}
      {activeJob && (
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Active Job</Text>
          <TouchableOpacity
            style={styles.activeJobCard}
            onPress={() => navigation.navigate('ActiveJob', { jobId: activeJob.id })}
          >
            <View style={styles.activeJobHeader}>
              <Text style={styles.activeJobNumber}>{activeJob.bookingNumber}</Text>
              <View style={styles.activeJobBadge}>
                <Text style={styles.activeJobBadgeText}>In Progress</Text>
              </View>
            </View>
            <View style={styles.locationContainer}>
              <View style={styles.locationRow}>
                <Ionicons name="location" size={16} color="#10b981" />
                <Text style={styles.locationText} numberOfLines={1}>
                  {activeJob.pickupLocation}
                </Text>
              </View>
              <View style={styles.locationDivider} />
              <View style={styles.locationRow}>
                <Ionicons name="location" size={16} color="#ef4444" />
                <Text style={styles.locationText} numberOfLines={1}>
                  {activeJob.dropoffLocation}
                </Text>
              </View>
            </View>
            <TouchableOpacity style={styles.navigateButton}>
              <Ionicons name="navigate" size={20} color="#fff" />
              <Text style={styles.navigateButtonText}>Navigate</Text>
            </TouchableOpacity>
          </TouchableOpacity>
        </View>
      )}

      {/* Earnings */}
      <View style={styles.section}>
        <View style={styles.sectionHeader}>
          <Text style={styles.sectionTitle}>Earnings</Text>
          <TouchableOpacity onPress={() => navigation.navigate('Earnings')}>
            <Text style={styles.seeAll}>See All</Text>
          </TouchableOpacity>
        </View>
        <View style={styles.earningsContainer}>
          <View style={styles.earningCard}>
            <Ionicons name="calendar-outline" size={24} color="#8b5cf6" />
            <Text style={styles.earningValue}>SAR {todayEarnings.toFixed(2)}</Text>
            <Text style={styles.earningLabel}>Today</Text>
          </View>
          <View style={styles.earningCard}>
            <Ionicons name="calendar-outline" size={24} color="#0ea5e9" />
            <Text style={styles.earningValue}>SAR {weekEarnings.toFixed(2)}</Text>
            <Text style={styles.earningLabel}>This Week</Text>
          </View>
          <View style={styles.earningCard}>
            <Ionicons name="calendar-outline" size={24} color="#10b981" />
            <Text style={styles.earningValue}>SAR {monthEarnings.toFixed(2)}</Text>
            <Text style={styles.earningLabel}>This Month</Text>
          </View>
        </View>
      </View>

      {/* Quick Stats */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>Today's Stats</Text>
        <View style={styles.statsContainer}>
          <View style={styles.statCard}>
            <Ionicons name="checkmark-circle" size={32} color="#10b981" />
            <Text style={styles.statValue}>8</Text>
            <Text style={styles.statLabel}>Completed</Text>
          </View>
          <View style={styles.statCard}>
            <Ionicons name="time" size={32} color="#f59e0b" />
            <Text style={styles.statValue}>2</Text>
            <Text style={styles.statLabel}>Pending</Text>
          </View>
          <View style={styles.statCard}>
            <Ionicons name="speedometer" size={32} color="#8b5cf6" />
            <Text style={styles.statValue}>245</Text>
            <Text style={styles.statLabel}>KM Driven</Text>
          </View>
        </View>
      </View>

      {/* Available Jobs */}
      {isOnline && !activeJob && (
        <View style={styles.section}>
          <View style={styles.sectionHeader}>
            <Text style={styles.sectionTitle}>Available Jobs</Text>
            <TouchableOpacity onPress={() => navigation.navigate('Jobs')}>
              <Text style={styles.seeAll}>See All</Text>
            </TouchableOpacity>
          </View>
          <TouchableOpacity
            style={styles.availableJobsButton}
            onPress={() => navigation.navigate('Jobs')}
          >
            <Ionicons name="search" size={24} color="#8b5cf6" />
            <Text style={styles.availableJobsText}>Browse Available Jobs</Text>
            <Ionicons name="chevron-forward" size={24} color="#8b5cf6" />
          </TouchableOpacity>
        </View>
      )}
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f9fafb',
  },
  header: {
    backgroundColor: '#8b5cf6',
    padding: 24,
    paddingTop: 60,
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    borderBottomLeftRadius: 24,
    borderBottomRightRadius: 24,
  },
  greeting: {
    fontSize: 16,
    color: 'rgba(255, 255, 255, 0.8)',
  },
  userName: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#fff',
    marginTop: 4,
  },
  notificationButton: {
    width: 48,
    height: 48,
    borderRadius: 24,
    backgroundColor: 'rgba(255, 255, 255, 0.2)',
    alignItems: 'center',
    justifyContent: 'center',
  },
  badge: {
    position: 'absolute',
    top: 8,
    right: 8,
    backgroundColor: '#ef4444',
    width: 18,
    height: 18,
    borderRadius: 9,
    alignItems: 'center',
    justifyContent: 'center',
  },
  badgeText: {
    color: '#fff',
    fontSize: 10,
    fontWeight: 'bold',
  },
  statusCard: {
    backgroundColor: '#fff',
    margin: 16,
    marginTop: -24,
    borderRadius: 16,
    padding: 16,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 2,
  },
  statusLeft: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
  },
  statusIndicator: {
    width: 12,
    height: 12,
    borderRadius: 6,
    backgroundColor: '#d1d5db',
    marginRight: 12,
  },
  statusIndicatorOnline: {
    backgroundColor: '#10b981',
  },
  statusTitle: {
    fontSize: 16,
    fontWeight: '600',
    color: '#111827',
  },
  statusSubtitle: {
    fontSize: 12,
    color: '#6b7280',
    marginTop: 2,
  },
  section: {
    padding: 16,
  },
  sectionHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 16,
  },
  sectionTitle: {
    fontSize: 20,
    fontWeight: 'bold',
    color: '#111827',
  },
  seeAll: {
    fontSize: 14,
    color: '#8b5cf6',
    fontWeight: '600',
  },
  activeJobCard: {
    backgroundColor: '#fff',
    borderRadius: 16,
    padding: 16,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 2,
  },
  activeJobHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 16,
  },
  activeJobNumber: {
    fontSize: 18,
    fontWeight: '600',
    color: '#111827',
  },
  activeJobBadge: {
    backgroundColor: '#8b5cf6',
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: 12,
  },
  activeJobBadgeText: {
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
    marginBottom: 8,
  },
  locationText: {
    flex: 1,
    marginLeft: 8,
    fontSize: 14,
    color: '#6b7280',
  },
  locationDivider: {
    width: 2,
    height: 16,
    backgroundColor: '#e5e7eb',
    marginLeft: 7,
    marginBottom: 8,
  },
  navigateButton: {
    backgroundColor: '#8b5cf6',
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    padding: 12,
    borderRadius: 12,
    gap: 8,
  },
  navigateButtonText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: '600',
  },
  earningsContainer: {
    flexDirection: 'row',
    gap: 12,
  },
  earningCard: {
    flex: 1,
    backgroundColor: '#fff',
    borderRadius: 16,
    padding: 16,
    alignItems: 'center',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 2,
  },
  earningValue: {
    fontSize: 18,
    fontWeight: 'bold',
    color: '#111827',
    marginTop: 8,
  },
  earningLabel: {
    fontSize: 12,
    color: '#6b7280',
    marginTop: 4,
  },
  statsContainer: {
    flexDirection: 'row',
    gap: 12,
  },
  statCard: {
    flex: 1,
    backgroundColor: '#fff',
    borderRadius: 16,
    padding: 16,
    alignItems: 'center',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 2,
  },
  statValue: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#111827',
    marginTop: 8,
  },
  statLabel: {
    fontSize: 12,
    color: '#6b7280',
    marginTop: 4,
    textAlign: 'center',
  },
  availableJobsButton: {
    backgroundColor: '#fff',
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    padding: 16,
    borderRadius: 16,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 2,
  },
  availableJobsText: {
    flex: 1,
    fontSize: 16,
    fontWeight: '600',
    color: '#111827',
    marginLeft: 12,
  },
});
