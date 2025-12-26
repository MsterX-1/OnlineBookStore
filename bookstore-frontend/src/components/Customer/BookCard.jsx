// src/components/Customer/BookCard.jsx
import React, { useState } from 'react';
import { useAuth } from '../../context/AuthContext';
import { useCart } from '../../context/CartContext';
import BookDetailModal from './BookDetailModal';
import toast from 'react-hot-toast';
import { FiEye } from 'react-icons/fi';

function BookCard({ book }) {
  const { user } = useAuth();
  const { addToCart } = useCart();
  const [adding, setAdding] = useState(false);
  const [showModal, setShowModal] = useState(false);

  const handleAddToCart = async (e) => {
    e.stopPropagation(); // Prevent opening modal when clicking add to cart
    
    if (!user) {
      toast.error('Please login to add items to cart');
      return;
    }

    if (book.stockQty === 0) {
      toast.error('This book is out of stock');
      return;
    }

    setAdding(true);
    try {
      await addToCart(book.isbn, 1);
    } catch (error) {
      // Error already handled in CartContext
    } finally {
      setAdding(false);
    }
  };

  const handleCardClick = () => {
    setShowModal(true);
  };

  const formatAuthors = (authors) => {
    if (!authors) return 'Unknown Author';
    return authors.length > 50 ? authors.substring(0, 50) + '...' : authors;
  };

  const truncateDescription = (description) => {
    if (!description) return '';
    return description.length > 80 ? description.substring(0, 80) + '...' : description;
  };

  return (
    <>
      <div 
        className="bg-white rounded-xl shadow-md overflow-hidden hover:shadow-2xl transition-all duration-300 transform hover:-translate-y-1 cursor-pointer"
        onClick={handleCardClick}
      >
        {/* Book Image Placeholder */}
        <div className="h-64 bg-gradient-to-br from-primary-100 via-purple-100 to-pink-100 flex items-center justify-center relative">
          {book.bookPhoto ? (
            <img
              src={`data:image/jpeg;base64,${book.bookPhoto}`}
              alt={book.title}
              className="h-full w-full object-cover"
            />
          ) : (
            <div className="text-7xl">📖</div>
          )}
          
          {/* View Details Badge */}
          <div className="absolute top-3 right-3 bg-white bg-opacity-90 rounded-full p-2 shadow-lg hover:bg-opacity-100 transition">
            <FiEye size={20} className="text-primary-600" />
          </div>
        </div>

        {/* Book Details */}
        <div className="p-5">
          <div className="mb-2">
            <span className="inline-block bg-primary-100 text-primary-800 text-xs font-semibold px-2.5 py-1 rounded">
              {book.category}
            </span>
          </div>

          <h3 className="font-bold text-lg mb-2 line-clamp-2 text-gray-800 min-h-[3.5rem]">
            {book.title}
          </h3>

          <p className="text-sm text-gray-600 mb-1 line-clamp-1">
            {formatAuthors(book.authors)}
          </p>

          {/* Description Preview */}
          {book.description && (
            <p className="text-xs text-gray-500 mb-3 line-clamp-2">
              {truncateDescription(book.description)}
            </p>
          )}

          <div className="flex justify-between items-center mb-4">
            <span className="text-2xl font-bold text-primary-600">
              ${book.price?.toFixed(2)}
            </span>
            <span className={`text-sm font-semibold ${book.stockQty > 0 ? 'text-green-600' : 'text-red-600'}`}>
              {book.stockQty > 0 ? `${book.stockQty} in stock` : 'Out of stock'}
            </span>
          </div>

          {user && user.role !== 'Admin' && (
            <div className="flex gap-2">
              <button
                onClick={handleAddToCart}
                disabled={adding || book.stockQty === 0}
                className="flex-1 btn-primary disabled:opacity-50"
              >
                {adding ? (
                  <span className="flex items-center justify-center gap-2">
                    <div className="animate-spin rounded-full h-4 w-4 border-t-2 border-b-2 border-white"></div>
                    Adding...
                  </span>
                ) : book.stockQty === 0 ? (
                  'Out of Stock'
                ) : (
                  'Add to Cart'
                )}
              </button>
              <button
                onClick={(e) => {
                  e.stopPropagation();
                  handleCardClick();
                }}
                className="px-4 py-2 bg-gray-200 hover:bg-gray-300 text-gray-800 font-semibold rounded-lg transition"
              >
                View
              </button>
            </div>
          )}

          {!user && (
            <button
              onClick={(e) => {
                e.stopPropagation();
                handleCardClick();
              }}
              className="w-full btn-primary"
            >
              View Details
            </button>
          )}
        </div>
      </div>

      {/* Book Detail Modal */}
      {showModal && (
        <BookDetailModal
          book={book}
          onClose={() => setShowModal(false)}
        />
      )}
    </>
  );
}

export default BookCard;