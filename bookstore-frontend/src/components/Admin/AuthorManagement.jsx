// src/components/Admin/AuthorManagement.jsx
import React, { useState, useEffect } from 'react';
import { authorApi } from '../../api/authorApi';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import toast from 'react-hot-toast';
import LoadingSpinner from '../Common/LoadingSpinner';
import { FiEdit2, FiTrash2, FiPlus, FiX } from 'react-icons/fi';

const authorSchema = Yup.object({
  name: Yup.string().required('Name is required'),
});

function AuthorManagement() {
  const [authors, setAuthors] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingAuthor, setEditingAuthor] = useState(null);

  useEffect(() => {
    fetchAuthors();
  }, []);

  const fetchAuthors = async () => {
    try {
      const response = await authorApi.getAllAuthors();
      setAuthors(response.data);
    } catch (error) {
      toast.error('Failed to fetch authors');
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (values, { setSubmitting, resetForm }) => {
    try {
      if (editingAuthor) {
        await authorApi.updateAuthor({ id: editingAuthor.authorId, name: values.name });
        toast.success('Author updated successfully');
      } else {
        await authorApi.createAuthor(values);
        toast.success('Author created successfully');
      }
      resetForm();
      setShowModal(false);
      setEditingAuthor(null);
      fetchAuthors();
    } catch (error) {
      toast.error(error.response?.data?.message || 'Operation failed');
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this author?')) return;
    
    try {
      await authorApi.deleteAuthor(id);
      toast.success('Author deleted successfully');
      fetchAuthors();
    } catch (error) {
      toast.error('Failed to delete author');
    }
  };

  const openModal = (author = null) => {
    setEditingAuthor(author);
    setShowModal(true);
  };

  const closeModal = () => {
    setShowModal(false);
    setEditingAuthor(null);
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex justify-between items-center mb-8">
        <h1 className="text-4xl font-bold text-gray-800">Author Management</h1>
        <button onClick={() => openModal()} className="btn-primary flex items-center gap-2">
          <FiPlus /> Add Author
        </button>
      </div>

      <div className="bg-white rounded-lg shadow-md overflow-hidden">
        <table className="w-full">
          <thead className="bg-gray-100">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase">ID</th>
              <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase">Name</th>
              <th className="px-6 py-3 text-right text-xs font-semibold text-gray-600 uppercase">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {authors.map((author) => (
              <tr key={author.authorId} className="hover:bg-gray-50">
                <td className="px-6 py-4 text-sm text-gray-800">{author.authorId}</td>
                <td className="px-6 py-4 text-sm font-medium text-gray-900">{author.name}</td>
                <td className="px-6 py-4 text-right text-sm">
                  <button
                    onClick={() => openModal(author)}
                    className="text-blue-600 hover:text-blue-800 mr-4"
                  >
                    <FiEdit2 className="inline" /> Edit
                  </button>
                  <button
                    onClick={() => handleDelete(author.authorId)}
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

      {/* Modal */}
      {showModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg shadow-xl max-w-md w-full p-6">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-2xl font-bold text-gray-800">
                {editingAuthor ? 'Edit Author' : 'Add Author'}
              </h2>
              <button onClick={closeModal} className="text-gray-500 hover:text-gray-700">
                <FiX size={24} />
              </button>
            </div>

            <Formik
              initialValues={{ name: editingAuthor?.name || '' }}
              validationSchema={authorSchema}
              onSubmit={handleSubmit}
            >
              {({ isSubmitting }) => (
                <Form className="space-y-4">
                  <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-2">
                      Author Name
                    </label>
                    <Field
                      type="text"
                      name="name"
                      className="input-field"
                      placeholder="Enter author name"
                    />
                    <ErrorMessage name="name" component="div" className="text-red-600 text-sm mt-1" />
                  </div>

                  <div className="flex gap-3">
                    <button type="submit" disabled={isSubmitting} className="btn-primary flex-1">
                      {isSubmitting ? 'Saving...' : 'Save'}
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

export default AuthorManagement;