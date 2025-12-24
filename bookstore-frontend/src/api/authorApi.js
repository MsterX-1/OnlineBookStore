// src/api/authorApi.js
import api from './axiosConfig';

export const authorApi = {
  getAllAuthors: () => api.get('/Author/GetAllAuthors'),
  getAuthorById: (id) => api.get(`/Author/GetAuthorById/${id}`),
  createAuthor: (data) => api.post('/Author/CreateAuthor', data),
  updateAuthor: (data) => api.put('/Author/updateauthor', data),
  deleteAuthor: (id) => api.delete(`/Author/DeleteAuthor/${id}`),
};