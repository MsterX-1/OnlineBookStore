// src/api/userApi.js
import api from './axiosConfig';

export const userApi = {
  getAllUsers: () => api.get('/User/GetAllUsers'),
  getUserById: (id) => api.get(`/User/GetUserById/${id}`),
  register: (data) => api.post('/User/Register', data),
  login: (data) => api.post('/User/Login', data),
  logout: (userId) => api.post(`/User/Logout/${userId}`),
  updateUser: (data) => api.put('/User/UpdateUser', data),
  changePassword: (data) => api.put('/User/ChangePassword', data),
  deleteUser: (id) => api.delete(`/User/DeleteUser/${id}`),
};