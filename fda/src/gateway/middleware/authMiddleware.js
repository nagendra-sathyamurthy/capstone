const axios = require('axios');
const services = require('../config/services');

const authMiddleware = async (req, res, next) => {
  try {
    const token = req.headers.authorization?.replace('Bearer ', '');

    if (!token) {
      return res.status(401).json({
        error: true,
        message: 'No authentication token provided'
      });
    }

    // Validate token with authentication service
    try {
      const response = await axios.get(`${services.auth.url}/api/auth/validate`, {
        headers: {
          Authorization: `Bearer ${token}`
        },
        timeout: services.auth.timeout
      });

      // Attach user info to request
      req.user = response.data;
      next();
    } catch (error) {
      return res.status(401).json({
        error: true,
        message: 'Invalid or expired token'
      });
    }
  } catch (error) {
    next(error);
  }
};

module.exports = authMiddleware;
