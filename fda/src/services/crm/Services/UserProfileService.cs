using Crm.Models;
using Crm.DataAccess;
using MongoDB.Driver;

namespace Crm.Services
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
    }
}