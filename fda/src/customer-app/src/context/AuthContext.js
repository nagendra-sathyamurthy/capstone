import React, { createContext, useContext, useReducer } from 'react';

const AuthContext = createContext();

const authInitialState = {
  isAuthenticated: false,
  user: null,
  phone: '',
  otp: '',
  loading: false,
  token: null
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
      return authInitialState;
    default:
      return state;
  }
};

export const AuthProvider = ({ children }) => {
  const [state, dispatch] = useReducer(authReducer, authInitialState);

  const login = (userData) => {
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
    
    // Save to localStorage synchronously
    localStorage.setItem('authToken', authData.token);
    localStorage.setItem('userId', authData.user.id);
    localStorage.setItem('userPhone', authData.user.phone);
    
    // Also save complete user object for restoration
    localStorage.setItem('user', JSON.stringify(authData.user));
    
    // Dispatch state update
    dispatch({ type: 'LOGIN_SUCCESS', payload: authData });
    
    // Return true to indicate success
    return true;
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

  // Check if user is already logged in on app start
  React.useEffect(() => {
    const token = localStorage.getItem('authToken');
    const userId = localStorage.getItem('userId');
    const userPhone = localStorage.getItem('userPhone');
    const userStr = localStorage.getItem('user');

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
          console.error('Failed to parse saved user:', e);
        }
      }
      
      const authData = {
        user: user,
        token: token
      };
      
      dispatch({ type: 'LOGIN_SUCCESS', payload: authData });
    }
  }, []);

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