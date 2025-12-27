// src/components/Customer/ProfilePage.jsx
import React, { useState, useEffect } from 'react';
import { useAuth } from '../../context/AuthContext';
import { userApi } from '../../api/userApi';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import toast from 'react-hot-toast';
import LoadingSpinner from '../Common/LoadingSpinner';

const profileSchema = Yup.object({
  firstName: Yup.string(),
  lastName: Yup.string(),
  email: Yup.string().email('Invalid email'),
  phone: Yup.string(),
  address: Yup.string(),
});

const passwordSchema = Yup.object({
  oldPassword: Yup.string().required('Old password is required'),
  newPassword: Yup.string().min(6, 'Password must be at least 6 characters').required('New password is required'),
});

function ProfilePage() {
  const { user, updateProfile, changePassword } = useAuth();
  const [activeTab, setActiveTab] = useState('profile');
  const [userDetails, setUserDetails] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchUserDetails();
  }, [user?.userid]);

  const fetchUserDetails = async () => {
    if (!user?.userid) return;
    
    try {
      setLoading(true);
      const response = await userApi.getUserById(user.userid);
      setUserDetails(response.data);
    } catch (error) {
      console.error('Error fetching user details:', error);
      toast.error('Failed to load profile data');
    } finally {
      setLoading(false);
    }
  };

  const handleProfileSubmit = async (values, { setSubmitting }) => {
    try {
      await updateProfile({ 
        userId: user.userid,
        firstName: values.firstName,
        lastName: values.lastName,
        email: values.email,
        phone: values.phone,
        address: values.address
      });
      // Refresh user details after update
      await fetchUserDetails();
    } catch (error) {
      // Error handled in context
    } finally {
      setSubmitting(false);
    }
  };

  const handlePasswordSubmit = async (values, { setSubmitting, resetForm }) => {
    try {
      await changePassword({ 
        userId: user.userid, 
        oldPassword: values.oldPassword,
        newPassword: values.newPassword
      });
      resetForm();
    } catch (error) {
      // Error handled in context
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) return <LoadingSpinner />;

  if (!userDetails) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="text-center">
          <p className="text-red-600">Failed to load profile data</p>
          <button onClick={fetchUserDetails} className="btn-primary mt-4">
            Retry
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8 max-w-4xl">
      <h1 className="text-4xl font-bold mb-8 text-gray-800">My Profile</h1>

      {/* User Info Card */}
      <div className="bg-gradient-to-r from-primary-500 to-purple-500 text-white rounded-lg shadow-lg p-6 mb-8">
        <div className="flex items-center gap-4">
          <div className="w-20 h-20 bg-white rounded-full flex items-center justify-center text-primary-600 text-3xl font-bold">
            {userDetails.username?.charAt(0).toUpperCase() || 'U'}
          </div>
          <div>
            <h2 className="text-2xl font-bold">{userDetails.username}</h2>
            <p className="opacity-90">{userDetails.email}</p>
            <p className="text-sm opacity-75 mt-1">
              Role: {userDetails.role} | User ID: {userDetails.userid}
            </p>
          </div>
        </div>
      </div>

      {/* Tabs */}
      <div className="flex gap-4 mb-6 border-b">
        <button
          onClick={() => setActiveTab('profile')}
          className={`px-4 py-2 font-semibold transition ${
            activeTab === 'profile'
              ? 'text-primary-600 border-b-2 border-primary-600'
              : 'text-gray-600 hover:text-gray-800'
          }`}
        >
          Profile Information
        </button>
        <button
          onClick={() => setActiveTab('password')}
          className={`px-4 py-2 font-semibold transition ${
            activeTab === 'password'
              ? 'text-primary-600 border-b-2 border-primary-600'
              : 'text-gray-600 hover:text-gray-800'
          }`}
        >
          Change Password
        </button>
      </div>

      {activeTab === 'profile' && (
        <div className="bg-white rounded-lg shadow-md p-6">
          <h3 className="text-xl font-bold mb-4 text-gray-800">Update Profile Information</h3>
          <Formik
            initialValues={{
              firstName: userDetails.first_Name || '',
              lastName: userDetails.last_Name || '',
              email: userDetails.email || '',
              phone: userDetails.phone || '',
              address: userDetails.address || '',
            }}
            validationSchema={profileSchema}
            onSubmit={handleProfileSubmit}
            enableReinitialize
          >
            {({ isSubmitting }) => (
              <Form className="space-y-4">
                <div className="grid md:grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-2">
                      First Name
                    </label>
                    <Field type="text" name="firstName" className="input-field" />
                    <ErrorMessage name="firstName" component="div" className="text-red-600 text-sm mt-1" />
                  </div>
                  <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-2">
                      Last Name
                    </label>
                    <Field type="text" name="lastName" className="input-field" />
                    <ErrorMessage name="lastName" component="div" className="text-red-600 text-sm mt-1" />
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-2">Email</label>
                  <Field type="email" name="email" className="input-field" />
                  <ErrorMessage name="email" component="div" className="text-red-600 text-sm mt-1" />
                </div>

                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-2">Phone</label>
                  <Field type="tel" name="phone" className="input-field" />
                  <ErrorMessage name="phone" component="div" className="text-red-600 text-sm mt-1" />
                </div>

                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-2">Address</label>
                  <Field as="textarea" name="address" rows="3" className="input-field" />
                  <ErrorMessage name="address" component="div" className="text-red-600 text-sm mt-1" />
                </div>

                <button type="submit" disabled={isSubmitting} className="btn-primary w-full md:w-auto">
                  {isSubmitting ? 'Updating...' : 'Update Profile'}
                </button>
              </Form>
            )}
          </Formik>
        </div>
      )}

      {activeTab === 'password' && (
        <div className="bg-white rounded-lg shadow-md p-6">
          <h3 className="text-xl font-bold mb-4 text-gray-800">Change Password</h3>
          <Formik
            initialValues={{ oldPassword: '', newPassword: '' }}
            validationSchema={passwordSchema}
            onSubmit={handlePasswordSubmit}
          >
            {({ isSubmitting }) => (
              <Form className="space-y-4">
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-2">
                    Old Password
                  </label>
                  <Field type="password" name="oldPassword" className="input-field" />
                  <ErrorMessage name="oldPassword" component="div" className="text-red-600 text-sm mt-1" />
                </div>

                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-2">
                    New Password
                  </label>
                  <Field type="password" name="newPassword" className="input-field" />
                  <ErrorMessage name="newPassword" component="div" className="text-red-600 text-sm mt-1" />
                  <p className="text-sm text-gray-500 mt-1">
                    Password must be at least 6 characters long
                  </p>
                </div>

                <button type="submit" disabled={isSubmitting} className="btn-primary w-full md:w-auto">
                  {isSubmitting ? 'Changing...' : 'Change Password'}
                </button>
              </Form>
            )}
          </Formik>
        </div>
      )}
    </div>
  );
}

export default ProfilePage;