using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Crm.Models;
using Crm.API;

namespace Crm.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UserProfileController : ControllerBase
    {
        private readonly UserProfileService _userProfileService;

        public UserProfileController(UserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        /// <summary>
        /// Create a new user profile
        /// </summary>
        [HttpPost]
        public ActionResult<UserProfile> CreateUserProfile([FromBody] CreateUserProfileRequest request)
        {
            try
            {
                var userProfile = new UserProfile
                {
                    UserId = request.UserId,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    Role = request.Role,
                    Organization = request.Organization,
                    DateOfBirth = request.DateOfBirth,
                    DietaryPreferences = request.DietaryPreferences,
                    FavoriteRestaurants = request.FavoriteRestaurants,
                    EmployeeInfo = request.EmployeeInfo,
                    BusinessInfo = request.BusinessInfo,
                    DeliveryInfo = request.DeliveryInfo,
                    TechInfo = request.TechInfo
                };

                var createdProfile = _userProfileService.CreateUserProfile(userProfile);
                return CreatedAtAction(nameof(GetUserProfileById), new { id = createdProfile.Id }, createdProfile);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the user profile.", details = ex.Message });
            }
        }

        /// <summary>
        /// Get user profile by profile ID
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<UserProfile> GetUserProfileById(string id)
        {
            try
            {
                var userProfile = _userProfileService.GetUserProfileById(id);
                if (userProfile == null)
                {
                    return NotFound(new { message = $"User profile with ID {id} not found." });
                }

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the user profile.", details = ex.Message });
            }
        }

        /// <summary>
        /// Get user profile by user ID (from Authentication service)
        /// </summary>
        [HttpGet("by-user/{userId}")]
        public ActionResult<UserProfile> GetUserProfileByUserId(string userId)
        {
            try
            {
                var userProfile = _userProfileService.GetUserProfileByUserId(userId);
                if (userProfile == null)
                {
                    return NotFound(new { message = $"User profile for user ID {userId} not found." });
                }

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the user profile.", details = ex.Message });
            }
        }

        /// <summary>
        /// Get user profile by email
        /// </summary>
        [HttpGet("by-email/{email}")]
        public ActionResult<UserProfile> GetUserProfileByEmail(string email)
        {
            try
            {
                var userProfile = _userProfileService.GetUserProfileByEmail(email);
                if (userProfile == null)
                {
                    return NotFound(new { message = $"User profile for email {email} not found." });
                }

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the user profile.", details = ex.Message });
            }
        }

        /// <summary>
        /// Get all user profiles by role
        /// </summary>
        [HttpGet("by-role/{role}")]
        public ActionResult<List<UserProfile>> GetUserProfilesByRole(UserRole role)
        {
            try
            {
                var userProfiles = _userProfileService.GetUserProfilesByRole(role);
                return Ok(userProfiles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving user profiles.", details = ex.Message });
            }
        }

        /// <summary>
        /// Search user profiles by name
        /// </summary>
        [HttpGet("search")]
        public ActionResult<List<UserProfile>> SearchUserProfiles([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new { message = "Search term is required." });
                }

                var userProfiles = _userProfileService.SearchUserProfiles(searchTerm);
                return Ok(userProfiles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching user profiles.", details = ex.Message });
            }
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        [HttpPut("{id}")]
        public ActionResult<UserProfile> UpdateUserProfile(string id, [FromBody] UpdateUserProfileRequest request)
        {
            try
            {
                var userProfile = new UserProfile
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    Role = request.Role,
                    Organization = request.Organization,
                    DateOfBirth = request.DateOfBirth,
                    DietaryPreferences = request.DietaryPreferences,
                    FavoriteRestaurants = request.FavoriteRestaurants,
                    EmployeeInfo = request.EmployeeInfo,
                    BusinessInfo = request.BusinessInfo,
                    DeliveryInfo = request.DeliveryInfo,
                    TechInfo = request.TechInfo
                };

                var updatedProfile = _userProfileService.UpdateUserProfile(id, userProfile);
                if (updatedProfile == null)
                {
                    return NotFound(new { message = $"User profile with ID {id} not found." });
                }

                return Ok(updatedProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the user profile.", details = ex.Message });
            }
        }

        /// <summary>
        /// Delete user profile
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult DeleteUserProfile(string id)
        {
            try
            {
                var deleted = _userProfileService.DeleteUserProfile(id);
                if (!deleted)
                {
                    return NotFound(new { message = $"User profile with ID {id} not found." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the user profile.", details = ex.Message });
            }
        }

        /// <summary>
        /// Get user's delivery addresses
        /// </summary>
        [HttpGet("by-user/{userId}/addresses")]
        public ActionResult<List<DeliveryAddress>> GetUserAddresses(string userId)
        {
            try
            {
                var addresses = _userProfileService.GetUserAddresses(userId);
                return Ok(addresses ?? new List<DeliveryAddress>());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving addresses.", details = ex.Message });
            }
        }

        /// <summary>
        /// Add delivery address
        /// </summary>
        [HttpPost("by-user/{userId}/addresses")]
        public ActionResult<DeliveryAddress> AddAddress(string userId, [FromBody] DeliveryAddress address)
        {
            try
            {
                var addedAddress = _userProfileService.AddAddress(userId, address);
                return Ok(addedAddress);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding address.", details = ex.Message });
            }
        }

        /// <summary>
        /// Update delivery address
        /// </summary>
        [HttpPut("by-user/{userId}/addresses/{addressId}")]
        public ActionResult<DeliveryAddress> UpdateAddress(string userId, string addressId, [FromBody] DeliveryAddress address)
        {
            try
            {
                var updatedAddress = _userProfileService.UpdateAddress(userId, addressId, address);
                if (updatedAddress == null)
                {
                    return NotFound(new { message = "Address not found." });
                }
                return Ok(updatedAddress);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating address.", details = ex.Message });
            }
        }

        /// <summary>
        /// Delete delivery address
        /// </summary>
        [HttpDelete("by-user/{userId}/addresses/{addressId}")]
        public IActionResult DeleteAddress(string userId, string addressId)
        {
            try
            {
                var deleted = _userProfileService.DeleteAddress(userId, addressId);
                if (!deleted)
                {
                    return NotFound(new { message = "Address not found." });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting address.", details = ex.Message });
            }
        }

        /// <summary>
        /// Update profile image
        /// </summary>
        [HttpPut("by-user/{userId}/profile-image")]
        public ActionResult UpdateProfileImage(string userId, [FromBody] ProfileImageRequest request)
        {
            try
            {
                _userProfileService.UpdateProfileImage(userId, request.ProfileImage);
                return Ok(new { message = "Profile image updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating profile image.", details = ex.Message });
            }
        }

        /// <summary>
        /// Update user profile basic info (name, email, phone, date of birth, dietary preferences)
        /// </summary>
        [HttpPut("by-user/{userId}")]
        public ActionResult UpdateProfile(string userId, [FromBody] UpdateProfileRequest request)
        {
            try
            {
                _userProfileService.UpdateProfileBasicInfo(
                    userId, 
                    request.Name, 
                    request.Email, 
                    request.Phone, 
                    request.DateOfBirth, 
                    request.DietaryPreferences
                );
                return Ok(new { message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating profile.", details = ex.Message });
            }
        }

        /// <summary>
        /// Update food preferences
        /// </summary>
        [HttpPut("by-user/{userId}/food-preferences")]
        public ActionResult<FoodPreferences> UpdateFoodPreferences(string userId, [FromBody] FoodPreferences preferences)
        {
            try
            {
                var updated = _userProfileService.UpdateFoodPreferences(userId, preferences);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating food preferences.", details = ex.Message });
            }
        }
    }

    // Request DTOs
    public class CreateUserProfileRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string? Organization { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Address? Address { get; set; }
        public List<string>? DietaryPreferences { get; set; }
        public List<string>? FavoriteRestaurants { get; set; }
        public EmployeeInfo? EmployeeInfo { get; set; }
        public BusinessInfo? BusinessInfo { get; set; }
        public DeliveryInfo? DeliveryInfo { get; set; }
        public TechInfo? TechInfo { get; set; }
    }

    public class UpdateUserProfileRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string? Organization { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public List<string>? DietaryPreferences { get; set; }
        public List<string>? FavoriteRestaurants { get; set; }
        public EmployeeInfo? EmployeeInfo { get; set; }
        public BusinessInfo? BusinessInfo { get; set; }
        public DeliveryInfo? DeliveryInfo { get; set; }
        public TechInfo? TechInfo { get; set; }
    }

    public class ProfileImageRequest
    {
        public string ProfileImage { get; set; } = string.Empty;
    }

    public class UpdateProfileRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public List<string>? DietaryPreferences { get; set; }
    }
}
