import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  Dimensions,
  Alert,
} from 'react-native';
import MapView, { Marker, Polyline } from 'react-native-maps';
import { Ionicons } from '@expo/vector-icons';
import * as Location from 'expo-location';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { jobsApi, trackingApi } from '../../lib/api';

const { width, height } = Dimensions.get('window');

export default function ActiveJobScreen({ route, navigation }: any) {
  const { jobId } = route.params;
  const queryClient = useQueryClient();
  const [location, setLocation] = useState<any>(null);
  const [region, setRegion] = useState({
    latitude: 24.7136,
    longitude: 46.6753,
    latitudeDelta: 0.0922,
    longitudeDelta: 0.0421,
  });

  useEffect(() => {
    (async () => {
      const { status } = await Location.requestForegroundPermissionsAsync();
      if (status !== 'granted') {
        return;
      }

      const currentLocation = await Location.getCurrentPositionAsync({});
      setLocation(currentLocation);
      setRegion({
        latitude: currentLocation.coords.latitude,
        longitude: currentLocation.coords.longitude,
        latitudeDelta: 0.0922,
        longitudeDelta: 0.0421,
      });

      Location.watchPositionAsync(
        {
          accuracy: Location.Accuracy.High,
          timeInterval: 10000,
          distanceInterval: 100,
        },
        (newLocation) => {
          setLocation(newLocation);
          trackingApi.updateLocation(
            jobId,
            newLocation.coords.latitude,
            newLocation.coords.longitude
          );
        }
      );
    })();
  }, [jobId]);

  const completeMutation = useMutation({
    mutationFn: () => jobsApi.complete(jobId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['jobs'] });
      Alert.alert('Success', 'Job completed successfully!', [
        { text: 'OK', onPress: () => navigation.navigate('MainTabs') },
      ]);
    },
  });

  const pickupLocation = { latitude: 24.7136, longitude: 46.6753 };
  const dropoffLocation = { latitude: 24.7736, longitude: 46.7353 };

  return (
    <View style={styles.container}>
      <MapView
        style={styles.map}
        region={region}
        showsUserLocation
        showsMyLocationButton
      >
        <Marker coordinate={pickupLocation} pinColor="#10b981">
          <View style={styles.markerContainer}>
            <View style={[styles.marker, { backgroundColor: '#10b981' }]}>
              <Ionicons name="location" size={20} color="#fff" />
            </View>
          </View>
        </Marker>

        <Marker coordinate={dropoffLocation} pinColor="#ef4444">
          <View style={styles.markerContainer}>
            <View style={[styles.marker, { backgroundColor: '#ef4444' }]}>
              <Ionicons name="location" size={20} color="#fff" />
            </View>
          </View>
        </Marker>

        {location && (
          <Polyline
            coordinates={[
              pickupLocation,
              {
                latitude: location.coords.latitude,
                longitude: location.coords.longitude,
              },
              dropoffLocation,
            ]}
            strokeColor="#8b5cf6"
            strokeWidth={3}
          />
        )}
      </MapView>

      {/* Header */}
      <View style={styles.header}>
        <TouchableOpacity onPress={() => navigation.goBack()} style={styles.backButton}>
          <Ionicons name="arrow-back" size={24} color="#111827" />
        </TouchableOpacity>
        <Text style={styles.title}>Active Job</Text>
        <TouchableOpacity style={styles.menuButton}>
          <Ionicons name="ellipsis-vertical" size={24} color="#111827" />
        </TouchableOpacity>
      </View>

      {/* Status Card */}
      <View style={styles.statusCard}>
        <View style={styles.statusRow}>
          <View style={styles.statusItem}>
            <Ionicons name="navigate-outline" size={20} color="#6b7280" />
            <View style={styles.statusInfo}>
              <Text style={styles.statusLabel}>Distance</Text>
              <Text style={styles.statusValue}>5.2 km</Text>
            </View>
          </View>
          <View style={styles.statusDivider} />
          <View style={styles.statusItem}>
            <Ionicons name="time-outline" size={20} color="#6b7280" />
            <View style={styles.statusInfo}>
              <Text style={styles.statusLabel}>ETA</Text>
              <Text style={styles.statusValue}>15 mins</Text>
            </View>
          </View>
          <View style={styles.statusDivider} />
          <View style={styles.statusItem}>
            <Ionicons name="wallet-outline" size={20} color="#6b7280" />
            <View style={styles.statusInfo}>
              <Text style={styles.statusLabel}>Earning</Text>
              <Text style={[styles.statusValue, { color: '#10b981' }]}>SAR 250</Text>
            </View>
          </View>
        </View>
      </View>

      {/* Action Buttons */}
      <View style={styles.actionContainer}>
        <TouchableOpacity style={styles.callButton}>
          <Ionicons name="call" size={24} color="#fff" />
          <Text style={styles.callButtonText}>Call Customer</Text>
        </TouchableOpacity>

        <TouchableOpacity
          style={[styles.completeButton, completeMutation.isPending && styles.completeButtonDisabled]}
          onPress={() => {
            Alert.alert(
              'Complete Job',
              'Are you sure you want to mark this job as completed?',
              [
                { text: 'Cancel', style: 'cancel' },
                { text: 'Complete', onPress: () => completeMutation.mutate() },
              ]
            );
          }}
          disabled={completeMutation.isPending}
        >
          <Ionicons name="checkmark-circle" size={24} color="#fff" />
          <Text style={styles.completeButtonText}>
            {completeMutation.isPending ? 'Completing...' : 'Complete Job'}
          </Text>
        </TouchableOpacity>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  map: {
    width,
    height,
  },
  header: {
    position: 'absolute',
    top: 0,
    left: 0,
    right: 0,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    padding: 16,
    paddingTop: 60,
    backgroundColor: 'rgba(255, 255, 255, 0.95)',
  },
  backButton: {
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: '#fff',
    alignItems: 'center',
    justifyContent: 'center',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 2,
  },
  title: {
    fontSize: 18,
    fontWeight: '600',
    color: '#111827',
  },
  menuButton: {
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: '#fff',
    alignItems: 'center',
    justifyContent: 'center',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 2,
  },
  markerContainer: {
    alignItems: 'center',
  },
  marker: {
    width: 36,
    height: 36,
    borderRadius: 18,
    alignItems: 'center',
    justifyContent: 'center',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.3,
    shadowRadius: 4,
    elevation: 4,
  },
  statusCard: {
    position: 'absolute',
    top: 140,
    left: 16,
    right: 16,
    backgroundColor: '#fff',
    borderRadius: 16,
    padding: 16,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.1,
    shadowRadius: 8,
    elevation: 4,
  },
  statusRow: {
    flexDirection: 'row',
  },
  statusItem: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
  },
  statusInfo: {
    marginLeft: 8,
  },
  statusLabel: {
    fontSize: 12,
    color: '#6b7280',
  },
  statusValue: {
    fontSize: 14,
    fontWeight: '600',
    color: '#111827',
    marginTop: 2,
  },
  statusDivider: {
    width: 1,
    backgroundColor: '#e5e7eb',
    marginHorizontal: 12,
  },
  actionContainer: {
    position: 'absolute',
    bottom: 32,
    left: 16,
    right: 16,
    gap: 12,
  },
  callButton: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: '#0ea5e9',
    padding: 16,
    borderRadius: 12,
    gap: 8,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.1,
    shadowRadius: 8,
    elevation: 4,
  },
  callButtonText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: '600',
  },
  completeButton: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: '#10b981',
    padding: 16,
    borderRadius: 12,
    gap: 8,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.1,
    shadowRadius: 8,
    elevation: 4,
  },
  completeButtonDisabled: {
    opacity: 0.6,
  },
  completeButtonText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: '600',
  },
});
