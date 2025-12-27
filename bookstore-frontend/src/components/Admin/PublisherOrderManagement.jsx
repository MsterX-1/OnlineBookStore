// src/components/Admin/PublisherOrderManagement.jsx
import React, { useState, useEffect } from 'react';
import { publisherOrderApi } from '../../api/publisherOrderApi';
import { bookApi } from '../../api/bookApi';
import { format } from 'date-fns';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import { FiPlus } from 'react-icons/fi';
import toast from 'react-hot-toast';
import LoadingSpinner from '../Common/LoadingSpinner';

function PublisherOrderManagement() {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState('all');

  // For creating orders
  const [books, setBooks] = useState([]);
  const [showCreateModal, setShowCreateModal] = useState(false);

  const fetchBooks = async () => {
    try {
      const res = await bookApi.getAllBooks();
      setBooks(res.data || []);
    } catch (error) {
      console.error('Failed to fetch books:', error);
    }
  };

  useEffect(() => {
    fetchOrders();
  }, [filter]);

  useEffect(() => {
    fetchBooks();
  }, []);

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

  const orderSchema = Yup.object({
    isbn: Yup.string().required('Book is required'),
    quantity: Yup.number().min(1, 'Minimum 1').required('Quantity is required'),
  });

  const handleCreateOrder = async (values, { setSubmitting, resetForm }) => {
    try {
      await publisherOrderApi.createPublisherOrder({ isbn: values.isbn, quantity: Number(values.quantity) });
      toast.success('Publisher order created');
      resetForm();
      setShowCreateModal(false);
      fetchOrders();
    } catch (error) {
      toast.error(error.response?.data?.message || 'Failed to create publisher order');
    } finally {
      setSubmitting(false);
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
          <button
            onClick={() => setShowCreateModal(true)}
            className="btn-primary flex items-center gap-2"
          >
            <FiPlus /> Create Order
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

      {showCreateModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg shadow-xl max-w-md w-full p-6">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-2xl font-bold text-gray-800">Create Publisher Order</h2>
              <button onClick={() => setShowCreateModal(false)} className="text-gray-500 hover:text-gray-700">✕</button>
            </div>

            <Formik
              initialValues={{ isbn: books[0]?.isbn || '', quantity: 1 }}
              validationSchema={orderSchema}
              onSubmit={handleCreateOrder}
              enableReinitialize
            >
              {({ isSubmitting }) => (
                <Form className="space-y-4">
                  <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-2">Book *</label>
                    <Field as="select" name="isbn" className="input-field">
                      <option value="">Select Book</option>
                      {books.map(b => (
                        <option key={b.isbn} value={b.isbn}>{b.title} — {b.isbn}</option>
                      ))}
                    </Field>
                    <ErrorMessage name="isbn" component="div" className="text-red-600 text-sm mt-1" />
                  </div>

                  <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-2">Quantity *</label>
                    <Field type="number" name="quantity" className="input-field" min="1" />
                    <ErrorMessage name="quantity" component="div" className="text-red-600 text-sm mt-1" />
                  </div>

                  <div className="flex gap-3 pt-4">
                    <button type="submit" disabled={isSubmitting} className="btn-primary flex-1">
                      {isSubmitting ? 'Creating...' : 'Create Order'}
                    </button>
                    <button type="button" onClick={() => setShowCreateModal(false)} className="btn-secondary flex-1">Cancel</button>
                  </div>
                </Form>
              )}
            </Formik>
          </div>
        </div>
      )}
    </div>
  );
}

export default PublisherOrderManagement;