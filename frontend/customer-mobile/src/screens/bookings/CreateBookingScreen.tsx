import React, { useState } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TextInput,
  TouchableOpacity,
  Alert,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { bookingsApi } from '../../lib/api';

export default function CreateBookingScreen({ navigation }: any) {
  const queryClient = useQueryClient();
  const [formData, setFormData] = useState({
    bookingType: 'OneWay',
    pickupLocation: '',
    dropoffLocation: '',
    pickupDate: '',
    pickupTime: '',
    vehicleType: 'Truck',
    loadWeight: '',
    loadDescription: '',
    specialInstructions: '',
  });

  const createMutation = useMutation({
    mutationFn: (data: any) => bookingsApi.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['my-bookings'] });
      Alert.alert('Success', 'Booking created successfully!', [
        { text: 'OK', onPress: () => navigation.goBack() },
      ]);
    },
    onError: (error: any) => {
      Alert.alert('Error', error.response?.data?.message || 'Failed to create booking');
    },
  });

  const handleSubmit = () => {
    if (!formData.pickupLocation || !formData.dropoffLocation || !formData.pickupDate) {
      Alert.alert('Error', 'Please fill in all required fields');
      return;
    }

    createMutation.mutate({
      ...formData,
      loadWeight: parseFloat(formData.loadWeight) || 0,
    });
  };

  const bookingTypes = [
    { id: 'OneWay', label: 'One-Way', icon: 'arrow-forward' },
    { id: 'Backload', label: 'Backload', icon: 'swap-horizontal' },
    { id: 'SharedLoad', label: 'Shared', icon: 'people' },
  ];

  const vehicleTypes = ['Truck', 'Van', 'Pickup', 'Trailer'];

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <TouchableOpacity onPress={() => navigation.goBack()} style={styles.backButton}>
          <Ionicons name="arrow-back" size={24} color="#111827" />
        </TouchableOpacity>
        <Text style={styles.title}>New Booking</Text>
        <View style={{ width: 40 }} />
      </View>

      <ScrollView style={styles.content} showsVerticalScrollIndicator={false}>
        {/* Booking Type */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Booking Type</Text>
          <View style={styles.typeContainer}>
            {bookingTypes.map((type) => (
              <TouchableOpacity
                key={type.id}
                style={[
                  styles.typeButton,
                  formData.bookingType === type.id && styles.typeButtonActive,
                ]}
                onPress={() => setFormData({ ...formData, bookingType: type.id })}
              >
                <Ionicons
                  name={type.icon as any}
                  size={24}
                  color={formData.bookingType === type.id ? '#fff' : '#6b7280'}
                />
                <Text
                  style={[
                    styles.typeButtonText,
                    formData.bookingType === type.id && styles.typeButtonTextActive,
                  ]}
                >
                  {type.label}
                </Text>
              </TouchableOpacity>
            ))}
          </View>
        </View>

        {/* Locations */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Route Details</Text>
          <View style={styles.inputContainer}>
            <Ionicons name="location" size={20} color="#10b981" style={styles.inputIcon} />
            <TextInput
              style={styles.input}
              placeholder="Pickup Location *"
              value={formData.pickupLocation}
              onChangeText={(text) => setFormData({ ...formData, pickupLocation: text })}
            />
          </View>
          <View style={styles.inputContainer}>
            <Ionicons name="location" size={20} color="#ef4444" style={styles.inputIcon} />
            <TextInput
              style={styles.input}
              placeholder="Dropoff Location *"
              value={formData.dropoffLocation}
              onChangeText={(text) => setFormData({ ...formData, dropoffLocation: text })}
            />
          </View>
        </View>

        {/* Schedule */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Schedule</Text>
          <View style={styles.row}>
            <View style={[styles.inputContainer, { flex: 1 }]}>
              <Ionicons name="calendar-outline" size={20} color="#6b7280" style={styles.inputIcon} />
              <TextInput
                style={styles.input}
                placeholder="Date (YYYY-MM-DD) *"
                value={formData.pickupDate}
                onChangeText={(text) => setFormData({ ...formData, pickupDate: text })}
              />
            </View>
            <View style={[styles.inputContainer, { flex: 1, marginLeft: 12 }]}>
              <Ionicons name="time-outline" size={20} color="#6b7280" style={styles.inputIcon} />
              <TextInput
                style={styles.input}
                placeholder="Time (HH:MM)"
                value={formData.pickupTime}
                onChangeText={(text) => setFormData({ ...formData, pickupTime: text })}
              />
            </View>
          </View>
        </View>

        {/* Load Details */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Load Details</Text>
          <View style={styles.inputContainer}>
            <Ionicons name="cube-outline" size={20} color="#6b7280" style={styles.inputIcon} />
            <View style={styles.pickerContainer}>
              <Text style={styles.pickerLabel}>Vehicle Type:</Text>
              <View style={styles.vehicleTypes}>
                {vehicleTypes.map((type) => (
                  <TouchableOpacity
                    key={type}
                    style={[
                      styles.vehicleTypeButton,
                      formData.vehicleType === type && styles.vehicleTypeButtonActive,
                    ]}
                    onPress={() => setFormData({ ...formData, vehicleType: type })}
                  >
                    <Text
                      style={[
                        styles.vehicleTypeText,
                        formData.vehicleType === type && styles.vehicleTypeTextActive,
                      ]}
                    >
                      {type}
                    </Text>
                  </TouchableOpacity>
                ))}
              </View>
            </View>
          </View>
          <View style={styles.inputContainer}>
            <Ionicons name="scale-outline" size={20} color="#6b7280" style={styles.inputIcon} />
            <TextInput
              style={styles.input}
              placeholder="Load Weight (kg)"
              value={formData.loadWeight}
              onChangeText={(text) => setFormData({ ...formData, loadWeight: text })}
              keyboardType="numeric"
            />
          </View>
          <View style={styles.inputContainer}>
            <Ionicons name="document-text-outline" size={20} color="#6b7280" style={styles.inputIcon} />
            <TextInput
              style={styles.input}
              placeholder="Load Description"
              value={formData.loadDescription}
              onChangeText={(text) => setFormData({ ...formData, loadDescription: text })}
              multiline
            />
          </View>
        </View>

        {/* Special Instructions */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Special Instructions</Text>
          <View style={[styles.inputContainer, styles.textAreaContainer]}>
            <TextInput
              style={[styles.input, styles.textArea]}
              placeholder="Any special requirements or instructions..."
              value={formData.specialInstructions}
              onChangeText={(text) => setFormData({ ...formData, specialInstructions: text })}
              multiline
              numberOfLines={4}
            />
          </View>
        </View>

        {/* Submit Button */}
        <TouchableOpacity
          style={[styles.submitButton, createMutation.isPending && styles.submitButtonDisabled]}
          onPress={handleSubmit}
          disabled={createMutation.isPending}
        >
          <Text style={styles.submitButtonText}>
            {createMutation.isPending ? 'Creating Booking...' : 'Create Booking'}
          </Text>
        </TouchableOpacity>
      </ScrollView>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f9fafb',
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
  section: {
    marginBottom: 24,
  },
  sectionTitle: {
    fontSize: 16,
    fontWeight: '600',
    color: '#111827',
    marginBottom: 12,
  },
  typeContainer: {
    flexDirection: 'row',
    gap: 12,
  },
  typeButton: {
    flex: 1,
    backgroundColor: '#fff',
    borderRadius: 12,
    padding: 16,
    alignItems: 'center',
    borderWidth: 2,
    borderColor: '#e5e7eb',
  },
  typeButtonActive: {
    backgroundColor: '#0ea5e9',
    borderColor: '#0ea5e9',
  },
  typeButtonText: {
    fontSize: 14,
    fontWeight: '600',
    color: '#6b7280',
    marginTop: 8,
  },
  typeButtonTextActive: {
    color: '#fff',
  },
  inputContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#fff',
    borderRadius: 12,
    paddingHorizontal: 12,
    marginBottom: 12,
    borderWidth: 1,
    borderColor: '#e5e7eb',
  },
  inputIcon: {
    marginRight: 8,
  },
  input: {
    flex: 1,
    height: 48,
    fontSize: 16,
    color: '#111827',
  },
  row: {
    flexDirection: 'row',
  },
  pickerContainer: {
    flex: 1,
    paddingVertical: 8,
  },
  pickerLabel: {
    fontSize: 14,
    color: '#6b7280',
    marginBottom: 8,
  },
  vehicleTypes: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: 8,
  },
  vehicleTypeButton: {
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: 8,
    backgroundColor: '#f9fafb',
    borderWidth: 1,
    borderColor: '#e5e7eb',
  },
  vehicleTypeButtonActive: {
    backgroundColor: '#0ea5e9',
    borderColor: '#0ea5e9',
  },
  vehicleTypeText: {
    fontSize: 12,
    fontWeight: '600',
    color: '#6b7280',
  },
  vehicleTypeTextActive: {
    color: '#fff',
  },
  textAreaContainer: {
    alignItems: 'flex-start',
    minHeight: 120,
  },
  textArea: {
    height: 100,
    textAlignVertical: 'top',
    paddingTop: 12,
  },
  submitButton: {
    backgroundColor: '#0ea5e9',
    borderRadius: 12,
    padding: 16,
    alignItems: 'center',
    marginBottom: 32,
  },
  submitButtonDisabled: {
    opacity: 0.6,
  },
  submitButtonText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: '600',
  },
});
