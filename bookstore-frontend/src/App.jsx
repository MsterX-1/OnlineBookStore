import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';
import { AuthProvider } from './context/AuthContext';
import { CartProvider } from './context/CartContext';
import Navbar from './components/Layout/Navbar';
import ProtectedRoute from './components/Layout/ProtectedRoute';

// Customer Pages
import HomePage from './components/Customer/HomePage';
import ShopPage from './components/Customer/ShopPage';
import CartPage from './components/Customer/CartPage';
import CheckoutPage from './components/Customer/CheckoutPage';
import MyOrdersPage from './components/Customer/MyOrdersPage';
import ProfilePage from './components/Customer/ProfilePage';

// Admin Pages
import AdminDashboard from './components/Admin/Dashboard';
import BookManagement from './components/Admin/BookManagement';
import AuthorManagement from './components/Admin/AuthorManagement';
import PublisherManagement from './components/Admin/PublisherManagement';
import OrderManagement from './components/Admin/OrderManagement';
import PublisherOrderManagement from './components/Admin/PublisherOrderManagement';
import Reports from './components/Admin/Reports';

// Auth Pages
import LoginPage from './components/Auth/LoginPage';
import RegisterPage from './components/Auth/RegisterPage';

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <CartProvider>
          <div className="min-h-screen flex flex-col">
            <Navbar />
            <main className="flex-grow">
              <Routes>
                <Route path="/" element={<HomePage />} />
                <Route path="/shop" element={<ShopPage />} />
                <Route path="/login" element={<LoginPage />} />
                <Route path="/register" element={<RegisterPage />} />

                <Route path="/cart" element={<ProtectedRoute><CartPage /></ProtectedRoute>} />
                <Route path="/checkout" element={<ProtectedRoute><CheckoutPage /></ProtectedRoute>} />
                <Route path="/orders" element={<ProtectedRoute><MyOrdersPage /></ProtectedRoute>} />
                <Route path="/profile" element={<ProtectedRoute><ProfilePage /></ProtectedRoute>} />

                <Route path="/admin/dashboard" element={<ProtectedRoute adminOnly><AdminDashboard /></ProtectedRoute>} />
                <Route path="/admin/books" element={<ProtectedRoute adminOnly><BookManagement /></ProtectedRoute>} />
                <Route path="/admin/authors" element={<ProtectedRoute adminOnly><AuthorManagement /></ProtectedRoute>} />
                <Route path="/admin/publishers" element={<ProtectedRoute adminOnly><PublisherManagement /></ProtectedRoute>} />
                <Route path="/admin/orders" element={<ProtectedRoute adminOnly><OrderManagement /></ProtectedRoute>} />
                <Route path="/admin/publisher-orders" element={<ProtectedRoute adminOnly><PublisherOrderManagement /></ProtectedRoute>} />
                <Route path="/admin/reports" element={<ProtectedRoute adminOnly><Reports /></ProtectedRoute>} />

                <Route path="*" element={<Navigate to="/" />} />
              </Routes>
            </main>
            <Toaster position="top-right" toastOptions={{
              duration: 3000,
              style: { background: '#363636', color: '#fff' },
              success: { duration: 3000, iconTheme: { primary: '#10b981', secondary: '#fff' } },
              error: { duration: 4000, iconTheme: { primary: '#ef4444', secondary: '#fff' } },
            }} />
          </div>
        </CartProvider>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;