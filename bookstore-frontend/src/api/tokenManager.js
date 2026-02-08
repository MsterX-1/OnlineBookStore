// src/api/tokenManager.js
/**
 * Token Manager - Secure In-Memory Token Storage
 * 
 * This module manages JWT tokens in memory to prevent XSS attacks.
 * Tokens are NOT stored in localStorage or sessionStorage.
 * Refresh tokens are stored in HTTP-only cookies by the backend.
 * 
 * Security Strategy:
 * - JWT token stored only in memory (JavaScript variable)
 * - Cannot be accessed by malicious scripts via XSS
 * - Token lost on page refresh (re-obtained via refresh token cookie)
 * - Refresh token in HTTP-only cookie (not accessible to JavaScript)
 */

let currentToken = null;
let tokenExpiration = null;
let listeners = [];

/**
 * Set the current JWT token in memory
 * @param {string} token - The JWT token
 * @param {Date} expiresOn - Token expiration datetime
 */
export const setToken = (token, expiresOn) => {
  currentToken = token;
  tokenExpiration = expiresOn;
  notifyListeners(token);
};

/**
 * Get the current JWT token from memory
 * @returns {string|null} The current JWT token or null if not set
 */
export const getToken = () => {
  return currentToken;
};

/**
 * Get token expiration time
 * @returns {Date|null} Token expiration datetime
 */
export const getTokenExpiration = () => {
  return tokenExpiration;
};

/**
 * Check if token exists and is valid
 * @returns {boolean}
 */
export const hasValidToken = () => {
  if (!currentToken) return false;
  if (!tokenExpiration) return false;
  return new Date() < new Date(tokenExpiration);
};

/**
 * Clear the current token from memory
 */
export const clearToken = () => {
  currentToken = null;
  tokenExpiration = null;
  notifyListeners(null);
};

/**
 * Subscribe to token changes
 * @param {Function} listener - Callback function when token changes
 * @returns {Function} Unsubscribe function
 */
export const subscribeToTokenChanges = (listener) => {
  listeners.push(listener);
  return () => {
    listeners = listeners.filter((l) => l !== listener);
  };
};

/**
 * Notify all subscribers of token change
 * @private
 */
const notifyListeners = (token) => {
  listeners.forEach((listener) => {
    try {
      listener(token);
    } catch (error) {
      console.error('Error in token listener:', error);
    }
  });
};

export default {
  setToken,
  getToken,
  getTokenExpiration,
  hasValidToken,
  clearToken,
  subscribeToTokenChanges,
};
