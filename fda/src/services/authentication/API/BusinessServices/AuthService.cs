using Authentication.Models;
using Authentication.DataAccess;
using Authentication.API.Commands;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

namespace Authentication.API.BusinessServices
{
    public class AuthService
    {
        private readonly IRepository<UserAccount> _repository;
        private readonly IRepository<OtpCode> _otpRepository;
        private readonly string _jwtSecret;

        public AuthService(IRepository<UserAccount> repository, IRepository<OtpCode> otpRepository)
        {
            _repository = repository;
            _otpRepository = otpRepository;
            _jwtSecret = "GJ0VFqmRVBR0iE2ojyzh28HlayZgRcUI"; // In production, use configuration
        }

        public async Task<LoginResponse?> Login(LoginRequest request)
        {
            // Use LoginCommand for authentication logic
            var command = new LoginCommand(request, _repository, _otpRepository, _jwtSecret);
            return await command.ExecuteAsync();
        }

        public async Task<UserAccount> Register(RegisterRequest request)
        {
            // Use RegisterUserCommand for registration logic
            var command = new RegisterUserCommand(request, _repository);
            return await command.ExecuteAsync();
        }

        // Role-specific validation removed - UserProfile data is now handled by CRM service

        public async Task<ValidateTokenResponse> ValidateToken(string token)
        {
            // Use ValidateTokenCommand for token validation logic
            var command = new ValidateTokenCommand(token, _repository, _jwtSecret);
            return await command.ExecuteAsync();
        }

        public async Task<UserAccount?> GetUserProfile(string userId)
        {
            return await _repository.GetByIdAsync(userId);
        }

        public async Task<bool> UpdatePassword(string userId, string currentPassword, string newPassword)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user == null) return false;

            if (!VerifyPassword(currentPassword, user.Password))
            {
                return false;
            }

            user.Password = HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(user);
            
            return true;
        }

        public async Task ForgotPassword(string email)
        {
            var user = await GetUserByEmail(email);
            if (user != null)
            {
                // TODO: Implement password reset token generation and email sending
                // For now, just log the request
                Console.WriteLine($"Password reset requested for: {email}");
            }
        }

        public async Task<bool> CheckPermissionAsync(string email, string permission)
        {
            var user = await GetUserByEmail(email);
            if (user == null) return false;

            var userPermissions = Permissions.GetPermissionsForRole(user.Role);
            return userPermissions.Contains(permission);
        }

        public bool HasPermission(UserAccount user, string permission)
        {
            return user.Permissions.Contains(permission);
        }

        public bool HasAnyPermission(UserAccount user, params string[] permissions)
        {
            return permissions.Any(p => user.Permissions.Contains(p));
        }

        // Simplified role-based validation methods - detailed role info now handled by CRM service
        public bool CanReceivePayments(UserAccount user)
        {
            return user.Role == UserRole.Biller;
        }

        public bool CanConfirmOrders(UserAccount user)
        {
            return user.Role == UserRole.Operator;
        }

        public bool CanPrepareFood(UserAccount user)
        {
            return user.Role == UserRole.Worker;
        }

        public bool CanConfirmDelivery(UserAccount user)
        {
            return user.Role == UserRole.DeliveryAgent;
        }

        public bool CanAccessAllEndpoints(UserAccount user)
        {
            return user.Role == UserRole.Developer || user.Role == UserRole.Tester;
        }

        public bool CanAccessHealthcheck(UserAccount user)
        {
            return user.Role == UserRole.NetworkAdmin;
        }

        public bool CanAccessDatabase(UserAccount user)
        {
            return user.Role == UserRole.DatabaseAdmin;
        }

        public bool IsRestaurantStaff(UserAccount user)
        {
            return user.Role == UserRole.Biller || 
                   user.Role == UserRole.Operator || 
                   user.Role == UserRole.Worker;
        }

        public bool IsITStaff(UserAccount user)
        {
            return user.Role == UserRole.Developer || 
                   user.Role == UserRole.Tester || 
                   user.Role == UserRole.NetworkAdmin || 
                   user.Role == UserRole.DatabaseAdmin;
        }

        public bool IsManagementLevel(UserAccount user)
        {
            return user.Role == UserRole.Biller || user.Role == UserRole.Operator;
        }

        private async Task<UserAccount?> GetUserByEmail(string email)
        {
            // Since we're using MongoDB, we need to query by email field, not ID
            var filter = Builders<UserAccount>.Filter.Eq(u => u.Email, email);
            var users = await _repository.FindAsync(filter);
            return users.FirstOrDefault();
        }

        // Removed GetUserByPhone - phone numbers are now handled by CRM service

        private bool ValidateLoginRequest(LoginRequest request)
        {
            switch (request.LoginMethod)
            {
                case LoginMethod.EmailPassword:
                    return !string.IsNullOrEmpty(request.Email) && !string.IsNullOrEmpty(request.Password);
                case LoginMethod.EmailOtp:
                    return !string.IsNullOrEmpty(request.Email) && !string.IsNullOrEmpty(request.Otp);
                case LoginMethod.PhonePassword:
                case LoginMethod.PhoneOtp:
                    // Phone-based authentication now handled by CRM service
                    return false;
                default:
                    return false;
            }
        }

        private async Task HandleFailedLogin(UserAccount? user)
        {
            if (user != null)
            {
                user.InvalidLogins++;
                await _repository.UpdateAsync(user);
            }
        }

        private string GenerateJwtToken(UserAccount user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);
            
            var claims = new List<Claim>
            {
                new Claim("userId", user.Id!),
                new Claim("email", user.Email),
                new Claim("role", user.Role.ToString()),
                new Claim("organization", user.Organization),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id!),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            // Add permissions as claims
            foreach (var permission in user.Permissions)
            {
                claims.Add(new Claim("permission", permission));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateCustomerToken(string phone, string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);
            
            var claims = new List<Claim>
            {
                new Claim("userId", userId),
                new Claim("phone", phone),
                new Claim("role", "Customer"),
                new Claim(ClaimTypes.Name, phone),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, "Customer")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private UserInfo MapToUserInfo(UserAccount user)
        {
            return new UserInfo
            {
                Id = user.Id!,
                Email = user.Email,
                Role = user.Role,
                Organization = user.Organization,
                Permissions = user.Permissions,
                LastLoginTime = user.LastLoginTime
            };
        }



        private string HashPassword(string password)
        {
            // In production, use BCrypt or similar
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(password + "salt"));
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            // In production, use proper password verification
            return HashPassword(password) == hashedPassword;
        }

        public async Task<GenerateOtpResponse> GenerateOtp(GenerateOtpRequest request)
        {
            // Validate request
            if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.PhoneNumber))
            {
                return new GenerateOtpResponse 
                { 
                    Success = false, 
                    Message = "Either email or phone number is required" 
                };
            }

            // Check if user exists for login OTP - only email supported now
            if (request.Purpose == OtpPurpose.Login)
            {
                if (!string.IsNullOrEmpty(request.Email))
                {
                    var user = await GetUserByEmail(request.Email);
                    if (user == null)
                    {
                        return new GenerateOtpResponse 
                        { 
                            Success = false, 
                            Message = "User not found" 
                        };
                    }
                }
                else
                {
                    return new GenerateOtpResponse 
                    { 
                        Success = false, 
                        Message = "Email is required for authentication" 
                    };
                }
            }

            // Generate 6-digit OTP
            var otp = GenerateRandomOtp();
            var expiresAt = DateTime.UtcNow.AddMinutes(5); // 5 minute expiry

            // Invalidate existing OTPs
            await InvalidateExistingOtps(request.Email, request.PhoneNumber, request.Purpose);

            // Save OTP to database
            var otpCode = new OtpCode
            {
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Otp = otp,
                Purpose = request.Purpose,
                ExpiresAt = expiresAt
            };

            await _otpRepository.AddAsync(otpCode);

            // In production, send OTP via SMS/Email
            // For now, return OTP for testing purposes
            return new GenerateOtpResponse
            {
                Success = true,
                Message = "OTP sent successfully",
                ExpiresAt = expiresAt,
                OtpForTesting = otp // Remove this in production
            };
        }

        public async Task<VerifyOtpResponse> VerifyOtpRequest(VerifyOtpRequest request)
        {
            var isValid = await VerifyOtp(request.Email, request.PhoneNumber, request.Otp, request.Purpose);
            
            return new VerifyOtpResponse
            {
                Success = isValid,
                Message = isValid ? "OTP verified successfully" : "Invalid or expired OTP",
                Error = isValid ? null : "OTP verification failed"
            };
        }

        private async Task<bool> VerifyOtp(string? email, string? phoneNumber, string otp, OtpPurpose purpose)
        {
            var filterBuilder = Builders<OtpCode>.Filter;
            var filters = new List<FilterDefinition<OtpCode>>
            {
                filterBuilder.Eq(o => o.Otp, otp),
                filterBuilder.Eq(o => o.Purpose, purpose),
                filterBuilder.Eq(o => o.IsUsed, false),
                filterBuilder.Gte(o => o.ExpiresAt, DateTime.UtcNow)
            };

            if (!string.IsNullOrEmpty(email))
            {
                filters.Add(filterBuilder.Eq(o => o.Email, email));
            }
            else if (!string.IsNullOrEmpty(phoneNumber))
            {
                filters.Add(filterBuilder.Eq(o => o.PhoneNumber, phoneNumber));
            }
            else
            {
                return false;
            }

            var filter = filterBuilder.And(filters);
            var otpCodes = await _otpRepository.FindAsync(filter);
            var otpCode = otpCodes.FirstOrDefault();

            if (otpCode == null || !otpCode.IsValid)
            {
                // Increment attempts if OTP exists
                if (otpCode != null)
                {
                    otpCode.Attempts++;
                    await _otpRepository.UpdateAsync(otpCode);
                }
                return false;
            }

            // Mark OTP as used
            otpCode.IsUsed = true;
            await _otpRepository.UpdateAsync(otpCode);

            return true;
        }

        private async Task InvalidateExistingOtps(string? email, string? phoneNumber, OtpPurpose purpose)
        {
            var filterBuilder = Builders<OtpCode>.Filter;
            var filters = new List<FilterDefinition<OtpCode>>
            {
                filterBuilder.Eq(o => o.Purpose, purpose),
                filterBuilder.Eq(o => o.IsUsed, false)
            };

            if (!string.IsNullOrEmpty(email))
            {
                filters.Add(filterBuilder.Eq(o => o.Email, email));
            }
            else if (!string.IsNullOrEmpty(phoneNumber))
            {
                filters.Add(filterBuilder.Eq(o => o.PhoneNumber, phoneNumber));
            }

            var filter = filterBuilder.And(filters);
            var existingOtps = await _otpRepository.FindAsync(filter);

            foreach (var existingOtp in existingOtps)
            {
                existingOtp.IsUsed = true;
                await _otpRepository.UpdateAsync(existingOtp);
            }
        }

        private string GenerateRandomOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // 6-digit OTP
        }
    }
}