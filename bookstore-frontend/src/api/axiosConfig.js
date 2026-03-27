// src/api/axiosConfig.js
import axios from 'axios';
import { getToken, setToken, clearToken } from './tokenManager';

const api = axios.create({
  baseURL: process.env.REACT_APP_API_URL || 'https://localhost:7069/api',
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true, // Important for cookies
});

let isRefreshing = false;
let failedQueue = [];

const processQueue = (error, token = null) => {
  failedQueue.forEach((prom) => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token);
    }
  });
  
  isRefreshing = false;
  failedQueue = [];
};

// Add request interceptor for auth token
api.interceptors.request.use(
  (config) => {
    const token = getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Add response interceptor for token refresh and error handling
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // If the failing request was the login or register endpoint, don't trigger refresh flow
    // (prevents redirect/reload that hides the login error message)
    if (originalRequest.url.includes('/Auth/Login') || originalRequest.url.includes('/Auth/Register')) {
      return Promise.reject(error);
    }

    // Don't try to refresh the refresh token endpoint itself
    if (originalRequest.url.includes('/Auth/RefreshToken')) {
      clearToken();
      localStorage.removeItem('user');
      // Refresh endpoint itself returned 401 or failed — force user to login
      window.location.assign('/login');
      return Promise.reject(error);
    }

    // Handle 401 errors with automatic token refresh
    if (error.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        // Another request is already refreshing, queue this one
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        })
          .then((token) => {
            originalRequest.headers.Authorization = `Bearer ${token}`;
            return api(originalRequest);
          })
          .catch((err) => {
            return Promise.reject(err);
          });
      }

      originalRequest._retry = true;
      isRefreshing = true;

      try {
        const response = await axios.post(
          `${process.env.REACT_APP_API_URL || 'https://localhost:7069/api'}/Auth/RefreshToken`,
          {},
          { withCredentials: true }
        );

        const newToken = response.data.token;
        const expiresOn = response.data.tokenExpiresOn;
        
        // Store token in memory (secure, not in localStorage)
        setToken(newToken, expiresOn);
        
        api.defaults.headers.common.Authorization = `Bearer ${newToken}`;
        originalRequest.headers.Authorization = `Bearer ${newToken}`;

        processQueue(null, newToken);
        return api(originalRequest);
      } catch (refreshError) {
        processQueue(refreshError, null);
        clearToken();
        localStorage.removeItem('user');
        // Refresh failed (expired/invalid refresh token) — force login
        window.location.assign('/login');
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);

export default api;