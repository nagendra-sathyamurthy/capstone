# Cookie Removal - Migration to localStorage and API

## Summary
Successfully removed all cookie dependencies from the customer app. The application now uses localStorage for session management and MongoDB/API for all data persistence.

## Changes Made

### 1. Removed js-cookie Package
- Uninstalled `js-cookie` from package.json
- No more cookie dependencies in the application

### 2. Updated AuthContext.js
**Before:** Used cookies with 30-day expiry for session management
- `user_token`, `user_id`, `user_phone`, `user_name` (30 days)
- `returning_user` flag (365 days)

**After:** Uses localStorage only
- `authToken` - Authentication token
- `userId` - User ID
- `userPhone` - User phone number
- Session restoration on app start from localStorage
- Removed returning user flag

**Key Changes:**
```javascript
// Old: Cookie-based storage
Cookies.set('user_token', userData.token, { expires: 30 });
Cookies.set('user_id', userData.id, { expires: 30 });
Cookies.set('user_phone', userData.phone, { expires: 30 });

// New: localStorage-based storage
localStorage.setItem('authToken', authData.token);
localStorage.setItem('userId', authData.user.id);
localStorage.setItem('userPhone', authData.user.phone);
```

### 3. Updated CartContext.js
**Changes:**
- Replaced `Cookies.get('user_id')` → `localStorage.getItem('userId')`
- Replaced `Cookies.get('user_phone')` → `localStorage.getItem('userPhone')`
- Replaced `Cookies.get('user_token')` → `localStorage.getItem('authToken')`
- Order saving now uses API with Bearer token from localStorage

### 4. Updated api.js Service Layer
**Changes:**
- Removed `js-cookie` import
- Request interceptor now reads token from localStorage
- `getCustomerOrders()` uses localStorage for user identification
- Response interceptor clears localStorage on 401 (unauthorized)

**Before:**
```javascript
const user = JSON.parse(localStorage.getItem('user') || '{}');
if (user.token) {
  config.headers.Authorization = `Bearer ${user.token}`;
}
```

**After:**
```javascript
const token = localStorage.getItem('authToken');
if (token) {
  config.headers.Authorization = `Bearer ${token}`;
}
```

### 5. Updated Component Files

#### Registration.js
- Removed cookie import
- Changed `Cookies.get('user_phone')` → `localStorage.getItem('userPhone')`
- Pre-fills phone number from localStorage for returning users

#### Checkout.js
- Removed cookie import
- All address operations use localStorage with user-specific keys
- Pattern: `customerAddresses_${userKey}` where userKey comes from localStorage

#### Profile.js
- Removed cookie import
- Replaced all 13 instances of `Cookies.get()` with `localStorage.getItem()`
- User identification: `userId` and `userPhone` from localStorage
- Profile image storage: `profileImage_${userKey}` in localStorage
- Order history: Fetched from API, falls back to localStorage

#### ProfileSetup.js
- Removed cookie import
- Replaced all cookie operations with localStorage
- Changed `Cookies.set('user_name', ...)` → `localStorage.setItem('userName', ...)`
- Changed `Cookies.get('user_name')` → `localStorage.getItem('userName')`

## Data Storage Strategy

### Session Data (localStorage)
- `authToken` - JWT token for API authentication
- `userId` - Customer ID from backend
- `userPhone` - Customer phone number
- `userName` - Customer name (optional)

### User-Specific Data (localStorage with keys)
- `orderHistory_${userKey}` - Order history (fallback when API unavailable)
- `customerAddresses_${userKey}` - Customer delivery addresses
- `profileImage_${userKey}` - Base64 encoded profile image

### Persistent Data (MongoDB via API)
- Orders - Stored in Order Service database
- Customer profile - Stored in CRM Service
- Menu items - Stored in Catalog Service
- Authentication records - Stored in Auth Service

## Benefits

1. **Simplified Architecture**
   - No cookie management complexity
   - Consistent localStorage API throughout the app
   - Easier to debug and test

2. **Better Security**
   - Tokens not sent automatically with every request
   - Explicit token inclusion via axios interceptor
   - No cookie-based CSRF vulnerabilities

3. **API-First Approach**
   - All data operations go through API
   - MongoDB as source of truth
   - localStorage as offline fallback only

4. **User Isolation**
   - User-specific storage keys prevent data leakage
   - Each user's data clearly separated
   - Pattern: `dataType_${userId}` or `dataType_${userPhone}`

## Migration Path for Existing Users

The app will automatically migrate users:
1. On app start, checks for existing cookies (none will exist for new sessions)
2. Checks localStorage for existing session data
3. If token exists in localStorage, restores session
4. If no valid session, redirects to login

**Note:** Users with active cookie sessions will need to log in again, as cookies are no longer read by the application.

## Testing Checklist

- [x] Registration flow works without cookies
- [x] OTP verification saves to localStorage
- [x] Profile setup creates user-specific storage keys
- [x] Dashboard loads with localStorage authentication
- [x] Cart operations use localStorage for user ID
- [x] Checkout saves addresses with user-specific keys
- [x] Order placement sends data to API
- [x] Order history fetches from API with fallback to localStorage
- [x] Profile image persists in localStorage
- [x] Logout clears all localStorage items
- [x] App compiles without errors
- [x] No cookie-related code remains

## localStorage Keys Reference

| Key | Type | Description | Example Value |
|-----|------|-------------|---------------|
| `authToken` | String | JWT authentication token | `eyJhbGciOiJIUzI1...` |
| `userId` | String | Customer ID from backend | `12345` |
| `userPhone` | String | Customer phone number | `9876543210` |
| `userName` | String | Customer name (optional) | `John Doe` |
| `orderHistory_${userKey}` | JSON Array | Order history fallback | `[{id: "ORD123", ...}]` |
| `customerAddresses_${userKey}` | JSON Array | Customer addresses | `[{id: "1", type: "home", ...}]` |
| `profileImage_${userKey}` | String | Base64 encoded image | `data:image/jpeg;base64,...` |

## Files Modified

1. `src/context/AuthContext.js` - Removed all cookie operations
2. `src/context/CartContext.js` - Replaced cookie calls with localStorage
3. `src/services/api.js` - Updated token retrieval and user identification
4. `src/components/Registration.js` - Removed cookie usage
5. `src/components/Checkout.js` - Replaced all cookie operations
6. `src/components/Profile.js` - Removed 13 cookie calls
7. `src/components/ProfileSetup.js` - Updated cookie operations to localStorage
8. `package.json` - Removed js-cookie dependency

## API Endpoints Used

### Authentication
- `POST /auth/send-otp` - Send OTP to phone
- `POST /auth/verify-otp` - Verify OTP and create session
- `POST /auth/register` - Register new customer

### Orders
- `POST /api/orders` - Create new order
- `GET /api/orders/customer/:customerId` - Get customer orders

### Customer (CRM)
- `GET /customers/:customerId` - Get customer profile
- `PUT /customers/:customerId` - Update customer profile
- `GET /customers/:customerId/addresses` - Get addresses
- `POST /customers/:customerId/addresses` - Add address

### Catalog
- `GET /restaurants` - Get all restaurants
- `GET /menu-items/restaurant/:restaurantId` - Get menu items

All API calls include `Authorization: Bearer ${token}` header via axios interceptor.

## Notes

- Token expiry handling should be implemented in production
- Consider implementing refresh token mechanism
- localStorage has size limits (~5-10MB per domain)
- Consider moving addresses to API in future for multi-device sync
- Profile images should use proper image hosting service in production
