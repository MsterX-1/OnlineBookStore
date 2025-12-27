// src/components/Customer/MyOrdersPage.jsx
import React, { useState, useEffect } from 'react';
import { useAuth } from '../../context/AuthContext';
import { orderApi } from '../../api/orderApi';
import { format } from 'date-fns';
import LoadingSpinner from '../Common/LoadingSpinner';
import { FiPackage, FiChevronDown, FiChevronUp } from 'react-icons/fi';

function MyOrdersPage() {
  const { user } = useAuth();
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [expandedOrders, setExpandedOrders] = useState(new Set());

  useEffect(() => {
    fetchOrders();
  }, []);

  const fetchOrders = async () => {
    try {
      const response = await orderApi.getOrdersByCustomer(user.userid);
      setOrders(response.data);
    } catch (error) {
      console.error('Error fetching orders:', error);
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
      // Fetch order details if not already loaded
      const order = orders.find(o => o.order_ID === orderId);
      if (!order.items) {
        try {
          const response = await orderApi.getOrderDetails(orderId);
          setOrders(orders.map(o => 
            o.order_ID === orderId ? { ...o, items: response.data } : o
          ));
        } catch (error) {
          console.error('Error fetching order details:', error);
        }
      }
    }
    setExpandedOrders(newExpanded);
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-4xl font-bold mb-8 text-gray-800">My Orders</h1>

      {orders.length === 0 ? (
        <div className="text-center py-16">
          <FiPackage className="mx-auto text-gray-300 mb-4" size={80} />
          <h3 className="text-2xl font-bold text-gray-700 mb-2">No orders yet</h3>
          <p className="text-gray-500">Your order history will appear here</p>
        </div>
      ) : (
        <div className="space-y-4">
          {orders.map((order) => (
            <div key={order.order_ID} className="bg-white rounded-lg shadow-md overflow-hidden">
              <div className="p-6">
                <div className="flex justify-between items-start mb-4">
                  <div>
                    <h3 className="text-xl font-bold text-gray-800">Order #{order.order_ID}</h3>
                    <p className="text-gray-600">
                      {format(new Date(order.order_Date), 'MMMM dd, yyyy')}
                    </p>
                  </div>
                  <div className="text-right">
                    <p className="text-3xl font-bold text-primary-600">
                      ${order.total_Amount?.toFixed(2)}
                    </p>
                  </div>
                </div>

                <button
                  onClick={() => toggleOrderDetails(order.order_ID)}
                  className="w-full flex items-center justify-center gap-2 text-primary-600 hover:text-primary-700 font-semibold py-2 border-t"
                >
                  {expandedOrders.has(order.order_ID) ? (
                    <>View Less <FiChevronUp /></>
                  ) : (
                    <>View Details <FiChevronDown /></>
                  )}
                </button>
              </div>

              {expandedOrders.has(order.order_ID) && order.items && (
                <div className="border-t bg-gray-50 p-6">
                  <h4 className="font-bold text-gray-800 mb-4">Order Items</h4>
                  <div className="space-y-3">
                    {order.items.map((item) => (
                      <div key={item.item_Id} className="flex justify-between items-center bg-white p-4 rounded">
                        <div>
                          <p className="font-semibold text-gray-800">ISBN: {item.isbn}</p>
                          <p className="text-sm text-gray-600">Quantity: {item.quantity}</p>
                        </div>
                        <p className="font-bold text-gray-800">
                          ${(item.unit_Price * item.quantity).toFixed(2)}
                        </p>
                      </div>
                    ))}
                  </div>
                </div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

export default MyOrdersPage;