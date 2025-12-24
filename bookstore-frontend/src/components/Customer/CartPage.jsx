// src/components/Customer/CartPage.jsx
import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useCart } from '../../context/CartContext';
import { FiTrash2, FiShoppingBag } from 'react-icons/fi';
import LoadingSpinner from '../Common/LoadingSpinner';

function CartPage() {
  const navigate = useNavigate();
  const { cartItems, loading, updateCartItem, removeFromCart, getCartTotal } = useCart();

  const handleQuantityChange = async (cartId, newQuantity) => {
    if (newQuantity < 1) return;
    try {
      await updateCartItem(cartId, newQuantity);
    } catch (error) {
      // Error handled in context
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-4xl font-bold mb-8 text-gray-800">Shopping Cart</h1>

      {cartItems.length === 0 ? (
        <div className="text-center py-16">
          <FiShoppingBag className="mx-auto text-gray-300 mb-4" size={80} />
          <h3 className="text-2xl font-bold text-gray-700 mb-2">Your cart is empty</h3>
          <p className="text-gray-500 mb-6">Start adding some books to your cart!</p>
          <button onClick={() => navigate('/shop')} className="btn-primary">
            Continue Shopping
          </button>
        </div>
      ) : (
        <div className="grid lg:grid-cols-3 gap-8">
          {/* Cart Items */}
          <div className="lg:col-span-2 space-y-4">
            {cartItems.map((item) => (
              <div key={item.cartId} className="bg-white rounded-lg shadow-md p-6 flex gap-4">
                <div className="w-24 h-32 bg-gradient-to-br from-primary-100 to-purple-100 rounded-lg flex items-center justify-center text-4xl flex-shrink-0">
                  📖
                </div>

                <div className="flex-grow">
                  <h3 className="font-bold text-lg mb-1 text-gray-800">{item.bookTitle}</h3>
                  <p className="text-sm text-gray-500 mb-3">ISBN: {item.isbn}</p>
                  <p className="text-primary-600 font-bold text-xl mb-3">
                    ${item.unitPrice?.toFixed(2)}
                  </p>

                  <div className="flex items-center gap-4">
                    <div className="flex items-center border border-gray-300 rounded-lg">
                      <button
                        onClick={() => handleQuantityChange(item.cartId, item.quantity - 1)}
                        className="px-3 py-1 hover:bg-gray-100 transition"
                        disabled={item.quantity <= 1}
                      >
                        -
                      </button>
                      <input
                        type="number"
                        value={item.quantity}
                        onChange={(e) => handleQuantityChange(item.cartId, parseInt(e.target.value) || 1)}
                        className="w-16 text-center border-x border-gray-300 py-1 focus:outline-none"
                        min="1"
                      />
                      <button
                        onClick={() => handleQuantityChange(item.cartId, item.quantity + 1)}
                        className="px-3 py-1 hover:bg-gray-100 transition"
                      >
                        +
                      </button>
                    </div>

                    <button
                      onClick={() => removeFromCart(item.cartId)}
                      className="text-red-600 hover:text-red-700 transition flex items-center gap-1"
                    >
                      <FiTrash2 /> Remove
                    </button>
                  </div>
                </div>

                <div className="text-right">
                  <p className="text-2xl font-bold text-gray-800">
                    ${item.totalPrice?.toFixed(2)}
                  </p>
                </div>
              </div>
            ))}
          </div>

          {/* Order Summary */}
          <div className="lg:col-span-1">
            <div className="bg-white rounded-lg shadow-md p-6 sticky top-24">
              <h2 className="text-2xl font-bold mb-6 text-gray-800">Order Summary</h2>

              <div className="space-y-3 mb-6">
                <div className="flex justify-between text-gray-600">
                  <span>Subtotal:</span>
                  <span className="font-semibold">${getCartTotal().toFixed(2)}</span>
                </div>
                <div className="flex justify-between text-gray-600">
                  <span>Shipping:</span>
                  <span className="font-semibold">Free</span>
                </div>
                <div className="border-t pt-3 flex justify-between text-lg font-bold text-gray-800">
                  <span>Total:</span>
                  <span className="text-primary-600">${getCartTotal().toFixed(2)}</span>
                </div>
              </div>

              <button
                onClick={() => navigate('/checkout')}
                className="w-full btn-primary text-lg py-3"
              >
                Proceed to Checkout
              </button>

              <button
                onClick={() => navigate('/shop')}
                className="w-full btn-secondary mt-3"
              >
                Continue Shopping
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default CartPage;