// src/context/AuthContext.jsx
import React, { createContext, useState, useContext, useEffect } from 'react';
import { authApi } from '../api/authApi';
import { userApi } from '../api/userApi';
import { setToken, clearToken, getToken } from '../api/tokenManager';
import toast from 'react-hot-toast';

const AuthContext = createContext();

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return context;
};

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  // Initialize auth state from refresh token cookie on component mount
  useEffect(() => {
    const initializeAuth = async () => {
      try {
        const storedUser = localStorage.getItem('user');

        // Only attempt to refresh token if user was previously logged in
        if (storedUser) {
          try {
            const response = await authApi.refreshToken();
            const { token, tokenExpiresOn } = response.data;

            // Store token in memory only (not in localStorage)
            setToken(token, tokenExpiresOn);

            // Fetch user info with the new token
            const userResponse = await authApi.getCurrentUser();
            setUser(userResponse.data);
            setIsAuthenticated(true);
            localStorage.setItem('user', JSON.stringify(userResponse.data));
          } catch (refreshError) {
            // Refresh token invalid or expired, clear auth state
            clearToken();
            localStorage.removeItem('user');
            setUser(null);
            setIsAuthenticated(false);
          }
        } else {
          // No previous session, just set loading to false
          setUser(null);
          setIsAuthenticated(false);
        }
      } catch (error) {
        console.error('Auth initialization error:', error);
        clearToken();
        localStorage.removeItem('user');
        setUser(null);
        setIsAuthenticated(false);
      } finally {
        setLoading(false);
      }
    };

    initializeAuth();
  }, []);

  const login = async (credentials) => {
    try {
      const response = await authApi.login(credentials);
      const { token, tokenExpiresOn } = response.data;

      // Store JWT token in memory only (secure against XSS)
      setToken(token, tokenExpiresOn);

      // Get user info from the me endpoint
      const userResponse = await authApi.getCurrentUser();
      const userData = userResponse.data;

      setUser(userData);
      setIsAuthenticated(true);
      localStorage.setItem('user', JSON.stringify(userData));

      toast.success('Login successful!');
      return userData;
    } catch (error) {
      const message = error.response?.data || 'Login failed';
      toast.error(message);
      setUser(null);
      setIsAuthenticated(false);
      clearToken();
      throw error;
    }
  };

  const register = async (data) => {
    try {
      const response = await authApi.register(data);
      const { token, tokenExpiresOn } = response.data;

      // Store JWT token in memory only (secure against XSS)
      setToken(token, tokenExpiresOn);

      // Get user info from the me endpoint
      const userResponse = await authApi.getCurrentUser();
      const userData = userResponse.data;

      setUser(userData);
      setIsAuthenticated(true);
      localStorage.setItem('user', JSON.stringify(userData));

      toast.success('Registration successful!');
      return userData;
    } catch (error) {
      const message = error.response?.data || 'Registration failed';
      toast.error(message);
      setUser(null);
      setIsAuthenticated(false);
      clearToken();
      throw error;
    }
  };

  const logout = async () => {
    try {
      // Call logout endpoint to revoke tokens server-side
      await authApi.logout();
    } catch (error) {
      console.error('Logout API error:', error);
      // Continue logout even if API call fails
    } finally {
      // Clear all auth state
      setUser(null);
      setIsAuthenticated(false);
      clearToken(); // Clear token from memory
      localStorage.removeItem('user');
      toast.success('Logged out successfully');
    }
  };

  const updateProfile = async (data) => {
    try {
      await userApi.updateUser(data);
      const updatedUser = { ...user, ...data };
      setUser(updatedUser);
      localStorage.setItem('user', JSON.stringify(updatedUser));
      toast.success('Profile updated successfully');
    } catch (error) {
      const message = error.response?.data || 'Update failed';
      toast.error(message);
      throw error;
    }
  };

  const changePassword = async (data) => {
    try {
      await userApi.changePassword(data);
      toast.success('Password changed successfully');
      // After changing password revoke session and force login
      try {
        await logout();
      } catch (err) {
        // logout will clear local state even if API call fails
        console.error('Logout after password change failed', err);
      }
      // Ensure the app navigates to the login page
      window.location.assign('/login');
    } catch (error) {
      const message = error.response?.data || 'Password change failed';
      toast.error(message);
      throw error;
    }
  };

  const isAdmin = () => {
    return user?.role?.toLowerCase() === 'admin';
  };

  const isCustomer = () => {
    return user?.role?.toLowerCase() === 'customer';
  };

  const value = {
    user,
    loading,
    isAuthenticated,
    login,
    register,
    logout,
    updateProfile,
    changePassword,
    isAdmin,
    isCustomer,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};