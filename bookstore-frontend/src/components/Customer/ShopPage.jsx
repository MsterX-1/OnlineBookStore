// src/components/Customer/ShopPage.jsx
import React, { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import { bookApi } from '../../api/bookApi';
import { publisherApi } from '../../api/publisherApi';
import { authorApi } from '../../api/authorApi';
import BookCard from './BookCard';
import LoadingSpinner from '../Common/LoadingSpinner';
import { FiSearch } from 'react-icons/fi';

function ShopPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [books, setBooks] = useState([]);
  const [publishers, setPublishers] = useState([]);
  const [authors, setAuthors] = useState([]);
  const [loading, setLoading] = useState(true);
  
  const [filters, setFilters] = useState({
    isbn: '',
    title: searchParams.get('title') || '',
    category: searchParams.get('category') || '',
    publisherName: '',
    authorName: '',
  });

  const categories = ['Science', 'Art', 'Religion', 'History', 'Geography'];

  useEffect(() => {
    fetchPublishers();
    fetchAuthors();
  }, []);

  useEffect(() => {
    // Debounce the search
    const timeoutId = setTimeout(() => {
      fetchBooks();
    }, 500);

    return () => clearTimeout(timeoutId);
  }, [filters]);

  const fetchPublishers = async () => {
    try {
      const response = await publisherApi.getAllPublishers();
      setPublishers(response.data);
    } catch (error) {
      console.error('Error fetching publishers:', error);
    }
  };

  const fetchAuthors = async () => {
    try {
      const response = await authorApi.getAllAuthors();
      setAuthors(response.data);
    } catch (error) {
      console.error('Error fetching authors:', error);
    }
  };

  const fetchBooks = async () => {
    setLoading(true);
    try {
      // Check if any filter is active
      const hasFilters = Object.values(filters).some(val => val && val !== '');
      
      if (hasFilters) {
        // Build search object with only non-empty values
        const searchParams = {};
        if (filters.isbn) searchParams.isbn = filters.isbn;
        if (filters.title) searchParams.title = filters.title;
        if (filters.category) searchParams.category = filters.category;
        if (filters.publisherName) searchParams.publisherName = filters.publisherName;
        if (filters.authorName) searchParams.authorName = filters.authorName;

        console.log('Searching with params:', searchParams);
        const response = await bookApi.searchBooksAdvanced(searchParams);
        setBooks(response.data || []);
      } else {
        // No filters, get all books
        const response = await bookApi.getAllBooks();
        setBooks(response.data || []);
      }
    } catch (error) {
      console.error('Error fetching books:', error);
      // If search fails, try to show all books as fallback
      try {
        const response = await bookApi.getAllBooks();
        setBooks(response.data || []);
      } catch (fallbackError) {
        console.error('Fallback also failed:', fallbackError);
        setBooks([]);
      }
    } finally {
      setLoading(false);
    }
  };

  const handleFilterChange = (key, value) => {
    setFilters(prev => ({ ...prev, [key]: value }));
  };

  const clearFilters = () => {
    setFilters({ isbn: '', title: '', category: '', publisherName: '', authorName: '' });
    setSearchParams({});
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-4xl font-bold mb-8 text-gray-800">Shop Books</h1>

      {/* Filters */}
      <div className="bg-white rounded-lg shadow-md p-6 mb-8">
        <h3 className="text-lg font-bold text-gray-800 mb-4">Search & Filter</h3>
        <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-4">
          {/* Search by ISBN */}
          <div>
            <label className="block text-sm font-semibold text-gray-700 mb-2">
              Search by ISBN
            </label>
            <div className="relative">
              <input
                type="text"
                placeholder="Enter ISBN..."
                value={filters.isbn}
                onChange={(e) => handleFilterChange('isbn', e.target.value)}
                className="input-field pl-10"
              />
              <FiSearch className="absolute left-3 top-3.5 text-gray-400" size={18} />
            </div>
          </div>

          {/* Search by Title */}
          <div>
            <label className="block text-sm font-semibold text-gray-700 mb-2">
              Search by Title
            </label>
            <div className="relative">
              <input
                type="text"
                placeholder="Enter book title..."
                value={filters.title}
                onChange={(e) => handleFilterChange('title', e.target.value)}
                className="input-field pl-10"
              />
              <FiSearch className="absolute left-3 top-3.5 text-gray-400" size={18} />
            </div>
          </div>

          {/* Category Filter */}
          <div>
            <label className="block text-sm font-semibold text-gray-700 mb-2">
              Category
            </label>
            <select
              value={filters.category}
              onChange={(e) => handleFilterChange('category', e.target.value)}
              className="input-field"
            >
              <option value="">All Categories</option>
              {categories.map((cat) => (
                <option key={cat} value={cat}>{cat}</option>
              ))}
            </select>
          </div>

          {/* Author Filter */}
          <div>
            <label className="block text-sm font-semibold text-gray-700 mb-2">
              Author
            </label>
            <select
              value={filters.authorName}
              onChange={(e) => handleFilterChange('authorName', e.target.value)}
              className="input-field"
            >
              <option value="">All Authors</option>
              {authors.map((author) => (
                <option key={author.authorId} value={author.name}>{author.name}</option>
              ))}
            </select>
          </div>

          {/* Publisher Filter */}
          <div>
            <label className="block text-sm font-semibold text-gray-700 mb-2">
              Publisher
            </label>
            <select
              value={filters.publisherName}
              onChange={(e) => handleFilterChange('publisherName', e.target.value)}
              className="input-field"
            >
              <option value="">All Publishers</option>
              {publishers.map((pub) => (
                <option key={pub.publisherId} value={pub.name}>{pub.name}</option>
              ))}
            </select>
          </div>
        </div>

        <div className="flex gap-4 mt-4">
          <button
            onClick={clearFilters}
            className="text-primary-600 hover:text-primary-700 font-semibold"
          >
            Clear All Filters
          </button>
          <button
            onClick={fetchBooks}
            className="btn-primary"
          >
            <FiSearch className="inline mr-2" />
            Search
          </button>
        </div>
      </div>

      {/* Results */}
      <div className="mb-4 flex justify-between items-center">
        <p className="text-gray-600">
          {books.length} book{books.length !== 1 ? 's' : ''} found
        </p>
        {loading && (
          <div className="flex items-center gap-2 text-primary-600">
            <div className="animate-spin rounded-full h-5 w-5 border-t-2 border-b-2 border-primary-600"></div>
            <span>Loading...</span>
          </div>
        )}
      </div>

      {loading && books.length === 0 ? (
        <LoadingSpinner />
      ) : books.length === 0 ? (
        <div className="text-center py-12 bg-white rounded-lg shadow-md">
          <div className="text-6xl mb-4">📚</div>
          <h3 className="text-2xl font-bold text-gray-700 mb-2">No books found</h3>
          <p className="text-gray-500 mb-4">Try adjusting your filters or search terms</p>
          <button onClick={clearFilters} className="btn-primary">
            Clear Filters
          </button>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
          {books.map((book) => (
            <BookCard key={book.isbn} book={book} />
          ))}
        </div>
      )}
    </div>
  );
}

export default ShopPage;