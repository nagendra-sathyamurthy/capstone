module.exports = {
  auth: {
    url: process.env.AUTH_SERVICE_URL || 'http://localhost:30001',
    timeout: 10000
  },
  catalog: {
    url: process.env.CATALOG_SERVICE_URL || 'http://localhost:30002',
    timeout: 10000
  },
  crm: {
    url: process.env.CRM_SERVICE_URL || 'http://localhost:30003',
    timeout: 10000
  },
  cart: {
    url: process.env.CART_SERVICE_URL || 'http://localhost:30004',
    timeout: 10000
  },
  order: {
    url: process.env.ORDER_SERVICE_URL || 'http://localhost:30005',
    timeout: 10000
  }
};
