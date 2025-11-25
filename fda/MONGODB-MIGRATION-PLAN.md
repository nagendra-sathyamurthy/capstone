# MongoDB Migration Plan - Remove localStorage, Use MongoDB

## Overview
This document outlines the plan to migrate from localStorage to MongoDB for all user data storage (addresses, profile images, food preferences) while keeping only authentication tokens in localStorage.

## Current State

### Data Stored in localStorage:
1. **Authentication** (keep in localStorage):
   - `authToken` - JWT token
   - `userId` - User ID
   - `userPhone` - User phone number

2. **User Data** (migrate to MongoDB):
   - `customerAddresses_${userKey}` - Delivery addresses
   - `profileImage_${userKey}` - Profile image (Base64)
   - `foodPreferences_${userKey}` - Food preferences
   - `orderHistory_${userKey}` - Order history (already partially in MongoDB)

## Backend Changes Required

### 1. Update CRM Service Models

#### File: `src/services/crm/Models/UserProfile.cs`

**Status:** ✅ COMPLETED
- Added `DeliveryAddresses` field (List<DeliveryAddress>)
- Added `ProfileImage` field (string, Base64)
- Added `FoodPreferences` field (FoodPreferences object)
- Created `DeliveryAddress` class
- Created `FoodPreferences` class

### 2. Update CRM API Controller

#### File: `src/services/crm/API/Controllers/UserProfileController.cs`

**Status:** ✅ COMPLETED
Added new endpoints:
- `GET /api/userprofile/by-user/{userId}/addresses` - Get all addresses
- `POST /api/userprofile/by-user/{userId}/addresses` - Add address
- `PUT /api/userprofile/by-user/{userId}/addresses/{addressId}` - Update address
- `DELETE /api/userprofile/by-user/{userId}/addresses/{addressId}` - Delete address
- `PUT /api/userprofile/by-user/{userId}/profile-image` - Update profile image
- `PUT /api/userprofile/by-user/{userId}/food-preferences` - Update food preferences

### 3. Update CRM Service Implementation

#### File: `src/services/crm/API/UserProfileService.cs`

**Status:** ⏳ PENDING
Need to add implementations for:
```csharp
public List<DeliveryAddress>? GetUserAddresses(string userId)
public DeliveryAddress AddAddress(string userId, DeliveryAddress address)
public DeliveryAddress? UpdateAddress(string userId, string addressId, DeliveryAddress updatedAddress)
public bool DeleteAddress(string userId, string addressId)
public void UpdateProfileImage(string userId, string profileImage)
public FoodPreferences UpdateFoodPreferences(string userId, FoodPreferences preferences)
```

**Implementation Template:**
```csharp
// Add before the closing brace of UserProfileService class

/// <summary>
/// Get user's delivery addresses
/// </summary>
public List<DeliveryAddress>? GetUserAddresses(string userId)
{
    var profile = _userProfileRepository.GetByUserId(userId);
    return profile?.DeliveryAddresses;
}

/// <summary>
/// Add delivery address
/// </summary>
public DeliveryAddress AddAddress(string userId, DeliveryAddress address)
{
    var profile = _userProfileRepository.GetByUserId(userId);
    if (profile == null)
    {
        throw new InvalidOperationException("User profile not found");
    }

    if (profile.DeliveryAddresses == null)
    {
        profile.DeliveryAddresses = new List<DeliveryAddress>();
    }

    address.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
    address.CreatedAt = DateTime.UtcNow;
    address.UpdatedAt = DateTime.UtcNow;

    profile.DeliveryAddresses.Add(address);
    profile.UpdatedAt = DateTime.UtcNow;

    _userProfileRepository.UpdateUserProfile(profile.Id!, profile);
    return address;
}

/// <summary>
/// Update delivery address
/// </summary>
public DeliveryAddress? UpdateAddress(string userId, string addressId, DeliveryAddress updatedAddress)
{
    var profile = _userProfileRepository.GetByUserId(userId);
    if (profile == null || profile.DeliveryAddresses == null)
    {
        return null;
    }

    var address = profile.DeliveryAddresses.FirstOrDefault(a => a.Id == addressId);
    if (address == null)
    {
        return null;
    }

    address.Type = updatedAddress.Type;
    address.Line1 = updatedAddress.Line1;
    address.Line2 = updatedAddress.Line2;
    address.Landmark = updatedAddress.Landmark;
    address.City = updatedAddress.City;
    address.State = updatedAddress.State;
    address.Pincode = updatedAddress.Pincode;
    address.Country = updatedAddress.Country;
    address.Latitude = updatedAddress.Latitude;
    address.Longitude = updatedAddress.Longitude;
    address.UpdatedAt = DateTime.UtcNow;

    profile.UpdatedAt = DateTime.UtcNow;
    _userProfileRepository.UpdateUserProfile(profile.Id!, profile);

    return address;
}

/// <summary>
/// Delete delivery address
/// </summary>
public bool DeleteAddress(string userId, string addressId)
{
    var profile = _userProfileRepository.GetByUserId(userId);
    if (profile == null || profile.DeliveryAddresses == null)
    {
        return false;
    }

    var address = profile.DeliveryAddresses.FirstOrDefault(a => a.Id == addressId);
    if (address == null)
    {
        return false;
    }

    profile.DeliveryAddresses.Remove(address);
    profile.UpdatedAt = DateTime.UtcNow;
    _userProfileRepository.UpdateUserProfile(profile.Id!, profile);

    return true;
}

/// <summary>
/// Update profile image
/// </summary>
public void UpdateProfileImage(string userId, string profileImage)
{
    var profile = _userProfileRepository.GetByUserId(userId);
    if (profile == null)
    {
        throw new InvalidOperationException("User profile not found");
    }

    profile.ProfileImage = profileImage;
    profile.UpdatedAt = DateTime.UtcNow;
    _userProfileRepository.UpdateUserProfile(profile.Id!, profile);
}

/// <summary>
/// Update food preferences
/// </summary>
public FoodPreferences UpdateFoodPreferences(string userId, FoodPreferences preferences)
{
    var profile = _userProfileRepository.GetByUserId(userId);
    if (profile == null)
    {
        throw new InvalidOperationException("User profile not found");
    }

    preferences.UpdatedAt = DateTime.UtcNow;
    profile.FoodPreferences = preferences;
    profile.UpdatedAt = DateTime.UtcNow;
    _userProfileRepository.UpdateUserProfile(profile.Id!, profile);

    return preferences;
}
```

### 4. Update Gateway Routes

#### File: `src/gateway/routes/crm.js`

**Status:** ⏳ PENDING
Ensure routes are configured for UserProfile endpoints:
```javascript
// UserProfile routes
router.get('/api/userprofile/by-user/:userId', proxy(CRM_SERVICE_URL));
router.get('/api/userprofile/by-user/:userId/addresses', proxy(CRM_SERVICE_URL));
router.post('/api/userprofile/by-user/:userId/addresses', proxy(CRM_SERVICE_URL));
router.put('/api/userprofile/by-user/:userId/addresses/:addressId', proxy(CRM_SERVICE_URL));
router.delete('/api/userprofile/by-user/:userId/addresses/:addressId', proxy(CRM_SERVICE_URL));
router.put('/api/userprofile/by-user/:userId/profile-image', proxy(CRM_SERVICE_URL));
router.put('/api/userprofile/by-user/:userId/food-preferences', proxy(CRM_SERVICE_URL));
```

## Frontend Changes Required

### 1. Update API Service

#### File: `src/customer-app/src/services/api.js`

**Status:** ⏳ PENDING

Replace `customerService` object with:
```javascript
// Customer/Profile service - MongoDB storage via CRM API
export const customerService = {
  // Get customer profile (UserProfile from CRM)
  getProfile: async () => {
    try {
      const userId = localStorage.getItem('userId');
      if (!userId) {
        throw new Error('User not authenticated');
      }
      const response = await crmAPI.get(`/api/userprofile/by-user/${userId}`);
      return response.data;
    } catch (error) {
      if (error.response?.status === 404) {
        return null;
      }
      throw new Error(error.response?.data?.message || 'Failed to fetch profile');
    }
  },

  // Get customer addresses from MongoDB
  getAddresses: async () => {
    try {
      const userId = localStorage.getItem('userId');
      if (!userId) {
        throw new Error('User not authenticated');
      }
      const response = await crmAPI.get(`/api/userprofile/by-user/${userId}/addresses`);
      return response.data || [];
    } catch (error) {
      console.error('Failed to fetch addresses from API:', error);
      return [];
    }
  },

  // Add new address to MongoDB
  addAddress: async (address) => {
    try {
      const userId = localStorage.getItem('userId');
      if (!userId) {
        throw new Error('User not authenticated');
      }
      const response = await crmAPI.post(`/api/userprofile/by-user/${userId}/addresses`, address);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Failed to add address');
    }
  },

  // Update address in MongoDB
  updateAddress: async (addressId, address) => {
    try {
      const userId = localStorage.getItem('userId');
      if (!userId) {
        throw new Error('User not authenticated');
      }
      const response = await crmAPI.put(`/api/userprofile/by-user/${userId}/addresses/${addressId}`, address);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Failed to update address');
    }
  },

  // Delete address from MongoDB
  deleteAddress: async (addressId) => {
    try {
      const userId = localStorage.getItem('userId');
      if (!userId) {
        throw new Error('User not authenticated');
      }
      await crmAPI.delete(`/api/userprofile/by-user/${userId}/addresses/${addressId}`);
      return true;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Failed to delete address');
    }
  },

  // Update profile image in MongoDB
  updateProfileImage: async (profileImage) => {
    try {
      const userId = localStorage.getItem('userId');
      if (!userId) {
        throw new Error('User not authenticated');
      }
      await crmAPI.put(`/api/userprofile/by-user/${userId}/profile-image`, { profileImage });
      return true;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Failed to update profile image');
    }
  },

  // Update food preferences in MongoDB
  updateFoodPreferences: async (preferences) => {
    try {
      const userId = localStorage.getItem('userId');
      if (!userId) {
        throw new Error('User not authenticated');
      }
      const response = await crmAPI.put(`/api/userprofile/by-user/${userId}/food-preferences`, preferences);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Failed to update food preferences');
    }
  }
};
```

### 2. Update Components

#### Files to Update:
1. `src/customer-app/src/components/Profile.js`
2. `src/customer-app/src/components/Dashboard.js`
3. `src/customer-app/src/components/Checkout.js`
4. `src/customer-app/src/components/ProfileSetup.js`

#### Changes Required:

**Profile.js:**
- Remove all `localStorage.getItem/setItem` for addresses, profile images
- Replace with API calls:
  * Load addresses: `await customerService.getAddresses()`
  * Load profile image: From profile data `profile.profileImage`
  * Save address: `await customerService.addAddress(address)`
  * Update address: `await customerService.updateAddress(addressId, address)`
  * Delete address: `await customerService.deleteAddress(addressId)`
  * Update profile image: `await customerService.updateProfileImage(base64Image)`

**Dashboard.js:**
- Replace `localStorage.getItem(addressStorageKey)` with `await customerService.getAddresses()`
- Load addresses from API on component mount
- Update `loadAddresses()` function

**Checkout.js:**
- Replace `localStorage.getItem(addressStorageKey)` with `await customerService.getAddresses()`
- Replace `localStorage.setItem(addressStorageKey, ...)` with `await customerService.addAddress(address)`
- Load addresses from API on component mount

**ProfileSetup.js:**
- Replace `localStorage.setItem(addressStorageKey, ...)` with `await customerService.addAddress(address)`
- Replace `localStorage.setItem(preferencesStorageKey, ...)` with `await customerService.updateFoodPreferences(preferences)`
- Replace `localStorage.setItem(profileStorageKey, ...)` with `await customerService.updateProfileImage(image)`
- Check addresses via API instead of localStorage: `await customerService.getAddresses()`

## Migration Strategy

### Phase 1: Backend Setup (Do First)
1. ✅ Update UserProfile model with new fields
2. ⏳ Add methods to UserProfileService
3. ✅ Update UserProfileController with new endpoints
4. ⏳ Update Gateway routes
5. ⏳ Test endpoints with Postman
6. ⏳ Deploy CRM service

### Phase 2: Frontend Migration (Do Second)
1. ⏳ Update api.js customerService
2. ⏳ Update Profile.js
3. ⏳ Update Dashboard.js
4. ⏳ Update Checkout.js
5. ⏳ Update ProfileSetup.js
6. ⏳ Test complete user flow
7. ⏳ Remove old localStorage code

### Phase 3: Cleanup (Do Last)
1. ⏳ Remove all localStorage references for user data
2. ⏳ Keep only authToken, userId, userPhone in localStorage
3. ⏳ Add migration script for existing users (optional)
4. ⏳ Update documentation

## Testing Checklist

### Backend Tests:
- [ ] Create user profile via API
- [ ] Add delivery address via API
- [ ] Get all addresses via API
- [ ] Update address via API
- [ ] Delete address via API
- [ ] Update profile image via API
- [ ] Update food preferences via API

### Frontend Tests:
- [ ] New user registration → ProfileSetup saves to MongoDB
- [ ] Profile page loads addresses from MongoDB
- [ ] Dashboard loads addresses from MongoDB
- [ ] Add new address saves to MongoDB
- [ ] Edit address updates MongoDB
- [ ] Delete address removes from MongoDB
- [ ] Profile image upload saves to MongoDB
- [ ] Profile image persists across sessions
- [ ] Food preferences save to MongoDB
- [ ] All localStorage references removed except auth

### Integration Tests:
- [ ] Complete user flow: Register → Setup → Dashboard → Profile → Checkout
- [ ] Address selection works across all pages
- [ ] Profile image displays correctly everywhere
- [ ] Food preferences applied to menu filtering
- [ ] Multiple address management
- [ ] Session persistence (auth tokens only)

## Data Model Comparison

### localStorage (Old)
```javascript
{
  "customerAddresses_123": [
    {
      "id": "1",
      "type": "home",
      "line1": "123 Street",
      "city": "Bangalore",
      "pincode": "560001"
    }
  ],
  "profileImage_123": "data:image/jpeg;base64,...",
  "foodPreferences_123": {
    "dietary": "veg",
    "cuisines": []
  }
}
```

### MongoDB (New)
```json
{
  "_id": "ObjectId",
  "userId": "123",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "phoneNumber": "9876543210",
  "role": "Customer",
  "deliveryAddresses": [
    {
      "id": "addr1",
      "type": "home",
      "line1": "123 Street",
      "line2": "",
      "landmark": "Near Metro",
      "city": "Bangalore",
      "state": "Karnataka",
      "pincode": "560001",
      "country": "India",
      "latitude": null,
      "longitude": null,
      "createdAt": "2025-11-25T10:00:00Z",
      "updatedAt": "2025-11-25T10:00:00Z"
    }
  ],
  "profileImage": "data:image/jpeg;base64,...",
  "foodPreferences": {
    "dietary": "veg",
    "cuisines": [],
    "allergies": [],
    "dislikedIngredients": [],
    "updatedAt": "2025-11-25T10:00:00Z"
  },
  "createdAt": "2025-11-25T10:00:00Z",
  "updatedAt": "2025-11-25T10:00:00Z"
}
```

## Benefits of Migration

1. **Data Persistence**: User data survives browser cache clears
2. **Multi-Device**: Access same data from any device
3. **Centralized**: All data in one database
4. **Scalable**: No localStorage size limitations
5. **Secure**: Server-side validation and authorization
6. **Backup**: Automatic database backups
7. **Analytics**: Query user data for insights
8. **Sync**: Real-time data sync across sessions

## Rollback Plan

If migration fails:
1. Keep localStorage code commented out (don't delete)
2. Add feature flag to toggle between localStorage and MongoDB
3. Test thoroughly before removing old code
4. Have database backup before migration

## Notes

- Profile images stored as Base64 strings (max 10MB)
- Consider future migration to cloud storage (S3, Azure Blob) for images
- Keep auth tokens in localStorage for performance
- Add proper error handling for API failures
- Implement offline mode with service workers if needed
- Add loading states during API calls
- Consider caching addresses in memory during session

## Next Steps

1. Complete UserProfileService implementation
2. Test all endpoints with Postman
3. Update Gateway routes
4. Update frontend API service
5. Update all React components
6. Test end-to-end flow
7. Deploy to production
