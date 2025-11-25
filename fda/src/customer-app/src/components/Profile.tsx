import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { ArrowLeft, User, MapPin, Settings, Heart, LogOut, Camera, ChevronRight, Package, Plus } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import { useCart } from '../context/CartContext';
import { customerService, orderService } from '../services/api';
import { DeliveryAddress, Order } from '../types';
import { toast } from 'react-toastify';
import '../styles/Profile.css';

interface ProfileData {
  name: string;
  phone: string;
  email: string;
  profileImage: string | null;
}

interface PreferencesData {
  notifications: boolean;
  emailUpdates: boolean;
  dietaryRestrictions: string;
  favoriteCuisines: string[];
}

const Profile: React.FC = () => {
  const navigate = useNavigate();
  const { user, logout } = useAuth();
  const { addToCart, clearCart } = useCart();
  const [activeTab, setActiveTab] = useState<string>('orders');
  const [profile, setProfile] = useState<ProfileData>({
    name: user?.name || user?.firstName || '',
    phone: user?.phone || '',
    email: user?.email || '',
    profileImage: null
  });
  const [orders, setOrders] = useState<Order[]>([]);
  const [addresses, setAddresses] = useState<DeliveryAddress[]>([]);
  const [editingAddress, setEditingAddress] = useState<DeliveryAddress | null>(null);
  const [showAddressForm, setShowAddressForm] = useState<boolean>(false);
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null);
  const [showOrderDetails, setShowOrderDetails] = useState<boolean>(false);
  const [addressFormData, setAddressFormData] = useState<Partial<DeliveryAddress>>({
    type: 'home',
    line1: '',
    line2: '',
    landmark: '',
    city: 'Bangalore',
    state: 'Karnataka',
    pincode: ''
  });
  const [preferences, setPreferences] = useState<PreferencesData>({
    notifications: true,
    emailUpdates: false,
    dietaryRestrictions: '',
    favoriteCuisines: []
  });
  const [loading, setLoading] = useState<boolean>(false);

  useEffect(() => {
    loadProfileData();
  }, []);

  const loadProfileData = async (): Promise<void> => {
    setLoading(true);
    try {
      // Get current user's identifier
      const userId = localStorage.getItem('userId');
      const userPhone = localStorage.getItem('userPhone');
      const userKey = userId || userPhone;
      
      // Load profile image from localStorage
      let savedImage: string | null = null;
      if (userKey) {
        const profileStorageKey = `profileImage_${userKey}`;
        savedImage = localStorage.getItem(profileStorageKey);
        if (savedImage) {
          setProfile(prev => ({
            ...prev,
            profileImage: savedImage
          }));
        }
      }
      
      // Load profile data
      try {
        const profileData = await customerService.getProfile();
        if (profileData) {
          // Combine firstName and lastName into name
          const fullName = [profileData.firstName, profileData.lastName]
            .filter(Boolean)
            .join(' ');
          
          setProfile(prev => ({
            ...prev,
            ...profileData,
            name: fullName || prev.name,
            // Use profile image from API if available, otherwise use localStorage
            profileImage: profileData.profileImage || savedImage || prev.profileImage
          }));
        }
      } catch (error) {
        console.log('Profile fetch failed, using stored data');
      }

      // Load orders from API (with localStorage fallback handled in API service)
      try {
        const orderData = await orderService.getCustomerOrders();
        if (orderData && orderData.length > 0) {
          // Transform API data to display format
          const formattedOrders = orderData.map((order: any) => ({
            id: order.id,
            userId: order.userId,
            date: order.date,
            restaurant: order.restaurant,
            items: Array.isArray(order.itemsList) ? order.itemsList.length : (Array.isArray(order.items) ? order.items.length : 0),
            itemsList: order.itemsList || order.items || [],
            total: order.total,
            status: order.status,
            rating: order.rating || 0,
            deliveryAddress: order.deliveryAddress
          }));
          setOrders(formattedOrders);
        }
      } catch (error) {
        console.error('Error loading orders:', error);
        setOrders([]);
      }

      // Load addresses from MongoDB via API
      try {
        const addressData = await customerService.getAddresses();
        if (addressData && Array.isArray(addressData)) {
          setAddresses(addressData);
        } else {
          setAddresses([]);
        }
      } catch (error) {
        console.error('Error loading addresses:', error);
        setAddresses([]);
      }
    } catch (error) {
      console.error('Error loading profile:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleProfileImageChange = (e: React.ChangeEvent<HTMLInputElement>): void => {
    const file = e.target.files?.[0];
    if (file) {
      const reader = new FileReader();
      reader.onloadend = () => {
        setProfile(prev => ({
          ...prev,
          profileImage: reader.result as string
        }));
        
        // Save to localStorage with user-specific key
        const userId = localStorage.getItem('userId');
        const userPhone = localStorage.getItem('userPhone');
        const userKey = userId || userPhone;
        
        if (userKey && reader.result) {
          const profileStorageKey = `profileImage_${userKey}`;
          localStorage.setItem(profileStorageKey, reader.result as string);
        }
        
        toast.success('Profile image updated!');
      };
      reader.readAsDataURL(file);
    }
  };

  const handleLogout = (): void => {
    logout();
    toast.success('Logged out successfully');
    navigate('/', { replace: true });
  };

  const handleViewDetails = (order: Order): void => {
    setSelectedOrder(order);
    setShowOrderDetails(true);
  };

  const handleCloseOrderDetails = (): void => {
    setShowOrderDetails(false);
    setSelectedOrder(null);
  };

  const handleReorder = (order: Order): void => {
    // Clear existing cart
    clearCart();
    
    // Add all items from the order to cart
    if (order.itemsList && Array.isArray(order.itemsList)) {
      order.itemsList.forEach(item => {
        // Convert OrderItem to MenuItem format
        const menuItem: any = {
          id: item.id || item.itemId || '',
          name: item.name,
          description: item.description || '',
          price: item.price,
          image: item.image || '🍽️',
          category: item.category || 'Food',
          restaurant: order.restaurant,
          restaurantId: order.restaurantId,
          isVeg: item.isVeg !== undefined ? item.isVeg : true,
          isAvailable: true
        };
        
        // Add each item the correct number of times based on quantity
        for (let i = 0; i < (item.quantity || 1); i++) {
          addToCart(menuItem);
        }
      });
      toast.success(`${order.itemsList.length} items added to cart from ${order.restaurant}`);
      navigate('/cart');
    } else {
      toast.error('Cannot reorder - order details not available');
    }
  };

  const handleClearHistory = (): void => {
    if (window.confirm('Are you sure you want to clear all order history? This action cannot be undone.')) {
      // Get current user's identifier
      const userId = localStorage.getItem('userId');
      const userPhone = localStorage.getItem('userPhone');
      const userKey = userId || userPhone;
      
      if (userKey) {
        const storageKey = `orderHistory_${userKey}`;
        localStorage.removeItem(storageKey);
      }
      
      setOrders([]);
      toast.success('Order history cleared');
    }
  };

  const handleDeleteAddress = async (addressId: string): Promise<void> => {
    try {
      await customerService.deleteAddress(addressId);
      const updatedAddresses = addresses.filter(addr => addr.id !== addressId);
      setAddresses(updatedAddresses);
      toast.success('Address deleted successfully');
    } catch (error) {
      console.error('Error deleting address:', error);
      toast.error('Failed to delete address');
    }
  };

  const handleEditAddress = (address: DeliveryAddress): void => {
    setEditingAddress(address);
    setAddressFormData({
      type: address.type,
      line1: address.line1,
      line2: address.line2 || '',
      landmark: address.landmark || '',
      city: address.city,
      state: address.state,
      pincode: address.pincode
    });
    setShowAddressForm(true);
  };

  const handleAddNewAddress = (): void => {
    setEditingAddress(null);
    setAddressFormData({
      type: 'home',
      line1: '',
      line2: '',
      landmark: '',
      city: 'Bangalore',
      state: 'Karnataka',
      pincode: ''
    });
    setShowAddressForm(true);
  };

  const handleSaveAddress = async (e: React.FormEvent): Promise<void> => {
    e.preventDefault();
    
    if (!addressFormData.line1 || !addressFormData.city || !addressFormData.state || !addressFormData.pincode) {
      toast.error('Please fill in all required fields');
      return;
    }

    if (addressFormData.pincode.length !== 6) {
      toast.error('Please enter a valid 6-digit pincode');
      return;
    }

    try {
      const addressPayload = {
        type: addressFormData.type,
        line1: addressFormData.line1,
        line2: addressFormData.line2 || '',
        landmark: addressFormData.landmark || '',
        city: addressFormData.city,
        state: addressFormData.state,
        pincode: addressFormData.pincode,
        country: 'India'
      };

      if (editingAddress) {
        // Update existing address in MongoDB
        const updatedAddress = await customerService.updateAddress(editingAddress.id!, addressPayload);
        const updatedAddresses = addresses.map(addr => 
          addr.id === editingAddress.id ? updatedAddress : addr
        );
        setAddresses(updatedAddresses);
        toast.success('Address updated successfully');
      } else {
        // Add new address to MongoDB
        const newAddress = await customerService.addAddress(addressPayload);
        setAddresses([...addresses, newAddress]);
        toast.success('Address added successfully');
      }

      setShowAddressForm(false);
      setEditingAddress(null);
    } catch (error) {
      console.error('Error saving address:', error);
      toast.error('Failed to save address');
    }
  };

  const handleCancelAddressForm = (): void => {
    setShowAddressForm(false);
    setEditingAddress(null);
    setAddressFormData({
      type: 'home',
      line1: '',
      line2: '',
      landmark: '',
      city: 'Bangalore',
      state: 'Karnataka',
      pincode: ''
    });
  };

  const handlePreferenceChange = (key: string, value: any): void => {
    setPreferences(prev => ({
      ...prev,
      [key]: value
    }));
  };

  const getStatusColor = (status: string): string => {
    switch (status.toLowerCase()) {
      case 'delivered':
        return 'status-delivered';
      case 'cancelled':
        return 'status-cancelled';
      case 'pending':
        return 'status-pending';
      default:
        return '';
    }
  };

  const renderOrders = () => (
    <div className="orders-section">
      <div className="section-header">
        <h3>Order History</h3>
        {orders.length > 0 && (
          <button className="btn-clear-history" onClick={handleClearHistory}>
            Clear History
          </button>
        )}
      </div>
      {orders.length === 0 ? (
        <div className="empty-state">
          <Package size={48} />
          <p>No orders yet</p>
          <button className="btn-primary" onClick={() => navigate('/dashboard')}>
            Start Ordering
          </button>
        </div>
      ) : (
        <div className="orders-list">
          {orders.map((order) => (
            <div key={order.id} className="order-card">
              <div className="order-header">
                <div>
                  <h4>{order.restaurant}</h4>
                  <p className="order-id">Order #{order.id}</p>
                </div>
                <span className={`order-status ${getStatusColor(order.status)}`}>
                  {order.status}
                </span>
              </div>
              <div className="order-details">
                <div className="order-info">
                  <span>{order.items} items</span>
                  <span>•</span>
                  <span>₹{order.total}</span>
                  <span>•</span>
                  <span>{new Date(order.date).toLocaleDateString()}</span>
                </div>
                {order.status === 'Delivered' && order.rating && (
                  <div className="order-rating">
                    {'⭐'.repeat(order.rating)}
                  </div>
                )}
              </div>
              <div className="order-actions">
                <button className="btn-secondary" onClick={() => handleViewDetails(order)}>
                  View Details
                </button>
                {order.status === 'Delivered' && (
                  <button className="btn-primary" onClick={() => handleReorder(order)}>
                    Reorder
                  </button>
                )}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );

  const renderAddresses = () => (
    <div className="addresses-section">
      <div className="section-header">
        <h3>Saved Addresses</h3>
        <button className="btn-primary" onClick={handleAddNewAddress}>
          <Plus size={16} /> Add New
        </button>
      </div>

      {showAddressForm && (
        <div className="address-form-overlay">
          <div className="address-form-modal">
            <div className="form-header">
              <h3>{editingAddress ? 'Edit Address' : 'Add New Address'}</h3>
              <button className="close-button" onClick={handleCancelAddressForm}>
                ×
              </button>
            </div>

            <form onSubmit={handleSaveAddress}>
              <div className="address-type-selector">
                <button
                  type="button"
                  className={`type-button ${addressFormData.type === 'home' ? 'active' : ''}`}
                  onClick={() => setAddressFormData({...addressFormData, type: 'home'})}
                >
                  🏠 Home
                </button>
                <button
                  type="button"
                  className={`type-button ${addressFormData.type === 'work' ? 'active' : ''}`}
                  onClick={() => setAddressFormData({...addressFormData, type: 'work'})}
                >
                  🏢 Work
                </button>
                <button
                  type="button"
                  className={`type-button ${addressFormData.type === 'other' ? 'active' : ''}`}
                  onClick={() => setAddressFormData({...addressFormData, type: 'other'})}
                >
                  📍 Other
                </button>
              </div>

              <div className="form-group">
                <label>Address Line 1 *</label>
                <input
                  type="text"
                  value={addressFormData.line1}
                  onChange={(e) => setAddressFormData({...addressFormData, line1: e.target.value})}
                  placeholder="House/Flat/Block No."
                  required
                />
              </div>

              <div className="form-group">
                <label>Address Line 2</label>
                <input
                  type="text"
                  value={addressFormData.line2}
                  onChange={(e) => setAddressFormData({...addressFormData, line2: e.target.value})}
                  placeholder="Area, Street, Sector"
                />
              </div>

              <div className="form-group">
                <label>Landmark</label>
                <input
                  type="text"
                  value={addressFormData.landmark}
                  onChange={(e) => setAddressFormData({...addressFormData, landmark: e.target.value})}
                  placeholder="Any nearby landmark"
                />
              </div>

              <div className="form-row">
                <div className="form-group">
                  <label>City *</label>
                  <input
                    type="text"
                    value={addressFormData.city}
                    onChange={(e) => setAddressFormData({...addressFormData, city: e.target.value})}
                    required
                  />
                </div>
                <div className="form-group">
                  <label>State *</label>
                  <input
                    type="text"
                    value={addressFormData.state}
                    onChange={(e) => setAddressFormData({...addressFormData, state: e.target.value})}
                    required
                  />
                </div>
              </div>

              <div className="form-group">
                <label>Pincode *</label>
                <input
                  type="text"
                  value={addressFormData.pincode}
                  onChange={(e) => setAddressFormData({...addressFormData, pincode: e.target.value.replace(/\D/g, '').slice(0, 6)})}
                  placeholder="560001"
                  maxLength={6}
                  required
                />
              </div>

              <div className="form-actions">
                <button type="button" className="btn-secondary" onClick={handleCancelAddressForm}>
                  Cancel
                </button>
                <button type="submit" className="btn-primary">
                  {editingAddress ? 'Update Address' : 'Save Address'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {addresses.length === 0 ? (
        <div className="empty-state">
          <MapPin size={48} />
          <p>No addresses saved yet</p>
          <button className="btn-primary" onClick={handleAddNewAddress}>
            Add Address
          </button>
        </div>
      ) : (
        <div className="addresses-list">
          {addresses.map((address) => (
            <div key={address.id} className="address-card">
              <div className="address-type">
                {address.type === 'home' ? '🏠' : address.type === 'work' ? '🏢' : '📍'} {address.type.toUpperCase()}
              </div>
              <p className="address-text">{address.line1}</p>
              {address.line2 && <p className="address-text">{address.line2}</p>}
              {address.landmark && <p className="address-text">Landmark: {address.landmark}</p>}
              <p className="address-text">{address.city}, {address.state} - {address.pincode}</p>
              <div className="address-actions">
                <button className="btn-link" onClick={() => handleEditAddress(address)}>Edit</button>
                <button className="btn-link delete" onClick={() => address.id && handleDeleteAddress(address.id)}>Delete</button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );

  const renderPreferences = () => (
    <div className="preferences-section">
      <h3>Preferences</h3>
      
      <div className="preference-group">
        <h4>Notifications</h4>
        <div className="preference-item">
          <label>
            <input
              type="checkbox"
              checked={preferences.notifications}
              onChange={(e) => handlePreferenceChange('notifications', e.target.checked)}
            />
            <span>Push Notifications</span>
          </label>
          <p className="preference-desc">Get notified about order updates</p>
        </div>
        <div className="preference-item">
          <label>
            <input
              type="checkbox"
              checked={preferences.emailUpdates}
              onChange={(e) => handlePreferenceChange('emailUpdates', e.target.checked)}
            />
            <span>Email Updates</span>
          </label>
          <p className="preference-desc">Receive offers and promotions via email</p>
        </div>
      </div>

      <div className="preference-group">
        <h4>Dietary Restrictions</h4>
        <select
          value={preferences.dietaryRestrictions}
          onChange={(e) => handlePreferenceChange('dietaryRestrictions', e.target.value)}
        >
          <option value="">None</option>
          <option value="vegetarian">Vegetarian</option>
          <option value="vegan">Vegan</option>
          <option value="gluten-free">Gluten Free</option>
          <option value="lactose-free">Lactose Free</option>
        </select>
      </div>

      <div className="preference-group">
        <h4>Favorite Cuisines</h4>
        <div className="cuisine-tags">
          {['Indian', 'Chinese', 'Italian', 'Mexican', 'Thai'].map(cuisine => (
            <button
              key={cuisine}
              className={`cuisine-tag ${preferences.favoriteCuisines.includes(cuisine) ? 'active' : ''}`}
              onClick={() => {
                const newCuisines = preferences.favoriteCuisines.includes(cuisine)
                  ? preferences.favoriteCuisines.filter(c => c !== cuisine)
                  : [...preferences.favoriteCuisines, cuisine];
                handlePreferenceChange('favoriteCuisines', newCuisines);
              }}
            >
              {cuisine}
            </button>
          ))}
        </div>
      </div>
    </div>
  );

  return (
    <div className="profile-container">
      <header className="profile-header">
        <button className="back-button" onClick={() => navigate('/dashboard')}>
          <ArrowLeft size={24} />
        </button>
        <h1>My Profile</h1>
        <div style={{ width: '24px' }}></div>
      </header>

      <div className="profile-content">
        {/* Profile Info Card */}
        <div className="profile-info-card">
          <div className="profile-image-section">
            <div className="profile-image">
              {profile.profileImage ? (
                <img src={profile.profileImage} alt="Profile" />
              ) : (
                <User size={48} />
              )}
              <label className="image-upload-btn">
                <Camera size={16} />
                <input
                  type="file"
                  accept="image/*"
                  onChange={handleProfileImageChange}
                  style={{ display: 'none' }}
                />
              </label>
            </div>
          </div>
          <div className="profile-details">
            <h2>{profile.name || 'Guest User'}</h2>
            <p className="phone-number">{profile.phone}</p>
            {profile.email && <p className="email">{profile.email}</p>}
          </div>
        </div>

        {/* Tabs */}
        <div className="profile-tabs">
          <button
            className={`tab ${activeTab === 'orders' ? 'active' : ''}`}
            onClick={() => setActiveTab('orders')}
          >
            <Package size={20} />
            Orders
          </button>
          <button
            className={`tab ${activeTab === 'addresses' ? 'active' : ''}`}
            onClick={() => setActiveTab('addresses')}
          >
            <MapPin size={20} />
            Addresses
          </button>
          <button
            className={`tab ${activeTab === 'preferences' ? 'active' : ''}`}
            onClick={() => setActiveTab('preferences')}
          >
            <Settings size={20} />
            Preferences
          </button>
        </div>

        {/* Tab Content */}
        <div className="tab-content">
          {loading ? (
            <div className="loading-spinner">
              <div className="spinner"></div>
            </div>
          ) : (
            <>
              {activeTab === 'orders' && renderOrders()}
              {activeTab === 'addresses' && renderAddresses()}
              {activeTab === 'preferences' && renderPreferences()}
            </>
          )}
        </div>

        {/* Logout Button */}
        <button className="logout-button" onClick={handleLogout}>
          <LogOut size={20} />
          Logout
        </button>
      </div>

      {/* Order Details Modal */}
      {showOrderDetails && selectedOrder && (
        <div className="order-details-overlay" onClick={handleCloseOrderDetails}>
          <div className="order-details-modal" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2>Order Details</h2>
              <button className="close-button" onClick={handleCloseOrderDetails}>
                ×
              </button>
            </div>
            
            <div className="modal-content">
              <div className="order-info-section">
                <div className="info-row">
                  <span className="label">Order ID:</span>
                  <span className="value">#{selectedOrder.id}</span>
                </div>
                <div className="info-row">
                  <span className="label">Restaurant:</span>
                  <span className="value">{selectedOrder.restaurant}</span>
                </div>
                <div className="info-row">
                  <span className="label">Date:</span>
                  <span className="value">{new Date(selectedOrder.date).toLocaleString()}</span>
                </div>
                <div className="info-row">
                  <span className="label">Status:</span>
                  <span className={`value order-status ${getStatusColor(selectedOrder.status)}`}>
                    {selectedOrder.status}
                  </span>
                </div>
              </div>

              <div className="order-items-section">
                <h3>Items Ordered</h3>
                {selectedOrder.itemsList && selectedOrder.itemsList.length > 0 ? (
                  <div className="items-list">
                    {selectedOrder.itemsList.map((item, index) => (
                      <div key={index} className="item-row">
                        <div className="item-info">
                          <span className="item-name">{item.name}</span>
                          <span className="item-qty">x {item.quantity || 1}</span>
                        </div>
                        <span className="item-price">₹{(item.price * (item.quantity || 1)).toFixed(2)}</span>
                      </div>
                    ))}
                  </div>
                ) : (
                  <p className="no-items">No item details available</p>
                )}
              </div>

              {selectedOrder.deliveryAddress && (
                <div className="delivery-address-section">
                  <h3>Delivery Address</h3>
                  <p className="address-text">
                    {selectedOrder.deliveryAddress.line1}
                    {selectedOrder.deliveryAddress.line2 && `, ${selectedOrder.deliveryAddress.line2}`}
                    {selectedOrder.deliveryAddress.landmark && `, ${selectedOrder.deliveryAddress.landmark}`}
                    <br />
                    {selectedOrder.deliveryAddress.city}, {selectedOrder.deliveryAddress.state} - {selectedOrder.deliveryAddress.pincode}
                  </p>
                </div>
              )}

              <div className="order-total-section">
                <div className="total-row">
                  <span className="total-label">Total Amount:</span>
                  <span className="total-value">₹{selectedOrder.total}</span>
                </div>
              </div>
            </div>

            <div className="modal-actions">
              <button className="btn-secondary" onClick={handleCloseOrderDetails}>
                Close
              </button>
              {selectedOrder.status === 'Delivered' && (
                <button className="btn-primary" onClick={() => {
                  handleCloseOrderDetails();
                  handleReorder(selectedOrder);
                }}>
                  Reorder
                </button>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default Profile;
