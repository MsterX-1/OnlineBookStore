// src/api/publisherOrderApi.js
import api from './axiosConfig';

export const publisherOrderApi = {
  getAllPublisherOrders: () => api.get('/PublisherOrder/GetAllPublisherOrders'),
  getPendingOrders: () => api.get('/PublisherOrder/GetPendingOrders'),
  createPublisherOrder: (data) => api.post('/PublisherOrder/CreatePublisherOrder', data),
  confirmOrder: (data) => api.put('/PublisherOrder/ConfirmOrder', data),
};