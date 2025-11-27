import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { Search, Filter, ShoppingCart, User, Leaf, Drumstick, Star, MapPin, ChevronDown } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import { useCart } from '../context/CartContext';
import { catalogService, customerService } from '../services/api';
import { Restaurant, MenuItem, DeliveryAddress, FilterOptions } from '../types';
import '../styles/Dashboard.css';

const Dashboard: React.FC = () => {
  const [restaurants, setRestaurants] = useState<Restaurant[]>([]);
  const [menuItems, setMenuItems] = useState<MenuItem[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [searchQuery, setSearchQuery] = useState<string>('');
  const [selectedRestaurant, setSelectedRestaurant] = useState<Restaurant | null>(null);
  const [selectedFilters, setSelectedFilters] = useState<FilterOptions>({
    cuisine: 'all',
    dietary: 'all', // all, veg, non-veg
    priceRange: 'all'
  });
  const [showFilters, setShowFilters] = useState<boolean>(false);
  const [addresses, setAddresses] = useState<DeliveryAddress[]>([]);
  const [selectedAddress, setSelectedAddress] = useState<DeliveryAddress | null>(null);
  const [showAddressDropdown, setShowAddressDropdown] = useState<boolean>(false);
  const [addressNotificationShown, setAddressNotificationShown] = useState<boolean>(false);
  const [userName, setUserName] = useState<string>('');

  const navigate = useNavigate();
  const { user } = useAuth();
  const { addToCart, totalItems } = useCart();

  useEffect(() => {
    if (!user) {
      navigate('/');
      return;
    }
    
    loadUserProfile();
    loadAddresses();
    fetchData();
  }, [user, navigate]);

  const loadUserProfile = async () => {
    try {
      const profileData = await customerService.getProfile();
      if (profileData) {
        // Combine firstName and lastName
        const fullName = [profileData.firstName, profileData.lastName]
          .filter(Boolean)
          .join(' ');
        setUserName(fullName || user?.name || 'User');
      }
    } catch (error) {
      console.log('Failed to load profile, using default name');
      setUserName(user?.name || 'User');
    }
  };

  const loadAddresses = async () => {
    const userId = localStorage.getItem('userId');

    console.log('[Dashboard] Loading addresses for userId:', userId);

    if (!userId) {
      console.log('[Dashboard] No userId found in localStorage');
      return;
    }

    try {
      // Fetch addresses from MongoDB via API
      console.log('[Dashboard] Calling customerService.getAddresses()...');
      const response = await customerService.getAddresses();
      console.log('[Dashboard] Addresses response:', response);
      
      if (response && response.length > 0) {
        console.log('[Dashboard] Found', response.length, 'addresses');
        setAddresses(response);
        // Select first address by default
        setSelectedAddress(response[0]);
      } else {
        console.log('[Dashboard] No addresses found in response');
        // No addresses found, show notification only once
        if (!addressNotificationShown) {
          toast.info('Please add your delivery address');
          setAddressNotificationShown(true);
        }
      }
    } catch (error) {
      console.error('[Dashboard] Error loading addresses:', error);
      if (error instanceof Error) {
        console.error('[Dashboard] Error details:', error.message);
      }
      // Show notification only once
      if (!addressNotificationShown) {
        toast.info('Please add your delivery address');
        setAddressNotificationShown(true);
      }
    }
  };

  const fetchData = async () => {
    try {
      setLoading(true);
      
      // Fetch menu items from catalog API
      console.log('[Dashboard] Fetching menu items from API...');
      const menuResponse = await catalogService.getAvailableMenuItems();
      console.log('[Dashboard] Menu items received:', menuResponse);
      
      if (menuResponse && menuResponse.length > 0) {
        // Transform API response to match frontend MenuItem type
        const transformedMenuItems = menuResponse.map((item: any) => ({
          id: item.id,
          name: item.name,
          description: item.description,
          price: item.pricePerUOM,
          category: item.category,
          image: getCategoryEmoji(item.category),
          restaurant: item.restaurantName || 'Restaurant',
          restaurantId: item.restaurantId || '1',
          isVeg: item.isVegetarian,
          isAvailable: item.isAvailable,
          rating: 4.0 + Math.random() // Random rating for demo
        }));
        
        setMenuItems(transformedMenuItems);
        
        // Extract unique restaurants from menu items
        const uniqueRestaurants = Array.from(
          new Map(
            menuResponse
              .filter((item: any) => item.restaurantName)
              .map((item: any) => [
                item.restaurantId,
                {
                  id: item.restaurantId,
                  name: item.restaurantName,
                  cuisine: item.cuisine || 'Multi-Cuisine',
                  rating: 4.0 + Math.random(),
                  deliveryTime: '25-35 min',
                  image: getRestaurantEmoji(item.cuisine),
                  isOpen: true
                }
              ])
          ).values()
        );
        
        setRestaurants(uniqueRestaurants);
        console.log('[Dashboard] Loaded', transformedMenuItems.length, 'menu items and', uniqueRestaurants.length, 'restaurants');
      } else {
        console.log('[Dashboard] No menu items found, displaying empty state');
        toast.info('No menu items available at the moment. Please run the seed script to add sample data.');
      }
    } catch (error) {
      console.error('[Dashboard] Error loading data:', error);
      toast.error('Failed to load menu data. Please try again later.');
    } finally {
      setLoading(false);
    }
  };
  
  // Helper function to get emoji based on category
  const getCategoryEmoji = (category: string): string => {
    const emojiMap: { [key: string]: string } = {
      'Appetizer': '🥗',
      'Main Course': '🍛',
      'Dessert': '🍰',
      'Beverage': '🥤',
      'Salad': '🥗',
      'Pizza': '🍕',
      'Burger': '🍔',
      'Default': '🍽️'
    };
    return emojiMap[category] || emojiMap['Default'];
  };
  
  // Helper function to get emoji based on cuisine
  const getRestaurantEmoji = (cuisine: string): string => {
    const emojiMap: { [key: string]: string } = {
      'Italian': '🍕',
      'American': '🍔',
      'Indian': '🍛',
      'Thai': '🍜',
      'French': '🥖',
      'Mediterranean': '🥙',
      'Health Food': '🥗',
      'Default': '🏪'
    };
    return emojiMap[cuisine] || emojiMap['Default'];
  };

  const filteredMenuItems = menuItems.filter(item => {
    // Restaurant filter
    if (selectedRestaurant && item.restaurant !== selectedRestaurant.name) {
      return false;
    }

    // Search filter
    if (searchQuery && !item.name.toLowerCase().includes(searchQuery.toLowerCase())) {
      return false;
    }

    // Cuisine filter (use category instead of cuisine since MenuItem doesn't have cuisine)
    if (selectedFilters.cuisine !== 'all' && item.category !== selectedFilters.cuisine) {
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

  const handleAddToCart = (item: MenuItem): void => {
    addToCart(item);
    toast.success(`${item.name} added to cart!`);
  };

  const handleRestaurantClick = (restaurant: Restaurant): void => {
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

  const handleClearRestaurantFilter = (): void => {
    setSelectedRestaurant(null);
    toast.info('Showing all restaurants');
  };

  const handleProfileClick = (): void => {
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
            <h1>Hi, {userName || user?.name || 'User'}!</h1>
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
                  <p className="restaurant-name">{item.restaurant || 'Restaurant'}</p>
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