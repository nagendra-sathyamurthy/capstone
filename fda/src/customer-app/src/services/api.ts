/* eslint-disable @typescript-eslint/no-explicit-any */
import axios from 'axios';

// API Gateway URL - single entry point for all services
const GATEWAY_URL = process.env.REACT_APP_GATEWAY_URL || 'http://localhost:5000';

// Helper function to convert order status enum to text
const getStatusText = (status: number): string => {
  const statusMap: Record<number, string> = {
    0: 'Pending',
    1: 'Accepted',
    2: 'Declined',
    3: 'Preparing',
    4: 'Ready for Pickup',
    5: 'Out for Delivery',
    6: 'Delivered',
    7: 'Cancelled'
  };
  return statusMap[status] || 'Unknown';
};

// Create axios instance for gateway
const api = axios.create({ 
  baseURL: GATEWAY_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

// For backward compatibility, create service-specific references
const authAPI = api;
const catalogAPI = api;
const cartAPI = api;
const crmAPI = api;

// Add request interceptors to include auth token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Add response interceptor for error handling
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      console.log('[API Interceptor] Received 401 for:', error.config?.url);
      console.log('[API Interceptor] Response:', error.response?.data);
      
      // Only clear auth if it's from the authentication service itself
      // Don't clear auth if other services (order, catalog, etc.) have authentication issues
      const isAuthEndpoint = error.config?.url?.includes('/api/auth/');
      
      if (isAuthEndpoint) {
        console.log('[API Interceptor] Auth service rejected token - clearing localStorage and redirecting to login');
        // Clear user data on unauthorized from auth service
        localStorage.removeItem('authToken');
        localStorage.removeItem('userId');
        localStorage.removeItem('userPhone');
        localStorage.removeItem('user');
        window.location.href = '/';
      } else {
        console.log('[API Interceptor] Non-auth service 401 - keeping auth data, just failing this request');
      }
    }
    return Promise.reject(error);
  }
);

// Authentication API calls
export const authService = {
  // Send OTP to phone number
  sendOTP: async (phoneNumber: string): Promise<any> => {
    try {
      const response = await authAPI.post('/auth/send-otp', { phoneNumber });
      return response.data;
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Failed to send OTP');
    }
  },

  // Verify OTP and register/login user
  verifyOTP: async (phoneNumber: string, otp: string): Promise<any> => {
    try {
      const response = await authAPI.post('/auth/verify-otp', { phoneNumber, otp });
      return response.data;
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Failed to verify OTP');
    }
  },

  // Customer phone login - get JWT token
  customerPhoneLogin: async (phone: string, userId: string, name: string): Promise<any> => {
    try {
      const response = await authAPI.post('/api/auth/customer/phone-login', {
        phone,
        userId,
        name: name || `User ${phone.slice(-4)}`
      });
      return response.data;
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Phone login failed');
    }
  },

  // Register new customer
  registerCustomer: async (customerData: any): Promise<any> => {
    try {
      const response = await authAPI.post('/auth/register', customerData);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Registration failed');
    }
  }
};

// Catalog API calls
export const catalogService = {
  // Get all restaurants
  getRestaurants: async () => {
    try {
      const response = await catalogAPI.get('/restaurants');
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Failed to fetch restaurants');
    }
  },

  // Get menu items for a restaurant
  getMenuItems: async (restaurantId: any): Promise<any> => {
    try {
      const response = await catalogAPI.get(`/menu-items/restaurant/${restaurantId}`);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Failed to fetch menu items');
    }
  },

  // Search menu items
  searchMenuItems: async (query: any, filters: any = {}): Promise<any> => {
    try {
      const params = new URLSearchParams({ query, ...filters });
      const response = await catalogAPI.get(`/menu-items/search?${params}`);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Search failed');
    }
  }
};

// Cart API calls
export const cartService = {
  // Get user's cart
  getCart: async (userId: any): Promise<any> => {
    try {
      const response = await cartAPI.get(`/cart/${userId}`);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Failed to fetch cart');
    }
  },

  // Add item to cart
  addToCart: async (userId: any, item: any): Promise<any> => {
    try {
      const response = await cartAPI.post(`/cart/${userId}/add`, item);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Failed to add to cart');
    }
  },

  // Update cart item
  updateCartItem: async (userId: any, itemId: any, quantity: any): Promise<any> => {
    try {
      const response = await cartAPI.put(`/cart/${userId}/update/${itemId}`, { quantity });
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Failed to update cart');
    }
  },

  // Remove item from cart
  removeFromCart: async (userId: any, itemId: any): Promise<any> => {
    try {
      const response = await cartAPI.delete(`/cart/${userId}/remove/${itemId}`);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Failed to remove from cart');
    }
  },

  // Clear cart
  clearCart: async (userId: any): Promise<any> => {
    try {
      const response = await cartAPI.delete(`/cart/${userId}/clear`);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Failed to clear cart');
    }
  }
};

// Order API calls
export const orderService = {
  // Create new order
  createOrder: async (orderData: any): Promise<any> => {
    try {
      const response = await catalogAPI.post('/orders', orderData);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Failed to create order');
    }
  },

  // Get customer orders
  getCustomerOrders: async () => {
    try {
      // Get current user's identifier from localStorage
      const userId = localStorage.getItem('userId');
      const userPhone = localStorage.getItem('userPhone');
      const userKey = userId || userPhone;
      const token = localStorage.getItem('authToken');
      
      if (!userKey) {
        console.error('No user identifier found');
        return [];
      }

      // Try API first
      try {
        const response = await api.get(`/api/orders/customer/${userKey}`, {
          headers: token ? { Authorization: `Bearer ${token}` } : {},
          timeout: 10000
        });
        
        if (response.data && Array.isArray(response.data)) {
          // Transform API response to match frontend format
          return response.data.map(order => ({
            id: order.id,
            userId: order.customerId,
            date: order.createdAt,
            restaurant: order.restaurantName || 'Restaurant',
            restaurantId: order.restaurantId,
            items: order.items || [],
            itemsList: (order.items || []).map((item: any) => ({
              id: item.menuItemId,
              name: item.name,
              quantity: item.quantity,
              price: item.price
            })),
            total: order.totalAmount,
            status: getStatusText(order.status),
            rating: 0,
            deliveryAddress: order.notes ? { line1: order.notes } : null
          }));
        }
      } catch (apiError) {
        console.log('API orders fetch failed, trying localStorage:', apiError.message);
      }
      
      // Fallback to localStorage
      const storageKey = `orderHistory_${userKey}`;
      const localOrders = JSON.parse(localStorage.getItem(storageKey) || '[]');
      
      if (localOrders.length > 0) {
        return localOrders;
      }

      return [];
    } catch (error) {
      console.error('Failed to fetch orders:', error);
      return [];
    }
  },

  // Get order details
  getOrder: async (orderId: any): Promise<any> => {
    try {
      const response = await catalogAPI.get(`/orders/${orderId}`);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Failed to fetch order');
    }
  },

  // Track order
  trackOrder: async (orderId: any): Promise<any> => {
    try {
      const response = await catalogAPI.get(`/orders/${orderId}/track`);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Failed to track order');
    }
  }
};

// Payment simulation service
export const paymentService = {
  // Generate QR code for payment
  generatePaymentQR: async (amount: any, orderId: any): Promise<any> => {
    try {
      // For demo purposes, we'll generate a mock QR code
      // In a real app, this would call a payment gateway
      const qrData = `upi://pay?pa=restaurant@upi&pn=Restaurant&am=${amount}&tn=Order${orderId}&cu=INR`;
      return {
        qrCode: qrData,
        amount: amount,
        orderId: orderId,
        expiryTime: new Date(Date.now() + 15 * 60 * 1000) // 15 minutes
      };
    } catch (error) {
      throw new Error('Failed to generate payment QR');
    }
  },

  // Simulate payment verification
  verifyPayment: async (paymentId: any): Promise<any> => {
    try {
      // Mock payment verification - in real app, this would verify with payment gateway
      return new Promise((resolve) => {
        setTimeout(() => {
          resolve({
            success: true,
            paymentId: paymentId,
            status: 'completed',
            transactionId: `TXN${Date.now()}`
          });
        }, 2000); // Simulate 2 second verification delay
      });
    } catch (error) {
      throw new Error('Payment verification failed');
    }
  }
};

// Customer service - MongoDB storage via CRM API
export const customerService = {
  // Get customer profile (UserProfile from CRM)
  getProfile: async () => {
    try {
      const userId = localStorage.getItem('userId');
      if (!userId) {
        throw new Error('User not authenticated');
      }
      const response = await crmAPI.get(`/api/crm/userprofile/by-user/${userId}`);
      return response.data;
    } catch (error) {
      if (error.response?.status === 404) {
        return null;
      }
      throw new Error(error.response?.data?.message || 'Failed to fetch profile');
    }
  },

  // Update customer profile
  updateProfile: async (profileData: any): Promise<any> => {
    try {
      const userId = localStorage.getItem('userId');
      if (!userId) {
        throw new Error('User not authenticated');
      }
      const response = await crmAPI.put(`/api/crm/userprofile/by-user/${userId}`, profileData);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Failed to update profile');
    }
  },

  // Get customer addresses from MongoDB
  getAddresses: async () => {
    try {
      const userId = localStorage.getItem('userId');
      if (!userId) {
        throw new Error('User not authenticated');
      }
      const response = await crmAPI.get(`/api/crm/userprofile/by-user/${userId}/addresses`);
      return response.data || [];
    } catch (error) {
      console.error('Failed to fetch addresses from API:', error);
      return [];
    }
  },

  // Add new address to MongoDB
  addAddress: async (address: any): Promise<any> => {
    try {
      const userId = localStorage.getItem('userId');
      if (!userId) {
        throw new Error('User not authenticated');
      }
      const response = await crmAPI.post(`/api/crm/userprofile/by-user/${userId}/addresses`, address);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Failed to add address');
    }
  },

  // Update address in MongoDB
  updateAddress: async (addressId: any, address: any): Promise<any> => {
    try {
      const userId = localStorage.getItem('userId');
      if (!userId) {
        throw new Error('User not authenticated');
      }
      const response = await crmAPI.put(`/api/crm/userprofile/by-user/${userId}/addresses/${addressId}`, address);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Failed to update address');
    }
  },

  // Delete address from MongoDB
  deleteAddress: async (addressId: any): Promise<any> => {
    try {
      const userId = localStorage.getItem('userId');
      if (!userId) {
        throw new Error('User not authenticated');
      }
      await crmAPI.delete(`/api/crm/userprofile/by-user/${userId}/addresses/${addressId}`);
      return true;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Failed to delete address');
    }
  },

  // Update profile image in MongoDB
  updateProfileImage: async (profileImage: any): Promise<any> => {
    try {
      const userId = localStorage.getItem('userId');
      if (!userId) {
        throw new Error('User not authenticated');
      }
      await crmAPI.put(`/api/crm/userprofile/by-user/${userId}/profile-image`, { profileImage });
      return true;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Failed to update profile image');
    }
  },

  // Update food preferences in MongoDB
  updateFoodPreferences: async (preferences: any): Promise<any> => {
    try {
      const userId = localStorage.getItem('userId');
      if (!userId) {
        throw new Error('User not authenticated');
      }
      const response = await crmAPI.put(`/api/crm/userprofile/by-user/${userId}/food-preferences`, preferences);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Failed to update food preferences');
    }
  }
};
