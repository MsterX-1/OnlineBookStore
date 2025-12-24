// src/components/Customer/CheckoutPage.jsx
import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { useCart } from '../../context/CartContext';
import { orderApi } from '../../api/orderApi';
import toast from 'react-hot-toast';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';

const checkoutSchema = Yup.object({
  ccNumber: Yup.string()
    .matches(/^\d{16}$/, 'Credit card must be 16 digits')
    .required('Credit card number is required'),
  ccExpiry: Yup.date()
    .min(new Date(), 'Card is expired')
    .required('Expiry date is required'),
});

function CheckoutPage() {
  const navigate = useNavigate();
  const { user } = useAuth();
  const { cartItems, getCartTotal, clearCartLocal } = useCart();
  const [processing, setProcessing] = useState(false);

  const handleSubmit = async (values) => {
    setProcessing(true);
    try {
      const response = await orderApi.placeOrder({
        customerId: user.userid,
        ccNumber: values.ccNumber,
        ccExpiry: values.ccExpiry,
      });

      toast.success(response.data.Messege || response.data.Message || 'Order placed successfully!');
      
      // Clear cart in frontend only (backend already cleared it)
      clearCartLocal();
      
      // Navigate to orders page
      navigate('/orders');
    } catch (error) {
      const message = error.response?.data?.message || error.response?.data || 'Checkout failed';
      toast.error(message);
    } finally {
      setProcessing(false);
    }
  };

  if (cartItems.length === 0) {
    navigate('/cart');
    return null;
  }

  return (
    <div className="container mx-auto px-4 py-8 max-w-6xl">
      <h1 className="text-4xl font-bold mb-8 text-gray-800">Checkout</h1>

      <div className="grid lg:grid-cols-3 gap-8">
        {/* Checkout Form */}
        <div className="lg:col-span-2">
          <div className="bg-white rounded-lg shadow-md p-6">
            <h2 className="text-2xl font-bold mb-6 text-gray-800">Payment Information</h2>

            <Formik
              initialValues={{
                ccNumber: '',
                ccExpiry: '',
              }}
              validationSchema={checkoutSchema}
              onSubmit={handleSubmit}
            >
              {({ isSubmitting }) => (
                <Form className="space-y-6">
                  <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-2">
                      Credit Card Number
                    </label>
                    <Field
                      type="text"
                      name="ccNumber"
                      placeholder="1234567890123456"
                      maxLength="16"
                      className="input-field"
                    />
                    <ErrorMessage name="ccNumber" component="div" className="text-red-600 text-sm mt-1" />
                    <p className="text-xs text-gray-500 mt-1">Enter 16-digit card number</p>
                  </div>

                  <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-2">
                      Expiry Date
                    </label>
                    <Field
                      type="date"
                      name="ccExpiry"
                      min={new Date().toISOString().split('T')[0]}
                      className="input-field"
                    />
                    <ErrorMessage name="ccExpiry" component="div" className="text-red-600 text-sm mt-1" />
                  </div>

                  <div className="bg-blue-50 border-l-4 border-blue-500 p-4 rounded">
                    <p className="text-sm text-blue-800">
                      <strong>Note:</strong> Your payment information is secure. The order will be processed once you click "Place Order".
                    </p>
                  </div>

                  <button
                    type="submit"
                    disabled={isSubmitting || processing}
                    className="w-full btn-primary text-lg py-3"
                  >
                    {processing ? (
                      <span className="flex items-center justify-center gap-2">
                        <div className="animate-spin rounded-full h-5 w-5 border-t-2 border-b-2 border-white"></div>
                        Processing Order...
                      </span>
                    ) : (
                      `Place Order - $${getCartTotal().toFixed(2)}`
                    )}
                  </button>
                </Form>
              )}
            </Formik>
          </div>
        </div>

        {/* Order Summary */}
        <div className="lg:col-span-1">
          <div className="bg-white rounded-lg shadow-md p-6 sticky top-24">
            <h2 className="text-2xl font-bold mb-6 text-gray-800">Order Summary</h2>

            <div className="space-y-3 mb-6 max-h-96 overflow-y-auto">
              {cartItems.map((item) => (
                <div key={item.cartId} className="border-b pb-3">
                  <div className="flex justify-between items-start">
                    <div className="flex-1">
                      <h4 className="font-semibold text-gray-800 text-sm line-clamp-2">
                        {item.bookTitle}
                      </h4>
                      <p className="text-xs text-gray-500">ISBN: {item.isbn}</p>
                      <p className="text-sm text-gray-600 mt-1">
                        Qty: {item.quantity} × ${item.unitPrice?.toFixed(2)}
                      </p>
                    </div>
                    <span className="font-bold text-gray-800 ml-2">
                      ${item.totalPrice?.toFixed(2)}
                    </span>
                  </div>
                </div>
              ))}
            </div>

            <div className="border-t pt-4 space-y-2">
              <div className="flex justify-between text-gray-600">
                <span>Subtotal:</span>
                <span className="font-semibold">${getCartTotal().toFixed(2)}</span>
              </div>
              <div className="flex justify-between text-gray-600">
                <span>Shipping:</span>
                <span className="font-semibold text-green-600">Free</span>
              </div>
              <div className="flex justify-between text-gray-600">
                <span>Tax:</span>
                <span className="font-semibold">$0.00</span>
              </div>
              <div className="flex justify-between text-xl font-bold text-gray-800 pt-3 border-t">
                <span>Total:</span>
                <span className="text-primary-600">${getCartTotal().toFixed(2)}</span>
              </div>
            </div>

            <div className="mt-6 pt-6 border-t">
              <div className="flex items-center gap-2 text-sm text-gray-600">
                <svg className="w-5 h-5 text-green-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                <span>Secure checkout process</span>
              </div>
              <div className="flex items-center gap-2 text-sm text-gray-600 mt-2">
                <svg className="w-5 h-5 text-green-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                <span>Free shipping on all orders</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default CheckoutPage;