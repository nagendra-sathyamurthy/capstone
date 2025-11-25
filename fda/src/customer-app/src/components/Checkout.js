import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { ArrowLeft, MapPin, Plus, Clock, CreditCard } from 'lucide-react';
import { useCart } from '../context/CartContext';
import { toast } from 'react-toastify';
import '../styles/Checkout.css';

const Checkout = () => {
  const navigate = useNavigate();
  const { items, totalAmount, setDeliveryAddress, setOrderSummary } = useCart();
  const [selectedAddress, setSelectedAddress] = useState(null);
  const [showAddressForm, setShowAddressForm] = useState(false);
  const [newAddress, setNewAddress] = useState({
    type: 'home',
    line1: '',
    line2: '',
    landmark: '',
    city: 'Bangalore',
    state: 'Karnataka',
    pincode: ''
  });

  // Load addresses from localStorage
  const [addresses, setAddresses] = useState([]);

  useEffect(() => {
    // Get current user's identifier from localStorage
    const userId = localStorage.getItem('userId');
    const userPhone = localStorage.getItem('userPhone');
    const userKey = userId || userPhone;
    
    if (!userKey) {
      console.error('No user identifier found');
      return;
    }
    
    const addressStorageKey = `customerAddresses_${userKey}`;
    
    // Load saved addresses from localStorage
    const savedAddresses = JSON.parse(localStorage.getItem(addressStorageKey) || '[]');
    
    // If no saved addresses, provide default mock addresses
    if (savedAddresses.length === 0) {
      const defaultAddresses = [
        {
          id: '1',
          type: 'home',
          line1: '123, MG Road',
          line2: 'Shivaji Nagar',
          landmark: 'Near Metro Station',
          city: 'Bangalore',
          state: 'Karnataka',
          pincode: '560001'
        },
        {
          id: '2',
          type: 'work',
          line1: '456, Brigade Road',
          line2: 'Commercial Street',
          landmark: 'Opposite Coffee Shop',
          city: 'Bangalore',
          state: 'Karnataka',
          pincode: '560025'
        }
      ];
      localStorage.setItem(addressStorageKey, JSON.stringify(defaultAddresses));
      setAddresses(defaultAddresses);
    } else {
      setAddresses(savedAddresses);
    }
  }, []);

  const deliveryFee = 29;
  const platformFee = 5;
  const gst = Math.round(totalAmount * 0.05);
  const finalTotal = totalAmount + deliveryFee + platformFee + gst;

  const handleAddressSelect = (address) => {
    setSelectedAddress(address);
  };

  const handleAddNewAddress = (e) => {
    e.preventDefault();
    if (!newAddress.line1 || !newAddress.pincode) {
      toast.error('Please fill in all required fields');
      return;
    }

    const addressToAdd = {
      id: Date.now().toString(),
      ...newAddress
    };

    const updatedAddresses = [...addresses, addressToAdd];
    setAddresses(updatedAddresses);
    
    // Get current user's identifier from localStorage
    const userId = localStorage.getItem('userId');
    const userPhone = localStorage.getItem('userPhone');
    const userKey = userId || userPhone;
    
    if (userKey) {
      const addressStorageKey = `customerAddresses_${userKey}`;
      localStorage.setItem(addressStorageKey, JSON.stringify(updatedAddresses));
    }
    
    setSelectedAddress(addressToAdd);
    setShowAddressForm(false);
    setNewAddress({
      type: 'home',
      line1: '',
      line2: '',
      landmark: '',
      city: 'Bangalore',
      state: 'Karnataka',
      pincode: ''
    });
    toast.success('Address added successfully!');
  };

  const handleProceedToPayment = () => {
    if (!selectedAddress) {
      toast.error('Please select a delivery address');
      return;
    }

    if (items.length === 0) {
      toast.error('Your cart is empty!');
      return;
    }

    // Extract restaurant name from first item (all items should be from same restaurant)
    const restaurantName = items[0]?.restaurantName || 'Restaurant';
    const restaurantId = items[0]?.restaurantId;

    const orderSummary = {
      items,
      restaurant: restaurantName,
      restaurantId: restaurantId,
      deliveryAddress: selectedAddress,
      totalAmount,
      deliveryFee,
      platformFee,
      gst,
      finalTotal,
      orderId: `ORD${Date.now()}`,
      estimatedDeliveryTime: '30-40 min'
    };

    setDeliveryAddress(selectedAddress);
    setOrderSummary(orderSummary);
    navigate('/payment');
  };

  return (
    <div className="checkout-container">
      <header className="checkout-header">
        <button
          className="back-button"
          onClick={() => navigate('/cart')}
        >
          <ArrowLeft size={20} />
        </button>
        <h1>Checkout</h1>
      </header>

      <div className="checkout-content">
        {/* Delivery Address Section */}
        <div className="section delivery-address-section">
          <div className="section-header">
            <h2><MapPin size={20} /> Delivery Address</h2>
            <button
              className="add-address-button"
              onClick={() => setShowAddressForm(true)}
            >
              <Plus size={16} /> Add New
            </button>
          </div>

          {showAddressForm && (
            <div className="address-form-overlay">
              <div className="address-form">
                <div className="form-header">
                  <h3>Add New Address</h3>
                  <button
                    className="close-form-button"
                    onClick={() => setShowAddressForm(false)}
                  >
                    ×
                  </button>
                </div>
                
                <form onSubmit={handleAddNewAddress}>
                  <div className="address-type-selector">
                    <button
                      type="button"
                      className={`type-button ${newAddress.type === 'home' ? 'active' : ''}`}
                      onClick={() => setNewAddress({...newAddress, type: 'home'})}
                    >
                      🏠 Home
                    </button>
                    <button
                      type="button"
                      className={`type-button ${newAddress.type === 'work' ? 'active' : ''}`}
                      onClick={() => setNewAddress({...newAddress, type: 'work'})}
                    >
                      🏢 Work
                    </button>
                    <button
                      type="button"
                      className={`type-button ${newAddress.type === 'other' ? 'active' : ''}`}
                      onClick={() => setNewAddress({...newAddress, type: 'other'})}
                    >
                      📍 Other
                    </button>
                  </div>

                  <div className="form-group">
                    <label>Address Line 1 *</label>
                    <input
                      type="text"
                      value={newAddress.line1}
                      onChange={(e) => setNewAddress({...newAddress, line1: e.target.value})}
                      placeholder="House/Flat/Block No."
                      required
                    />
                  </div>

                  <div className="form-group">
                    <label>Address Line 2</label>
                    <input
                      type="text"
                      value={newAddress.line2}
                      onChange={(e) => setNewAddress({...newAddress, line2: e.target.value})}
                      placeholder="Area, Street, Sector"
                    />
                  </div>

                  <div className="form-group">
                    <label>Landmark</label>
                    <input
                      type="text"
                      value={newAddress.landmark}
                      onChange={(e) => setNewAddress({...newAddress, landmark: e.target.value})}
                      placeholder="Any nearby landmark"
                    />
                  </div>

                  <div className="form-row">
                    <div className="form-group">
                      <label>City *</label>
                      <input
                        type="text"
                        value={newAddress.city}
                        onChange={(e) => setNewAddress({...newAddress, city: e.target.value})}
                        required
                      />
                    </div>
                    <div className="form-group">
                      <label>State *</label>
                      <input
                        type="text"
                        value={newAddress.state}
                        onChange={(e) => setNewAddress({...newAddress, state: e.target.value})}
                        required
                      />
                    </div>
                    <div className="form-group">
                      <label>Pincode *</label>
                      <input
                        type="text"
                        value={newAddress.pincode}
                        onChange={(e) => setNewAddress({...newAddress, pincode: e.target.value.replace(/\D/g, '').slice(0, 6)})}
                        placeholder="560001"
                        required
                      />
                    </div>
                  </div>

                  <button type="submit" className="save-address-button">
                    Save Address
                  </button>
                </form>
              </div>
            </div>
          )}

          <div className="addresses-list">
            {addresses.map(address => (
              <div
                key={address.id}
                className={`address-card ${selectedAddress?.id === address.id ? 'selected' : ''}`}
                onClick={() => handleAddressSelect(address)}
              >
                <div className="address-type">
                  {address.type === 'home' ? '🏠' : address.type === 'work' ? '🏢' : '📍'}
                  <span className="type-label">{address.type.charAt(0).toUpperCase() + address.type.slice(1)}</span>
                </div>
                <div className="address-details">
                  <p className="address-text">{address.line1}</p>
                  {address.line2 && <p className="address-text">{address.line2}</p>}
                  {address.landmark && <p className="landmark">Landmark: {address.landmark}</p>}
                  <p className="city-pincode">{address.city}, {address.state} - {address.pincode}</p>
                </div>
                {selectedAddress?.id === address.id && (
                  <div className="selected-indicator">✓</div>
                )}
              </div>
            ))}
          </div>
        </div>

        {/* Order Summary */}
        <div className="section order-summary-section">
          <h2>Order Summary</h2>
          <div className="order-items">
            {items.map(item => (
              <div key={item.id} className="order-item">
                <div className="item-info">
                  <span className="item-name">{item.name}</span>
                  <span className="item-quantity">× {item.quantity}</span>
                </div>
                <span className="item-total">₹{item.price * item.quantity}</span>
              </div>
            ))}
          </div>

          <div className="bill-breakdown">
            <div className="bill-row">
              <span>Item Total</span>
              <span>₹{totalAmount}</span>
            </div>
            <div className="bill-row">
              <span>Delivery Fee</span>
              <span>₹{deliveryFee}</span>
            </div>
            <div className="bill-row">
              <span>Platform Fee</span>
              <span>₹{platformFee}</span>
            </div>
            <div className="bill-row">
              <span>GST (5%)</span>
              <span>₹{gst}</span>
            </div>
            <hr />
            <div className="bill-row total">
              <span>Total Amount</span>
              <span>₹{finalTotal}</span>
            </div>
          </div>
        </div>

        {/* Delivery Info */}
        <div className="section delivery-info-section">
          <h2><Clock size={20} /> Delivery Information</h2>
          <div className="delivery-info">
            <div className="info-item">
              <span className="label">Estimated Delivery Time:</span>
              <span className="value">30-40 minutes</span>
            </div>
            <div className="info-item">
              <span className="label">Delivery Partner:</span>
              <span className="value">Will be assigned shortly</span>
            </div>
          </div>
        </div>

        {/* Proceed to Payment */}
        <div className="payment-section">
          <button
            className="proceed-payment-button"
            onClick={handleProceedToPayment}
          >
            <CreditCard size={20} />
            Proceed to Payment • ₹{finalTotal}
          </button>
        </div>
      </div>
    </div>
  );
};

export default Checkout;