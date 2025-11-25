const express = require('express');
const axios = require('axios');
const router = express.Router();
const services = require('../config/services');
const { asyncHandler } = require('../middleware/errorHandler');
const authMiddleware = require('../middleware/authMiddleware');

// Initiate payment
router.post('/initiate', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.catalog.url}/api/payments/initiate`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Verify payment
router.post('/verify', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.catalog.url}/api/payments/verify`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Get payment status
router.get('/:id/status', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.catalog.url}/api/payments/${req.params.id}/status`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Get payment history
router.get('/', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.catalog.url}/api/payments`,
    {
      headers: { Authorization: req.headers.authorization },
      params: req.query,
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Refund payment
router.post('/:id/refund', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.catalog.url}/api/payments/${req.params.id}/refund`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

module.exports = router;
