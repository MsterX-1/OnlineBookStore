// src/components/Admin/PublisherOrderManagement.jsx
import React, { useState, useEffect } from 'react';
import { publisherOrderApi } from '../../api/publisherOrderApi';
import { format } from 'date-fns';
import toast from 'react-hot-toast';
import LoadingSpinner from '../Common/LoadingSpinner';

function PublisherOrderManagement() {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState('all');

  useEffect(() => {
    fetchOrders();
  }, [filter]);

  const fetchOrders = async () => {
    try {
      const response = filter === 'pending'
        ? await publisherOrderApi.getPendingOrders()
        : await publisherOrderApi.getAllPublisherOrders();
      setOrders(response.data);
    } catch (error) {
      console.error('Error:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleConfirm = async (orderId) => {
    if (!window.confirm('Confirm this publisher order?')) return;

    try {
      await publisherOrderApi.confirmOrder({ pubOrderId: orderId });
      toast.success('Order confirmed successfully');
      fetchOrders();
    } catch (error) {
      toast.error('Failed to confirm order');
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex justify-between items-center mb-8">
        <h1 className="text-4xl font-bold text-gray-800">Publisher Orders</h1>
        <div className="flex gap-2">
          <button
            onClick={() => setFilter('all')}
            className={`px-4 py-2 rounded-lg font-semibold ${
              filter === 'all' ? 'bg-primary-600 text-white' : 'bg-gray-200 text-gray-800'
            }`}
          >
            All Orders
          </button>
          <button
            onClick={() => setFilter('pending')}
            className={`px-4 py-2 rounded-lg font-semibold ${
              filter === 'pending' ? 'bg-primary-600 text-white' : 'bg-gray-200 text-gray-800'
            }`}
          >
            Pending Only
          </button>
        </div>
      </div>

      <div className="bg-white rounded-lg shadow-md overflow-x-auto">
        <table className="w-full">
          <thead className="bg-gray-100">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase">Order ID</th>
              <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase">Book</th>
              <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase">Quantity</th>
              <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase">Date</th>
              <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase">Status</th>
              <th className="px-6 py-3 text-right text-xs font-semibold text-gray-600 uppercase">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {orders.map((order) => (
              <tr key={order.pubOrderId} className="hover:bg-gray-50">
                <td className="px-6 py-4 text-sm text-gray-800">#{order.pubOrderId}</td>
                <td className="px-6 py-4 text-sm">
                  <div>
                    <p className="font-semibold text-gray-900">{order.bookTitle}</p>
                    <p className="text-gray-500 text-xs">{order.isbn}</p>
                  </div>
                </td>
                <td className="px-6 py-4 text-sm text-gray-800">{order.quantity}</td>
                <td className="px-6 py-4 text-sm text-gray-600">
                  {format(new Date(order.orderDate), 'MMM dd, yyyy')}
                </td>
                <td className="px-6 py-4">
                  <span className={`px-3 py-1 rounded-full text-xs font-semibold ${
                    order.status === 'Confirmed'
                      ? 'bg-green-100 text-green-800'
                      : 'bg-yellow-100 text-yellow-800'
                  }`}>
                    {order.status}
                  </span>
                </td>
                <td className="px-6 py-4 text-right">
                  {order.status === 'Pending' && (
                    <button
                      onClick={() => handleConfirm(order.pubOrderId)}
                      className="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded-lg text-sm font-semibold"
                    >
                      Confirm
                    </button>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}

export default PublisherOrderManagement;