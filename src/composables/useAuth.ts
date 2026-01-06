import { computed } from 'vue';
import { useRouter } from 'vue-router';
import useStore from '@src/store/store';
import authService from '@src/services/authService';
import type { AuthResponse, RegisterData } from '@src/services/authService';

export const useAuth = () => {
  const store = useStore();
  const router = useRouter();

  const isAuthenticated = computed(() => store.isAuthenticated);
  const user = computed(() => store.user);
  const accessToken = computed(() => store.accessToken);

  const login = async (email: string, password: string) => {
    try {
      await authService.login({ email, password });
      await router.push('/');
    } catch (error) {
      console.error('Login failed:', error);
      throw error;
    }
  };

  const register = async (payload: RegisterData) => {
    try {
      const response: AuthResponse = await authService.register(payload);
      await router.push('/');
      return response;
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
    if (store.accessToken) {
      try {
        const isValid = await authService.verifyToken();
        if (!isValid) {
          store.clearTokens();
          return false;
        }
        return true;
      } catch (error) {
        store.clearTokens();
        return false;
      }
    }
    return false;
  };

  return {
    isAuthenticated,
    user,
    accessToken,
    login,
    register,
    logout,
    checkAuth,
  };
};
