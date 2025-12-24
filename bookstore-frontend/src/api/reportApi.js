// src/api/reportApi.js
import api from './axiosConfig';

export const reportApi = {
  getTotalSalesPreviousMonth: () => api.get('/Report/TotalSalesPreviousMonth'),
  getTotalSalesForDate: (date) => api.get(`/Report/TotalSalesForDate?date=${date}`),
  getTop5Customers: () => api.get('/Report/Top5Customers'),
  getTop10SellingBooks: () => api.get('/Report/Top10SellingBooks'),
  getBookOrderCount: (isbn) => api.get(`/Report/BookOrderCount/${isbn}`),
};