import React, { useState, useEffect } from 'react';
import { View, Text, StyleSheet, TouchableOpacity, Dimensions } from 'react-native';
import MapView, { Marker, Polyline } from 'react-native-maps';
import { Ionicons } from '@expo/vector-icons';
import * as Location from 'expo-location';

const { width, height } = Dimensions.get('window');

export default function TrackingScreen({ navigation }: any) {
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
    })();
  }, []);

  const pickupLocation = { latitude: 24.7136, longitude: 46.6753 };
  const dropoffLocation = { latitude: 24.7736, longitude: 46.7353 };
  const driverLocation = { latitude: 24.7436, longitude: 46.7053 };

  return (
    <View style={styles.container}>
      <MapView
        style={styles.map}
        region={region}
        showsUserLocation
        showsMyLocationButton
      >
        {/* Pickup Marker */}
        <Marker coordinate={pickupLocation} pinColor="#10b981">
          <View style={styles.markerContainer}>
            <View style={[styles.marker, { backgroundColor: '#10b981' }]}>
              <Ionicons name="location" size={20} color="#fff" />
            </View>
          </View>
        </Marker>

        {/* Dropoff Marker */}
        <Marker coordinate={dropoffLocation} pinColor="#ef4444">
          <View style={styles.markerContainer}>
            <View style={[styles.marker, { backgroundColor: '#ef4444' }]}>
              <Ionicons name="location" size={20} color="#fff" />
            </View>
          </View>
        </Marker>

        {/* Driver Marker */}
        <Marker coordinate={driverLocation}>
          <View style={styles.driverMarker}>
            <Ionicons name="car" size={24} color="#fff" />
          </View>
        </Marker>

        {/* Route Line */}
        <Polyline
          coordinates={[pickupLocation, driverLocation, dropoffLocation]}
          strokeColor="#0ea5e9"
          strokeWidth={3}
        />
      </MapView>

      {/* Header */}
      <View style={styles.header}>
        <TouchableOpacity onPress={() => navigation.goBack()} style={styles.backButton}>
          <Ionicons name="arrow-back" size={24} color="#111827" />
        </TouchableOpacity>
        <Text style={styles.title}>Live Tracking</Text>
        <TouchableOpacity style={styles.refreshButton}>
          <Ionicons name="refresh" size={24} color="#111827" />
        </TouchableOpacity>
      </View>

      {/* Driver Info Card */}
      <View style={styles.driverCard}>
        <View style={styles.driverInfo}>
          <View style={styles.driverAvatar}>
            <Ionicons name="person" size={24} color="#fff" />
          </View>
          <View style={styles.driverDetails}>
            <Text style={styles.driverName}>Ahmed Al-Rashid</Text>
            <Text style={styles.driverVehicle}>Toyota Hilux • ABC-1234</Text>
            <View style={styles.ratingContainer}>
              <Ionicons name="star" size={14} color="#f59e0b" />
              <Text style={styles.rating}>4.8</Text>
            </View>
          </View>
        </View>
        <View style={styles.driverActions}>
          <TouchableOpacity style={styles.actionButton}>
            <Ionicons name="call" size={20} color="#0ea5e9" />
          </TouchableOpacity>
          <TouchableOpacity style={styles.actionButton}>
            <Ionicons name="chatbubble" size={20} color="#0ea5e9" />
          </TouchableOpacity>
        </View>
      </View>

      {/* Status Card */}
      <View style={styles.statusCard}>
        <View style={styles.statusItem}>
          <Ionicons name="time-outline" size={20} color="#6b7280" />
          <View style={styles.statusInfo}>
            <Text style={styles.statusLabel}>ETA</Text>
            <Text style={styles.statusValue}>15 mins</Text>
          </View>
        </View>
        <View style={styles.statusDivider} />
        <View style={styles.statusItem}>
          <Ionicons name="navigate-outline" size={20} color="#6b7280" />
          <View style={styles.statusInfo}>
            <Text style={styles.statusLabel}>Distance</Text>
            <Text style={styles.statusValue}>5.2 km</Text>
          </View>
        </View>
        <View style={styles.statusDivider} />
        <View style={styles.statusItem}>
          <Ionicons name="speedometer-outline" size={20} color="#6b7280" />
          <View style={styles.statusInfo}>
            <Text style={styles.statusLabel}>Status</Text>
            <Text style={[styles.statusValue, { color: '#10b981' }]}>On Route</Text>
          </View>
        </View>
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
  refreshButton: {
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
  driverMarker: {
    width: 48,
    height: 48,
    borderRadius: 24,
    backgroundColor: '#0ea5e9',
    alignItems: 'center',
    justifyContent: 'center',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.3,
    shadowRadius: 4,
    elevation: 4,
  },
  driverCard: {
    position: 'absolute',
    bottom: 120,
    left: 16,
    right: 16,
    backgroundColor: '#fff',
    borderRadius: 16,
    padding: 16,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.1,
    shadowRadius: 8,
    elevation: 4,
  },
  driverInfo: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
  },
  driverAvatar: {
    width: 48,
    height: 48,
    borderRadius: 24,
    backgroundColor: '#0ea5e9',
    alignItems: 'center',
    justifyContent: 'center',
  },
  driverDetails: {
    marginLeft: 12,
    flex: 1,
  },
  driverName: {
    fontSize: 16,
    fontWeight: '600',
    color: '#111827',
  },
  driverVehicle: {
    fontSize: 12,
    color: '#6b7280',
    marginTop: 2,
  },
  ratingContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    marginTop: 4,
  },
  rating: {
    fontSize: 12,
    fontWeight: '600',
    color: '#111827',
    marginLeft: 4,
  },
  driverActions: {
    flexDirection: 'row',
    gap: 8,
  },
  actionButton: {
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: '#eff6ff',
    alignItems: 'center',
    justifyContent: 'center',
  },
  statusCard: {
    position: 'absolute',
    bottom: 32,
    left: 16,
    right: 16,
    backgroundColor: '#fff',
    borderRadius: 16,
    padding: 16,
    flexDirection: 'row',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.1,
    shadowRadius: 8,
    elevation: 4,
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
});
