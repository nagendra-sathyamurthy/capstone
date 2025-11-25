const express = require('express');
const axios = require('axios');
const router = express.Router();
const services = require('../config/services');
const { asyncHandler } = require('../middleware/errorHandler');
const authMiddleware = require('../middleware/authMiddleware');

// Get customer profile
router.get('/profile', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.crm.url}/api/customers/profile`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Update customer profile
router.put('/profile', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.put(
    `${services.crm.url}/api/customers/profile`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Get customer addresses
router.get('/addresses', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.crm.url}/api/customers/addresses`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Add customer address
router.post('/addresses', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.crm.url}/api/customers/addresses`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Update customer address
router.put('/addresses/:id', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.put(
    `${services.crm.url}/api/customers/addresses/${req.params.id}`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Delete customer address
router.delete('/addresses/:id', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.delete(
    `${services.crm.url}/api/customers/addresses/${req.params.id}`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Get order history
router.get('/orders', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.crm.url}/api/customers/orders`,
    {
      headers: { Authorization: req.headers.authorization },
      params: req.query,
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Get favorites
router.get('/favorites', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.crm.url}/api/customers/favorites`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Add to favorites
router.post('/favorites', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.crm.url}/api/customers/favorites`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Remove from favorites
router.delete('/favorites/:id', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.delete(
    `${services.crm.url}/api/customers/favorites/${req.params.id}`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// ===== NEW UserProfile Endpoints =====

// Get user profile by userId
router.get('/api/userprofile/by-user/:userId', asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.crm.url}/api/userprofile/by-user/${req.params.userId}`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// ===== NEW UserProfile Endpoints =====

// Get user profile by userId
router.get('/userprofile/by-user/:userId', asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.crm.url}/api/UserProfile/by-user/${req.params.userId}`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Get user addresses
router.get('/userprofile/by-user/:userId/addresses', asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.crm.url}/api/UserProfile/by-user/${req.params.userId}/addresses`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Add user address
router.post('/userprofile/by-user/:userId/addresses', asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.crm.url}/api/UserProfile/by-user/${req.params.userId}/addresses`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Update user address
router.put('/userprofile/by-user/:userId/addresses/:addressId', asyncHandler(async (req, res) => {
  const response = await axios.put(
    `${services.crm.url}/api/UserProfile/by-user/${req.params.userId}/addresses/${req.params.addressId}`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Delete user address
router.delete('/userprofile/by-user/:userId/addresses/:addressId', asyncHandler(async (req, res) => {
  const response = await axios.delete(
    `${services.crm.url}/api/UserProfile/by-user/${req.params.userId}/addresses/${req.params.addressId}`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Update profile image
router.put('/userprofile/by-user/:userId/profile-image', asyncHandler(async (req, res) => {
  const response = await axios.put(
    `${services.crm.url}/api/UserProfile/by-user/${req.params.userId}/profile-image`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Update food preferences
router.put('/userprofile/by-user/:userId/food-preferences', asyncHandler(async (req, res) => {
  const response = await axios.put(
    `${services.crm.url}/api/UserProfile/by-user/${req.params.userId}/food-preferences`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Update profile (name, email)
router.put('/userprofile/by-user/:userId', asyncHandler(async (req, res) => {
  const response = await axios.put(
    `${services.crm.url}/api/UserProfile/by-user/${req.params.userId}`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

module.exports = router;
