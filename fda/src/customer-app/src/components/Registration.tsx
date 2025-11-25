import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { Phone, ArrowRight } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import { authService } from '../services/api';
import '../styles/Registration.css';

const Registration: React.FC = () => {
  const [phoneNumber, setPhoneNumber] = useState<string>('');
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();

  useEffect(() => {
    // Pre-fill phone number for returning users
    const savedPhone = localStorage.getItem('userPhone');
    if (savedPhone) {
      setPhoneNumber(savedPhone);
    }
  }, []);

  const validatePhoneNumber = (phone: string): boolean => {
    const phoneRegex = /^[6-9]\d{9}$/;
    return phoneRegex.test(phone);
  };

  const handleSubmit = async (e: React.FormEvent): Promise<void> => {
    e.preventDefault();
    
    if (!validatePhoneNumber(phoneNumber)) {
      toast.error('Please enter a valid 10-digit mobile number');
      return;
    }

    setIsLoading(true);

    try {
      // For demo purposes, we'll simulate OTP sending
      // In production, this would call the actual API
      await new Promise(resolve => setTimeout(resolve, 1000)); // Simulate API call
      
      // Store phone in localStorage for OTP verification page
      localStorage.setItem('pendingPhone', phoneNumber);
      
      toast.success('OTP sent successfully!');
      navigate('/verify-otp', { state: { phone: phoneNumber } });
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Failed to send OTP';
      toast.error(errorMessage);
    } finally {
      setIsLoading(false);
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