import React, { createContext, useContext, useReducer } from 'react';

const AuthContext = createContext();

// Check localStorage immediately on initialization (client-side only)
const getInitialAuthState = () => {
  // Ensure we're on the client side
  if (typeof window === 'undefined') {
    console.log('[AuthContext] Server-side rendering detected, returning unauthenticated state');
    return {
      isAuthenticated: false,
      user: null,
      phone: '',
      otp: '',
      loading: false,
      token: null
    };
  }

  try {
    const token = localStorage.getItem('authToken');
    const userId = localStorage.getItem('userId');
    const userPhone = localStorage.getItem('userPhone');
    const userStr = localStorage.getItem('user');

    console.log('[AuthContext] Reading from localStorage:');
    console.log('  - authToken:', token ? token.substring(0, 30) + '...' : 'NULL');
    console.log('  - userId:', userId || 'NULL');
    console.log('  - userPhone:', userPhone || 'NULL');
    console.log('  - user:', userStr ? userStr.substring(0, 50) + '...' : 'NULL');
    console.log('[AuthContext] localStorage.length:', localStorage.length);
    console.log('[AuthContext] All localStorage keys:', Object.keys(localStorage));
    
    console.log('[AuthContext] Initializing auth state:', { 
      hasToken: !!token, 
      hasUserId: !!userId, 
      token: token?.substring(0, 20) + '...',
      userId 
    });

    if (token && userId) {
      // Restore session from localStorage
      let user = {
        id: userId,
        customerId: userId,
        phone: userPhone,
        name: '', 
        email: ''
      };
      
      // Try to restore full user object if available
      if (userStr) {
        try {
          const savedUser = JSON.parse(userStr);
          user = { ...user, ...savedUser };
        } catch (e) {
          console.error('[AuthContext] Failed to parse saved user:', e);
        }
      }
      
      console.log('[AuthContext] User authenticated on init:', user);
      return {
        isAuthenticated: true,
        user: user,
        phone: userPhone || '',
        otp: '',
        loading: false,
        token: token
      };
    }
  } catch (error) {
    console.error('[AuthContext] Error reading from localStorage:', error);
  }

  console.log('[AuthContext] No authentication found, returning unauthenticated state');
  return {
    isAuthenticated: false,
    user: null,
    phone: '',
    otp: '',
    loading: false,
    token: null
  };
};

const authReducer = (state, action) => {
  switch (action.type) {
    case 'SET_LOADING':
      return { ...state, loading: action.payload };
    case 'SET_PHONE':
      return { ...state, phone: action.payload };
    case 'SET_OTP':
      return { ...state, otp: action.payload };
    case 'LOGIN_SUCCESS':
      return {
        ...state,
        isAuthenticated: true,
        user: action.payload.user,
        token: action.payload.token,
        phone: action.payload.user.phone || state.phone, // Preserve phone from user or existing state
        loading: false
      };
    case 'LOGOUT':
      return {
        isAuthenticated: false,
        user: null,
        phone: '',
        otp: '',
        loading: false,
        token: null
      };
    default:
      return state;
  }
};

export const AuthProvider = ({ children }) => {
  // Initialize state with a function to ensure it runs on client-side
  const [state, dispatch] = useReducer(authReducer, null, getInitialAuthState);

  const login = (userData) => {
    console.log('[AuthContext] Login called with userData:', userData);
    
    // Save user data and token to localStorage FIRST
    const authData = {
      user: {
        id: userData.id || userData.customerId,
        customerId: userData.id || userData.customerId,
        phone: userData.phone,
        name: userData.name || userData.firstName || '',
        email: userData.email || ''
      },
      token: userData.token
    };
    
    console.log('[AuthContext] Saving auth data to localStorage:', authData);
    
    try {
      // Save to localStorage synchronously
      localStorage.setItem('authToken', authData.token);
      localStorage.setItem('userId', authData.user.id);
      localStorage.setItem('userPhone', authData.user.phone);
      
      // Also save complete user object for restoration
      localStorage.setItem('user', JSON.stringify(authData.user));
      
      console.log('[AuthContext] Successfully saved to localStorage');
      console.log('[AuthContext] authToken:', localStorage.getItem('authToken'));
      console.log('[AuthContext] userId:', localStorage.getItem('userId'));
      console.log('[AuthContext] userPhone:', localStorage.getItem('userPhone'));
      
      // Dispatch state update
      dispatch({ type: 'LOGIN_SUCCESS', payload: authData });
      
      console.log('[AuthContext] Login completed successfully');
      
      // Return true to indicate success
      return true;
    } catch (error) {
      console.error('[AuthContext] Error saving to localStorage:', error);
      return false;
    }
  };

  const logout = () => {
    // Clear all auth data from localStorage
    localStorage.removeItem('authToken');
    localStorage.removeItem('userId');
    localStorage.removeItem('userPhone');
    localStorage.removeItem('user');
    
    dispatch({ type: 'LOGOUT' });
  };

  const setPhone = (phone) => {
    dispatch({ type: 'SET_PHONE', payload: phone });
  };

  const setOtp = (otp) => {
    dispatch({ type: 'SET_OTP', payload: otp });
  };

  const setLoading = (loading) => {
    dispatch({ type: 'SET_LOADING', payload: loading });
  };

  return (
    <AuthContext.Provider
      value={{
        ...state,
        login,
        logout,
        setPhone,
        setOtp,
        setLoading
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};