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

// POST /menu - Allow without auth for seeding
router.post('/menu', asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.catalog.url}/api/menu`,
    req.body,
    { 
      timeout: services.catalog.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

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

// Get all restaurants
router.get('/restaurants', asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.catalog.url}/api/restaurants`,
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
    `${services.catalog.url}/api/restaurants/${req.params.id}`,
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
