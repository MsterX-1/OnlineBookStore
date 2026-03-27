// src/api/authApi.js
import api from './axiosConfig';

export const authApi = {
  login: (credentials) => api.post('/Auth/Login', credentials),
  register: (data) => api.post('/Auth/Register', data),
  refreshToken: () => api.post('/Auth/RefreshToken'),
  logout: () => api.post('/Auth/Logout'),
  getCurrentUser: () => api.get('/Auth/me'),
};

export default api;
