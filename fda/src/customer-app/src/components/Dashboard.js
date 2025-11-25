import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { Search, Filter, ShoppingCart, User, Leaf, Drumstick, Star, MapPin, ChevronDown } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import { useCart } from '../context/CartContext';
import { catalogService } from '../services/api';
import '../styles/Dashboard.css';

const Dashboard = () => {
  const [restaurants, setRestaurants] = useState([]);
  const [menuItems, setMenuItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedRestaurant, setSelectedRestaurant] = useState(null);
  const [selectedFilters, setSelectedFilters] = useState({
    cuisine: 'all',
    dietary: 'all', // all, veg, non-veg
    priceRange: 'all'
  });
  const [showFilters, setShowFilters] = useState(false);
  const [addresses, setAddresses] = useState([]);
  const [selectedAddress, setSelectedAddress] = useState(null);
  const [showAddressDropdown, setShowAddressDropdown] = useState(false);
  const [addressNotificationShown, setAddressNotificationShown] = useState(false);

  const navigate = useNavigate();
  const { user } = useAuth();
  const { addToCart, totalItems } = useCart();

  // Mock data for demonstration
  const mockRestaurants = [
    {
      id: '1',
      name: 'Spice Kitchen',
      cuisine: 'Indian',
      rating: 4.5,
      deliveryTime: '30-35 min',
      image: '🏪',
      isOpen: true
    },
    {
      id: '2',
      name: 'Pizza Palace',
      cuisine: 'Italian',
      rating: 4.2,
      deliveryTime: '25-30 min',
      image: '🍕',
      isOpen: true
    },
    {
      id: '3',
      name: 'Burger Junction',
      cuisine: 'American',
      rating: 4.0,
      deliveryTime: '20-25 min',
      image: '🍔',
      isOpen: false
    }
  ];

  const mockMenuItems = [
    {
      id: '1',
      name: 'Chicken Biryani',
      description: 'Aromatic basmati rice with tender chicken',
      price: 299,
      category: 'Main Course',
      cuisine: 'Indian',
      isVeg: false,
      rating: 4.6,
      image: '🍛',
      restaurantId: '1',
      restaurantName: 'Spice Kitchen'
    },
    {
      id: '2',
      name: 'Margherita Pizza',
      description: 'Fresh tomato sauce, mozzarella, and basil',
      price: 399,
      category: 'Pizza',
      cuisine: 'Italian',
      isVeg: true,
      rating: 4.3,
      image: '🍕',
      restaurantId: '2',
      restaurantName: 'Pizza Palace'
    },
    {
      id: '3',
      name: 'Paneer Butter Masala',
      description: 'Creamy curry with cottage cheese',
      price: 249,
      category: 'Main Course',
      cuisine: 'Indian',
      isVeg: true,
      rating: 4.4,
      image: '🍛',
      restaurantId: '1',
      restaurantName: 'Spice Kitchen'
    },
    {
      id: '4',
      name: 'Chicken Burger',
      description: 'Grilled chicken patty with fresh vegetables',
      price: 199,
      category: 'Burger',
      cuisine: 'American',
      isVeg: false,
      rating: 4.1,
      image: '🍔',
      restaurantId: '3',
      restaurantName: 'Burger Junction'
    },
    {
      id: '5',
      name: 'Veg Biryani',
      description: 'Fragrant rice with mixed vegetables',
      price: 229,
      category: 'Main Course',
      cuisine: 'Indian',
      isVeg: true,
      rating: 4.2,
      image: '🍛',
      restaurantId: '1',
      restaurantName: 'Spice Kitchen'
    }
  ];

  useEffect(() => {
    if (!user) {
      navigate('/');
      return;
    }
    
    loadAddresses();
    fetchData();
  }, [user, navigate]);

  const loadAddresses = () => {
    const userId = localStorage.getItem('userId');
    const userPhone = localStorage.getItem('userPhone');
    const userKey = userId || userPhone;

    if (!userKey) {
      return;
    }

    const addressStorageKey = `customerAddresses_${userKey}`;
    const savedAddresses = JSON.parse(localStorage.getItem(addressStorageKey) || '[]');
    
    if (savedAddresses.length === 0) {
      // No addresses found, show notification only once
      if (!addressNotificationShown) {
        toast.info('Please add your delivery address');
        setAddressNotificationShown(true);
        // Don't redirect, just show the notification
      }
      return;
    }

    setAddresses(savedAddresses);
    // Select first address by default
    setSelectedAddress(savedAddresses[0]);
  };

  const fetchData = async () => {
    try {
      setLoading(true);
      // For demo purposes, using mock data
      // In production, these would be actual API calls
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      setRestaurants(mockRestaurants);
      setMenuItems(mockMenuItems);
    } catch (error) {
      toast.error('Failed to load data');
    } finally {
      setLoading(false);
    }
  };

  const filteredMenuItems = menuItems.filter(item => {
    // Restaurant filter
    if (selectedRestaurant && item.restaurantName !== selectedRestaurant.name) {
      return false;
    }

    // Search filter
    if (searchQuery && !item.name.toLowerCase().includes(searchQuery.toLowerCase()) &&
        !item.cuisine.toLowerCase().includes(searchQuery.toLowerCase())) {
      return false;
    }

    // Cuisine filter
    if (selectedFilters.cuisine !== 'all' && item.cuisine !== selectedFilters.cuisine) {
      return false;
    }

    // Dietary filter
    if (selectedFilters.dietary === 'veg' && !item.isVeg) {
      return false;
    }
    if (selectedFilters.dietary === 'non-veg' && item.isVeg) {
      return false;
    }

    // Price range filter
    if (selectedFilters.priceRange === 'budget' && item.price > 200) {
      return false;
    }
    if (selectedFilters.priceRange === 'premium' && item.price < 300) {
      return false;
    }

    return true;
  });

  const handleAddToCart = (item) => {
    addToCart(item);
    toast.success(`${item.name} added to cart!`);
  };

  const handleRestaurantClick = (restaurant) => {
    if (!restaurant.isOpen) {
      toast.info(`${restaurant.name} is currently closed`);
      return;
    }
    
    setSelectedRestaurant(restaurant);
    toast.success(`Showing menu from ${restaurant.name}`);
    
    // Scroll to menu section
    const menuSection = document.querySelector('.menu-section');
    if (menuSection) {
      menuSection.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  };

  const handleClearRestaurantFilter = () => {
    setSelectedRestaurant(null);
    toast.info('Showing all restaurants');
  };

  const handleProfileClick = () => {
    navigate('/profile');
  };

  if (loading) {
    return (
      <div className="loading-container">
        <div className="loading-spinner"></div>
        <p>Loading delicious food...</p>
      </div>
    );
  }

  return (
    <div className="dashboard-container">
      {/* Header */}
      <header className="dashboard-header">
        <div className="header-content">
          <div className="user-info">
            <h1>Hi, {user?.name}!</h1>
            <p>What would you like to eat today?</p>
          </div>
          <div className="header-actions">
            <button
              className="cart-button"
              onClick={() => navigate('/cart')}
            >
              <ShoppingCart size={24} />
              {totalItems > 0 && <span className="cart-badge">{totalItems}</span>}
            </button>
            <button className="profile-button" onClick={handleProfileClick}>
              <User size={24} />
            </button>
          </div>
        </div>
      </header>

      {/* Address Selection */}
      {selectedAddress && (
        <div className="address-selector">
          <div className="address-dropdown" onClick={() => setShowAddressDropdown(!showAddressDropdown)}>
            <MapPin size={20} className="address-icon" />
            <div className="address-text">
              <span className="address-label">Delivering to</span>
              <span className="address-value">
                {selectedAddress.type === 'home' ? '🏠' : selectedAddress.type === 'work' ? '💼' : '📍'} {selectedAddress.line1}, {selectedAddress.city}
              </span>
            </div>
            <ChevronDown size={20} className={`dropdown-icon ${showAddressDropdown ? 'rotate' : ''}`} />
          </div>
          
          {showAddressDropdown && (
            <div className="address-dropdown-menu">
              {addresses.map((address) => (
                <div
                  key={address.id}
                  className={`address-option ${selectedAddress.id === address.id ? 'selected' : ''}`}
                  onClick={() => {
                    setSelectedAddress(address);
                    setShowAddressDropdown(false);
                    toast.success(`Delivery address updated to ${address.line1}`);
                  }}
                >
                  <MapPin size={16} />
                  <div className="address-option-text">
                    <span className="address-type">
                      {address.type === 'home' ? '🏠 Home' : address.type === 'work' ? '💼 Work' : '📍 Other'}
                    </span>
                    <span className="address-details">
                      {address.line1}, {address.city}, {address.pincode}
                    </span>
                  </div>
                </div>
              ))}
              <div
                className="address-option add-new"
                onClick={() => {
                  setShowAddressDropdown(false);
                  navigate('/profile');
                }}
              >
                <MapPin size={16} />
                <span>Add New Address</span>
              </div>
            </div>
          )}
        </div>
      )}

      {/* Search and Filters */}
      <div className="search-section">
        <div className="search-bar">
          <Search className="search-icon" size={20} />
          <input
            type="text"
            placeholder="Search for food, restaurants..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
          />
        </div>
        <button
          className={`filter-button ${showFilters ? 'active' : ''}`}
          onClick={() => setShowFilters(!showFilters)}
        >
          <Filter size={20} />
        </button>
      </div>

      {/* Filter Panel */}
      {showFilters && (
        <div className="filter-panel">
          <div className="filter-group">
            <label>Cuisine</label>
            <select
              value={selectedFilters.cuisine}
              onChange={(e) => setSelectedFilters({...selectedFilters, cuisine: e.target.value})}
            >
              <option value="all">All Cuisines</option>
              <option value="Indian">Indian</option>
              <option value="Italian">Italian</option>
              <option value="American">American</option>
              <option value="Chinese">Chinese</option>
            </select>
          </div>
          
          <div className="filter-group">
            <label>Dietary Preference</label>
            <div className="dietary-filters">
              <button
                className={selectedFilters.dietary === 'all' ? 'active' : ''}
                onClick={() => setSelectedFilters({...selectedFilters, dietary: 'all'})}
              >
                All
              </button>
              <button
                className={selectedFilters.dietary === 'veg' ? 'active' : ''}
                onClick={() => setSelectedFilters({...selectedFilters, dietary: 'veg'})}
              >
                <Leaf size={16} /> Veg
              </button>
              <button
                className={selectedFilters.dietary === 'non-veg' ? 'active' : ''}
                onClick={() => setSelectedFilters({...selectedFilters, dietary: 'non-veg'})}
              >
                <Drumstick size={16} /> Non-Veg
              </button>
            </div>
          </div>

          <div className="filter-group">
            <label>Price Range</label>
            <select
              value={selectedFilters.priceRange}
              onChange={(e) => setSelectedFilters({...selectedFilters, priceRange: e.target.value})}
            >
              <option value="all">All Prices</option>
              <option value="budget">Under ₹200</option>
              <option value="premium">₹300+</option>
            </select>
          </div>
        </div>
      )}

      {/* Restaurants Section */}
      <div className="restaurants-section">
        <h2>Restaurants Near You</h2>
        <div className="restaurants-grid">
          {restaurants.map(restaurant => (
            <div 
              key={restaurant.id} 
              className={`restaurant-card ${!restaurant.isOpen ? 'closed' : ''} ${selectedRestaurant?.id === restaurant.id ? 'selected' : ''}`}
              onClick={() => handleRestaurantClick(restaurant)}
            >
              <div className="restaurant-image">{restaurant.image}</div>
              <div className="restaurant-info">
                <h3>{restaurant.name}</h3>
                <p className="cuisine">{restaurant.cuisine}</p>
                <div className="restaurant-meta">
                  <span className="rating">
                    <Star size={14} /> {restaurant.rating}
                  </span>
                  <span className="delivery-time">{restaurant.deliveryTime}</span>
                </div>
                {!restaurant.isOpen && <span className="closed-badge">Closed</span>}
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Menu Items Section */}
      <div className="menu-section">
        <div className="menu-section-header">
          <h2>{selectedRestaurant ? `Menu from ${selectedRestaurant.name}` : 'Popular Dishes'}</h2>
          {selectedRestaurant && (
            <button className="btn-clear-filter" onClick={handleClearRestaurantFilter}>
              Show All Restaurants
            </button>
          )}
        </div>
        {filteredMenuItems.length === 0 ? (
          <div className="no-results">
            <p>No items found matching your criteria</p>
          </div>
        ) : (
          <div className="menu-grid">
            {filteredMenuItems.map(item => (
              <div key={item.id} className="menu-item-card">
                <div className="item-image">{item.image}</div>
                <div className="item-info">
                  <div className="item-header">
                    <h3>{item.name}</h3>
                    <span className={`veg-indicator ${item.isVeg ? 'veg' : 'non-veg'}`}>
                      {item.isVeg ? <Leaf size={12} /> : <Drumstick size={12} />}
                    </span>
                  </div>
                  <p className="item-description">{item.description}</p>
                  <p className="restaurant-name">{item.restaurantName}</p>
                  <div className="item-footer">
                    <div className="item-rating">
                      <Star size={14} /> {item.rating}
                    </div>
                    <div className="price-add">
                      <span className="price">₹{item.price}</span>
                      <button
                        className="add-button"
                        onClick={() => handleAddToCart(item)}
                      >
                        Add
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default Dashboard;