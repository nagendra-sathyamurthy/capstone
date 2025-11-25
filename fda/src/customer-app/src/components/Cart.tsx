import React from 'react';
import { useNavigate } from 'react-router-dom';
import { ArrowLeft, Plus, Minus, Trash2, ShoppingBag } from 'lucide-react';
import { useCart } from '../context/CartContext';
import { toast } from 'react-toastify';
import { MenuItem } from '../types';
import '../styles/Cart.css';

const Cart: React.FC = () => {
  const navigate = useNavigate();
  const { items, totalAmount, totalItems, addToCart, removeFromCart, clearCart } = useCart();

  const deliveryFee = totalAmount > 0 ? 29 : 0;
  const platformFee = totalAmount > 0 ? 5 : 0;
  const gst = totalAmount > 0 ? Math.round(totalAmount * 0.05) : 0;
  const finalTotal = totalAmount + deliveryFee + platformFee + gst;

  const handleIncrement = (item: MenuItem): void => {
    addToCart(item);
  };

  const handleDecrement = (item: MenuItem): void => {
    removeFromCart(item.id);
  };

  const handleClearCart = (): void => {
    clearCart();
    toast.success('Cart cleared!');
  };

  const handleCheckout = (): void => {
    if (items.length === 0) {
      toast.error('Your cart is empty!');
      return;
    }
    navigate('/checkout');
  };

  if (items.length === 0) {
    return (
      <div className="cart-container">
        <header className="cart-header">
          <button
            className="back-button"
            onClick={() => navigate('/dashboard')}
          >
            <ArrowLeft size={20} />
          </button>
          <h1>Your Cart</h1>
        </header>

        <div className="empty-cart">
          <ShoppingBag size={80} className="empty-cart-icon" />
          <h2>Your cart is empty</h2>
          <p>Looks like you haven't added anything to your cart yet</p>
          <button
            className="browse-button"
            onClick={() => navigate('/dashboard')}
          >
            Browse Food
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="cart-container">
      <header className="cart-header">
        <button
          className="back-button"
          onClick={() => navigate('/dashboard')}
        >
          <ArrowLeft size={20} />
        </button>
        <div className="cart-title">
          <h1>Your Cart</h1>
          <span className="item-count">{totalItems} item{totalItems > 1 ? 's' : ''}</span>
        </div>
        <button
          className="clear-cart-button"
          onClick={handleClearCart}
        >
          <Trash2 size={18} />
        </button>
      </header>

      <div className="cart-content">
        {/* Cart Items */}
        <div className="cart-items">
          {items.map(item => (
            <div key={item.id} className="cart-item">
              <div className="item-info">
                <div className="item-image">{item.image}</div>
                <div className="item-details">
                  <h3>{item.name}</h3>
                  <p className="restaurant-name">{item.restaurant || 'Restaurant'}</p>
                  <span className="item-price">₹{item.price}</span>
                </div>
              </div>
              
              <div className="quantity-controls">
                <button
                  className="quantity-button"
                  onClick={() => handleDecrement(item)}
                >
                  <Minus size={16} />
                </button>
                <span className="quantity">{item.quantity}</span>
                <button
                  className="quantity-button"
                  onClick={() => handleIncrement(item)}
                >
                  <Plus size={16} />
                </button>
              </div>
              
              <div className="item-total">
                ₹{item.price * item.quantity}
              </div>
            </div>
          ))}
        </div>

        {/* Bill Details */}
        <div className="bill-details">
          <h3>Bill Details</h3>
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
          <hr className="bill-divider" />
          <div className="bill-row total">
            <span>Total Amount</span>
            <span>₹{finalTotal}</span>
          </div>
        </div>

        {/* Offers Section */}
        <div className="offers-section">
          <h3>🎉 Available Offers</h3>
          <div className="offer-card">
            <div className="offer-info">
              <h4>SAVE20</h4>
              <p>Save ₹20 on orders above ₹200</p>
            </div>
            <button className="apply-offer-button">Apply</button>
          </div>
        </div>

        {/* Checkout Button */}
        <div className="checkout-section">
          <button
            className="checkout-button"
            onClick={handleCheckout}
          >
            Proceed to Checkout • ₹{finalTotal}
          </button>
        </div>
      </div>
    </div>
  );
};

export default Cart;