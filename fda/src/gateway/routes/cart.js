const express = require('express');
const axios = require('axios');
const router = express.Router();
const services = require('../config/services');
const { asyncHandler } = require('../middleware/errorHandler');
const authMiddleware = require('../middleware/authMiddleware');

// Create cart for user
router.post('/', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.cart.url}/api/cart`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.cart.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Get cart by ID
router.get('/:cartId', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.cart.url}/api/cart/${req.params.cartId}`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.cart.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Convenience route: Add item to user's cart (creates cart if needed)
// Must be BEFORE /:cartId/items to avoid route conflict
router.post('/:userId/add', authMiddleware, asyncHandler(async (req, res) => {
  try {
    // Try to add to existing cart (userId is used as cartId)
    const response = await axios.post(
      `${services.cart.url}/api/cart/${req.params.userId}/items`,
      req.body,
      {
        headers: { Authorization: req.headers.authorization },
        timeout: services.cart.timeout
      }
    );
    res.status(response.status).json(response.data);
  } catch (error) {
    if (error.response && error.response.status === 404) {
      // Cart doesn't exist, create it first
      try {
        await axios.post(
          `${services.cart.url}/api/cart`,
          { userId: req.params.userId },
          {
            headers: { Authorization: req.headers.authorization },
            timeout: services.cart.timeout
          }
        );
        // Now add the item
        const response = await axios.post(
          `${services.cart.url}/api/cart/${req.params.userId}/items`,
          req.body,
          {
            headers: { Authorization: req.headers.authorization },
            timeout: services.cart.timeout
          }
        );
        res.status(response.status).json(response.data);
      } catch (createError) {
        throw createError;
      }
    } else {
      throw error;
    }
  }
}));

// Add item to cart
router.post('/:cartId/items', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.cart.url}/api/cart/${req.params.cartId}/items`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.cart.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Remove item from cart
router.delete('/:cartId/items/:itemId', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.delete(
    `${services.cart.url}/api/cart/${req.params.cartId}/items/${req.params.itemId}`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.cart.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

module.exports = router;
