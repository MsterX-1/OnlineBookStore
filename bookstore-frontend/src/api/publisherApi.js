// src/api/publisherApi.js
import api from './axiosConfig';

export const publisherApi = {
  getAllPublishers: () => api.get('/Publisher/GetAllPublishers'),
  getPublisherById: (id) => api.get(`/Publisher/GetPublisher/${id}`),
  createPublisher: (data) => api.post('/Publisher/CreatePublisher', data),
  updatePublisher: (data) => api.put('/Publisher/UpdatePublisher', data),
  deletePublisher: (id) => api.delete(`/Publisher/DeletePublisher/${id}`),
};