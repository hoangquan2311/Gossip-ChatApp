import api from './api';
import useStore from '@src/store/store';
import type { IUser } from '@src/types';

export interface LoginCredentials {
  email: string;
  password: string;
}

export interface RegisterData {
  email: string;
  displayName: string;
  password: string;
}

export interface AuthResponse {
  accessToken: string;
  user: IUser;
}

class AuthService {
  async login(credentials: LoginCredentials): Promise<AuthResponse> {
    const response = await api.post<AuthResponse>('/api/auth/login', credentials);
    const { accessToken, user } = response.data;

    const store = useStore();
    store.setTokens(accessToken);
    store.setUser(user);

    return response.data;
  }

  async register(data: RegisterData): Promise<AuthResponse> {
    const response = await api.post<AuthResponse>('/api/auth/register', data);
    const { accessToken, user } = response.data;

    const store = useStore();
    store.setTokens(accessToken);
    store.setUser(user);

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

  async getCurrentUser(): Promise<IUser> {
    const response = await api.get<IUser>('/api/auth/me');
    const store = useStore();
    store.setUser(response.data);
    return response.data;
  }

  async verifyToken(): Promise<boolean> {
    try {
      await this.getCurrentUser();
      return true;
    } catch (error) {
      return false;
    }
  }
}

export default new AuthService();
