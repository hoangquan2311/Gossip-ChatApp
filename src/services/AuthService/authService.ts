import api from '../api';
import useStore from '@src/store/store';
import type { IUser } from '@src/types';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  displayName: string;
  password: string;
}

export interface AuthResponse {
  userId: string;
  email: string;
  displayName: string;
  avatarUrl?: string;
  token: string;
}

class AuthService {
  async login(credentials: LoginRequest): Promise<AuthResponse> {
    const response = await api.post<AuthResponse>('/api/auth/login', credentials);
    const { userId, email, displayName, avatarUrl, token } = response.data;

    const store = useStore();
    store.setToken(token);
    store.setUser({ userId, email, displayName, avatarUrl});

    return response.data;
  }

  async register(data: RegisterRequest): Promise<AuthResponse> {
    const response = await api.post<AuthResponse>('/api/auth/register', data);
    const { userId, email, displayName, avatarUrl, token } = response.data;

    const store = useStore();
    store.setToken(token);
    store.setUser({ userId, email, displayName, avatarUrl});

    return response.data;
  }

  async logout(): Promise<void> {
    try {
      const store = useStore();
      await api.post('/api/auth/logout');
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      const store = useStore();
      store.clearTokens();
    }
  }

  async verifyToken(): Promise<boolean> {
    try {
      if(!useStore().token) return false;
      const response = await api.get<IUser>('/api/auth/verify-me');
      if (response.status !== 200) {
        useStore().clearTokens();
        return false;
      }
      useStore().setUser(response.data);
      return true;
    } catch (error) {
      useStore().clearTokens();
      return false;
    }
  }
}

export default new AuthService();
