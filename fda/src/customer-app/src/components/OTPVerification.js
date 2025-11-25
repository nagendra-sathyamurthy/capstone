import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { ArrowLeft, ArrowRight } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import { authService, customerService } from '../services/api';
import '../styles/OTPVerification.css';

const OTPVerification = () => {
  const [otp, setOtp] = useState(['', '', '', '']);
  const [isLoading, setIsLoading] = useState(false);
  const [resendTimer, setResendTimer] = useState(30);
  const [canResend, setCanResend] = useState(false);
  const navigate = useNavigate();
  const { phone, setLoading, login, isAuthenticated } = useAuth();

  useEffect(() => {
    // If already authenticated, go to profile setup
    if (isAuthenticated) {
      navigate('/profile-setup', { replace: true });
      return;
    }
    
    // If not authenticated and no phone, go to registration
    if (!phone) {
      navigate('/', { replace: true });
      return;
    }

    const timer = setInterval(() => {
      setResendTimer((prev) => {
        if (prev <= 1) {
          setCanResend(true);
          return 0;
        }
        return prev - 1;
      });
    }, 1000);

    return () => clearInterval(timer);
  }, [phone, navigate, isAuthenticated]);

  const handleOtpChange = (index, value) => {
    if (value.length <= 1 && /^\d*$/.test(value)) {
      const newOtp = [...otp];
      newOtp[index] = value;
      setOtp(newOtp);

      // Auto-focus next input
      if (value && index < 3) {
        const nextInput = document.getElementById(`otp-${index + 1}`);
        if (nextInput) nextInput.focus();
      }
    }
  };

  const handleKeyDown = (index, e) => {
    if (e.key === 'Backspace' && !otp[index] && index > 0) {
      const prevInput = document.getElementById(`otp-${index - 1}`);
      if (prevInput) prevInput.focus();
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    const otpString = otp.join('');
    if (otpString.length !== 4) {
      toast.error('Please enter complete OTP');
      return;
    }

    setIsLoading(true);
    setLoading(true);

    try {
      // For demo purposes, we'll simulate OTP verification
      // In production, this would call the actual API
      await new Promise(resolve => setTimeout(resolve, 1500)); // Simulate API call
      
      // Generate userId from timestamp
      const userId = Date.now().toString();
      
      // Call the authentication service to get a real JWT token
      const authResponse = await authService.customerPhoneLogin(phone, userId);
      
      // Use the real JWT token and user data from authentication service
      const userData = {
        id: authResponse.user.id,
        phone: authResponse.user.phone,
        name: authResponse.user.name,
        token: authResponse.token
      };
      
      // Login - this saves to localStorage synchronously
      login(userData);
      
      toast.success('OTP verified successfully!');
      
      // Check if user already has profile data
      try {
        console.log('[OTPVerification] Checking if user has existing profile...');
        const addresses = await customerService.getAddresses();
        
        if (addresses && addresses.length > 0) {
          // Existing user - go to dashboard
          console.log('[OTPVerification] Existing user found, redirecting to dashboard');
          setTimeout(() => {
            navigate('/dashboard', { replace: true });
          }, 500);
        } else {
          // New user - go to profile setup
          console.log('[OTPVerification] New user, redirecting to profile setup');
          setTimeout(() => {
            navigate('/profile-setup', { replace: true });
          }, 500);
        }
      } catch (error) {
        // If error checking addresses, assume new user
        console.log('[OTPVerification] Error checking profile, assuming new user:', error.message);
        setTimeout(() => {
          navigate('/profile-setup', { replace: true });
        }, 500);
      }
    } catch (error) {
      toast.error(error.message || 'Invalid OTP');
      setIsLoading(false);
      setLoading(false);
    }
  };

  const handleResendOtp = async () => {
    try {
      setCanResend(false);
      setResendTimer(30);
      
      // Simulate resending OTP
      await new Promise(resolve => setTimeout(resolve, 1000));
      toast.success('OTP resent successfully!');
    } catch (error) {
      toast.error('Failed to resend OTP');
    }
  };

  return (
    <div className="otp-container">
      <div className="otp-card">
        <button
          className="back-button"
          onClick={() => navigate('/')}
          type="button"
        >
          <ArrowLeft size={20} />
        </button>
        
        <div className="otp-header">
          <h2>Verify OTP</h2>
          <p>We've sent a 4-digit code to</p>
          <p className="phone-display">+91 {phone}</p>
        </div>
        
        <form onSubmit={handleSubmit} className="otp-form">
          <div className="otp-inputs">
            {otp.map((digit, index) => (
              <input
                key={index}
                id={`otp-${index}`}
                type="text"
                value={digit}
                onChange={(e) => handleOtpChange(index, e.target.value)}
                onKeyDown={(e) => handleKeyDown(index, e)}
                className="otp-input"
                maxLength={1}
                inputMode="numeric"
              />
            ))}
          </div>
          
          <button
            type="submit"
            className={`verify-button ${isLoading ? 'loading' : ''}`}
            disabled={isLoading || otp.join('').length !== 4}
          >
            {isLoading ? (
              <span className="loading-spinner"></span>
            ) : (
              <>
                Verify & Continue <ArrowRight size={20} />
              </>
            )}
          </button>
        </form>
        
        <div className="resend-section">
          {canResend ? (
            <button
              className="resend-button"
              onClick={handleResendOtp}
              type="button"
            >
              Resend OTP
            </button>
          ) : (
            <p className="resend-timer">
              Resend OTP in {resendTimer}s
            </p>
          )}
        </div>
      </div>
    </div>
  );
};

export default OTPVerification;