import { create } from 'zustand';
import AsyncStorage from '@react-native-async-storage/async-storage';

interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  userType: 'Individual' | 'Commercial' | 'ServiceProvider';
  phoneNumber: string;
}

interface AuthState {
  token: string | null;
  user: User | null;
  isAuthenticated: boolean;
  login: (token: string, user: User) => Promise<void>;
  logout: () => Promise<void>;
  loadAuth: () => Promise<void>;
}

export const useAuthStore = create<AuthState>((set) => ({
  token: null,
  user: null,
  isAuthenticated: false,
  
  login: async (token: string, user: User) => {
    await AsyncStorage.setItem('token', token);
    await AsyncStorage.setItem('user', JSON.stringify(user));
    set({ token, user, isAuthenticated: true });
  },
  
  logout: async () => {
    await AsyncStorage.removeItem('token');
    await AsyncStorage.removeItem('user');
    set({ token: null, user: null, isAuthenticated: false });
  },
  
  loadAuth: async () => {
    const token = await AsyncStorage.getItem('token');
    const userStr = await AsyncStorage.getItem('user');
    if (token && userStr) {
      const user = JSON.parse(userStr);
      set({ token, user, isAuthenticated: true });
    }
  },
}));
