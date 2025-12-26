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

  // Pagination
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(12);
  const pageSizes = [8, 12, 16, 24];

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

        // Fetch all books locally so we can enforce per-field checks (AND semantics + early empty-match detection)
        const allResponse = await bookApi.getAllBooks();
        const allBooks = allResponse.data || [];

        const hasMatchForField = (field, value) => {
          const val = value.toString().toLowerCase();
          switch (field) {
            case 'isbn':
              return allBooks.some(b => b.isbn?.toString().includes(value));
            case 'title':
              return allBooks.some(b => b.title?.toLowerCase().includes(val));
            case 'category':
              return allBooks.some(b => b.category?.toLowerCase() === val);
            case 'publisherName':
              return allBooks.some(b => b.publisherName?.toLowerCase().includes(val));
            case 'authorName': {
              return allBooks.some(b => {
                const authors = typeof b.authors === 'string' ? b.authors : (Array.isArray(b.authors) ? b.authors.join(' ') : '');
                return authors.toLowerCase().includes(val);
              });
            }
            default:
              return true;
          }
        };

        // If any active field has zero matches across the whole dataset, show no results
        for (const [k, v] of Object.entries(searchParams)) {
          if (v && !hasMatchForField(k, v)) {
            console.log(`No matches for filter ${k}=${v}; returning no results`);
            setBooks([]);
            setLoading(false);
            return;
          }
        }

        // Apply AND filter across allBooks
        const matches = (book) => {
          if (filters.isbn && !(book.isbn?.toString().includes(filters.isbn))) return false;
          if (filters.title && !(book.title?.toLowerCase().includes(filters.title.toLowerCase()))) return false;
          if (filters.category && !(book.category?.toLowerCase() === filters.category.toLowerCase())) return false;
          if (filters.publisherName && !(book.publisherName?.toLowerCase().includes(filters.publisherName.toLowerCase()))) return false;
          if (filters.authorName) {
            const authors = typeof book.authors === 'string' ? book.authors : (Array.isArray(book.authors) ? book.authors.join(' ') : '');
            if (!authors.toLowerCase().includes(filters.authorName.toLowerCase())) return false;
          }
          return true;
        };

        const filtered = allBooks.filter(matches);
        console.log(`Found ${allBooks.length} total books, ${filtered.length} match all filters`);
        setBooks(filtered);
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
    setPage(1); // reset to first page on any filter change
  };

  const clearFilters = () => {
    setFilters({ isbn: '', title: '', category: '', publisherName: '', authorName: '' });
    setSearchParams({});
    setPage(1);
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
                <option key={author.authorId ?? author.author_ID ?? author.id} value={author.name}>{author.name}</option>
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
        <>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            {(() => {
              const total = books.length;
              const start = (page - 1) * pageSize;
              const end = Math.min(start + pageSize, total);
              const paged = books.slice(start, end);
              // If current page is out of range after filtering, reset to first page
              if (paged.length === 0 && total > 0) {
                setPage(1);
                return null;
              }
              return paged.map((book) => (
                <BookCard key={book.isbn} book={book} />
              ));
            })()}
          </div>

          {/* Pagination Controls */}
          <div className="flex items-center justify-between mt-6">
            <div className="flex items-center gap-3">
              <span className="text-sm text-gray-600">Show</span>
              <select
                value={pageSize}
                onChange={(e) => { setPageSize(parseInt(e.target.value, 10)); setPage(1); }}
                className="input-field w-24"
              >
                {pageSizes.map(s => (
                  <option key={s} value={s}>{s}</option>
                ))}
              </select>
              <span className="text-sm text-gray-600">per page</span>
            </div>

            <div className="flex items-center gap-2">
              {(() => {
                const total = books.length;
                const totalPages = Math.max(1, Math.ceil(total / pageSize));
                const pages = Array.from({ length: totalPages }, (_, i) => i + 1);

                return (
                  <>
                    <button
                      onClick={() => setPage(p => Math.max(1, p - 1))}
                      className="px-3 py-1 bg-white border rounded disabled:opacity-50"
                      disabled={page <= 1}
                    >
                      Prev
                    </button>

                    {pages.map(pn => (
                      <button
                        key={pn}
                        onClick={() => setPage(pn)}
                        className={`px-3 py-1 border rounded ${pn === page ? 'bg-primary-600 text-white' : 'bg-white'}`}
                      >
                        {pn}
                      </button>
                    ))}

                    <button
                      onClick={() => setPage(p => Math.min(totalPages, p + 1))}
                      className="px-3 py-1 bg-white border rounded disabled:opacity-50"
                      disabled={page >= totalPages}
                    >
                      Next
                    </button>
                  </>
                );
              })()}
            </div>
          </div>
        </>
      )}
    </div>
  );
}

export default ShopPage;