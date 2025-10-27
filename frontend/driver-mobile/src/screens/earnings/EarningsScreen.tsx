import React, { useState } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  FlatList,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { useQuery } from '@tanstack/react-query';
import { earningsApi } from '../../lib/api';
import { BarChart } from 'react-native-chart-kit';
import { Dimensions } from 'react-native';

const screenWidth = Dimensions.get('window').width;

export default function EarningsScreen() {
  const [period, setPeriod] = useState('week');

  const { data: stats } = useQuery({
    queryKey: ['earnings-stats'],
    queryFn: () => earningsApi.getStats(),
  });

  const { data: history } = useQuery({
    queryKey: ['earnings-history'],
    queryFn: () => earningsApi.getHistory(),
  });

  const periods = [
    { id: 'week', label: 'Week' },
    { id: 'month', label: 'Month' },
    { id: 'year', label: 'Year' },
  ];

  const chartData = {
    labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
    datasets: [
      {
        data: [120, 150, 180, 140, 200, 250, 180],
      },
    ],
  };

  const renderHistoryItem = ({ item }: any) => (
    <View style={styles.historyItem}>
      <View style={styles.historyLeft}>
        <View style={styles.historyIcon}>
          <Ionicons name="checkmark-circle" size={24} color="#10b981" />
        </View>
        <View style={styles.historyInfo}>
          <Text style={styles.historyTitle}>{item.bookingNumber}</Text>
          <Text style={styles.historyDate}>
            {new Date(item.completedAt).toLocaleDateString()}
          </Text>
        </View>
      </View>
      <Text style={styles.historyAmount}>+SAR {item.amount?.toFixed(2)}</Text>
    </View>
  );

  return (
    <ScrollView style={styles.container} showsVerticalScrollIndicator={false}>
      {/* Header */}
      <View style={styles.header}>
        <Text style={styles.title}>Earnings</Text>
      </View>

      {/* Total Earnings Card */}
      <View style={styles.totalCard}>
        <Text style={styles.totalLabel}>Total Earnings</Text>
        <Text style={styles.totalAmount}>
          SAR {stats?.data?.total?.toFixed(2) || '0.00'}
        </Text>
        <View style={styles.totalStats}>
          <View style={styles.totalStatItem}>
            <Text style={styles.totalStatLabel}>This Month</Text>
            <Text style={styles.totalStatValue}>
              SAR {stats?.data?.month?.toFixed(2) || '0.00'}
            </Text>
          </View>
          <View style={styles.totalStatDivider} />
          <View style={styles.totalStatItem}>
            <Text style={styles.totalStatLabel}>This Week</Text>
            <Text style={styles.totalStatValue}>
              SAR {stats?.data?.week?.toFixed(2) || '0.00'}
            </Text>
          </View>
        </View>
      </View>

      {/* Quick Stats */}
      <View style={styles.statsContainer}>
        <View style={styles.statCard}>
          <Ionicons name="cube-outline" size={24} color="#8b5cf6" />
          <Text style={styles.statValue}>{stats?.data?.completedJobs || 0}</Text>
          <Text style={styles.statLabel}>Completed Jobs</Text>
        </View>
        <View style={styles.statCard}>
          <Ionicons name="trending-up-outline" size={24} color="#10b981" />
          <Text style={styles.statValue}>
            SAR {stats?.data?.avgPerJob?.toFixed(0) || 0}
          </Text>
          <Text style={styles.statLabel}>Avg per Job</Text>
        </View>
        <View style={styles.statCard}>
          <Ionicons name="star-outline" size={24} color="#f59e0b" />
          <Text style={styles.statValue}>{stats?.data?.rating?.toFixed(1) || '0.0'}</Text>
          <Text style={styles.statLabel}>Rating</Text>
        </View>
      </View>

      {/* Chart */}
      <View style={styles.chartContainer}>
        <View style={styles.chartHeader}>
          <Text style={styles.chartTitle}>Earnings Overview</Text>
          <View style={styles.periodSelector}>
            {periods.map((p) => (
              <TouchableOpacity
                key={p.id}
                style={[styles.periodButton, period === p.id && styles.periodButtonActive]}
                onPress={() => setPeriod(p.id)}
              >
                <Text
                  style={[styles.periodText, period === p.id && styles.periodTextActive]}
                >
                  {p.label}
                </Text>
              </TouchableOpacity>
            ))}
          </View>
        </View>
        <BarChart
          data={chartData}
          width={screenWidth - 48}
          height={220}
          yAxisLabel="SAR "
          chartConfig={{
            backgroundColor: '#fff',
            backgroundGradientFrom: '#fff',
            backgroundGradientTo: '#fff',
            decimalPlaces: 0,
            color: (opacity = 1) => `rgba(139, 92, 246, ${opacity})`,
            labelColor: (opacity = 1) => `rgba(107, 114, 128, ${opacity})`,
            style: {
              borderRadius: 16,
            },
            propsForBackgroundLines: {
              strokeDasharray: '',
              stroke: '#f3f4f6',
            },
          }}
          style={styles.chart}
        />
      </View>

      {/* History */}
      <View style={styles.historyContainer}>
        <Text style={styles.historyTitle}>Recent Transactions</Text>
        {history?.data?.slice(0, 10).map((item: any, index: number) => (
          <View key={index}>
            {renderHistoryItem({ item })}
          </View>
        ))}
      </View>
    </ScrollView>
  );
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
  totalCard: {
    backgroundColor: '#8b5cf6',
    margin: 16,
    borderRadius: 16,
    padding: 24,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 2,
  },
  totalLabel: {
    fontSize: 14,
    color: 'rgba(255, 255, 255, 0.8)',
    marginBottom: 8,
  },
  totalAmount: {
    fontSize: 36,
    fontWeight: 'bold',
    color: '#fff',
    marginBottom: 16,
  },
  totalStats: {
    flexDirection: 'row',
    paddingTop: 16,
    borderTopWidth: 1,
    borderTopColor: 'rgba(255, 255, 255, 0.2)',
  },
  totalStatItem: {
    flex: 1,
  },
  totalStatLabel: {
    fontSize: 12,
    color: 'rgba(255, 255, 255, 0.8)',
    marginBottom: 4,
  },
  totalStatValue: {
    fontSize: 18,
    fontWeight: '600',
    color: '#fff',
  },
  totalStatDivider: {
    width: 1,
    backgroundColor: 'rgba(255, 255, 255, 0.2)',
    marginHorizontal: 16,
  },
  statsContainer: {
    flexDirection: 'row',
    paddingHorizontal: 16,
    gap: 12,
    marginBottom: 16,
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
    fontSize: 20,
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
  chartContainer: {
    backgroundColor: '#fff',
    marginHorizontal: 16,
    marginBottom: 16,
    borderRadius: 16,
    padding: 16,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 2,
  },
  chartHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 16,
  },
  chartTitle: {
    fontSize: 18,
    fontWeight: '600',
    color: '#111827',
  },
  periodSelector: {
    flexDirection: 'row',
    backgroundColor: '#f9fafb',
    borderRadius: 8,
    padding: 2,
  },
  periodButton: {
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: 6,
  },
  periodButtonActive: {
    backgroundColor: '#fff',
  },
  periodText: {
    fontSize: 12,
    fontWeight: '600',
    color: '#6b7280',
  },
  periodTextActive: {
    color: '#8b5cf6',
  },
  chart: {
    marginVertical: 8,
    borderRadius: 16,
  },
  historyContainer: {
    backgroundColor: '#fff',
    marginHorizontal: 16,
    marginBottom: 32,
    borderRadius: 16,
    padding: 16,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 2,
  },
  historyTitle: {
    fontSize: 18,
    fontWeight: '600',
    color: '#111827',
    marginBottom: 16,
  },
  historyItem: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingVertical: 12,
    borderBottomWidth: 1,
    borderBottomColor: '#f3f4f6',
  },
  historyLeft: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
  },
  historyIcon: {
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: '#d1fae5',
    alignItems: 'center',
    justifyContent: 'center',
  },
  historyInfo: {
    marginLeft: 12,
    flex: 1,
  },
  historyTitle: {
    fontSize: 14,
    fontWeight: '600',
    color: '#111827',
  },
  historyDate: {
    fontSize: 12,
    color: '#6b7280',
    marginTop: 2,
  },
  historyAmount: {
    fontSize: 16,
    fontWeight: 'bold',
    color: '#10b981',
  },
});
