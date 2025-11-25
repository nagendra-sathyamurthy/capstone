const express = require('express');
const axios = require('axios');
const router = express.Router();
const services = require('../config/services');
const { asyncHandler } = require('../middleware/errorHandler');
const authMiddleware = require('../middleware/authMiddleware');

// Create order
router.post('/', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.order.url}/api/order`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.order.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Get order by ID
router.get('/:id', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.order.url}/api/order/${req.params.id}`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.order.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Get customer orders
router.get('/customer/:customerId', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.order.url}/api/order/customer/${req.params.customerId}`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.order.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Get all orders (for authenticated user)
router.get('/', authMiddleware, asyncHandler(async (req, res) => {
  // Extract customer ID from token or query params
  const customerId = req.query.customerId || req.user?.id;
  
  if (!customerId) {
    return res.status(400).json({ error: 'Customer ID is required' });
  }
  
  const response = await axios.get(
    `${services.order.url}/api/order/customer/${customerId}`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.order.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Update order status
router.patch('/:id/status', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.patch(
    `${services.order.url}/api/order/${req.params.id}/status`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.order.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Cancel order
router.post('/:id/cancel', authMiddleware, asyncHandler(async (req, res) => {
  // To cancel, we update status to Cancelled
  const response = await axios.patch(
    `${services.order.url}/api/order/${req.params.id}/status`,
    { status: 7 }, // OrderStatus.Cancelled = 7
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.order.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

module.exports = router;
