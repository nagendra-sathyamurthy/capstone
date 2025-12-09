const express = require('express');
const axios = require('axios');
const router = express.Router();
const services = require('../config/services');
const { asyncHandler } = require('../middleware/errorHandler');
const authMiddleware = require('../middleware/authMiddleware');

// Menu endpoints - forward to catalog service /api/menu
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

// POST /menu - Forward auth header if present (Create menu item)
router.post('/menu', authMiddleware, asyncHandler(async (req, res) => {
  const token = req.headers.authorization;
  
  const response = await axios.post(
    `${services.catalog.url}/api/menu`,
    req.body,
    { 
      timeout: services.catalog.timeout,
      headers: { Authorization: token }
    }
  );
  res.status(response.status).json(response.data);
}));

// GET /menu/:id - Get menu item by ID (with auth)
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

// PUT /menu/:id - Update menu item (requires auth)
router.put('/menu/:id', authMiddleware, asyncHandler(async (req, res) => {
  const token = req.headers.authorization;
  const response = await axios.put(
    `${services.catalog.url}/api/menu/${req.params.id}`,
    req.body,
    { 
      timeout: services.catalog.timeout,
      headers: { Authorization: token }
    }
  );
  res.status(response.status).json(response.data);
}));

// DELETE /menu/:id - Delete menu item (requires auth)
router.delete('/menu/:id', authMiddleware, asyncHandler(async (req, res) => {
  const token = req.headers.authorization;
  const response = await axios.delete(
    `${services.catalog.url}/api/menu/${req.params.id}`,
    { 
      timeout: services.catalog.timeout,
      headers: { Authorization: token }
    }
  );
  res.status(response.status).json(response.data);
}));

// PATCH /menu/:id/availability - Update menu item availability (requires auth)
router.patch('/menu/:id/availability', authMiddleware, asyncHandler(async (req, res) => {
  const token = req.headers.authorization;
  const response = await axios.patch(
    `${services.catalog.url}/api/menu/${req.params.id}/availability`,
    req.body,
    { 
      timeout: services.catalog.timeout,
      headers: { Authorization: token }
    }
  );
  res.status(response.status).json(response.data);
}));

// GET /menu/owner/:ownerId - Get menu items by owner (requires auth)
router.get('/menu/owner/:ownerId', authMiddleware, asyncHandler(async (req, res) => {
  const token = req.headers.authorization;
  const response = await axios.get(
    `${services.catalog.url}/api/menu/owner/${req.params.ownerId}`,
    { 
      timeout: services.catalog.timeout,
      headers: { Authorization: token }
    }
  );
  res.status(response.status).json(response.data);
}));

// POST /menu/bulk - Bulk create menu items (requires auth)
router.post('/menu/bulk', authMiddleware, asyncHandler(async (req, res) => {
  const token = req.headers.authorization;
  const response = await axios.post(
    `${services.catalog.url}/api/menu/bulk`,
    req.body,
    { 
      timeout: services.catalog.timeout,
      headers: { Authorization: token }
    }
  );
  res.status(response.status).json(response.data);
}));

// Get all restaurants (no auth required for browsing)
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

// Register restaurant (requires auth)
router.post('/restaurant/register', authMiddleware, asyncHandler(async (req, res) => {
  const token = req.headers.authorization;
  const response = await axios.post(
    `${services.catalog.url}/api/restaurant/register`,
    req.body,
    { 
      timeout: services.catalog.timeout,
      headers: { Authorization: token }
    }
  );
  res.status(response.status).json(response.data);
}));

// Get restaurants by owner (requires auth)
router.get('/restaurant/owner/:ownerId', authMiddleware, asyncHandler(async (req, res) => {
  const token = req.headers.authorization;
  const response = await axios.get(
    `${services.catalog.url}/api/restaurant/owner/${req.params.ownerId}`,
    { 
      timeout: services.catalog.timeout,
      headers: { Authorization: token }
    }
  );
  res.status(response.status).json(response.data);
}));

// Get all menu items
router.get('/items', asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.catalog.url}/api/item`,
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
    `${services.catalog.url}/api/item/${req.params.id}`,
    { timeout: services.catalog.timeout }
  );
  res.status(response.status).json(response.data);
}));

// Get menu items by restaurant
router.get('/menu/restaurant/:restaurantId', asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.catalog.url}/api/menu/restaurant/${req.params.restaurantId}`,
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
