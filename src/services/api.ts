simport axios, { type AxiosInstance, type InternalAxiosRequestConfig } from 'axios';
import useStore from '../store/store';

const API_BASE_URL = import.meta.env.VITE_API_URL;

// Create axios instance
const api: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor
api.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const store = useStore();
    const token = store.token;

    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      const store = useStore();
      store.clearTokens();
    }

    return Promise.reject(error);
  }
);

export default api;
