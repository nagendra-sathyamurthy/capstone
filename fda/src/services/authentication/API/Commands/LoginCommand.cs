using Authentication.Models;
using Authentication.DataAccess;
using System.Security.Cryptography;
using System.Text;

namespace Authentication.API.Commands
{
    /// <summary>
    /// Command for user login operation
    /// </summary>
    public class LoginCommand : ICommand<LoginResponse?>
    {
        private readonly LoginRequest _request;
        private readonly IRepository<UserAccount> _userRepository;
        private readonly IRepository<OtpCode> _otpRepository;
        private readonly string _jwtSecret;

        public LoginCommand(
            LoginRequest request,
            IRepository<UserAccount> userRepository,
            IRepository<OtpCode> otpRepository,
            string jwtSecret)
        {
            _request = request;
            _userRepository = userRepository;
            _otpRepository = otpRepository;
            _jwtSecret = jwtSecret;
        }

        public async Task<LoginResponse?> ExecuteAsync()
        {
            if (!ValidateRequest())
            {
                return null;
            }

            UserAccount? user = null;

            switch (_request.LoginMethod)
            {
                case LoginMethod.EmailPassword:
                    user = await AuthenticateWithEmailPassword();
                    break;

                case LoginMethod.EmailOtp:
                    user = await AuthenticateWithEmailOtp();
                    break;

                case LoginMethod.PhoneOtp:
                    user = await AuthenticateWithPhoneOtp();
                    break;

                default:
                    return null;
            }

            if (user == null)
            {
                return null;
            }

            // Update last login
            user.LastLoginTime = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user.Id!, user);

            // Generate token
            var token = GenerateJwtToken(user);

            return new LoginResponse
            {
                Token = token,
                UserId = user.Id!,
                Email = user.Email!,
                Role = user.Role,
                Organization = user.Organization ?? "default"
            };
        }

        private bool ValidateRequest()
        {
            return _request.LoginMethod switch
            {
                LoginMethod.EmailPassword => !string.IsNullOrEmpty(_request.Email) && !string.IsNullOrEmpty(_request.Password),
                LoginMethod.EmailOtp => !string.IsNullOrEmpty(_request.Email) && !string.IsNullOrEmpty(_request.Otp),
                LoginMethod.PhoneOtp => !string.IsNullOrEmpty(_request.PhoneNumber) && !string.IsNullOrEmpty(_request.Otp),
                _ => false
            };
        }

        private async Task<UserAccount?> AuthenticateWithEmailPassword()
        {
            var users = await _userRepository.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Email == _request.Email);

            if (user == null || !VerifyPassword(_request.Password!, user.Password) || !user.IsActive)
            {
                if (user != null)
                {
                    user.FailedLoginAttempts++;
                    user.LastFailedLoginTime = DateTime.UtcNow;
                    if (user.FailedLoginAttempts >= 5)
                    {
                        user.IsActive = false;
                    }
                    await _userRepository.UpdateAsync(user.Id!, user);
                }
                return null;
            }

            // Reset failed attempts on successful login
            user.FailedLoginAttempts = 0;
            user.LastFailedLoginTime = null;

            return user;
        }

        private async Task<UserAccount?> AuthenticateWithEmailOtp()
        {
            var users = await _userRepository.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Email == _request.Email);

            if (user == null || !user.IsActive)
            {
                return null;
            }

            var isValidOtp = await VerifyOtp(_request.Email!, _request.Otp!, OtpPurpose.Login);
            if (!isValidOtp)
            {
                return null;
            }

            return user;
        }

        private async Task<UserAccount?> AuthenticateWithPhoneOtp()
        {
            var users = await _userRepository.GetAllAsync();
            var user = users.FirstOrDefault(u => u.PhoneNumber == _request.PhoneNumber);

            if (user == null || !user.IsActive)
            {
                return null;
            }

            var isValidOtp = await VerifyOtp(_request.PhoneNumber!, _request.Otp!, OtpPurpose.Login);
            if (!isValidOtp)
            {
                return null;
            }

            return user;
        }

        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(inputPassword));
            var hash = Convert.ToBase64String(hashBytes);
            return hash == storedPassword;
        }

        private async Task<bool> VerifyOtp(string identifier, string otp, OtpPurpose purpose)
        {
            var allOtps = await _otpRepository.GetAllAsync();
            var otpCode = allOtps
                .Where(o => o.Purpose == purpose && o.Code == otp)
                .Where(o => o.Email == identifier || o.PhoneNumber == identifier)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefault();

            if (otpCode == null || otpCode.ExpiresAt < DateTime.UtcNow || otpCode.IsUsed)
            {
                return false;
            }

            // Mark OTP as used
            otpCode.IsUsed = true;
            await _otpRepository.UpdateAsync(otpCode.Id!, otpCode);

            return true;
        }

        private string GenerateJwtToken(UserAccount user)
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim("userId", user.Id!),
                new System.Security.Claims.Claim("email", user.Email!),
                new System.Security.Claims.Claim("role", user.Role.ToString()),
                new System.Security.Claims.Claim("organization", user.Organization ?? "default")
            };

            // Add permissions as claims
            foreach (var permission in user.Permissions)
            {
                claims.Add(new System.Security.Claims.Claim("permission", permission));
            }

            var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                    new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                    Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
