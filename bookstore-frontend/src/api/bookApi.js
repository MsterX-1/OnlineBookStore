// src/api/bookApi.js
import api from './axiosConfig';

export const bookApi = {
  getAllBooks: () => api.get('/Book/GetAllBooks'),
  getBookByISBN: (isbn) => api.get(`/Book/GetBookByISBN/${isbn}`),
  getBooksByCategory: (category) => api.get(`/Book/GetBooksByCategory/${category}`),
  searchBooksByTitle: (title) => api.get(`/Book/SearchBooksByTitle/${title}`),
  getLowStockBooks: () => api.get('/Book/GetLowStockBooks'),
  searchBooksAdvanced: (data) => api.post('/Book/SearchBooksAdvanced', data),
  createBook: (data) => api.post('/Book/CreateBook', data),
  updateBook: (data) => api.put('/Book/UpdateBook', data),
  deleteBook: (isbn) => api.delete(`/Book/DeleteBook/${isbn}`),
  addBookAuthors: (data) => api.post('/Book/AddBookAuthors', data),
  removeBookAuthors: (data) => api.delete('/Book/RemoveBookAuthors', { data }),
  uploadBookPhoto: (formData) => api.post('/Book/UploadBookPhoto', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  }),
};