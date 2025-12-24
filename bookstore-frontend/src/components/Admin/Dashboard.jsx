// src/components/Admin/Dashboard.jsx
import React, { useState, useEffect } from 'react';
import { reportApi } from '../../api/reportApi';
import { bookApi } from '../../api/bookApi';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import LoadingSpinner from '../Common/LoadingSpinner';
import { FiDollarSign, FiShoppingBag, FiPackage, FiAlertCircle } from 'react-icons/fi';

function AdminDashboard() {
  const [stats, setStats] = useState({ totalSales: 0, totalOrders: 0, totalItemsSold: 0 });
  const [topBooks, setTopBooks] = useState([]);
  const [topCustomers, setTopCustomers] = useState([]);
  const [lowStockBooks, setLowStockBooks] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchDashboardData();
  }, []);

  const fetchDashboardData = async () => {
    try {
      const [salesRes, booksRes, customersRes, lowStockRes] = await Promise.all([
        reportApi.getTotalSalesPreviousMonth().catch(() => ({ data: { totalSales: 0, totalOrders: 0, totalItemsSold: 0 } })),
        reportApi.getTop10SellingBooks().catch(() => ({ data: [] })),
        reportApi.getTop5Customers().catch(() => ({ data: [] })),
        bookApi.getLowStockBooks().catch(() => ({ data: [] })),
      ]);

      setStats(salesRes.data);
      setTopBooks(booksRes.data);
      setTopCustomers(customersRes.data);
      setLowStockBooks(lowStockRes.data);
    } catch (error) {
      console.error('Error fetching dashboard data:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-4xl font-bold mb-8 text-gray-800">Admin Dashboard</h1>

      {/* Stats Cards */}
      <div className="grid md:grid-cols-4 gap-6 mb-8">
        <div className="bg-gradient-to-br from-green-500 to-green-600 text-white rounded-xl p-6 shadow-lg">
          <FiDollarSign className="text-4xl mb-2 opacity-80" />
          <h3 className="text-lg opacity-90 mb-1">Total Sales</h3>
          <p className="text-3xl font-bold">${stats.totalSales?.toFixed(2)}</p>
          <p className="text-sm opacity-75 mt-2">Previous month</p>
        </div>

        <div className="bg-gradient-to-br from-blue-500 to-blue-600 text-white rounded-xl p-6 shadow-lg">
          <FiShoppingBag className="text-4xl mb-2 opacity-80" />
          <h3 className="text-lg opacity-90 mb-1">Total Orders</h3>
          <p className="text-3xl font-bold">{stats.totalOrders}</p>
          <p className="text-sm opacity-75 mt-2">Previous month</p>
        </div>

        <div className="bg-gradient-to-br from-purple-500 to-purple-600 text-white rounded-xl p-6 shadow-lg">
          <FiPackage className="text-4xl mb-2 opacity-80" />
          <h3 className="text-lg opacity-90 mb-1">Items Sold</h3>
          <p className="text-3xl font-bold">{stats.totalItemsSold}</p>
          <p className="text-sm opacity-75 mt-2">Previous month</p>
        </div>

        <div className="bg-gradient-to-br from-red-500 to-red-600 text-white rounded-xl p-6 shadow-lg">
          <FiAlertCircle className="text-4xl mb-2 opacity-80" />
          <h3 className="text-lg opacity-90 mb-1">Low Stock</h3>
          <p className="text-3xl font-bold">{lowStockBooks.length}</p>
          <p className="text-sm opacity-75 mt-2">Books need restock</p>
        </div>
      </div>

      {/* Charts */}
      <div className="grid lg:grid-cols-2 gap-8 mb-8">
        {/* Top Selling Books Chart */}
        <div className="bg-white rounded-xl shadow-md p-6">
          <h2 className="text-2xl font-bold mb-6 text-gray-800">Top 10 Selling Books</h2>
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={topBooks.slice(0, 5)}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="title" angle={-45} textAnchor="end" height={100} />
              <YAxis />
              <Tooltip />
              <Bar dataKey="totalCopiesSold" fill="#3b82f6" name="Copies Sold" />
            </BarChart>
          </ResponsiveContainer>
        </div>

        {/* Top Customers Chart */}
        <div className="bg-white rounded-xl shadow-md p-6">
          <h2 className="text-2xl font-bold mb-6 text-gray-800">Top 5 Customers</h2>
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={topCustomers}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="customerName" angle={-45} textAnchor="end" height={100} />
              <YAxis />
              <Tooltip />
              <Bar dataKey="totalPurchaseAmount" fill="#10b981" name="Total Spent ($)" />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>

      {/* Low Stock Alert */}
      {lowStockBooks.length > 0 && (
        <div className="bg-red-50 border-l-4 border-red-500 rounded-lg p-6">
          <h2 className="text-2xl font-bold mb-4 text-red-800 flex items-center gap-2">
            <FiAlertCircle /> Low Stock Alert
          </h2>
          <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-4">
            {lowStockBooks.slice(0, 6).map((book) => (
              <div key={book.isbn} className="bg-white rounded-lg p-4 shadow">
                <h3 className="font-bold text-gray-800 mb-1">{book.title}</h3>
                <p className="text-sm text-gray-600 mb-2">ISBN: {book.isbn}</p>
                <div className="flex justify-between items-center">
                  <span className="text-red-600 font-semibold">Stock: {book.stockQty}</span>
                  <span className="text-gray-500 text-sm">Threshold: {book.threshold}</span>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}

export default AdminDashboard;