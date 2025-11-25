const express = require('express');
const axios = require('axios');
const router = express.Router();
const services = require('../config/services');
const { asyncHandler } = require('../middleware/errorHandler');

// Register customer
router.post('/register', asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.auth.url}/api/auth/register`,
    req.body,
    { timeout: services.auth.timeout }
  );
  res.status(response.status).json(response.data);
}));

// Send OTP
router.post('/send-otp', asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.auth.url}/api/auth/send-otp`,
    req.body,
    { timeout: services.auth.timeout }
  );
  res.status(response.status).json(response.data);
}));

// Verify OTP
router.post('/verify-otp', asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.auth.url}/api/auth/verify-otp`,
    req.body,
    { timeout: services.auth.timeout }
  );
  res.status(response.status).json(response.data);
}));

// Login
router.post('/login', asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.auth.url}/api/auth/login`,
    req.body,
    { timeout: services.auth.timeout }
  );
  res.status(response.status).json(response.data);
}));

// Validate token
router.get('/validate', asyncHandler(async (req, res) => {
  const response = await axios.get(
    `${services.auth.url}/api/auth/validate`,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.auth.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Refresh token
router.post('/refresh', asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.auth.url}/api/auth/refresh`,
    req.body,
    { timeout: services.auth.timeout }
  );
  res.status(response.status).json(response.data);
}));

// Logout
router.post('/logout', asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.auth.url}/api/auth/logout`,
    req.body,
    {
      headers: { Authorization: req.headers.authorization },
      timeout: services.auth.timeout
    }
  );
  res.status(response.status).json(response.data);
}));

// Customer phone login - generate JWT token
router.post('/customer/phone-login', asyncHandler(async (req, res) => {
  const response = await axios.post(
    `${services.auth.url}/api/auth/customer/phone-login`,
    req.body,
    { timeout: services.auth.timeout }
  );
  res.status(response.status).json(response.data);
}));

module.exports = router;
