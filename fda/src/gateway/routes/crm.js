const express = require('express');
const axios = require('axios');
const router = express.Router();
const services = require('../config/services');
const { asyncHandler } = require('../middleware/errorHandler');
const authMiddleware = require('../middleware/authMiddleware');

// Get all customers (admin only)
router.get('/customers', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.crm.url}/api/customer`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Get customer by ID
router.get('/customers/:id', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.crm.url}/api/customer/${req.params.id}`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Create customer
router.post('/customers', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.crm.url}/api/customer`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Update customer
router.put('/customers/:id', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.put(
    `${services.crm.url}/api/customer/${req.params.id}`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Delete customer
router.delete('/customers/:id', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.delete(
    `${services.crm.url}/api/customer/${req.params.id}`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// ===== UserProfile Endpoints =====

// Get user profile by userId
router.get('/userprofile/by-user/:userId', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.crm.url}/api/userprofile/by-user/${req.params.userId}`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Get user addresses
router.get('/userprofile/by-user/:userId/addresses', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.crm.url}/api/userprofile/by-user/${req.params.userId}/addresses`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Add user address
router.post('/userprofile/by-user/:userId/addresses', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.crm.url}/api/userprofile/by-user/${req.params.userId}/addresses`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Update user address
router.put('/userprofile/by-user/:userId/addresses/:addressId', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.put(
    `${services.crm.url}/api/userprofile/by-user/${req.params.userId}/addresses/${req.params.addressId}`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Delete user address
router.delete('/userprofile/by-user/:userId/addresses/:addressId', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.delete(
    `${services.crm.url}/api/userprofile/by-user/${req.params.userId}/addresses/${req.params.addressId}`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Update profile image
router.put('/userprofile/by-user/:userId/profile-image', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.put(
    `${services.crm.url}/api/userprofile/by-user/${req.params.userId}/profile-image`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Update food preferences
router.put('/userprofile/by-user/:userId/food-preferences', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.put(
    `${services.crm.url}/api/userprofile/by-user/${req.params.userId}/food-preferences`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Update profile (name, email)
router.put('/userprofile/by-user/:userId', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.put(
    `${services.crm.url}/api/userprofile/by-user/${req.params.userId}`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.crm.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

module.exports = router;
