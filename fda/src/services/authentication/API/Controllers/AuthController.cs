using Authentication.Models;
using Authentication.API.BusinessServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Linq;
using System;

namespace Authentication.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var response = await _authService.Login(loginRequest);
                if (response == null)
                {
                    return Unauthorized(new { message = "Invalid credentials or account is inactive" });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Login failed", error = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            try
            {
                var user = await _authService.Register(registerRequest);
                
                var response = new 
                {
                    message = "Registration successful",
                    user = new
                    {
                        id = user.Id,
                        email = user.Email,
                        role = user.Role.ToString(),
                        organization = user.Organization,
                        permissions = user.Permissions
                    }
                };
                
                return Created($"/api/auth/profile/{user.Id}", response);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Registration failed", error = ex.Message });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            try
            {
                await _authService.ForgotPassword(email);
                return Ok(new { message = "If an account with this email exists, a password reset link has been sent" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to process password reset request", error = ex.Message });
            }
        }

        [HttpGet("validate")]
        [Authorize]
        public async Task<IActionResult> ValidateToken()
        {
            try
            {
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { message = "No token provided" });
                }

                var validation = await _authService.ValidateToken(token);
                if (!validation.IsValid)
                {
                    return Unauthorized(new { message = validation.Error });
                }

                return Ok(new { 
                    valid = true, 
                    user = validation.User,
                    message = "Token is valid"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Token validation failed", error = ex.Message });
            }
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = User.FindFirstValue("userId");
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                var user = await _authService.GetUserProfile(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var profile = new
                {
                    id = user.Id,
                    email = user.Email,
                    role = user.Role.ToString(),
                    organization = user.Organization,
                    permissions = user.Permissions,
                    isActive = user.IsActive,
                    isEmailVerified = user.IsEmailVerified,
                    lastLoginTime = user.LastLoginTime,
                    createdAt = user.CreatedAt
                };

                return Ok(profile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to retrieve profile", error = ex.Message });
            }
        }

        [HttpPut("password")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest updateRequest)
        {
            try
            {
                var userId = User.FindFirstValue("userId");
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                var success = await _authService.UpdatePassword(userId, updateRequest.CurrentPassword, updateRequest.NewPassword);
                if (!success)
                {
                    return BadRequest(new { message = "Current password is incorrect" });
                }

                return Ok(new { 
                    message = "Password updated successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to update password", error = ex.Message });
            }
        }

        [HttpGet("roles")]
        [Authorize]
        public IActionResult GetRoles()
        {
            var roles = Enum.GetValues(typeof(UserRole))
                .Cast<UserRole>()
                .Select(role => new
                {
                    value = role.ToString(),
                    name = role.ToString(),
                    permissions = Permissions.GetPermissionsForRole(role),
                    organization = Organizations.GetOrganizationForRole(role)
                })
                .ToList();

            return Ok(roles);
        }

        [HttpGet("permissions/{role}")]
        [Authorize]
        public IActionResult GetPermissionsForRole(UserRole role)
        {
            var permissions = Permissions.GetPermissionsForRole(role);
            return Ok(new { role = role.ToString(), permissions });
        }

        // Utility endpoint for checking specific permissions (useful for frontend)
        [HttpPost("check-permission")]
        [Authorize]
        public async Task<IActionResult> CheckPermission([FromBody] string permission)
        {
            try
            {
                var userId = User.FindFirstValue("userId");
                var user = await _authService.GetUserProfile(userId!);
                
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var hasPermission = _authService.HasPermission(user, permission);
                return Ok(new { 
                    hasPermission, 
                    permission,
                    userRole = user.Role.ToString()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Permission check failed", error = ex.Message });
            }
        }

        /// <summary>
        /// Generate OTP for login, password reset, or verification
        /// </summary>
        [HttpPost("generate-otp")]
        public async Task<IActionResult> GenerateOtp([FromBody] GenerateOtpRequest request)
        {
            try
            {
                var response = await _authService.GenerateOtp(request);
                if (!response.Success)
                {
                    return BadRequest(new { message = response.Message });
                }
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "OTP generation failed", error = ex.Message });
            }
        }

        /// <summary>
        /// Verify OTP for various purposes
        /// </summary>
        [HttpPost("verify-otp")]
        [Authorize]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            try
            {
                var response = await _authService.VerifyOtpRequest(request);
                if (!response.Success)
                {
                    return BadRequest(new { message = response.Message, error = response.Error });
                }
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "OTP verification failed", error = ex.Message });
            }
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        [HttpPost("login/email")]
        public async Task<IActionResult> LoginWithEmail([FromBody] EmailPasswordLoginRequest request)
        {
            try
            {
                var loginRequest = new LoginRequest
                {
                    Email = request.Email,
                    Password = request.Password,
                    LoginMethod = LoginMethod.EmailPassword
                };

                var response = await _authService.Login(loginRequest);
                if (response == null)
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Email login failed", error = ex.Message });
            }
        }



        /// <summary>
        /// Login with email and OTP
        /// </summary>
        [HttpPost("login/email-otp")]
        public async Task<IActionResult> LoginWithEmailOtp([FromBody] EmailOtpLoginRequest request)
        {
            try
            {
                var loginRequest = new LoginRequest
                {
                    Email = request.Email,
                    Otp = request.Otp,
                    LoginMethod = LoginMethod.EmailOtp
                };

                var response = await _authService.Login(loginRequest);
                if (response == null)
                {
                    return Unauthorized(new { message = "Invalid email or OTP" });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Email OTP login failed", error = ex.Message });
            }
        }


    }

    // Simplified login and update request classes
    public class EmailPasswordLoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class EmailOtpLoginRequest
    {
        public required string Email { get; set; }
        public required string Otp { get; set; }
    }

    public class UpdatePasswordRequest
    {
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}