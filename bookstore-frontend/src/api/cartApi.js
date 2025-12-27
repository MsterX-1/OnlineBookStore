// src/api/cartApi.js
import api from './axiosConfig';

export const cartApi = {
  getCart: (customerId) => api.get(`/ShoppingCart/GetCart/${customerId}`),
  addToCart: (data) => api.post('/ShoppingCart/AddToCart', data),
  updateCartItem: (data) => api.put('/ShoppingCart/UpdateCartItem', data),
  deleteCartItem: (cartId) => api.delete(`/ShoppingCart/DeleteCart/${cartId}`),
  clearCart: (customerId) => api.delete(`/ShoppingCart/ClearCart/${customerId}`),
};