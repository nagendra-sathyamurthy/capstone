import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { Phone, ArrowRight } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import { authService } from '../services/api';
import '../styles/Registration.css';

const Registration = () => {
  const [phoneNumber, setPhoneNumber] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const navigate = useNavigate();
  const { setPhone, setLoading } = useAuth();

  useEffect(() => {
    // Pre-fill phone number for returning users
    const savedPhone = localStorage.getItem('userPhone');
    if (savedPhone) {
      setPhoneNumber(savedPhone);
    }
  }, []);

  const validatePhoneNumber = (phone) => {
    const phoneRegex = /^[6-9]\d{9}$/;
    return phoneRegex.test(phone);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!validatePhoneNumber(phoneNumber)) {
      toast.error('Please enter a valid 10-digit mobile number');
      return;
    }

    setIsLoading(true);
    setLoading(true);

    try {
      // For demo purposes, we'll simulate OTP sending
      // In production, this would call the actual API
      await new Promise(resolve => setTimeout(resolve, 1000)); // Simulate API call
      
      setPhone(phoneNumber);
      toast.success('OTP sent successfully!');
      navigate('/verify-otp');
    } catch (error) {
      toast.error(error.message || 'Failed to send OTP');
    } finally {
      setIsLoading(false);
      setLoading(false);
    }
  };

  return (
    <div className="registration-container">
      <div className="registration-card">
        <div className="logo-section">
          <h1>🍽️ FoodApp</h1>
          <p>Delicious food delivered to your doorstep</p>
        </div>
        
        <form onSubmit={handleSubmit} className="registration-form">
          <h2>Welcome!</h2>
          <p className="subtitle">
            Enter your mobile number to get started
          </p>
          
          <div className="phone-input-container">
            <div className="phone-input-group">
              <span className="country-code">+91</span>
              <Phone className="phone-icon" size={20} />
              <input
                type="tel"
                placeholder="Enter mobile number"
                value={phoneNumber}
                onChange={(e) => setPhoneNumber(e.target.value.replace(/\D/g, '').slice(0, 10))}
                className="phone-input"
                maxLength={10}
                required
              />
            </div>
          </div>
          
          <button
            type="submit"
            className={`continue-button ${isLoading ? 'loading' : ''}`}
            disabled={isLoading || phoneNumber.length !== 10}
          >
            {isLoading ? (
              <span className="loading-spinner"></span>
            ) : (
              <>
                Continue <ArrowRight size={20} />
              </>
            )}
          </button>
          
          <div className="terms-text">
            By continuing, you agree to our Terms of Service and Privacy Policy
          </div>
        </form>
      </div>
    </div>
  );
};

export default Registration;