import React, { useState, useEffect } from 'react';
import { bookApi } from '../../api/bookApi';
import { publisherApi } from '../../api/publisherApi';
import { authorApi } from '../../api/authorApi';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import toast from 'react-hot-toast';
import LoadingSpinner from '../Common/LoadingSpinner';
import { FiEdit2, FiTrash2, FiPlus, FiX, FiUpload } from 'react-icons/fi';

const bookSchema = Yup.object({
  isbn: Yup.string().required('ISBN is required'),
  title: Yup.string().required('Title is required'),
  description: Yup.string(),
  pubYear: Yup.number().min(1000).max(new Date().getFullYear()),
  price: Yup.number().min(0).required('Price is required'),
  category: Yup.string().oneOf(['Science', 'Art', 'Religion', 'History', 'Geography']).required(),
  stockQty: Yup.number().min(0).required('Stock quantity is required'),
  threshold: Yup.number().min(0).required('Threshold is required'),
  publisherId: Yup.number().required('Publisher is required'),
});

function BookManagement() {
  const [books, setBooks] = useState([]);
  const [publishers, setPublishers] = useState([]);
  const [authors, setAuthors] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingBook, setEditingBook] = useState(null);
  const [selectedAuthors, setSelectedAuthors] = useState([]);
  const [existingAuthors, setExistingAuthors] = useState([]);
  const [bookPhoto, setBookPhoto] = useState(null);
  const [photoPreview, setPhotoPreview] = useState(null);

  // Helper to get a stable author identifier (supports many API naming variants)
  const getAuthorId = (author) => author?.authorId ?? author?.author_ID ?? author?.Author_ID ?? author?.AuthorID ?? author?.authorID ?? author?.id ?? author?._id ?? author?.ID;

  const categories = ['Science', 'Art', 'Religion', 'History', 'Geography'];

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      const [booksRes, publishersRes, authorsRes] = await Promise.all([
        bookApi.getAllBooks(),
        publisherApi.getAllPublishers(),
        authorApi.getAllAuthors(),
      ]);
      setBooks(booksRes.data);
      setPublishers(publishersRes.data);
      // Normalize authors so each author has an `authorId` field
      const normalizedAuthors = (authorsRes.data || []).map((a, idx) => {
        const id = a?.authorId ?? a?.author_ID ?? a?.Author_ID ?? a?.AuthorID ?? a?.authorID ?? a?.id ?? a?._id ?? a?.ID;
        return {
          ...a,
          authorId: id
        };
      });
      const missingIdAuthors = normalizedAuthors.filter(a => a.authorId == null);
      if (missingIdAuthors.length) {
        console.warn('[BookManagement] Received authors without an id from API:', missingIdAuthors);
      }
      console.debug('[BookManagement] normalizedAuthors:', normalizedAuthors);
      setAuthors(normalizedAuthors);
    } catch (error) {
      toast.error('Failed to fetch data');
    } finally {
      setLoading(false);
    }
  };

  // Parse existing authors from the book's authors string
  const parseExistingAuthors = (authorsString) => {
    if (!authorsString) return [];
    
    const authorNames = authorsString.split(',').map(name => name.trim());
    const authorIds = authors
      .filter(author => authorNames.includes(author.name))
      .map(author => String(getAuthorId(author)));
    
    return authorIds;
  };

  const handlePhotoChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      if (file.size > 5 * 1024 * 1024) {
        toast.error('File size must be less than 5MB');
        return;
      }
      if (!['image/jpeg', 'image/png', 'image/webp'].includes(file.type)) {
        toast.error('Only JPG, PNG, and WEBP images are allowed');
        return;
      }
      setBookPhoto(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setPhotoPreview(reader.result);
      };
      reader.readAsDataURL(file);
    }
  };

  const uploadPhoto = async (isbn) => {
    if (!bookPhoto) return;

    const formData = new FormData();
    formData.append('ISBN', isbn);
    formData.append('image', bookPhoto);

    try {
      await bookApi.uploadBookPhoto(formData);
      toast.success('Photo uploaded successfully');
    } catch (error) {
      toast.error('Failed to upload photo');
      throw error;
    }
  };

  const handleSubmit = async (values, { setSubmitting, resetForm }) => {
    try {
      if (editingBook) {
        await bookApi.updateBook(values);
        
        if (bookPhoto) {
          await uploadPhoto(values.isbn);
        }

        if (selectedAuthors.length > 0) {
          for (const authorIdStr of selectedAuthors) {
            const authorIdToSend = isNaN(Number(authorIdStr)) ? authorIdStr : Number(authorIdStr);
            try {
              await bookApi.addBookAuthors({ isbn: values.isbn, authorId: authorIdToSend });
            } catch (error) {
              console.log('Author might already be linked');
            }
          }
        }

        toast.success('Book updated successfully');
      } else {
        await bookApi.createBook(values);
        
        if (bookPhoto) {
          await uploadPhoto(values.isbn);
        }

        if (selectedAuthors.length > 0) {
          for (const authorIdStr of selectedAuthors) {
            const authorIdToSend = isNaN(Number(authorIdStr)) ? authorIdStr : Number(authorIdStr);
            await bookApi.addBookAuthors({ isbn: values.isbn, authorId: authorIdToSend });
          }
        }

        toast.success('Book created successfully');
      }
      
      resetForm();
      setShowModal(false);
      setEditingBook(null);
      setSelectedAuthors([]);
      setExistingAuthors([]);
      setBookPhoto(null);
      setPhotoPreview(null);
      fetchData();
    } catch (error) {
      toast.error(error.response?.data?.message || 'Operation failed');
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (isbn) => {
    if (!window.confirm('Are you sure you want to delete this book?')) return;
    
    try {
      await bookApi.deleteBook(isbn);
      toast.success('Book deleted successfully');
      fetchData();
    } catch (error) {
      toast.error('Failed to delete book');
    }
  };

  const handleRemoveAuthor = async (authorId) => {
    if (!editingBook) return;
    
    if (!window.confirm('Remove this author from the book?')) return;

    const idToSend = isNaN(Number(authorId)) ? authorId : Number(authorId);

    try {
      await bookApi.removeBookAuthors({ 
        isbn: editingBook.isbn, 
        authorId: idToSend
      });
      
      setExistingAuthors(prev => prev.filter(id => id !== String(authorId)));
      toast.success('Author removed successfully');
      fetchData();
    } catch (error) {
      toast.error('Failed to remove author');
    }
  };

  const openModal = (book = null) => {
    setEditingBook(book);
    setSelectedAuthors([]);
    setBookPhoto(null);
    setPhotoPreview(book?.bookPhoto ? `data:image/jpeg;base64,${book.bookPhoto}` : null);
    
    if (book) {
      const existingAuthorIds = parseExistingAuthors(book.authors);
      setExistingAuthors(existingAuthorIds);
    } else {
      setExistingAuthors([]);
    }
    
    setShowModal(true);
  };

  const closeModal = () => {
    setShowModal(false);
    setEditingBook(null);
    setSelectedAuthors([]);
    setExistingAuthors([]);
    setBookPhoto(null);
    setPhotoPreview(null);
  };

  const toggleAuthor = (authorId) => {
    console.debug('[BookManagement] toggleAuthor called with:', authorId);
    if (authorId == null) { console.debug('[BookManagement] toggleAuthor ignored null/undefined'); return; } // ignore undefined/null ids
    const idStr = String(authorId);
    setSelectedAuthors(prev => {
      console.debug('[BookManagement] prev selectedAuthors:', prev);
      const result = prev.includes(idStr) ? prev.filter(id => id !== idStr) : [...prev, idStr];
      console.debug('[BookManagement] new selectedAuthors:', result);
      return result;
    });
  };

  const excludedAuthorsMissingId = [];
  const availableAuthors = authors.filter(author => {
    const id = getAuthorId(author);
    if (id == null) {
      excludedAuthorsMissingId.push(author);
      return false;
    }
    return !existingAuthors.includes(String(id));
  });
  if (excludedAuthorsMissingId.length) {
    console.warn('[BookManagement] Excluding authors without id from selection list:', excludedAuthorsMissingId);
  }
  console.debug('[BookManagement] availableAuthors ids:', availableAuthors.map(a => String(getAuthorId(a))));
  console.debug('[BookManagement] existingAuthors:', existingAuthors);
  console.debug('[BookManagement] selectedAuthors:', selectedAuthors);

  const getExistingAuthorsList = () => {
    return authors.filter(author => {
      const id = getAuthorId(author);
      return id != null && existingAuthors.includes(String(id));
    });
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex justify-between items-center mb-8">
        <h1 className="text-4xl font-bold text-gray-800">Book Management</h1>
        <button onClick={() => openModal()} className="btn-primary flex items-center gap-2">
          <FiPlus /> Add Book
        </button>
      </div>

      <div className="bg-white rounded-lg shadow-md overflow-x-auto">
        <table className="w-full">
          <thead className="bg-gray-100">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase">ISBN</th>
              <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase">Title</th>
              <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase">Category</th>
              <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase">Price</th>
              <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase">Stock</th>
              <th className="px-6 py-3 text-right text-xs font-semibold text-gray-600 uppercase">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {books.map((book) => (
              <tr key={book.isbn} className="hover:bg-gray-50">
                <td className="px-6 py-4 text-sm text-gray-600">{book.isbn}</td>
                <td className="px-6 py-4 text-sm font-medium text-gray-900">{book.title}</td>
                <td className="px-6 py-4 text-sm">
                  <span className="px-2 py-1 rounded-full text-xs font-semibold bg-primary-100 text-primary-800">
                    {book.category}
                  </span>
                </td>
                <td className="px-6 py-4 text-sm text-gray-800">${book.price}</td>
                <td className="px-6 py-4 text-sm">
                  <span className={book.stockQty < book.threshold ? 'text-red-600 font-bold' : 'text-gray-800'}>
                    {book.stockQty}
                  </span>
                </td>
                <td className="px-6 py-4 text-right text-sm">
                  <button
                    onClick={() => openModal(book)}
                    className="text-blue-600 hover:text-blue-800 mr-4"
                  >
                    <FiEdit2 className="inline" /> Edit
                  </button>
                  <button
                    onClick={() => handleDelete(book.isbn)}
                    className="text-red-600 hover:text-red-800"
                  >
                    <FiTrash2 className="inline" /> Delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {showModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4 overflow-y-auto">
          <div className="bg-white rounded-lg shadow-xl max-w-4xl w-full p-6 my-8 max-h-[90vh] overflow-y-auto">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-2xl font-bold text-gray-800">
                {editingBook ? 'Edit Book' : 'Add New Book'}
              </h2>
              <button onClick={closeModal} className="text-gray-500 hover:text-gray-700">
                <FiX size={24} />
              </button>
            </div>

            <Formik
              initialValues={{
                isbn: editingBook?.isbn || '',
                title: editingBook?.title || '',
                description: editingBook?.description || '',
                pubYear: editingBook?.pubYear || new Date().getFullYear(),
                price: editingBook?.price || 0,
                category: editingBook?.category || 'Science',
                stockQty: editingBook?.stockQty || 0,
                threshold: editingBook?.threshold || 10,
                publisherId: editingBook?.publisherId || publishers[0]?.publisherId || '',
              }}
              validationSchema={bookSchema}
              onSubmit={handleSubmit}
              enableReinitialize
            >
              {({ isSubmitting }) => (
                <Form className="space-y-4">
                  <div className="grid md:grid-cols-2 gap-4">
                    <div>
                      <label className="block text-sm font-semibold text-gray-700 mb-2">ISBN *</label>
                      <Field
                        type="text"
                        name="isbn"
                        disabled={!!editingBook}
                        className="input-field disabled:bg-gray-100"
                      />
                      <ErrorMessage name="isbn" component="div" className="text-red-600 text-sm mt-1" />
                    </div>

                    <div>
                      <label className="block text-sm font-semibold text-gray-700 mb-2">Title *</label>
                      <Field type="text" name="title" className="input-field" />
                      <ErrorMessage name="title" component="div" className="text-red-600 text-sm mt-1" />
                    </div>

                    <div className="md:col-span-2">
                      <label className="block text-sm font-semibold text-gray-700 mb-2">Description</label>
                      <Field as="textarea" name="description" rows="3" className="input-field" />
                      <ErrorMessage name="description" component="div" className="text-red-600 text-sm mt-1" />
                    </div>

                    <div>
                      <label className="block text-sm font-semibold text-gray-700 mb-2">Publication Year</label>
                      <Field type="number" name="pubYear" className="input-field" />
                      <ErrorMessage name="pubYear" component="div" className="text-red-600 text-sm mt-1" />
                    </div>

                    <div>
                      <label className="block text-sm font-semibold text-gray-700 mb-2">Price *</label>
                      <Field type="number" step="0.01" name="price" className="input-field" />
                      <ErrorMessage name="price" component="div" className="text-red-600 text-sm mt-1" />
                    </div>

                    <div>
                      <label className="block text-sm font-semibold text-gray-700 mb-2">Category *</label>
                      <Field as="select" name="category" className="input-field">
                        {categories.map(cat => (
                          <option key={cat} value={cat}>{cat}</option>
                        ))}
                      </Field>
                      <ErrorMessage name="category" component="div" className="text-red-600 text-sm mt-1" />
                    </div>

                    <div>
                      <label className="block text-sm font-semibold text-gray-700 mb-2">Publisher *</label>
                      <Field as="select" name="publisherId" className="input-field">
                        <option value="">Select Publisher</option>
                        {publishers.map(pub => (
                          <option key={pub.publisherId} value={pub.publisherId}>
                            {pub.name}
                          </option>
                        ))}
                      </Field>
                      <ErrorMessage name="publisherId" component="div" className="text-red-600 text-sm mt-1" />
                    </div>

                    <div>
                      <label className="block text-sm font-semibold text-gray-700 mb-2">Stock Quantity *</label>
                      <Field type="number" name="stockQty" className="input-field" />
                      <ErrorMessage name="stockQty" component="div" className="text-red-600 text-sm mt-1" />
                    </div>

                    <div>
                      <label className="block text-sm font-semibold text-gray-700 mb-2">Threshold *</label>
                      <Field type="number" name="threshold" className="input-field" />
                      <ErrorMessage name="threshold" component="div" className="text-red-600 text-sm mt-1" />
                    </div>
                  </div>

                  <div className="border-t pt-4">
                    <label className="block text-sm font-semibold text-gray-700 mb-2">
                      <FiUpload className="inline mr-2" />
                      Book Photo (JPG, PNG, WEBP - Max 5MB)
                    </label>
                    <input
                      type="file"
                      accept="image/jpeg,image/png,image/webp"
                      onChange={handlePhotoChange}
                      className="input-field"
                    />
                    {photoPreview && (
                      <div className="mt-4">
                        <img src={photoPreview} alt="Preview" className="h-48 object-cover rounded-lg" />
                      </div>
                    )}
                  </div>

                  {editingBook && existingAuthors.length > 0 && (
                    <div className="border-t pt-4">
                      <label className="block text-sm font-semibold text-gray-700 mb-2">
                        Current Authors ({existingAuthors.length})
                      </label>
                      <div className="flex flex-wrap gap-2">
                        {getExistingAuthorsList().map(author => (
                          <div
                            key={String(getAuthorId(author))}
                            className="bg-green-100 text-green-800 px-3 py-2 rounded-lg flex items-center gap-2"
                          >
                            <span className="font-medium">{author.name}</span>
                            <button
                              type="button"
                              onClick={() => handleRemoveAuthor(String(getAuthorId(author)))}
                              className="text-red-600 hover:text-red-800"
                            >
                              <FiX size={16} />
                            </button>
                          </div>
                        ))} 
                      </div>
                    </div>
                  )}

                  <div className="border-t pt-4">
                    <label className="block text-sm font-semibold text-gray-700 mb-2">
                      {editingBook ? 'Add More Authors' : 'Select Authors'}
                    </label>
                    {availableAuthors.length > 0 ? (
                      <>
                        <div className="grid grid-cols-2 md:grid-cols-3 gap-2 max-h-40 overflow-y-auto border rounded-lg p-3">
                          {availableAuthors.map(author => (
                            <label key={String(getAuthorId(author))} className="flex items-center gap-2 cursor-pointer hover:bg-gray-50 p-2 rounded" htmlFor={`author-${String(getAuthorId(author))}`}>
                              <input
                                id={`author-${String(getAuthorId(author))}`}
                                type="checkbox"
                                checked={selectedAuthors.includes(String(getAuthorId(author)))}
                                onChange={(e) => { e.stopPropagation(); console.debug('[BookManagement] input onChange', getAuthorId(author)); toggleAuthor(getAuthorId(author)); }}
                                className="w-4 h-4"
                              />
                              <span className="text-sm">{author.name}</span>
                            </label>
                          ))}
                        </div>
                        <p className="text-sm text-gray-500 mt-2">
                          {selectedAuthors.length} new author(s) to be added
                        </p>
                        <pre className="text-xs text-gray-500 mt-2">Selected IDs: {JSON.stringify(selectedAuthors)}</pre>
                      </>
                    ) : (
                      <p className="text-sm text-gray-500 p-4 bg-gray-50 rounded-lg">
                        {editingBook 
                          ? 'All available authors are already assigned to this book.'
                          : 'No authors available. Please create authors first.'}
                      </p>
                    )}
                  </div>

                  <div className="flex gap-3 pt-4">
                    <button type="submit" disabled={isSubmitting} className="btn-primary flex-1">
                      {isSubmitting ? 'Saving...' : editingBook ? 'Update Book' : 'Create Book'}
                    </button>
                    <button type="button" onClick={closeModal} className="btn-secondary flex-1">
                      Cancel
                    </button>
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

export default BookManagement;