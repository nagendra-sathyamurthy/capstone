const express = require('express');
const axios = require('axios');
const router = express.Router();
const services = require('../config/services');
const { asyncHandler } = require('../middleware/errorHandler');
const authMiddleware = require('../middleware/authMiddleware');

// Get cart
router.get('/', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.cart.url}/api/cart`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.cart.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Add item to cart
router.post('/items', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.cart.url}/api/cart/items`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.cart.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Update cart item quantity
router.put('/items/:itemId', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.put(
    `${services.cart.url}/api/cart/items/${req.params.itemId}`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.cart.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Remove item from cart
router.delete('/items/:itemId', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.delete(
    `${services.cart.url}/api/cart/items/${req.params.itemId}`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.cart.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Clear cart
router.delete('/', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.delete(
    `${services.cart.url}/api/cart`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.cart.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Apply coupon
router.post('/coupon', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.cart.url}/api/cart/coupon`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.cart.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Remove coupon
router.delete('/coupon', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.delete(
    `${services.cart.url}/api/cart/coupon`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.cart.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

module.exports = router;
