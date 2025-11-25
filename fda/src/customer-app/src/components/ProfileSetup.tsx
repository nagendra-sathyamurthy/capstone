import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { User, MapPin, Camera, ArrowRight } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import { customerService } from '../services/api';
import { ProfileSetupData, AddressFormData } from '../types';
import '../styles/ProfileSetup.css';

const ProfileSetup: React.FC = () => {
  const navigate = useNavigate();
  const { user } = useAuth();
  const [step, setStep] = useState<number>(1);
  const [profileData, setProfileData] = useState({
    name: '',
    profileImage: null as string | null,
    profileImagePreview: null as string | null,
    foodPreferences: {
      dietary: 'all' as 'all' | 'veg' | 'non-veg',
      cuisines: [] as string[]
    }
  });
  const [addressData, setAddressData] = useState<AddressFormData>({
    type: 'home',
    line1: '',
    line2: '',
    landmark: '',
    city: 'Bangalore',
    state: 'Karnataka',
    pincode: ''
  });
  const [isLoading, setIsLoading] = useState<boolean>(false);

  // Check if user already has profile data - skip setup for returning users
  useEffect(() => {
    const userId = localStorage.getItem('userId');
    const userPhone = localStorage.getItem('userPhone');

    if (!userId && !userPhone) {
      navigate('/');
      return;
    }

    // Check if user already has addresses in MongoDB
    checkExistingProfile();
  }, [navigate]);

  const checkExistingProfile = async (): Promise<void> => {
    try {
      console.log('[ProfileSetup] Checking for existing profile...');
      const addresses = await customerService.getAddresses();
      console.log('[ProfileSetup] Addresses response:', addresses);
      
      if (addresses && addresses.length > 0) {
        // User already has addresses, redirect to dashboard
        console.log('[ProfileSetup] Existing user found, redirecting to dashboard');
        navigate('/dashboard');
      } else {
        console.log('[ProfileSetup] No addresses found, proceeding with setup');
      }
    } catch (error) {
      // If error, assume new user and continue with setup
      const errorMessage = error instanceof Error ? error.message : 'Unknown error';
      console.log('[ProfileSetup] Error checking profile:', errorMessage);
      console.log('[ProfileSetup] Proceeding with setup as new user');
    }
  };

  const handleProfileImageChange = (e: React.ChangeEvent<HTMLInputElement>): void => {
    const file = e.target.files?.[0];
    if (file) {
      if (file.size > 5 * 1024 * 1024) { // 5MB limit
        toast.error('Image size should be less than 5MB');
        return;
      }

      const reader = new FileReader();
      reader.onloadend = () => {
        setProfileData(prev => ({
          ...prev,
          profileImage: reader.result as string,
          profileImagePreview: reader.result as string
        }));
      };
      reader.readAsDataURL(file);
    }
  };

  const handleRemoveImage = (): void => {
    setProfileData(prev => ({
      ...prev,
      profileImage: null,
      profileImagePreview: null
    }));
  };

  const handleStep1Submit = (e: React.FormEvent): void => {
    e.preventDefault();
    
    if (!profileData.name.trim()) {
      toast.error('Please enter your name');
      return;
    }

    if (profileData.name.trim().length < 2) {
      toast.error('Name must be at least 2 characters');
      return;
    }

    setStep(2);
  };

  const handleStep2Submit = async (e: React.FormEvent): Promise<void> => {
    e.preventDefault();
    
    if (!addressData.line1.trim()) {
      toast.error('Please enter address line 1');
      return;
    }

    if (!addressData.city.trim()) {
      toast.error('Please enter city');
      return;
    }

    if (!addressData.state.trim()) {
      toast.error('Please enter state');
      return;
    }

    if (addressData.pincode.length !== 6) {
      toast.error('Please enter valid 6-digit pincode');
      return;
    }

    setIsLoading(true);

    try {
      console.log('[ProfileSetup] Saving profile data...');
      
      // Save profile name to localStorage (update existing user data)
      const userName = localStorage.getItem('userName');
      if (!userName || userName === '') {
        localStorage.setItem('userName', profileData.name);
      }

      // Update user profile name in MongoDB
      try {
        console.log('[ProfileSetup] Updating profile with name:', profileData.name);
        await customerService.updateProfile({
          name: profileData.name
        });
        console.log('[ProfileSetup] Profile name updated successfully');
      } catch (error) {
        console.error('[ProfileSetup] Failed to update profile name:', error);
      }

      // Save address to MongoDB via API
      const newAddress = {
        type: addressData.type,
        line1: addressData.line1,
        line2: addressData.line2 || '',
        landmark: addressData.landmark || '',
        city: addressData.city,
        state: addressData.state,
        pincode: addressData.pincode,
        country: 'India'
      };
      
      console.log('[ProfileSetup] Saving address:', newAddress);
      await customerService.addAddress(newAddress);
      console.log('[ProfileSetup] Address saved successfully');

      // Save profile image to MongoDB if provided
      if (profileData.profileImagePreview) {
        console.log('[ProfileSetup] Saving profile image...');
        await customerService.updateProfileImage(profileData.profileImagePreview);
        console.log('[ProfileSetup] Profile image saved successfully');
      }

      // Save food preferences to MongoDB
      if (profileData.foodPreferences) {
        console.log('[ProfileSetup] Saving food preferences:', profileData.foodPreferences);
        await customerService.updateFoodPreferences(profileData.foodPreferences);
        console.log('[ProfileSetup] Food preferences saved successfully');
      }

      toast.success('Profile setup completed!');
      
      // Wait a bit before navigating
      setTimeout(() => {
        navigate('/dashboard');
      }, 1000);
    } catch (error) {
      console.error('Error saving profile:', error);
      toast.error('Failed to save profile. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  const handleSkipImage = (): void => {
    setStep(2);
  };

  return (
    <div className="profile-setup-container">
      <div className="profile-setup-card">
        <div className="setup-progress">
          <div className={`progress-step ${step >= 1 ? 'active' : ''}`}>
            <div className="step-circle">1</div>
            <span>Profile</span>
          </div>
          <div className={`progress-line ${step >= 2 ? 'active' : ''}`}></div>
          <div className={`progress-step ${step >= 2 ? 'active' : ''}`}>
            <div className="step-circle">2</div>
            <span>Address</span>
          </div>
        </div>

        {step === 1 && (
          <form onSubmit={handleStep1Submit} className="setup-form">
            <div className="setup-header">
              <h2>Let's set up your profile</h2>
              <p>Tell us a bit about yourself</p>
            </div>

            <div className="profile-image-section">
              <div className="image-upload-container">
                {profileData.profileImagePreview ? (
                  <div className="image-preview">
                    <img src={profileData.profileImagePreview} alt="Profile" />
                    <button
                      type="button"
                      className="remove-image-btn"
                      onClick={handleRemoveImage}
                    >
                      ×
                    </button>
                  </div>
                ) : (
                  <label className="image-upload-label">
                    <Camera size={40} />
                    <span>Upload Photo</span>
                    <span className="optional-text">(Optional)</span>
                    <input
                      type="file"
                      accept="image/*"
                      onChange={handleProfileImageChange}
                      style={{ display: 'none' }}
                    />
                  </label>
                )}
              </div>
            </div>

            <div className="form-group">
              <label>
                <User size={18} />
                Full Name *
              </label>
              <input
                type="text"
                value={profileData.name}
                onChange={(e) => setProfileData(prev => ({ ...prev, name: e.target.value }))}
                placeholder="Enter your full name"
                required
                maxLength={50}
              />
            </div>

            <div className="form-group">
              <label>
                🍽️ Food Preference (Optional)
              </label>
              <div className="preference-buttons">
                <button
                  type="button"
                  className={`preference-btn ${profileData.foodPreferences.dietary === 'all' ? 'active' : ''}`}
                  onClick={() => setProfileData(prev => ({
                    ...prev,
                    foodPreferences: { ...prev.foodPreferences, dietary: 'all' }
                  }))}
                >
                  All
                </button>
                <button
                  type="button"
                  className={`preference-btn ${profileData.foodPreferences.dietary === 'veg' ? 'active' : ''}`}
                  onClick={() => setProfileData(prev => ({
                    ...prev,
                    foodPreferences: { ...prev.foodPreferences, dietary: 'veg' }
                  }))}
                >
                  🥬 Vegetarian
                </button>
                <button
                  type="button"
                  className={`preference-btn ${profileData.foodPreferences.dietary === 'non-veg' ? 'active' : ''}`}
                  onClick={() => setProfileData(prev => ({
                    ...prev,
                    foodPreferences: { ...prev.foodPreferences, dietary: 'non-veg' }
                  }))}
                >
                  🍗 Non-Veg
                </button>
              </div>
            </div>

            <button type="submit" className="continue-btn">
              Continue <ArrowRight size={20} />
            </button>
          </form>
        )}

        {step === 2 && (
          <form onSubmit={handleStep2Submit} className="setup-form">
            <div className="setup-header">
              <h2>Add your delivery address</h2>
              <p>We'll deliver your orders here</p>
            </div>

            <div className="address-type-selector">
              <button
                type="button"
                className={`type-btn ${addressData.type === 'home' ? 'active' : ''}`}
                onClick={() => setAddressData(prev => ({ ...prev, type: 'home' }))}
              >
                🏠 Home
              </button>
              <button
                type="button"
                className={`type-btn ${addressData.type === 'work' ? 'active' : ''}`}
                onClick={() => setAddressData(prev => ({ ...prev, type: 'work' }))}
              >
                🏢 Work
              </button>
              <button
                type="button"
                className={`type-btn ${addressData.type === 'other' ? 'active' : ''}`}
                onClick={() => setAddressData(prev => ({ ...prev, type: 'other' }))}
              >
                📍 Other
              </button>
            </div>

            <div className="form-group">
              <label>
                <MapPin size={18} />
                Address Line 1 *
              </label>
              <input
                type="text"
                value={addressData.line1}
                onChange={(e) => setAddressData(prev => ({ ...prev, line1: e.target.value }))}
                placeholder="House/Flat/Block No."
                required
              />
            </div>

            <div className="form-group">
              <label>Address Line 2</label>
              <input
                type="text"
                value={addressData.line2}
                onChange={(e) => setAddressData(prev => ({ ...prev, line2: e.target.value }))}
                placeholder="Area, Street, Sector"
              />
            </div>

            <div className="form-group">
              <label>Landmark</label>
              <input
                type="text"
                value={addressData.landmark}
                onChange={(e) => setAddressData(prev => ({ ...prev, landmark: e.target.value }))}
                placeholder="Any nearby landmark"
              />
            </div>

            <div className="form-row">
              <div className="form-group">
                <label>City *</label>
                <input
                  type="text"
                  value={addressData.city}
                  onChange={(e) => setAddressData(prev => ({ ...prev, city: e.target.value }))}
                  required
                />
              </div>
              <div className="form-group">
                <label>State *</label>
                <input
                  type="text"
                  value={addressData.state}
                  onChange={(e) => setAddressData(prev => ({ ...prev, state: e.target.value }))}
                  required
                />
              </div>
            </div>

            <div className="form-group">
              <label>Pincode *</label>
              <input
                type="text"
                value={addressData.pincode}
                onChange={(e) => setAddressData(prev => ({ 
                  ...prev, 
                  pincode: e.target.value.replace(/\D/g, '').slice(0, 6) 
                }))}
                placeholder="560001"
                maxLength={6}
                required
              />
            </div>

            <div className="form-actions">
              <button
                type="button"
                className="back-btn"
                onClick={() => setStep(1)}
              >
                Back
              </button>
              <button
                type="submit"
                className="continue-btn"
                disabled={isLoading}
              >
                {isLoading ? 'Saving...' : 'Complete Setup'}
                {!isLoading && <ArrowRight size={20} />}
              </button>
            </div>
          </form>
        )}
      </div>
    </div>
  );
};

export default ProfileSetup;
