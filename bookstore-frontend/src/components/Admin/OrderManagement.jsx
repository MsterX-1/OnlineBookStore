// src/components/Admin/OrderManagement.jsx
import React, { useState, useEffect } from 'react';
import { orderApi } from '../../api/orderApi';
import { format } from 'date-fns';
import LoadingSpinner from '../Common/LoadingSpinner';
import { FiChevronDown, FiChevronUp } from 'react-icons/fi';

function OrderManagement() {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [expandedOrders, setExpandedOrders] = useState(new Set());

  useEffect(() => {
    fetchOrders();
  }, []);

  const fetchOrders = async () => {
    try {
      const response = await orderApi.getAllOrders();
      setOrders(response.data);
    } catch (error) {
      console.error('Error:', error);
    } finally {
      setLoading(false);
    }
  };

  const toggleOrderDetails = async (orderId) => {
    const newExpanded = new Set(expandedOrders);
    if (newExpanded.has(orderId)) {
      newExpanded.delete(orderId);
    } else {
      newExpanded.add(orderId);
      const order = orders.find(o => o.order_ID === orderId);
      if (!order.items) {
        try {
          const response = await orderApi.getOrderDetails(orderId);
          setOrders(orders.map(o => 
            o.order_ID === orderId ? { ...o, items: response.data } : o
          ));
        } catch (error) {
          console.error('Error fetching details:', error);
        }
      }
    }
    setExpandedOrders(newExpanded);
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-4xl font-bold mb-8 text-gray-800">Order Management</h1>

      <div className="space-y-4">
        {orders.map((order) => (
          <div key={order.order_ID} className="bg-white rounded-lg shadow-md overflow-hidden">
            <div className="p-6">
              <div className="grid md:grid-cols-4 gap-4 mb-4">
                <div>
                  <h3 className="text-sm text-gray-600 mb-1">Order ID</h3>
                  <p className="font-bold text-gray-800">#{order.order_ID}</p>
                </div>
                <div>
                  <h3 className="text-sm text-gray-600 mb-1">Customer</h3>
                  <p className="font-semibold text-gray-800">{order.customerName}</p>
                </div>
                <div>
                  <h3 className="text-sm text-gray-600 mb-1">Date</h3>
                  <p className="text-gray-800">
                    {format(new Date(order.order_Date), 'MMM dd, yyyy')}
                  </p>
                </div>
                <div>
                  <h3 className="text-sm text-gray-600 mb-1">Total</h3>
                  <p className="text-2xl font-bold text-green-600">
                    ${order.total_Amount?.toFixed(2)}
                  </p>
                </div>
              </div>

              <button
                onClick={() => toggleOrderDetails(order.order_ID)}
                className="w-full flex items-center justify-center gap-2 text-primary-600 hover:text-primary-700 font-semibold py-2 border-t"
              >
                {expandedOrders.has(order.order_ID) ? (
                  <>Hide Details <FiChevronUp /></>
                ) : (
                  <>View Details <FiChevronDown /></>
                )}
              </button>
            </div>

            {expandedOrders.has(order.order_ID) && order.items && (
              <div className="border-t bg-gray-50 p-6">
                <h4 className="font-bold text-gray-800 mb-4">Order Items</h4>
                <div className="space-y-2">
                  {order.items.map((item) => (
                    <div key={item.item_Id} className="bg-white p-4 rounded flex justify-between">
                      <div>
                        <p className="font-semibold">ISBN: {item.isbn}</p>
                        <p className="text-sm text-gray-600">Qty: {item.quantity} × ${item.unit_Price}</p>
                      </div>
                      <p className="font-bold">${(item.quantity * item.unit_Price).toFixed(2)}</p>
                    </div>
                  ))}
                </div>
              </div>
            )}
          </div>
        ))}
      </div>
    </div>
  );
}

export default OrderManagement;