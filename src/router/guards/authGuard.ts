import type { NavigationGuardNext, RouteLocationNormalized } from 'vue-router';
import useStore from '@src/store/store';
import authService from '@src/services/authService';

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
    if (!store.accessToken) {
      next({ path: loginPath, query: { redirect: to.fullPath } });
      return;
    }

    try {
      // Verify token is still valid
      const isValid = await authService.verifyToken();
      if (!isValid) {
        store.clearTokens();
        next({ path: loginPath, query: { redirect: to.fullPath } });
        return;
      }
      next();
    } catch (error) {
      store.clearTokens();
      next({ path: loginPath, query: { redirect: to.fullPath } });
    }
  } else if (isAuthRoute && store.accessToken) {
    // If user is already logged in, redirect to home
    next('/');
  } else {
    next();
  }
};
