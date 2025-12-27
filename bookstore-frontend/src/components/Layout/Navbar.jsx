import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { FiShoppingCart, FiUser, FiMenu, FiX } from 'react-icons/fi';
import { useAuth } from '../../context/AuthContext';
import { useCart } from '../../context/CartContext';

function Navbar() {
  const { user, logout, isAdmin } = useAuth();
  const { getCartCount } = useCart();
  const navigate = useNavigate();
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

  const handleLogout = async () => {
    await logout();
    navigate('/');
  };

  return (
    <nav className="bg-gradient-to-r from-primary-600 to-primary-700 text-white shadow-lg sticky top-0 z-50">
      <div className="container mx-auto px-4">
        <div className="flex justify-between items-center h-16">
          {/* Logo */}
          <Link to="/" className="text-2xl font-bold flex items-center gap-2 hover:opacity-90 transition">
            📚 Bookstore
          </Link>

          {/* Desktop Navigation */}
          <div className="hidden md:flex items-center gap-6">
            {!isAdmin() && (
              <>
                <Link to="/shop" className="hover:text-primary-100 transition">Shop</Link>
                <Link to="/cart" className="relative hover:text-primary-100 transition">
                  <FiShoppingCart size={20} />
                  {getCartCount() > 0 && (
                    <span className="absolute -top-2 -right-2 bg-red-500 text-white text-xs rounded-full h-5 w-5 flex items-center justify-center font-bold">
                      {getCartCount()}
                    </span>
                  )}
                </Link>
              </>
            )}

            {user ? (
              <>
                {isAdmin() ? (
                  <>
                    <Link to="/admin/dashboard" className="hover:text-primary-100 transition">Dashboard</Link>
                    <Link to="/admin/books" className="hover:text-primary-100 transition">Books</Link>
                    <Link to="/admin/authors" className="hover:text-primary-100 transition">Authors</Link>
                    <Link to="/admin/publishers" className="hover:text-primary-100 transition">Publishers</Link>
                    <Link to="/admin/orders" className="hover:text-primary-100 transition">Orders</Link>
                    <Link to="/admin/publisher-orders" className="hover:text-primary-100 transition">Restock Orders</Link>
                    <Link to="/admin/reports" className="hover:text-primary-100 transition">Reports</Link>
                  </>
                ) : (
                  <>
                    <Link to="/orders" className="hover:text-primary-100 transition">My Orders</Link>
                    <Link to="/profile" className="hover:text-primary-100 transition flex items-center gap-1">
                      <FiUser size={18} /> Profile
                    </Link>
                  </>
                )}
                <button onClick={handleLogout} className="bg-red-500 hover:bg-red-600 px-4 py-2 rounded-lg transition font-semibold">
                  Logout
                </button>
              </>
            ) : (
              <>
                <Link to="/login" className="hover:text-primary-100 transition">Login</Link>
                <Link to="/register" className="bg-green-500 hover:bg-green-600 px-4 py-2 rounded-lg transition font-semibold">
                  Register
                </Link>
              </>
            )}
          </div>

          {/* Mobile Menu Button */}
          <button onClick={() => setMobileMenuOpen(!mobileMenuOpen)} className="md:hidden">
            {mobileMenuOpen ? <FiX size={24} /> : <FiMenu size={24} />}
          </button>
        </div>

        {/* Mobile Menu */}
        {mobileMenuOpen && (
          <div className="md:hidden pb-4">
            {!isAdmin() && (
              <>
                <Link to="/shop" className="block py-2 hover:text-primary-100" onClick={() => setMobileMenuOpen(false)}>Shop</Link>
                <Link to="/cart" className="block py-2 hover:text-primary-100" onClick={() => setMobileMenuOpen(false)}>Cart ({getCartCount()})</Link>
              </>
            )}
            {user ? (
              <>
                {isAdmin() ? (
                  <>
                    <Link to="/admin/dashboard" className="block py-2 hover:text-primary-100" onClick={() => setMobileMenuOpen(false)}>Dashboard</Link>
                    <Link to="/admin/books" className="block py-2 hover:text-primary-100" onClick={() => setMobileMenuOpen(false)}>Books</Link>
                    <Link to="/admin/authors" className="block py-2 hover:text-primary-100" onClick={() => setMobileMenuOpen(false)}>Authors</Link>
                    <Link to="/admin/publishers" className="block py-2 hover:text-primary-100" onClick={() => setMobileMenuOpen(false)}>Publishers</Link>
                    <Link to="/admin/orders" className="block py-2 hover:text-primary-100" onClick={() => setMobileMenuOpen(false)}>Orders</Link>
                    <Link to="/admin/publisher-orders" className="block py-2 hover:text-primary-100" onClick={() => setMobileMenuOpen(false)}>Restock Orders</Link>
                    <Link to="/admin/reports" className="block py-2 hover:text-primary-100" onClick={() => setMobileMenuOpen(false)}>Reports</Link>
                  </>
                ) : (
                  <>
                    <Link to="/orders" className="block py-2 hover:text-primary-100" onClick={() => setMobileMenuOpen(false)}>My Orders</Link>
                    <Link to="/profile" className="block py-2 hover:text-primary-100" onClick={() => setMobileMenuOpen(false)}>Profile</Link>
                  </>
                )}
                <button onClick={() => { handleLogout(); setMobileMenuOpen(false); }} className="block w-full text-left py-2 text-red-300">
                  Logout
                </button>
              </>
            ) : (
              <>
                <Link to="/login" className="block py-2 hover:text-primary-100" onClick={() => setMobileMenuOpen(false)}>Login</Link>
                <Link to="/register" className="block py-2 hover:text-primary-100" onClick={() => setMobileMenuOpen(false)}>Register</Link>
              </>
            )}
          </div>
        )}
      </div>
    </nav>
  );
}

export default Navbar;