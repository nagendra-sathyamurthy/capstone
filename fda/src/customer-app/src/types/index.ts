// User and Authentication Types
export interface User {
  id?: string;
  userId?: string;
  name?: string;
  firstName?: string;
  lastName?: string;
  phone?: string;
  email?: string;
  profileImage?: string | null;
  role?: 'Customer' | 'RestaurantOwner' | 'KitchenWorker' | 'DeliveryAgent' | 'Operator';
}

export interface AuthState {
  isAuthenticated: boolean;
  user: User | null;
  token: string | null;
}

export interface AuthContextType extends AuthState {
  login: (userData: User, token: string) => void;
  logout: () => void;
}

// Address Types
export interface DeliveryAddress {
  id?: string;
  type: 'home' | 'work' | 'other';
  line1: string;
  line2?: string;
  landmark?: string;
  city: string;
  state: string;
  pincode: string;
  country?: string;
  latitude?: number;
  longitude?: number;
  createdAt?: string;
  updatedAt?: string;
}

// Food Preferences Types
export interface FoodPreferences {
  dietary?: 'veg' | 'non-veg' | 'vegan' | 'all';
  cuisines?: string[];
  spiceLevel?: 'mild' | 'medium' | 'hot';
  allergens?: string[];
  updatedAt?: string;
}

// Profile Types
export interface UserProfile {
  id?: string;
  userId: string;
  firstName?: string;
  lastName?: string;
  email?: string;
  phoneNumber?: string;
  profileImage?: string;
  role?: string;
  deliveryAddresses?: DeliveryAddress[];
  foodPreferences?: FoodPreferences;
  createdAt?: string;
  updatedAt?: string;
}

// Restaurant Types
export interface Restaurant {
  id: string;
  name: string;
  cuisine: string;
  rating: number;
  deliveryTime: string;
  image: string;
  isOpen: boolean;
  description?: string;
  address?: string;
  priceRange?: string;
}

// Menu Item Types
export interface MenuItem {
  id: string;
  name: string;
  description: string;
  price: number;
  image: string;
  category: string;
  restaurant?: string;
  restaurantId?: string;
  isVeg: boolean;
  isAvailable: boolean;
  rating?: number;
  preparationTime?: number;
}

// Cart Types
export interface CartItem extends MenuItem {
  quantity: number;
}

export interface CartContextType {
  items: CartItem[];
  totalItems: number;
  totalAmount: number;
  deliveryAddress: DeliveryAddress | null;
  orderSummary: any | null;
  addToCart: (item: MenuItem) => void;
  removeFromCart: (itemId: string) => void;
  updateQuantity: (itemId: string, quantity: number) => void;
  clearCart: () => void;
  setDeliveryAddress: (address: DeliveryAddress) => void;
  setOrderSummary: (summary: any) => void;
  saveCompletedOrder: (orderData: any) => Promise<any>;
}

// Order Types
export type OrderStatus = 
  | 'Pending' 
  | 'Confirmed' 
  | 'Preparing' 
  | 'Ready' 
  | 'PickedUp' 
  | 'Delivered' 
  | 'Cancelled';

export interface OrderItem {
  id?: string;
  itemId?: string;
  name: string;
  quantity: number;
  price: number;
  image?: string;
  description?: string;
  category?: string;
  isVeg?: boolean;
}

export interface Order {
  id: string;
  userId: string;
  date: string;
  restaurant: string;
  restaurantId?: string;
  items: number;
  itemsList: OrderItem[];
  total: number;
  status: OrderStatus;
  rating?: number;
  deliveryAddress?: DeliveryAddress;
  createdAt?: string;
  updatedAt?: string;
}

// API Response Types
export interface ApiResponse<T> {
  data: T;
  message?: string;
  success?: boolean;
}

export interface LoginResponse {
  userId: string;
  phone: string;
  token: string;
  name?: string;
}

// Filter Types
export interface FilterOptions {
  cuisine: string;
  dietary: 'all' | 'veg' | 'non-veg';
  priceRange: string;
}

// Form Data Types
export interface ProfileSetupData {
  name: string;
  email: string;
  profileImage: string | null;
  address: Partial<DeliveryAddress>;
  preferences: FoodPreferences;
}

export interface AddressFormData {
  type: 'home' | 'work' | 'other';
  line1: string;
  line2: string;
  landmark: string;
  city: string;
  state: string;
  pincode: string;
}
