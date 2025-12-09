const express = require('express');
const axios = require('axios');
const router = express.Router();
const services = require('../config/services');
const { asyncHandler } = require('../middleware/errorHandler');
const authMiddleware = require('../middleware/authMiddleware');

// All operator routes require authentication
router.use(authMiddleware);

// ===== Order Management =====

// Get pending orders for restaurant
router.get('/orders/pending/:restaurantId', asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.catalog.url}/api/operator/orders/pending/${req.params.restaurantId}`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Get orders ready for pickup
router.get('/orders/ready/:restaurantId', asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.catalog.url}/api/operator/orders/ready/${req.params.restaurantId}`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Get order details
router.get('/orders/:orderId', asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.catalog.url}/api/operator/orders/${req.params.orderId}`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Accept an order
router.post('/orders/:orderId/accept', asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.catalog.url}/api/operator/orders/${req.params.orderId}/accept`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Decline an order
router.post('/orders/:orderId/decline', asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.catalog.url}/api/operator/orders/${req.params.orderId}/decline`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Handle order packaging
router.post('/orders/:orderId/package', asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.catalog.url}/api/operator/orders/${req.params.orderId}/package`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Generate OTP for delivery agent handover
router.post('/orders/:orderId/generate-handover-otp', asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.catalog.url}/api/operator/orders/${req.params.orderId}/generate-handover-otp`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Verify OTP and handover order to delivery agent
router.post('/orders/handover', asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.catalog.url}/api/operator/orders/handover`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// ===== Inventory Management =====

// View kitchen inventory
router.get('/inventory/:restaurantId', asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.catalog.url}/api/operator/inventory/${req.params.restaurantId}`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Get low stock items
router.get('/inventory/:restaurantId/low-stock', asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.catalog.url}/api/operator/inventory/${req.params.restaurantId}/low-stock`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Update food item availability
router.patch('/inventory/:menuItemId/availability', asyncHandler(async (req, res) => {
  const response = await axios.patch(
    `${services.catalog.url}/api/operator/inventory/${req.params.menuItemId}/availability`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

module.exports = router;
