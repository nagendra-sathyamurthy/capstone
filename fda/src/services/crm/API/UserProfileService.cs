using Crm.Models;
using Crm.DataAccess;
using MongoDB.Driver;

namespace Crm.API
{
    public class UserProfileService
    {
        private readonly UserProfileRepository _userProfileRepository;
        private readonly CustomerRepository _customerRepository;

        public UserProfileService(IMongoDatabase database)
        {
            _userProfileRepository = new UserProfileRepository(database);
            _customerRepository = new CustomerRepository(database);
        }

        /// <summary>
        /// Create a new user profile
        /// </summary>
        public UserProfile CreateUserProfile(UserProfile userProfile)
        {
            if (string.IsNullOrEmpty(userProfile.UserId))
            {
                throw new ArgumentException("UserId is required");
            }

            // Check if profile already exists for this user
            var existingProfile = _userProfileRepository.GetByUserId(userProfile.UserId);
            if (existingProfile != null)
            {
                throw new InvalidOperationException($"User profile already exists for UserId: {userProfile.UserId}");
            }

            userProfile.CreatedAt = DateTime.UtcNow;
            userProfile.UpdatedAt = DateTime.UtcNow;

            _userProfileRepository.Insert(userProfile);

            // If this is a customer, also create a Customer record for CRM purposes
            if (userProfile.Role == UserRole.Customer)
            {
                CreateCustomerRecord(userProfile);
            }

            return userProfile;
        }

        /// <summary>
        /// Get user profile by user ID (from Authentication service)
        /// </summary>
        public UserProfile? GetUserProfileByUserId(string userId)
        {
            return _userProfileRepository.GetByUserId(userId);
        }

        /// <summary>
        /// Get user profile by profile ID
        /// </summary>
        public UserProfile? GetUserProfileById(string id)
        {
            return _userProfileRepository.GetById(id);
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        public UserProfile? UpdateUserProfile(string id, UserProfile userProfile)
        {
            var existingProfile = _userProfileRepository.GetById(id);
            if (existingProfile == null)
            {
                return null;
            }

            // Preserve certain fields
            userProfile.Id = id;
            userProfile.UserId = existingProfile.UserId;
            userProfile.CreatedAt = existingProfile.CreatedAt;

            var updated = _userProfileRepository.UpdateUserProfile(id, userProfile);
            if (!updated)
            {
                return null;
            }

            // Update associated Customer record if role is Customer
            if (userProfile.Role == UserRole.Customer)
            {
                UpdateCustomerRecord(userProfile);
            }

            return userProfile;
        }

        /// <summary>
        /// Delete user profile
        /// </summary>
        public bool DeleteUserProfile(string id)
        {
            var profile = _userProfileRepository.GetById(id);
            if (profile == null)
            {
                return false;
            }

            // Delete associated Customer record if exists
            if (profile.Role == UserRole.Customer)
            {
                DeleteCustomerRecord(profile.UserId);
            }

            _userProfileRepository.Delete(id);
            return true;
        }

        /// <summary>
        /// Get all user profiles by role
        /// </summary>
        public List<UserProfile> GetUserProfilesByRole(UserRole role)
        {
            return _userProfileRepository.GetByRole(role);
        }

        /// <summary>
        /// Search user profiles by name
        /// </summary>
        public List<UserProfile> SearchUserProfiles(string searchTerm)
        {
            return _userProfileRepository.SearchByName(searchTerm);
        }

        /// <summary>
        /// Get user profile by email
        /// </summary>
        public UserProfile? GetUserProfileByEmail(string email)
        {
            return _userProfileRepository.GetByEmail(email);
        }

        /// <summary>
        /// Create a Customer record for CRM purposes when a Customer UserProfile is created
        /// </summary>
        private void CreateCustomerRecord(UserProfile userProfile)
        {
            if (userProfile.Role != UserRole.Customer || string.IsNullOrEmpty(userProfile.Id))
                return;

            var customer = new Customer
            {
                UserProfileId = userProfile.Id,
                UserId = userProfile.UserId,
                FirstName = userProfile.FirstName,
                LastName = userProfile.LastName,
                Email = userProfile.Email,
                PhoneNumber = userProfile.PhoneNumber,
                Status = CustomerStatus.Active,
                Tier = CustomerTier.Standard,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _customerRepository.Insert(customer);
        }

        /// <summary>
        /// Update associated Customer record when UserProfile is updated
        /// </summary>
        private void UpdateCustomerRecord(UserProfile userProfile)
        {
            if (userProfile.Role != UserRole.Customer)
                return;

            var customers = _customerRepository.GetAll();
            var customer = customers.FirstOrDefault(c => c.UserId == userProfile.UserId);
            
            if (customer != null)
            {
                customer.FirstName = userProfile.FirstName;
                customer.LastName = userProfile.LastName;
                customer.Email = userProfile.Email;
                customer.PhoneNumber = userProfile.PhoneNumber;
                customer.UpdatedAt = DateTime.UtcNow;

                _customerRepository.Update(customer.Id!, customer);
            }
        }

        /// <summary>
        /// Delete associated Customer record when UserProfile is deleted
        /// </summary>
        private void DeleteCustomerRecord(string userId)
        {
            var customers = _customerRepository.GetAll();
            var customer = customers.FirstOrDefault(c => c.UserId == userId);
            
            if (customer != null && !string.IsNullOrEmpty(customer.Id))
            {
                _customerRepository.Delete(customer.Id);
            }
        }

        /// <summary>
        /// Get user's delivery addresses
        /// </summary>
        public List<DeliveryAddress>? GetUserAddresses(string userId)
        {
            var profile = _userProfileRepository.GetByUserId(userId);
            return profile?.DeliveryAddresses;
        }

        /// <summary>
        /// Add delivery address - creates user profile if it doesn't exist
        /// </summary>
        public DeliveryAddress AddAddress(string userId, DeliveryAddress address)
        {
            var profile = _userProfileRepository.GetByUserId(userId);
            
            // If profile doesn't exist, create it
            if (profile == null)
            {
                profile = new UserProfile
                {
                    UserId = userId,
                    Role = UserRole.Customer,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    DeliveryAddresses = new List<DeliveryAddress>()
                };
                _userProfileRepository.Insert(profile);
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
            
            // If profile doesn't exist, create it
            if (profile == null)
            {
                profile = new UserProfile
                {
                    UserId = userId,
                    Role = UserRole.Customer,
                    ProfileImage = profileImage,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _userProfileRepository.Insert(profile);
                return;
            }

            profile.ProfileImage = profileImage;
            profile.UpdatedAt = DateTime.UtcNow;
            _userProfileRepository.UpdateUserProfile(profile.Id!, profile);
        }

        /// <summary>
        /// Update profile basic info (name, email)
        /// </summary>
        public void UpdateProfileBasicInfo(string userId, string? name, string? email)
        {
            var profile = _userProfileRepository.GetByUserId(userId);
            
            // If profile doesn't exist, create it
            if (profile == null)
            {
                profile = new UserProfile
                {
                    UserId = userId,
                    Role = UserRole.Customer,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                if (!string.IsNullOrEmpty(name))
                {
                    var nameParts = name.Split(' ', 2);
                    profile.FirstName = nameParts[0];
                    profile.LastName = nameParts.Length > 1 ? nameParts[1] : "";
                }
                
                if (!string.IsNullOrEmpty(email))
                {
                    profile.Email = email;
                }
                
                _userProfileRepository.Insert(profile);
                return;
            }

            // Update existing profile
            if (!string.IsNullOrEmpty(name))
            {
                var nameParts = name.Split(' ', 2);
                profile.FirstName = nameParts[0];
                profile.LastName = nameParts.Length > 1 ? nameParts[1] : "";
            }
            
            if (!string.IsNullOrEmpty(email))
            {
                profile.Email = email;
            }
            
            profile.UpdatedAt = DateTime.UtcNow;
            _userProfileRepository.UpdateUserProfile(profile.Id!, profile);
        }

        /// <summary>
        /// Update food preferences
        /// </summary>
        public FoodPreferences UpdateFoodPreferences(string userId, FoodPreferences preferences)
        {
            var profile = _userProfileRepository.GetByUserId(userId);
            
            // If profile doesn't exist, create it
            if (profile == null)
            {
                profile = new UserProfile
                {
                    UserId = userId,
                    Role = UserRole.Customer,
                    FoodPreferences = preferences,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _userProfileRepository.Insert(profile);
                return preferences;
            }

            preferences.UpdatedAt = DateTime.UtcNow;
            profile.FoodPreferences = preferences;
            profile.UpdatedAt = DateTime.UtcNow;
            _userProfileRepository.UpdateUserProfile(profile.Id!, profile);

            return preferences;
        }
    }
}