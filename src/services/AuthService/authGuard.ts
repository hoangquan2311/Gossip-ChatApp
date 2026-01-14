import type { NavigationGuardNext, RouteLocationNormalized } from 'vue-router';
import useStore from '../../store/store';
import authService from '@src/services/AuthService/authService';
import { useAuth } from '@src/services/AuthService/useAuth';
import { clear } from 'node:console';

const loginPath = '/access/sign-in/';
const registerPath = '/access/sign-up/';

export const authGuard = async (
  to: RouteLocationNormalized,
  from: RouteLocationNormalized,
  next: NavigationGuardNext
) => {
  const store = useStore();
  const requiresAuth = to.matched.some((record) => record.meta.requiresAuth);
  const isAuthRoute = to.path === loginPath || to.path === registerPath;

  if (requiresAuth) {
    if (!store.token) {
      next({ path: loginPath});
      console.log('No token found, redirecting to login.');
      return;
    }

    try {
      // Verify token is still valid
      const isValid = await useAuth().checkAuth();
      if (!isValid) {
        next({ path: loginPath });
        console.log('Token invalid, redirecting to login.');
        return;
      }
      console.log('Token valid, proceeding to route.');
      next();
    } catch (error) {
      console.log('Error verifying token, redirecting to login.', error);
      next({ path: loginPath });
    }
  } else if (isAuthRoute && store.token) {
    // If user is already logged in, redirect to home
    next("/");
    console.log('Redirecting authenticated user away from auth route.');
  } else {
    console.log(`No auth required for this route, proceeding.`);
    next();
  }
};
