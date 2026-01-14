import { computed } from 'vue';
import { useRouter } from 'vue-router';
import useStore from '@src/store/store';
import authService from '@src/services/AuthService/authService';
import type { AuthResponse, LoginRequest, RegisterRequest } from '@src/services/AuthService/authService';
import type { Ref, ComputedRef } from 'vue';
import type { IUser } from '@src/types';

export const useAuth = () => {
  const store = useStore();
  const router = useRouter();

  const isAuthenticated = computed(() => store.isAuthenticated);
  const token = computed(() => store.token);

  const login = async (data: LoginRequest) => {
    try {
      await authService.login(data);
      await router.push('/');
    } catch (error) {
      console.error('Login failed:', error);
      throw error;
    }
  };


  const register = async (data: RegisterRequest) => {
    try {
      await authService.register(data);
      await router.push('/');
    } catch (error) {
      console.error('Registration failed:', error);
      throw error;
    }
  };

  const logout = async () => {
    try {
      await authService.logout();
      await router.push('/access/sign-in/');
    } catch (error) {
      console.error('Logout failed:', error);
    }
  };

  const checkAuth = async () => {
    if (store.token) {
      try {
        const isValid = await authService.verifyToken();
        if (!isValid) {
          console.log('Token verification failed.');
          return false;
        }
        return true;
      } catch (error) {
        console.log('Token verification error:', error);
        return false;
      }
    }
    return false;
  };

  return {
    isAuthenticated,
    token,
    login,
    register,
    logout,
    checkAuth,
  };
};
