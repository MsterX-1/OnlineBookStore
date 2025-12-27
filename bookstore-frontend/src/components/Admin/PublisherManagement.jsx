// src/components/Admin/PublisherManagement.jsx
import React, { useState, useEffect } from 'react';
import { publisherApi } from '../../api/publisherApi';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import toast from 'react-hot-toast';
import LoadingSpinner from '../Common/LoadingSpinner';
import { FiEdit2, FiTrash2, FiPlus, FiX } from 'react-icons/fi';

const publisherSchema = Yup.object({
  name: Yup.string().required('Name is required'),
  address: Yup.string().required('Address is required'),
  phone: Yup.string().required('Phone is required'),
});

function PublisherManagement() {
  const [publishers, setPublishers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingPublisher, setEditingPublisher] = useState(null);

  useEffect(() => {
    fetchPublishers();
  }, []);

  const fetchPublishers = async () => {
    try {
      const response = await publisherApi.getAllPublishers();
      setPublishers(response.data);
    } catch (error) {
      toast.error('Failed to fetch publishers');
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (values, { setSubmitting, resetForm }) => {
    try {
      if (editingPublisher) {
        await publisherApi.updatePublisher({ publisherId: editingPublisher.publisherId, ...values });
        toast.success('Publisher updated successfully');
      } else {
        await publisherApi.createPublisher(values);
        toast.success('Publisher created successfully');
      }
      resetForm();
      setShowModal(false);
      setEditingPublisher(null);
      fetchPublishers();
    } catch (error) {
      toast.error(error.response?.data?.message || 'Operation failed');
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this publisher?')) return;
    
    try {
      await publisherApi.deletePublisher(id);
      toast.success('Publisher deleted successfully');
      fetchPublishers();
    } catch (error) {
      toast.error('Failed to delete publisher');
    }
  };

  const openModal = (publisher = null) => {
    setEditingPublisher(publisher);
    setShowModal(true);
  };

  const closeModal = () => {
    setShowModal(false);
    setEditingPublisher(null);
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex justify-between items-center mb-8">
        <h1 className="text-4xl font-bold text-gray-800">Publisher Management</h1>
        <button onClick={() => openModal()} className="btn-primary flex items-center gap-2">
          <FiPlus /> Add Publisher
        </button>
      </div>

      <div className="bg-white rounded-lg shadow-md overflow-hidden">
        <table className="w-full">
          <thead className="bg-gray-100">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase">Name</th>
              <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase">Address</th>
              <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase">Phone</th>
              <th className="px-6 py-3 text-right text-xs font-semibold text-gray-600 uppercase">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {publishers.map((publisher) => (
              <tr key={publisher.publisherId} className="hover:bg-gray-50">
                <td className="px-6 py-4 text-sm font-medium text-gray-900">{publisher.name}</td>
                <td className="px-6 py-4 text-sm text-gray-600">{publisher.address}</td>
                <td className="px-6 py-4 text-sm text-gray-600">{publisher.phone}</td>
                <td className="px-6 py-4 text-right text-sm">
                  <button
                    onClick={() => openModal(publisher)}
                    className="text-blue-600 hover:text-blue-800 mr-4"
                  >
                    <FiEdit2 className="inline" /> Edit
                  </button>
                  <button
                    onClick={() => handleDelete(publisher.publisherId)}
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
                {editingPublisher ? 'Edit Publisher' : 'Add Publisher'}
              </h2>
              <button onClick={closeModal} className="text-gray-500 hover:text-gray-700">
                <FiX size={24} />
              </button>
            </div>

            <Formik
              initialValues={{
                name: editingPublisher?.name || '',
                address: editingPublisher?.address || '',
                phone: editingPublisher?.phone || '',
              }}
              validationSchema={publisherSchema}
              onSubmit={handleSubmit}
            >
              {({ isSubmitting }) => (
                <Form className="space-y-4">
                  <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-2">Name</label>
                    <Field type="text" name="name" className="input-field" />
                    <ErrorMessage name="name" component="div" className="text-red-600 text-sm mt-1" />
                  </div>

                  <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-2">Address</label>
                    <Field type="text" name="address" className="input-field" />
                    <ErrorMessage name="address" component="div" className="text-red-600 text-sm mt-1" />
                  </div>

                  <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-2">Phone</label>
                    <Field type="text" name="phone" className="input-field" />
                    <ErrorMessage name="phone" component="div" className="text-red-600 text-sm mt-1" />
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

export default PublisherManagement;