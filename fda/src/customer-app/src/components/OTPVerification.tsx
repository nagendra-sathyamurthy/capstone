import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { toast } from 'react-toastify';
import { ArrowLeft, ArrowRight } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import { authService, customerService } from '../services/api';
import '../styles/OTPVerification.css';

interface LocationState {
  phone?: string;
}

const OTPVerification: React.FC = () => {
  const [otp, setOtp] = useState<string[]>(['', '', '', '']);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [resendTimer, setResendTimer] = useState<number>(30);
  const [canResend, setCanResend] = useState<boolean>(false);
  const navigate = useNavigate();
  const location = useLocation();
  const { login, isAuthenticated } = useAuth();
  
  // Get phone from location state (passed from Registration)
  const phone = (location.state as LocationState)?.phone || localStorage.getItem('pendingPhone') || '';

  useEffect(() => {
    // If already authenticated, check if user has profile
    if (isAuthenticated) {
      const checkProfile = async () => {
        try {
          console.log('[OTPVerification] User already authenticated, checking profile...');
          const addresses = await customerService.getAddresses();
          
          if (addresses && addresses.length > 0) {
            console.log('[OTPVerification] Existing user found, redirecting to dashboard');
            navigate('/dashboard', { replace: true });
          } else {
            console.log('[OTPVerification] No addresses found, redirecting to profile setup');
            navigate('/profile-setup', { replace: true });
          }
        } catch (error) {
          console.log('[OTPVerification] Error checking profile:', error);
          navigate('/profile-setup', { replace: true });
        }
      };
      checkProfile();
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

  const handleOtpChange = (index: number, value: string): void => {
    if (value.length <= 1 && /^\d*$/.test(value)) {
      const newOtp = [...otp];
      newOtp[index] = value;
      setOtp(newOtp);

      // Auto-focus next input
      if (value && index < 3) {
        const nextInput = document.getElementById(`otp-${index + 1}`);
        if (nextInput) (nextInput as HTMLInputElement).focus();
      }
    }
  };

  const handleKeyDown = (index: number, e: React.KeyboardEvent): void => {
    if (e.key === 'Backspace' && !otp[index] && index > 0) {
      const prevInput = document.getElementById(`otp-${index - 1}`);
      if (prevInput) (prevInput as HTMLInputElement).focus();
    }
  };

  const handleSubmit = async (e: React.FormEvent): Promise<void> => {
    e.preventDefault();
    
    const otpString = otp.join('');
    if (otpString.length !== 4) {
      toast.error('Please enter complete OTP');
      return;
    }

    setIsLoading(true);

    try {
      // For demo purposes, we'll simulate OTP verification
      // In production, this would call the actual API
      await new Promise(resolve => setTimeout(resolve, 1500)); // Simulate API call
      
      // Use phone number as userId for consistent identification across sessions
      // This ensures returning users can find their existing profile in MongoDB
      const userId = phone;
      
      // Call the authentication service to get a real JWT token
      const authResponse = await authService.customerPhoneLogin(phone, userId, 'Customer');
      
      // Use the real JWT token and user data from authentication service
      const userData = {
        id: authResponse.user.id,
        phone: authResponse.user.phone,
        name: authResponse.user.name,
        token: authResponse.token
      };
      
      // Login - this saves to localStorage synchronously
      login(userData, authResponse.token);
      
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
        const errorMessage = error instanceof Error ? error.message : 'Unknown error';
        console.log('[OTPVerification] Error checking profile, assuming new user:', errorMessage);
        setTimeout(() => {
          navigate('/profile-setup', { replace: true });
        }, 500);
      }
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Invalid OTP';
      toast.error(errorMessage);
      setIsLoading(false);
    }
  };

  const handleResendOtp = async (): Promise<void> => {
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