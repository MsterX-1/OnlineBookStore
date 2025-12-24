// src/api/orderApi.js
import api from './axiosConfig';

export const orderApi = {
  getAllOrders: () => api.get('/Order/GetAllOrders'),
  getOrder: (orderId) => api.get(`/Order/GetOrder/${orderId}`),
  getOrdersByCustomer: (customerId) => api.get(`/Order/GetOrdersByCustomer/${customerId}`),
  getOrderDetails: (orderId) => api.get(`/Order/GetOrderDetails/${orderId}`),
  placeOrder: (data) => api.post('/Order/PlaceOrder', data), // ✅ Matches your PlaceOrder endpoint
};