# Profile Setup & Address Selection Features

## Summary
Implemented comprehensive profile setup for new users and address-based restaurant filtering in the Dashboard.

## New Features

### 1. Profile Setup Flow for New Users
**Location:** `/profile-setup` route

**Features:**
- **2-Step Registration Process**
  - Step 1: Profile Information
    - Full name (required)
    - Profile photo upload (optional, max 5MB)
    - Food preferences (optional): All, Vegetarian, or Non-Veg
  - Step 2: Delivery Address
    - Address type: Home 🏠, Work 🏢, or Other 📍
    - Complete address fields (line1, line2, landmark, city, state, pincode)
    - All fields validated before submission

**User Flow:**
1. New user registers via OTP
2. After OTP verification → Redirected to `/profile-setup`
3. Completes profile and address information
4. Redirected to `/dashboard` with all data saved

**Returning User Detection:**
- If user already has saved addresses → Skip profile setup, go directly to dashboard
- Checked in `useEffect` on component mount

### 2. Address Selection in Dashboard
**Location:** Dashboard header (below main header)

**Features:**
- **Address Dropdown Selector**
  - Shows currently selected delivery address
  - Icon indicator based on address type (🏠 Home, 💼 Work, 📍 Other)
  - Displays: "Delivering to [address details]"
  
- **Address Dropdown Menu**
  - Lists all saved addresses
  - Visual indicator for currently selected address (purple highlight)
  - Click to switch between addresses
  - "Add New Address" option → Redirects to Profile page

- **Auto-Selection on Load**
  - First address is automatically selected when dashboard loads
  - If no addresses exist → Redirects user to profile setup

**Purpose:**
- Allows filtering restaurants and menu items based on delivery location
- Foundation for future "Near Me" feature using address coordinates
- Quick address switching without leaving dashboard

### 3. Food Preferences Storage
**Storage Key:** `foodPreferences_${userKey}`

**Structure:**
```javascript
{
  dietary: 'all' | 'veg' | 'non-veg',
  cuisines: [] // Reserved for future use
}
```

**Usage:**
- Saved during profile setup
- Can be used to filter menu items in dashboard
- Stored in localStorage with user-specific key

## Data Storage

### New localStorage Keys

| Key | Type | Description | Example |
|-----|------|-------------|---------|
| `foodPreferences_${userKey}` | JSON Object | User's food preferences | `{dietary: "veg", cuisines: []}` |
| `customerAddresses_${userKey}` | JSON Array | User's delivery addresses | `[{id: "1", type: "home", line1: "...", ...}]` |
| `userName` | String | User's full name | `"John Doe"` |
| `profileImage_${userKey}` | String | Base64 profile image | `"data:image/jpeg;base64,..."` |

### Address Object Structure
```javascript
{
  id: string,
  type: 'home' | 'work' | 'other',
  line1: string,
  line2: string,
  landmark: string,
  city: string,
  state: string,
  pincode: string (6 digits)
}
```

## Files Modified

### 1. ProfileSetup.js
**Changes:**
- Added `useEffect` to check for existing addresses and skip setup if found
- Added `foodPreferences` state with dietary preference options
- Added preference selector UI with three buttons (All, Vegetarian, Non-Veg)
- Updated save function to store food preferences in localStorage
- Fixed userName storage logic

### 2. Dashboard.js
**Changes:**
- Added address-related imports: `MapPin`, `ChevronDown`
- Added state for addresses, selected address, and dropdown visibility
- Added `loadAddresses()` function to load user addresses from localStorage
- Added logic to redirect to profile setup if no addresses found
- Auto-selects first address on load
- Added complete address selector UI with dropdown menu
- Address switching functionality with toast notifications

### 3. Dashboard.css
**New Styles Added:**
- `.address-selector` - Main container for address bar
- `.address-dropdown` - Clickable dropdown trigger
- `.address-text` - Address display text styling
- `.dropdown-icon` - Chevron rotation animation
- `.address-dropdown-menu` - Dropdown menu container with shadow
- `.address-option` - Individual address items in dropdown
- `.address-option.selected` - Purple highlight for selected address
- `.address-option.add-new` - Special styling for "Add New Address" option
- Responsive styles for mobile devices

### 4. ProfileSetup.css
**New Styles Added:**
- `.preference-buttons` - Container for food preference buttons
- `.preference-btn` - Individual preference button styling
- `.preference-btn:hover` - Hover effect with purple border and shadow
- `.preference-btn.active` - Purple gradient for selected preference
- Responsive styles for mobile (stacks buttons vertically)

## User Experience Flow

### New User Registration
```
1. Phone Entry → 2. OTP Verification → 3. Profile Setup (Step 1: Profile) 
   → 4. Profile Setup (Step 2: Address) → 5. Dashboard with address selected
```

### Returning User Login
```
1. Phone Entry → 2. OTP Verification → 3. Dashboard (address auto-selected)
```

### Address Management
```
Dashboard → Click address dropdown → Select different address OR 
   → Click "Add New Address" → Profile Page → Add new address → Return to Dashboard
```

## Visual Enhancements

### Profile Setup
- Clean 2-step progress indicator
- Visual step circles (1, 2) with connecting line
- Active/inactive state styling
- Food preference buttons with emoji icons
- Hover effects on all interactive elements

### Dashboard Address Selector
- Prominent placement below main header
- Light gray background (#f8f9fa) for visibility
- Purple accent color (#7c3aed) matching app theme
- Smooth dropdown animation
- Address type emoji indicators
- Selected address has purple left border
- Dropdown has subtle shadow for depth

## Future Enhancements (Ready for Implementation)

1. **Location-Based Filtering**
   - Use selected address to filter restaurants by distance
   - Show "Near Me" restaurants first
   - Implement delivery radius logic

2. **Preference-Based Filtering**
   - Filter menu items based on dietary preferences
   - Show vegetarian-only items if user preference is "veg"
   - Highlight recommended items based on past orders

3. **Address Geocoding**
   - Add latitude/longitude to address objects
   - Use Google Maps API for accurate location
   - Calculate actual delivery distance

4. **Smart Address Suggestions**
   - Suggest addresses based on time of day (Home in evening, Work in day)
   - Remember last used address per restaurant
   - Quick address switch based on user patterns

## Validation Rules

### Profile Setup - Step 1
- Name: Required, minimum 2 characters, max 50 characters
- Profile image: Optional, max 5MB
- Food preference: Optional, defaults to "All"

### Profile Setup - Step 2
- Address line 1: Required, cannot be empty
- City: Required, cannot be empty
- State: Required, cannot be empty
- Pincode: Required, exactly 6 digits
- Other fields: Optional

## Testing Scenarios

### Scenario 1: New User First Time
✅ User completes OTP → Sees profile setup
✅ User fills name and preferences → Proceeds to step 2
✅ User fills address → Successfully reaches dashboard
✅ Dashboard shows selected address
✅ User can switch addresses from dropdown

### Scenario 2: Returning User
✅ User completes OTP → Directly goes to dashboard
✅ Profile setup is skipped automatically
✅ First address is auto-selected
✅ User can switch between existing addresses

### Scenario 3: User Without Address
✅ User logs in without saved address
✅ Dashboard detects missing address
✅ User is redirected to profile setup with info toast
✅ After adding address, user returns to dashboard

### Scenario 4: Address Management
✅ User clicks address dropdown in dashboard
✅ Sees all saved addresses
✅ Can switch between addresses with single click
✅ Toast confirms address change
✅ Can add new address via "Add New Address" option

## Responsive Design

### Mobile (max-width: 768px)
- Address selector padding reduced
- Address dropdown menu fits screen width
- Food preference buttons stack vertically
- Touch-friendly button sizes maintained
- Dropdown animations smooth on mobile

### Desktop
- Address selector centered with max-width: 1200px
- Dropdown menu aligns with content width
- Hover effects on all interactive elements
- Larger touch targets for better UX

## Technical Notes

1. **User Identification**: Uses `userId` or `userPhone` as fallback for storage keys
2. **Data Persistence**: All data stored in localStorage (no cookies)
3. **Address Validation**: Pincode must be exactly 6 digits
4. **Image Upload**: Base64 encoding for profile images
5. **Navigation Guards**: Checks for user session and addresses before rendering

## Integration Points

- **AuthContext**: Provides user authentication state
- **CartContext**: Can use selected address for delivery
- **Profile Page**: Linked from "Add New Address" option
- **Checkout**: Will use selected address as default delivery location

## API Integration (Future)

Currently uses localStorage. Ready for API integration:
- `POST /api/customers/profile` - Save profile and preferences
- `POST /api/customers/addresses` - Save new address
- `GET /api/customers/addresses` - Fetch addresses
- `PUT /api/customers/addresses/:id` - Update address
- `GET /api/restaurants/nearby?lat=&lng=` - Location-based restaurant search
