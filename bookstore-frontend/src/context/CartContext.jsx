// src/context/CartContext.jsx
import React, { createContext, useState, useContext, useEffect } from 'react';
import { cartApi } from '../api/cartApi';
import { bookApi } from '../api/bookApi';
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
      // Safely fetch latest cart (handle 404 when cart doesn't exist yet)
      let latestCart = [];
      try {
        const cartResp = await cartApi.getCart(user.userid);
        latestCart = cartResp.data || [];
      } catch (err) {
        if (err.response?.status === 404) {
          latestCart = [];
        } else {
          throw err;
        }
      }

      // Safely fetch book to get stock info
      let stockQty = Infinity;
      try {
        const bookResp = await bookApi.getBookByISBN(isbn);
        stockQty = bookResp?.data?.stockQty ?? Infinity;
      } catch (err) {
        console.error('Error fetching book data:', err);
        // If book lookup fails, fall back to allowing the add and let server-side enforce stock
      }

      const existing = latestCart.find(ci => ci.isbn === isbn);
      const existingQty = existing?.quantity || 0;

      const allowed = stockQty - existingQty;
      if (allowed <= 0) {
        toast.error(`Only ${stockQty} copies available`);
        return;
      }

      const toAdd = Math.min(quantity, allowed);

      if (toAdd < quantity) {
        toast.error(`Only ${stockQty} copies available; adding ${toAdd} instead`);
      }

      await cartApi.addToCart({
        customerId: user.userid,
        isbn,
        quantity: toAdd,
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
      // Fetch latest cart to get item details; handle missing cart
      let latestCart = [];
      try {
        const cartResp = await cartApi.getCart(user.userid);
        latestCart = cartResp.data || [];
      } catch (err) {
        if (err.response?.status === 404) {
          toast.error('Cart not found');
          return;
        }
        throw err;
      }

      const item = latestCart.find(ci => ci.cartId === cartId);
      if (!item) {
        toast.error('Cart item not found');
        return;
      }

      // Fetch book stock info
      let stockQty = Infinity;
      try {
        const bookResp = await bookApi.getBookByISBN(item.isbn);
        stockQty = bookResp?.data?.stockQty ?? Infinity;
      } catch (err) {
        console.error('Error fetching book data:', err);
      }

      if (quantity > stockQty) {
        toast.error(`Only ${stockQty} copies available; setting to ${stockQty}`);
        quantity = stockQty;
      }

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