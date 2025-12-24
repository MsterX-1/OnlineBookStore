// src/context/CartContext.jsx
import React, { createContext, useState, useContext, useEffect } from 'react';
import { cartApi } from '../api/cartApi';
import { useAuth } from './AuthContext';
import toast from 'react-hot-toast';

const CartContext = createContext();

export const useCart = () => {
  const context = useContext(CartContext);
  if (!context) {
    throw new Error('useCart must be used within CartProvider');
  }
  return context;
};

export const CartProvider = ({ children }) => {
  const [cartItems, setCartItems] = useState([]);
  const [loading, setLoading] = useState(false);
  const { user } = useAuth();

  useEffect(() => {
    if (user?.userid) {
      fetchCart();
    } else {
      setCartItems([]);
    }
  }, [user]);

  const fetchCart = async () => {
    if (!user?.userid) return;
    
    try {
      setLoading(true);
      const response = await cartApi.getCart(user.userid);
      setCartItems(response.data || []);
    } catch (error) {
      if (error.response?.status !== 404) {
        console.error('Error fetching cart:', error);
      }
      setCartItems([]);
    } finally {
      setLoading(false);
    }
  };

  const addToCart = async (isbn, quantity = 1) => {
    if (!user?.userid) {
      toast.error('Please login to add items to cart');
      return;
    }

    try {
      await cartApi.addToCart({
        customerId: user.userid,
        isbn,
        quantity,
      });
      await fetchCart();
      toast.success('Item added to cart');
    } catch (error) {
      const message = error.response?.data?.message || 'Failed to add item to cart';
      toast.error(message);
      throw error;
    }
  };

  const updateCartItem = async (cartId, quantity) => {
    if (quantity < 1) {
      toast.error('Quantity must be at least 1');
      return;
    }

    try {
      await cartApi.updateCartItem({ cartId, quantity });
      await fetchCart();
      toast.success('Cart updated');
    } catch (error) {
      const message = error.response?.data?.message || 'Failed to update cart';
      toast.error(message);
      throw error;
    }
  };

  const removeFromCart = async (cartId) => {
    try {
      await cartApi.deleteCartItem(cartId);
      await fetchCart();
      toast.success('Item removed from cart');
    } catch (error) {
      const message = error.response?.data?.message || 'Failed to remove item';
      toast.error(message);
      throw error;
    }
  };

  const clearCart = async () => {
    if (!user?.userid) return;

    try {
      await cartApi.clearCart(user.userid);
      setCartItems([]);
      // Don't show toast here since it's called during checkout
    } catch (error) {
      const message = error.response?.data?.message || 'Failed to clear cart';
      toast.error(message);
      throw error;
    }
  };

  // Function to manually clear cart in frontend only (after successful order placement)
  const clearCartLocal = () => {
    setCartItems([]);
  };

  const getCartTotal = () => {
    return cartItems.reduce((total, item) => total + (item.totalPrice || 0), 0);
  };

  const getCartCount = () => {
    return cartItems.reduce((count, item) => count + item.quantity, 0);
  };

  const value = {
    cartItems,
    loading,
    fetchCart,
    addToCart,
    updateCartItem,
    removeFromCart,
    clearCart,
    clearCartLocal,
    getCartTotal,
    getCartCount,
  };

  return <CartContext.Provider value={value}>{children}</CartContext.Provider>;
};