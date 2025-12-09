// Updated catalog.js with Restaurant and Operator routes
const express = require('express');
const axios = require('axios');
const router = express.Router();
const services = require('../config/services');
const { asyncHandler } = require('../middleware/errorHandler');
const authMiddleware = require('../middleware/authMiddleware');

// ============================================================================
// MENU ENDPOINTS - Forward to catalog service /api/menu
// ============================================================================

// GET /menu - Allow without auth for browsing
router.get('/menu', asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.catalog.url}/api/menu`,
    { 
      params: req.query,
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// POST /menu - Forward auth header if present
router.post('/menu', asyncHandler(async (req, res) => {
  const headers = {};
  if (req.headers.authorization) {
    headers.Authorization = req.headers.authorization;
  }
  
  const response = await axios.post(
    `${services.catalog.url}/api/menu`,
    req.body,
    { 
      timeout: services.catalog.timeout,
      headers: headers
    }
  );
  res.status(response.status).json(response.data);
}));

// GET /menu/:id
router.get('/menu/:id', authMiddleware, asyncHandler(async (req, res) => {
  const token = req.headers.authorization;
  const response = await axios.get(
    `${services.catalog.url}/api/menu/${req.params.id}`,
    { 
      timeout: services.catalog.timeout,
      headers: { Authorization: token }
    }
  );
  res.status(response.status).json(response.data);
}));

// PUT /menu/:id
router.put('/menu/:id', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.put(
    `${services.catalog.url}/api/menu/${req.params.id}`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// PATCH /menu/:id/availability
router.patch('/menu/:id/availability', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.patch(
    `${services.catalog.url}/api/menu/${req.params.id}/availability`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// DELETE /menu/:id
router.delete('/menu/:id', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.delete(
    `${services.catalog.url}/api/menu/${req.params.id}`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// POST /menu/bulk - Bulk create menu items
router.post('/menu/bulk', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.catalog.url}/api/menu/bulk`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// ============================================================================
// RESTAURANT ENDPOINTS - Forward to catalog service /api/restaurant
// ============================================================================

// POST /restaurant/register - Register new restaurant
router.post('/restaurant/register', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.catalog.url}/api/restaurant/register`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// GET /restaurant - Get active restaurants
router.get('/restaurant', asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.catalog.url}/api/restaurant`,
    { 
      params: req.query,
      timeout: services.catalog.timeout 
    }
  );
  res.status(response.status).json(response.data);
}));

// GET /restaurant/all - Get all restaurants (admin)
router.get('/restaurant/all', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.catalog.url}/api/restaurant/all`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// GET /restaurant/owner/:ownerId - Get restaurants by owner
router.get('/restaurant/owner/:ownerId', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.catalog.url}/api/restaurant/owner/${req.params.ownerId}`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// GET /restaurant/:id - Get restaurant by ID
router.get('/restaurant/:id', asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.catalog.url}/api/restaurant/${req.params.id}`,
    { timeout: services.catalog.timeout }
  );
  res.status(response.status).json(response.data);
}));

// PUT /restaurant/:id - Update restaurant
router.put('/restaurant/:id', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.put(
    `${services.catalog.url}/api/restaurant/${req.params.id}`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// PATCH /restaurant/:id/status - Update restaurant status
router.patch('/restaurant/:id/status', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.patch(
    `${services.catalog.url}/api/restaurant/${req.params.id}/status`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// PATCH /restaurant/:id/contact - Update contact info
router.patch('/restaurant/:id/contact', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.patch(
    `${services.catalog.url}/api/restaurant/${req.params.id}/contact`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// PATCH /restaurant/:id/address - Update address
router.patch('/restaurant/:id/address', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.patch(
    `${services.catalog.url}/api/restaurant/${req.params.id}/address`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// PATCH /restaurant/:id/hours - Update business hours
router.patch('/restaurant/:id/hours', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.patch(
    `${services.catalog.url}/api/restaurant/${req.params.id}/hours`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// DELETE /restaurant/:id - Delete restaurant
router.delete('/restaurant/:id', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.delete(
    `${services.catalog.url}/api/restaurant/${req.params.id}`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// ============================================================================
// OPERATOR INVENTORY ENDPOINTS - Forward to catalog service /api/operator
// ============================================================================

// GET /operator/inventory/:restaurantId - View kitchen inventory
router.get('/operator/inventory/:restaurantId', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.catalog.url}/api/operator/inventory/${req.params.restaurantId}`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// GET /operator/inventory/:restaurantId/low-stock - Get low stock items
router.get('/operator/inventory/:restaurantId/low-stock', authMiddleware, asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.catalog.url}/api/operator/inventory/${req.params.restaurantId}/low-stock`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// PATCH /operator/inventory/:menuItemId/availability - Update item availability
router.patch('/operator/inventory/:menuItemId/availability', authMiddleware, asyncHandler(async (req, res) => {
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

// ============================================================================
// LEGACY ENDPOINTS (Keep for backward compatibility)
// ============================================================================

// Get all restaurants
router.get('/restaurants', asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.catalog.url}/api/restaurant`,
    { 
      params: req.query,
      timeout: services.catalog.timeout 
    }
  );
  res.status(response.status).json(response.data);
}));

// Get restaurant by ID
router.get('/restaurants/:id', asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.catalog.url}/api/restaurant/${req.params.id}`,
    { timeout: services.catalog.timeout }
  );
  res.status(response.status).json(response.data);
}));

// Get all menu items
router.get('/items', asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.catalog.url}/api/items`,
    { 
      params: req.query,
      timeout: services.catalog.timeout 
    }
  );
  res.status(response.status).json(response.data);
}));

// Get menu item by ID
router.get('/items/:id', asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.catalog.url}/api/items/${req.params.id}`,
    { timeout: services.catalog.timeout }
  );
  res.status(response.status).json(response.data);
}));

// Get menu items by restaurant
router.get('/restaurants/:restaurantId/items', asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.catalog.url}/api/restaurants/${req.params.restaurantId}/items`,
    { 
      params: req.query,
      timeout: services.catalog.timeout 
    }
  );
  res.status(response.status).json(response.data);
}));

// Search menu items
router.get('/search', asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.catalog.url}/api/items/search`,
    { 
      params: req.query,
      timeout: services.catalog.timeout 
    }
  );
  res.status(response.status).json(response.data);
}));

// Get cuisines
router.get('/cuisines', asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.catalog.url}/api/cuisines`,
    { timeout: services.catalog.timeout }
  );
  res.status(response.status).json(response.data);
}));

module.exports = router;
