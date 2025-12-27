// src/components/Customer/HomePage.jsx
import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { bookApi } from '../../api/bookApi';
import BookCard from './BookCard';
import LoadingSpinner from '../Common/LoadingSpinner';

function HomePage() {
  const [featuredBooks, setFeaturedBooks] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchFeaturedBooks = async () => {
      try {
        const response = await bookApi.getAllBooks();
        setFeaturedBooks(response.data.slice(0, 6));
      } catch (error) {
        console.error('Error fetching featured books:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchFeaturedBooks();
  }, []);

  return (
    <div className="container mx-auto px-4 py-8">
      {/* Hero Section */}
      <div className="bg-gradient-to-r from-primary-500 via-purple-500 to-pink-500 text-white rounded-2xl p-12 mb-12 text-center shadow-2xl">
        <h1 className="text-5xl md:text-6xl font-bold mb-4 animate-fade-in">
          Welcome to Our Bookstore
        </h1>
        <p className="text-xl md:text-2xl mb-8 opacity-90">
          Discover your next favorite book from our curated collection
        </p>
        <Link
          to="/shop"
          className="inline-block bg-white text-primary-600 px-8 py-4 rounded-lg font-bold text-lg hover:bg-gray-100 transform hover:scale-105 transition-all duration-200 shadow-lg"
        >
          Browse Our Collection
        </Link>
      </div>

      {/* Features */}
      <div className="grid md:grid-cols-3 gap-8 mb-12">
        <div className="text-center p-6">
          <div className="text-5xl mb-4">📚</div>
          <h3 className="text-xl font-bold mb-2">Wide Selection</h3>
          <p className="text-gray-600">Thousands of books across all genres</p>
        </div>
        <div className="text-center p-6">
          <div className="text-5xl mb-4">🚚</div>
          <h3 className="text-xl font-bold mb-2">Fast Delivery</h3>
          <p className="text-gray-600">Get your books delivered quickly</p>
        </div>
        <div className="text-center p-6">
          <div className="text-5xl mb-4">💯</div>
          <h3 className="text-xl font-bold mb-2">Best Prices</h3>
          <p className="text-gray-600">Competitive prices on all titles</p>
        </div>
      </div>

      {/* Featured Books */}
      <div className="mb-8">
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-3xl font-bold text-gray-800">Featured Books</h2>
          <Link to="/shop" className="text-primary-600 hover:text-primary-700 font-semibold">
            View All →
          </Link>
        </div>

        {loading ? (
          <LoadingSpinner />
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {featuredBooks.map((book) => (
              <BookCard key={book.isbn} book={book} />
            ))}
          </div>
        )}
      </div>

      {/* Categories */}
      <div className="bg-gray-100 rounded-2xl p-8 mt-12">
        <h2 className="text-3xl font-bold text-gray-800 mb-6 text-center">
          Browse by Category
        </h2>
        <div className="grid grid-cols-2 md:grid-cols-5 gap-4">
          {['Science', 'Art', 'Religion', 'History', 'Geography'].map((category) => (
            <Link
              key={category}
              to={`/shop?category=${category}`}
              className="bg-white p-6 rounded-lg text-center hover:shadow-lg transition-shadow"
            >
              <div className="text-3xl mb-2">
                {category === 'Science' && '🔬'}
                {category === 'Art' && '🎨'}
                {category === 'Religion' && '🕊️'}
                {category === 'History' && '📜'}
                {category === 'Geography' && '🌍'}
              </div>
              <h3 className="font-semibold text-gray-800">{category}</h3>
            </Link>
          ))}
        </div>
      </div>
    </div>
  );
}

export default HomePage;