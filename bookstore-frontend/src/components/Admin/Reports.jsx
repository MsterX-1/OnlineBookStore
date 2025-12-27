// src/components/Admin/Reports.jsx
import React, { useState, useEffect } from 'react';
import { reportApi } from '../../api/reportApi';
import { format } from 'date-fns';
import LoadingSpinner from '../Common/LoadingSpinner';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';

function Reports() {
  const [activeTab, setActiveTab] = useState('sales');
  const [loading, setLoading] = useState(false);
  const [salesData, setSalesData] = useState(null);
  const [specificDate, setSpecificDate] = useState('');
  const [specificDateSales, setSpecificDateSales] = useState(null);
  const [topCustomers, setTopCustomers] = useState([]);
  const [topBooks, setTopBooks] = useState([]);

  // Book order count lookup (replenishment orders)
  const [isbnQuery, setIsbnQuery] = useState('');
  const [bookOrderCount, setBookOrderCount] = useState(null);

  useEffect(() => {
    if (activeTab === 'sales') fetchSalesData();
    if (activeTab === 'customers') fetchTopCustomers();
    if (activeTab === 'books') fetchTopBooks();
  }, [activeTab]);

  const fetchSalesData = async () => {
    setLoading(true);
    try {
      const response = await reportApi.getTotalSalesPreviousMonth();
      setSalesData(response.data);
    } catch (error) {
      console.error('Error fetching sales data:', error);
      setSalesData({ totalSales: 0, totalOrders: 0, totalItemsSold: 0 });
    } finally {
      setLoading(false);
    }
  };

  const fetchSpecificDateSales = async () => {
    if (!specificDate) {
      alert('Please select a date');
      return;
    }
    setLoading(true);
    try {
      const response = await reportApi.getTotalSalesForDate(specificDate);
      setSpecificDateSales(response.data);
    } catch (error) {
      console.error('Error fetching specific date sales:', error);
      setSpecificDateSales({ totalSales: 0, totalOrders: 0, totalItemsSold: 0 });
    } finally {
      setLoading(false);
    }
  };

  const fetchTopCustomers = async () => {
    setLoading(true);
    try {
      const response = await reportApi.getTop5Customers();
      setTopCustomers(response.data);
    } catch (error) {
      console.error('Error fetching top customers:', error);
      setTopCustomers([]);
    } finally {
      setLoading(false);
    }
  };

  const fetchTopBooks = async () => {
    setLoading(true);
    try {
      const response = await reportApi.getTop10SellingBooks();
      setTopBooks(response.data);
    } catch (error) {
      console.error('Error fetching top books:', error);
      setTopBooks([]);
    } finally {
      setLoading(false);
    }
  };

  const fetchBookOrderCount = async () => {
    if (!isbnQuery || !isbnQuery.trim()) {
      alert('Please enter an ISBN');
      return;
    }

    setLoading(true);
    try {
      const response = await reportApi.getBookOrderCount(isbnQuery.trim());
      const data = response.data;

      // Map response into BookOrderCountDto shape for frontend (with fallbacks for older shapes)
      const dto = {
        ISBN: data?.ISBN ?? data?.isbn ?? isbnQuery.trim(),
        Title: data?.Title ?? data?.title ?? data?.bookTitle ?? '',
        TotalTimesOrdered: data?.TotalTimesOrdered ?? data?.totalTimesOrdered ?? data?.orderCount ?? data?.count ?? 0,
        TotalQuantityOrdered: data?.TotalQuantityOrdered ?? data?.totalQuantityOrdered ?? data?.quantity ?? 0,
        PendingOrders: data?.PendingOrders ?? data?.pendingOrders ?? data?.pending ?? 0,
        ConfirmedOrders: data?.ConfirmedOrders ?? data?.confirmedOrders ?? data?.confirmed ?? 0,
      };

      if (typeof data === 'number') {
        dto.TotalTimesOrdered = data;
      }

      setBookOrderCount(dto);
    } catch (error) {
      console.error('Error fetching book order count:', error);
      setBookOrderCount(null);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-4xl font-bold mb-8 text-gray-800">Reports</h1>

      {/* Tabs */}
      <div className="flex gap-2 mb-6 border-b overflow-x-auto">
        {['sales', 'customers', 'books'].map((tab) => (
          <button
            key={tab}
            onClick={() => setActiveTab(tab)}
            className={`px-6 py-3 font-semibold transition whitespace-nowrap ${
              activeTab === tab
                ? 'text-primary-600 border-b-2 border-primary-600'
                : 'text-gray-600 hover:text-gray-800'
            }`}
          >
            {tab === 'sales' && 'Sales Reports'}
            {tab === 'customers' && 'Top Customers'}
            {tab === 'books' && 'Top Books'}
          </button>
        ))}
      </div>

      {loading && <LoadingSpinner />}

      {/* Sales Tab */}
      {activeTab === 'sales' && !loading && (
        <div className="space-y-6">
          {/* Previous Month Sales */}
          <div className="bg-white rounded-lg shadow-md p-6">
            <h2 className="text-2xl font-bold mb-4 text-gray-800">Previous Month Sales</h2>
            {salesData && (
              <div className="grid md:grid-cols-3 gap-4">
                <div className="bg-green-50 p-4 rounded-lg">
                  <p className="text-gray-600 mb-1">Total Sales</p>
                  <p className="text-3xl font-bold text-green-600">${salesData.totalSales?.toFixed(2) || '0.00'}</p>
                </div>
                <div className="bg-blue-50 p-4 rounded-lg">
                  <p className="text-gray-600 mb-1">Total Orders</p>
                  <p className="text-3xl font-bold text-blue-600">{salesData.totalOrders || 0}</p>
                </div>
                <div className="bg-purple-50 p-4 rounded-lg">
                  <p className="text-gray-600 mb-1">Items Sold</p>
                  <p className="text-3xl font-bold text-purple-600">{salesData.totalItemsSold || 0}</p>
                </div>
              </div>
            )}
          </div>

          {/* Sales for Specific Date */}
          <div className="bg-white rounded-lg shadow-md p-6">
            <h2 className="text-2xl font-bold mb-4 text-gray-800">Sales for Specific Date</h2>
            <div className="flex gap-4 mb-4">
              <input
                type="date"
                value={specificDate}
                onChange={(e) => setSpecificDate(e.target.value)}
                className="input-field"
              />
              <button onClick={fetchSpecificDateSales} className="btn-primary">
                Get Sales
              </button>
            </div>
            {specificDateSales && (
              <div className="grid md:grid-cols-3 gap-4">
                <div className="bg-green-50 p-4 rounded-lg">
                  <p className="text-gray-600 mb-1">Total Sales</p>
                  <p className="text-3xl font-bold text-green-600">${specificDateSales.totalSales?.toFixed(2) || '0.00'}</p>
                </div>
                <div className="bg-blue-50 p-4 rounded-lg">
                  <p className="text-gray-600 mb-1">Total Orders</p>
                  <p className="text-3xl font-bold text-blue-600">{specificDateSales.totalOrders || 0}</p>
                </div>
                <div className="bg-purple-50 p-4 rounded-lg">
                  <p className="text-gray-600 mb-1">Items Sold</p>
                  <p className="text-3xl font-bold text-purple-600">{specificDateSales.totalItemsSold || 0}</p>
                </div>
              </div>
            )}
          </div>
        </div>
      )}

      {/* Top Customers Tab */}
      {activeTab === 'customers' && !loading && (
        <div className="bg-white rounded-lg shadow-md p-6">
          <h2 className="text-2xl font-bold mb-6 text-gray-800">Top 5 Customers (Last 3 Months)</h2>
          
          {topCustomers.length === 0 ? (
            <div className="text-center py-8 text-gray-500">
              No customer data available for the last 3 months
            </div>
          ) : (
            <>
              {/* Chart */}
              <div className="mb-8">
                <ResponsiveContainer width="100%" height={300}>
                  <BarChart data={topCustomers}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="customerName" angle={-45} textAnchor="end" height={100} />
                    <YAxis />
                    <Tooltip />
                    <Legend />
                    <Bar dataKey="totalPurchaseAmount" fill="#10b981" name="Total Spent ($)" />
                  </BarChart>
                </ResponsiveContainer>
              </div>

              {/* Table */}
              <div className="overflow-x-auto">
                <table className="w-full">
                  <thead className="bg-gray-100">
                    <tr>
                      <th className="px-4 py-3 text-left text-xs font-semibold text-gray-600 uppercase">Rank</th>
                      <th className="px-4 py-3 text-left text-xs font-semibold text-gray-600 uppercase">Customer</th>
                      <th className="px-4 py-3 text-left text-xs font-semibold text-gray-600 uppercase">Email</th>
                      <th className="px-4 py-3 text-left text-xs font-semibold text-gray-600 uppercase">Total Orders</th>
                      <th className="px-4 py-3 text-right text-xs font-semibold text-gray-600 uppercase">Total Spent</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-gray-200">
                    {topCustomers.map((customer, index) => (
                      <tr key={customer.customerId} className="hover:bg-gray-50">
                        <td className="px-4 py-3 text-sm font-bold text-gray-800">#{index + 1}</td>
                        <td className="px-4 py-3 text-sm font-medium text-gray-900">{customer.customerName}</td>
                        <td className="px-4 py-3 text-sm text-gray-600">{customer.email}</td>
                        <td className="px-4 py-3 text-sm text-gray-800">{customer.totalOrders}</td>
                        <td className="px-4 py-3 text-sm text-right font-bold text-green-600">
                          ${customer.totalPurchaseAmount?.toFixed(2)}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </>
          )}
        </div>
      )}

      {/* Top Books Tab */}
      {activeTab === 'books' && !loading && (
        <div className="bg-white rounded-lg shadow-md p-6">
          <h2 className="text-2xl font-bold mb-6 text-gray-800">Top 10 Selling Books (Last 3 Months)</h2>

          {/* Book replenishment orders lookup */}
          <div className="bg-white rounded-lg shadow-md p-4 mb-6">
            <h3 className="text-lg font-semibold mb-3">Total Replenishment Orders for a Book</h3>
            <div className="flex gap-3 items-center mb-3">
              <input
                type="text"
                placeholder="Enter ISBN..."
                value={isbnQuery}
                onChange={(e) => setIsbnQuery(e.target.value)}
                className="input-field"
              />
              <button onClick={fetchBookOrderCount} className="btn-primary">Get Count</button>
              <button onClick={() => { setIsbnQuery(''); setBookOrderCount(null); }} className="btn-secondary">Clear</button>
            </div>
            {bookOrderCount !== null && (
              <div className="grid grid-cols-1 md:grid-cols-2 gap-3 text-sm text-gray-700">
                <div>ISBN: <strong>{bookOrderCount.ISBN}</strong></div>
                <div>Title: <strong>{bookOrderCount.Title || '—'}</strong></div>
                <div>Total Times Ordered: <strong>{bookOrderCount.TotalTimesOrdered ?? 0}</strong></div>
                <div>Total Quantity Ordered: <strong>{bookOrderCount.TotalQuantityOrdered ?? 0}</strong></div>
                <div>Pending Orders: <strong>{bookOrderCount.PendingOrders ?? 0}</strong></div>
                <div>Confirmed Orders: <strong>{bookOrderCount.ConfirmedOrders ?? 0}</strong></div>
              </div>
            )}
          </div>

          {topBooks.length === 0 ? (
            <div className="text-center py-8 text-gray-500">
              No book sales data available for the last 3 months
            </div>
          ) : (
            <>
              {/* Chart */}
              <div className="mb-8">
                <ResponsiveContainer width="100%" height={400}>
                  <BarChart data={topBooks} layout="vertical">
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis type="number" />
                    <YAxis dataKey="title" type="category" width={150} />
                    <Tooltip />
                    <Legend />
                    <Bar dataKey="totalCopiesSold" fill="#3b82f6" name="Copies Sold" />
                  </BarChart>
                </ResponsiveContainer>
              </div>

              {/* Table */}
              <div className="overflow-x-auto">
                <table className="w-full">
                  <thead className="bg-gray-100">
                    <tr>
                      <th className="px-4 py-3 text-left text-xs font-semibold text-gray-600 uppercase">Rank</th>
                      <th className="px-4 py-3 text-left text-xs font-semibold text-gray-600 uppercase">Title</th>
                      <th className="px-4 py-3 text-left text-xs font-semibold text-gray-600 uppercase">ISBN</th>
                      <th className="px-4 py-3 text-left text-xs font-semibold text-gray-600 uppercase">Category</th>
                      <th className="px-4 py-3 text-right text-xs font-semibold text-gray-600 uppercase">Copies Sold</th>
                      <th className="px-4 py-3 text-right text-xs font-semibold text-gray-600 uppercase">Revenue</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-gray-200">
                    {topBooks.map((book, index) => (
                      <tr key={book.isbn} className="hover:bg-gray-50">
                        <td className="px-4 py-3 text-sm font-bold text-gray-800">#{index + 1}</td>
                        <td className="px-4 py-3 text-sm font-medium text-gray-900">{book.title}</td>
                        <td className="px-4 py-3 text-sm text-gray-600">
                          <button
                            onClick={() => fetchBookOrderCount(book.isbn)}
                            className="text-primary-600 hover:underline"
                          >
                            {book.isbn}
                          </button>
                        </td>
                        <td className="px-4 py-3 text-sm">
                          <span className="px-2 py-1 rounded-full text-xs font-semibold bg-primary-100 text-primary-800">
                            {book.category}
                          </span>
                        </td>
                        <td className="px-4 py-3 text-sm text-right font-bold text-blue-600">
                          {book.totalCopiesSold}
                        </td>
                        <td className="px-4 py-3 text-sm text-right font-bold text-green-600">
                          ${book.totalRevenue?.toFixed(2)}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </>
          )}
        </div>
      )}
    </div>
  );
}

export default Reports;