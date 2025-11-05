using Authentication.Models;
using Authentication.DataAccess;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

namespace Authentication.Services
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
            // Validate login request
            if (!ValidateLoginRequest(request))
            {
                return null;
            }

            UserAccount? user = null;

            // Get user based on login method
            switch (request.LoginMethod)
            {
                case LoginMethod.EmailPassword:
                    user = await GetUserByEmail(request.Email!);
                    if (user == null || !VerifyPassword(request.Password!, user.Password) || !user.IsActive)
                    {
                        await HandleFailedLogin(user);
                        return null;
                    }
                    break;

                case LoginMethod.PhonePassword:
                    user = await GetUserByPhone(request.PhoneNumber!);
                    if (user == null || !VerifyPassword(request.Password!, user.Password) || !user.IsActive)
                    {
                        await HandleFailedLogin(user);
                        return null;
                    }
                    break;

                case LoginMethod.EmailOtp:
                    if (!await VerifyOtp(request.Email, null, request.Otp!, OtpPurpose.Login))
                    {
                        return null;
                    }
                    user = await GetUserByEmail(request.Email!);
                    if (user == null || !user.IsActive)
                    {
                        return null;
                    }
                    break;

                case LoginMethod.PhoneOtp:
                    if (!await VerifyOtp(null, request.PhoneNumber, request.Otp!, OtpPurpose.Login))
                    {
                        return null;
                    }
                    user = await GetUserByPhone(request.PhoneNumber!);
                    if (user == null || !user.IsActive)
                    {
                        return null;
                    }
                    break;

                default:
                    return null;
            }

            // Reset invalid logins on successful login
            user.InvalidLogins = 0;
            user.LastLoginTime = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(user);

            var token = GenerateJwtToken(user);
            var expiresAt = DateTime.UtcNow.AddHours(8); // 8 hour token

            return new LoginResponse
            {
                Token = token,
                User = MapToUserInfo(user),
                ExpiresAt = expiresAt
            };
        }

        public async Task<UserAccount> Register(RegisterRequest request)
        {
            var existingEmail = await GetUserByEmail(request.Email);
            if (existingEmail != null)
            {
                throw new InvalidOperationException("User with this email already exists.");
            }

            var existingPhone = await GetUserByPhone(request.PhoneNumber);
            if (existingPhone != null)
            {
                throw new InvalidOperationException("User with this phone number already exists.");
            }

            // Validate role-specific required information
            ValidateRegistrationData(request);

            var user = new UserAccount
            {
                Email = request.Email,
                Password = HashPassword(request.Password), // In production, use proper hashing
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Role = request.Role,
                Organization = DetermineOrganization(request.Role, request.Organization),
                Permissions = Permissions.GetPermissionsForRole(request.Role),
                DateOfBirth = request.DateOfBirth,
                Address = request.Address,
                DietaryPreferences = request.DietaryPreferences,
                FavoriteRestaurants = request.FavoriteRestaurants,
                EmployeeInfo = request.EmployeeInfo,
                BusinessInfo = request.BusinessInfo,
                DeliveryInfo = request.DeliveryInfo,
                TechInfo = request.TechInfo,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(user);
            return user;
        }

        private void ValidateRegistrationData(RegisterRequest request)
        {
            switch (request.Role)
            {
                case UserRole.Biller:
                    if (request.BusinessInfo == null)
                        throw new InvalidOperationException("BusinessInfo is required for Biller role.");
                    if (string.IsNullOrEmpty(request.BusinessInfo.RestaurantName))
                        throw new InvalidOperationException("Restaurant name is required for Biller role.");
                    if (string.IsNullOrEmpty(request.BusinessInfo.UpiId))
                        throw new InvalidOperationException("UPI ID is required for Biller role (payment recipient).");
                    break;

                case UserRole.Operator:
                case UserRole.Worker:
                    if (request.EmployeeInfo == null)
                        throw new InvalidOperationException($"EmployeeInfo is required for {request.Role} role.");
                    if (string.IsNullOrEmpty(request.EmployeeInfo.EmployeeId))
                        throw new InvalidOperationException($"Employee ID is required for {request.Role} role.");
                    if (string.IsNullOrEmpty(request.EmployeeInfo.Position))
                        throw new InvalidOperationException($"Position is required for {request.Role} role.");
                    break;

                case UserRole.DeliveryAgent:
                    if (request.DeliveryInfo == null)
                        throw new InvalidOperationException("DeliveryInfo is required for DeliveryAgent role.");
                    if (string.IsNullOrEmpty(request.DeliveryInfo.EmployeeId))
                        throw new InvalidOperationException("Employee ID is required for DeliveryAgent role.");
                    if (string.IsNullOrEmpty(request.DeliveryInfo.VehicleType))
                        throw new InvalidOperationException("Vehicle type is required for DeliveryAgent role.");
                    if (string.IsNullOrEmpty(request.DeliveryInfo.LicensePlate))
                        throw new InvalidOperationException("License plate is required for DeliveryAgent role.");
                    break;

                case UserRole.Developer:
                case UserRole.Tester:
                case UserRole.NetworkAdmin:
                case UserRole.DatabaseAdmin:
                    if (request.TechInfo == null)
                        throw new InvalidOperationException($"TechInfo is required for {request.Role} role.");
                    if (string.IsNullOrEmpty(request.TechInfo.EmployeeId))
                        throw new InvalidOperationException($"Employee ID is required for {request.Role} role.");
                    if (string.IsNullOrEmpty(request.TechInfo.Specialization))
                        throw new InvalidOperationException($"Specialization is required for {request.Role} role.");
                    break;

                case UserRole.Customer:
                    // No additional validation required for customers
                    break;

                default:
                    throw new InvalidOperationException($"Unknown role: {request.Role}");
            }
        }

        public async Task<ValidateTokenResponse> ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSecret);
                
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == "userId").Value;
                
                var user = await _repository.GetByIdAsync(userId);
                if (user == null || !user.IsActive)
                {
                    return new ValidateTokenResponse { IsValid = false, Error = "User not found or inactive" };
                }

                return new ValidateTokenResponse 
                { 
                    IsValid = true, 
                    User = MapToUserInfo(user) 
                };
            }
            catch
            {
                return new ValidateTokenResponse { IsValid = false, Error = "Invalid token" };
            }
        }

        public async Task<UserAccount?> GetUserProfile(string userId)
        {
            return await _repository.GetByIdAsync(userId);
        }

        public async Task<UserAccount?> UpdateProfile(string userId, UpdateProfileRequest request)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user == null) return null;

            if (!string.IsNullOrEmpty(request.FirstName)) user.FirstName = request.FirstName;
            if (!string.IsNullOrEmpty(request.LastName)) user.LastName = request.LastName;
            if (!string.IsNullOrEmpty(request.PhoneNumber)) user.PhoneNumber = request.PhoneNumber;
            if (request.Address != null) user.Address = request.Address;
            if (request.DietaryPreferences != null) user.DietaryPreferences = request.DietaryPreferences;
            
            user.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(user);
            
            return user;
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

        // Enhanced role-specific validation methods for refined RBAC
        public bool CanReceivePayments(UserAccount user)
        {
            return user.Role == UserRole.Biller && 
                   user.BusinessInfo != null && 
                   !string.IsNullOrEmpty(user.BusinessInfo.UpiId);
        }

        public bool CanConfirmOrders(UserAccount user)
        {
            return user.Role == UserRole.Operator && 
                   user.EmployeeInfo != null;
        }

        public bool CanPrepareFood(UserAccount user)
        {
            return user.Role == UserRole.Worker && 
                   user.EmployeeInfo != null;
        }

        public bool CanConfirmDelivery(UserAccount user)
        {
            return user.Role == UserRole.DeliveryAgent && 
                   user.DeliveryInfo != null && 
                   !string.IsNullOrEmpty(user.DeliveryInfo.VehicleType);
        }

        public bool CanAccessAllEndpoints(UserAccount user)
        {
            return (user.Role == UserRole.Developer || user.Role == UserRole.Tester) && 
                   user.TechInfo != null;
        }

        public bool CanAccessHealthcheck(UserAccount user)
        {
            return user.Role == UserRole.NetworkAdmin && 
                   user.TechInfo != null;
        }

        public bool CanAccessDatabase(UserAccount user)
        {
            return user.Role == UserRole.DatabaseAdmin && 
                   user.TechInfo != null;
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

        private async Task<UserAccount?> GetUserByPhone(string phoneNumber)
        {
            var filter = Builders<UserAccount>.Filter.Eq(u => u.PhoneNumber, phoneNumber);
            var users = await _repository.FindAsync(filter);
            return users.FirstOrDefault();
        }

        private bool ValidateLoginRequest(LoginRequest request)
        {
            switch (request.LoginMethod)
            {
                case LoginMethod.EmailPassword:
                    return !string.IsNullOrEmpty(request.Email) && !string.IsNullOrEmpty(request.Password);
                case LoginMethod.PhonePassword:
                    return !string.IsNullOrEmpty(request.PhoneNumber) && !string.IsNullOrEmpty(request.Password);
                case LoginMethod.EmailOtp:
                    return !string.IsNullOrEmpty(request.Email) && !string.IsNullOrEmpty(request.Otp);
                case LoginMethod.PhoneOtp:
                    return !string.IsNullOrEmpty(request.PhoneNumber) && !string.IsNullOrEmpty(request.Otp);
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
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName),
                new Claim("role", user.Role.ToString()),
                new Claim("organization", user.Organization),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id!)
            };

            // Add permissions as claims
            foreach (var permission in user.Permissions)
            {
                claims.Add(new Claim("permission", permission));
            }

            // Add role-specific claims based on refined RBAC system
            switch (user.Role)
            {
                case UserRole.Biller:
                    if (user.BusinessInfo != null)
                    {
                        claims.Add(new Claim("restaurantName", user.BusinessInfo.RestaurantName ?? ""));
                        claims.Add(new Claim("upiId", user.BusinessInfo.UpiId ?? ""));
                        claims.Add(new Claim("isUpiVerified", user.BusinessInfo.IsUpiVerified.ToString()));
                        claims.Add(new Claim("businessLicense", user.BusinessInfo.BusinessLicense ?? ""));
                    }
                    break;

                case UserRole.Operator:
                case UserRole.Worker:
                    if (user.EmployeeInfo != null)
                    {
                        claims.Add(new Claim("employeeId", user.EmployeeInfo.EmployeeId ?? ""));
                        claims.Add(new Claim("position", user.EmployeeInfo.Position ?? ""));
                        claims.Add(new Claim("department", user.EmployeeInfo.Department ?? ""));
                    }
                    break;

                case UserRole.DeliveryAgent:
                    if (user.DeliveryInfo != null)
                    {
                        claims.Add(new Claim("employeeId", user.DeliveryInfo.EmployeeId ?? ""));
                        claims.Add(new Claim("vehicleType", user.DeliveryInfo.VehicleType ?? ""));
                        claims.Add(new Claim("licensePlate", user.DeliveryInfo.LicensePlate ?? ""));
                        claims.Add(new Claim("averageRating", user.DeliveryInfo.AverageRating.ToString()));
                    }
                    break;

                case UserRole.Developer:
                case UserRole.Tester:
                case UserRole.NetworkAdmin:
                case UserRole.DatabaseAdmin:
                    if (user.TechInfo != null)
                    {
                        claims.Add(new Claim("employeeId", user.TechInfo.EmployeeId ?? ""));
                        claims.Add(new Claim("specialization", user.TechInfo.Specialization ?? ""));
                        claims.Add(new Claim("department", user.TechInfo.Department ?? ""));
                        claims.Add(new Claim("securityClearance", user.TechInfo.SecurityClearance.ToString()));
                    }
                    break;

                case UserRole.Customer:
                    // Add customer-specific claims if needed
                    if (user.DietaryPreferences?.Count > 0)
                    {
                        claims.Add(new Claim("dietaryPreferences", string.Join(",", user.DietaryPreferences)));
                    }
                    break;
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

        private UserInfo MapToUserInfo(UserAccount user)
        {
            return new UserInfo
            {
                Id = user.Id!,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                Organization = user.Organization,
                Permissions = user.Permissions,
                LastLoginTime = user.LastLoginTime,
                EmployeeInfo = user.EmployeeInfo,
                BusinessInfo = user.BusinessInfo,
                DeliveryInfo = user.DeliveryInfo,
                TechInfo = user.TechInfo
            };
        }

        private string DetermineOrganization(UserRole role, string? customOrganization)
        {
            return Organizations.GetOrganizationForRole(role, customOrganization);
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

            // Check if user exists for login OTP
            if (request.Purpose == OtpPurpose.Login)
            {
                UserAccount? user = null;
                if (!string.IsNullOrEmpty(request.Email))
                {
                    user = await GetUserByEmail(request.Email);
                }
                else if (!string.IsNullOrEmpty(request.PhoneNumber))
                {
                    user = await GetUserByPhone(request.PhoneNumber);
                }

                if (user == null)
                {
                    return new GenerateOtpResponse 
                    { 
                        Success = false, 
                        Message = "User not found" 
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