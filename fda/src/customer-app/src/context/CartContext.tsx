import React, { createContext, useContext, useReducer, ReactNode } from 'react';
import axios from 'axios';
import { CartItem, MenuItem, DeliveryAddress, CartContextType } from '../types';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000';

interface CartState {
  items: CartItem[];
  totalAmount: number;
  totalItems: number;
  deliveryAddress: DeliveryAddress | null;
  orderSummary: any | null;
}

type CartAction =
  | { type: 'ADD_TO_CART'; payload: MenuItem }
  | { type: 'REMOVE_FROM_CART'; payload: { id: string } }
  | { type: 'UPDATE_QUANTITY'; payload: { id: string; quantity: number } }
  | { type: 'CLEAR_CART' }
  | { type: 'SET_DELIVERY_ADDRESS'; payload: DeliveryAddress }
  | { type: 'SET_ORDER_SUMMARY'; payload: any }
  | { type: 'LOAD_CART'; payload: CartState };

const CartContext = createContext<CartContextType | undefined>(undefined);

const cartInitialState: CartState = {
  items: [],
  totalAmount: 0,
  totalItems: 0,
  deliveryAddress: null,
  orderSummary: null
};

const cartReducer = (state: CartState, action: CartAction): CartState => {
  switch (action.type) {
    case 'ADD_TO_CART':
      const existingItem = state.items.find(item => item.id === action.payload.id);
      if (existingItem) {
        const updatedItems = state.items.map(item =>
          item.id === action.payload.id
            ? { ...item, quantity: item.quantity + 1 }
            : item
        );
        return {
          ...state,
          items: updatedItems,
          totalItems: state.totalItems + 1,
          totalAmount: state.totalAmount + action.payload.price
        };
      } else {
        return {
          ...state,
          items: [...state.items, { ...action.payload, quantity: 1 }],
          totalItems: state.totalItems + 1,
          totalAmount: state.totalAmount + action.payload.price
        };
      }

    case 'REMOVE_FROM_CART':
      const itemToRemove = state.items.find(item => item.id === action.payload.id);
      if (!itemToRemove) return state;
      if (itemToRemove.quantity === 1) {
        return {
          ...state,
          items: state.items.filter(item => item.id !== action.payload.id),
          totalItems: state.totalItems - 1,
          totalAmount: state.totalAmount - itemToRemove.price
        };
      } else {
        const updatedItems = state.items.map(item =>
          item.id === action.payload.id
            ? { ...item, quantity: item.quantity - 1 }
            : item
        );
        return {
          ...state,
          items: updatedItems,
          totalItems: state.totalItems - 1,
          totalAmount: state.totalAmount - itemToRemove.price
        };
      }

    case 'UPDATE_QUANTITY':
      return {
        ...state,
        items: state.items.map(item =>
          item.id === action.payload.id
            ? { ...item, quantity: action.payload.quantity }
            : item
        ),
        totalItems: state.items.reduce((sum, item) =>
          item.id === action.payload.id ? sum - item.quantity + action.payload.quantity : sum + item.quantity, 0
        ),
        totalAmount: state.items.reduce((sum, item) =>
          item.id === action.payload.id ? sum + (action.payload.quantity * item.price) : sum + (item.quantity * item.price), 0
        )
      };

    case 'CLEAR_CART':
      return cartInitialState;

    case 'SET_DELIVERY_ADDRESS':
      return {
        ...state,
        deliveryAddress: action.payload
      };

    case 'SET_ORDER_SUMMARY':
      return {
        ...state,
        orderSummary: action.payload
      };

    case 'LOAD_CART':
      return action.payload;

    default:
      return state;
  }
};

interface CartProviderProps {
  children: ReactNode;
}

export const CartProvider: React.FC<CartProviderProps> = ({ children }) => {
  const [state, dispatch] = useReducer(cartReducer, cartInitialState);

  const addToCart = (item: MenuItem) => {
    dispatch({ type: 'ADD_TO_CART', payload: item });
  };

  const removeFromCart = (itemId: string) => {
    dispatch({ type: 'REMOVE_FROM_CART', payload: { id: itemId } });
  };

  const updateQuantity = (itemId: string, quantity: number) => {
    dispatch({ type: 'UPDATE_QUANTITY', payload: { id: itemId, quantity } });
  };

  const clearCart = () => {
    dispatch({ type: 'CLEAR_CART' });
  };

  const setDeliveryAddress = (address: DeliveryAddress) => {
    dispatch({ type: 'SET_DELIVERY_ADDRESS', payload: address });
  };

  const setOrderSummary = (summary: any) => {
    dispatch({ type: 'SET_ORDER_SUMMARY', payload: summary });
  };

  const saveCompletedOrder = async (orderData: any) => {
    try {
      // Get current user's identifier from localStorage
      const userId = localStorage.getItem('userId');
      const userPhone = localStorage.getItem('userPhone');
      const userKey = userId || userPhone;
      const token = localStorage.getItem('authToken');
      
      if (!userKey) {
        console.error('No user identifier found');
        return null;
      }

      // Prepare order for API
      const orderPayload = {
        customerId: userKey,
        restaurantId: orderData.restaurantId || '1',
        restaurantName: orderData.restaurant || 'Unknown',
        items: (orderData.items || []).map((item: any) => ({
          menuItemId: item.id,
          name: item.name,
          quantity: item.quantity || 1,
          price: item.price,
          specialInstructions: item.specialInstructions || ''
        })),
        totalAmount: orderData.finalTotal || orderData.total || 0,
        status: 6, // Delivered status (for demo purposes, in real app this would be Pending)
        notes: orderData.deliveryAddress ? 
          `Deliver to: ${orderData.deliveryAddress.line1}, ${orderData.deliveryAddress.city}` : 
          undefined
      };

      // Try to save to API
      try {
        const response = await axios.post(
          `${API_BASE_URL}/api/orders`,
          orderPayload,
          {
            headers: token ? { Authorization: `Bearer ${token}` } : {},
            timeout: 10000
          }
        );
        
        console.log('Order saved to database:', response.data);
        return response.data;
      } catch (apiError) {
        const errorMessage = apiError instanceof Error ? apiError.message : 'Unknown error';
        console.error('Failed to save order to API:', errorMessage);
        
        // Fallback to localStorage for offline support
        const storageKey = `orderHistory_${userKey}`;
        const existingOrders = JSON.parse(localStorage.getItem(storageKey) || '[]');
        
        const newOrder = {
          id: orderData.orderId || 'ORD' + Date.now(),
          userId: userKey,
          date: new Date().toISOString(),
          restaurant: orderData.restaurant || 'Unknown',
          restaurantId: orderData.restaurantId,
          items: orderData.items || [],
          total: orderData.finalTotal || orderData.total,
          status: 'Delivered',
          rating: 0,
          deliveryAddress: orderData.deliveryAddress
        };
        
        existingOrders.unshift(newOrder);
        const limitedOrders = existingOrders.slice(0, 50);
        localStorage.setItem(storageKey, JSON.stringify(limitedOrders));
        
        return newOrder;
      }
    } catch (error) {
      console.error('Error saving order:', error);
      return null;
    }
  };

  return (
    <CartContext.Provider
      value={{
        items: state.items,
        totalAmount: state.totalAmount,
        totalItems: state.totalItems,
        deliveryAddress: state.deliveryAddress,
        orderSummary: state.orderSummary,
        addToCart,
        removeFromCart,
        updateQuantity,
        clearCart,
        setDeliveryAddress,
        setOrderSummary,
        saveCompletedOrder
      }}
    >
      {children}
    </CartContext.Provider>
  );
};

export const useCart = (): CartContextType => {
  const context = useContext(CartContext);
  if (!context) {
    throw new Error('useCart must be used within a CartProvider');
  }
  return context;
};