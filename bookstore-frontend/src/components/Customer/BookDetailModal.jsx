// src/components/Customer/BookDetailModal.jsx
import React, { useState } from 'react';
import { FiX, FiShoppingCart } from 'react-icons/fi';
import { useAuth } from '../../context/AuthContext';
import { useCart } from '../../context/CartContext';
import toast from 'react-hot-toast';

function BookDetailModal({ book, onClose }) {
  const { user } = useAuth();
  const { addToCart } = useCart();
  const [adding, setAdding] = useState(false);
  const [quantity, setQuantity] = useState(1);

  if (!book) return null;

  const handleAddToCart = async () => {
    if (!user) {
      toast.error('Please login to add items to cart');
      return;
    }

    if (book.stockQty === 0) {
      toast.error('This book is out of stock');
      return;
    }

    if (quantity > book.stockQty) {
      toast.error(`Only ${book.stockQty} copies available`);
      return;
    }

    setAdding(true);
    try {
      await addToCart(book.isbn, quantity);
    } catch (error) {
      // Error already handled in CartContext
    } finally {
      setAdding(false);
    }
  };

  const handleQuantityChange = (newQty) => {
    if (newQty < 1) return;
    if (newQty > book.stockQty) {
      toast.error(`Only ${book.stockQty} copies available`);
      return;
    }
    setQuantity(newQty);
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4 overflow-y-auto">
      <div className="bg-white rounded-xl shadow-2xl max-w-4xl w-full max-h-[90vh] overflow-y-auto">
        {/* Header */}
        <div className="sticky top-0 bg-white border-b px-6 py-4 flex justify-between items-center">
          <h2 className="text-2xl font-bold text-gray-800">Book Details</h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 transition"
          >
            <FiX size={28} />
          </button>
        </div>

        {/* Content */}
        <div className="p-6">
          <div className="grid md:grid-cols-2 gap-8">
            {/* Book Image */}
            <div className="flex flex-col items-center">
              <div className="w-full aspect-[3/4] bg-gradient-to-br from-primary-100 via-purple-100 to-pink-100 rounded-xl flex items-center justify-center shadow-lg overflow-hidden">
                {book.bookPhoto ? (
                  <img
                    src={`data:image/jpeg;base64,${book.bookPhoto}`}
                    alt={book.title}
                    className="w-full h-full object-cover"
                  />
                ) : (
                  <div className="text-9xl">📖</div>
                )}
              </div>
              
              {/* Stock Status */}
              <div className="mt-4 w-full">
                <div className={`text-center py-3 rounded-lg font-semibold ${
                  book.stockQty > 10 
                    ? 'bg-green-100 text-green-800' 
                    : book.stockQty > 0 
                    ? 'bg-yellow-100 text-yellow-800' 
                    : 'bg-red-100 text-red-800'
                }`}>
                  {book.stockQty > 10 && `In Stock (${book.stockQty} available)`}
                  {book.stockQty > 0 && book.stockQty <= 10 && `Low Stock (${book.stockQty} left)`}
                  {book.stockQty === 0 && 'Out of Stock'}
                </div>
              </div>
            </div>

            {/* Book Information */}
            <div className="flex flex-col">
              {/* Category Badge */}
              <span className="inline-block bg-primary-100 text-primary-800 text-sm font-semibold px-3 py-1 rounded-full mb-3 w-fit">
                {book.category}
              </span>

              {/* Title */}
              <h1 className="text-3xl font-bold text-gray-800 mb-3">
                {book.title}
              </h1>

              {/* Authors */}
              {book.authors && (
                <div className="mb-4">
                  <h3 className="text-sm font-semibold text-gray-600 mb-1">Author(s)</h3>
                  <p className="text-lg text-gray-700">{book.authors}</p>
                </div>
              )}

              {/* Publisher */}
              {book.publisherName && (
                <div className="mb-4">
                  <h3 className="text-sm font-semibold text-gray-600 mb-1">Publisher</h3>
                  <p className="text-gray-700">{book.publisherName}</p>
                </div>
              )}

              {/* Publication Year */}
              {book.pubYear && (
                <div className="mb-4">
                  <h3 className="text-sm font-semibold text-gray-600 mb-1">Publication Year</h3>
                  <p className="text-gray-700">{book.pubYear}</p>
                </div>
              )}

              {/* ISBN */}
              <div className="mb-4">
                <h3 className="text-sm font-semibold text-gray-600 mb-1">ISBN</h3>
                <p className="text-gray-700 font-mono">{book.isbn}</p>
              </div>

              {/* Description */}
              {book.description && (
                <div className="mb-6">
                  <h3 className="text-sm font-semibold text-gray-600 mb-2">Description</h3>
                  <p className="text-gray-700 leading-relaxed whitespace-pre-line">
                    {book.description}
                  </p>
                </div>
              )}

              {/* Price */}
              <div className="mb-6 py-4 border-t border-b">
                <div className="flex items-baseline gap-2">
                  <span className="text-4xl font-bold text-primary-600">
                    ${book.price?.toFixed(2)}
                  </span>
                  <span className="text-gray-500">per copy</span>
                </div>
              </div>

              {/* Add to Cart Section */}
              {user?.role !== 'Admin' && book.stockQty > 0 && (
                <div className="space-y-4">
                  {/* Quantity Selector */}
                  <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-2">
                      Quantity
                    </label>
                    <div className="flex items-center border border-gray-300 rounded-lg w-fit">
                      <button
                        onClick={() => handleQuantityChange(quantity - 1)}
                        disabled={quantity <= 1}
                        className="px-4 py-2 hover:bg-gray-100 transition disabled:opacity-50"
                      >
                        -
                      </button>
                      <input
                        type="number"
                        value={quantity}
                        onChange={(e) => handleQuantityChange(parseInt(e.target.value) || 1)}
                        className="w-20 text-center border-x border-gray-300 py-2 focus:outline-none"
                        min="1"
                        max={book.stockQty}
                      />
                      <button
                        onClick={() => handleQuantityChange(quantity + 1)}
                        disabled={quantity >= book.stockQty}
                        className="px-4 py-2 hover:bg-gray-100 transition disabled:opacity-50"
                      >
                        +
                      </button>
                    </div>
                    <p className="text-sm text-gray-500 mt-1">
                      Maximum: {book.stockQty} available
                    </p>
                  </div>

                  {/* Total Price */}
                  <div className="bg-gray-50 rounded-lg p-4">
                    <div className="flex justify-between items-center">
                      <span className="text-gray-700 font-semibold">Total:</span>
                      <span className="text-2xl font-bold text-primary-600">
                        ${(book.price * quantity).toFixed(2)}
                      </span>
                    </div>
                  </div>

                  {/* Add to Cart Button */}
                  <button
                    onClick={handleAddToCart}
                    disabled={adding}
                    className="w-full btn-primary py-3 text-lg flex items-center justify-center gap-2"
                  >
                    {adding ? (
                      <>
                        <div className="animate-spin rounded-full h-5 w-5 border-t-2 border-b-2 border-white"></div>
                        Adding to Cart...
                      </>
                    ) : (
                      <>
                        <FiShoppingCart size={20} />
                        Add {quantity > 1 ? `${quantity} ` : ''}to Cart
                      </>
                    )}
                  </button>
                </div>
              )}

              {user?.role !== 'Admin' && book.stockQty === 0 && (
                <div className="bg-red-50 border border-red-200 rounded-lg p-4 text-center">
                  <p className="text-red-800 font-semibold">
                    This book is currently out of stock
                  </p>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default BookDetailModal;