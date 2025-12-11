using Authentication.Models;
using Authentication.DataAccess;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Authentication.API.Commands
{
    /// <summary>
    /// Command for token validation operation
    /// </summary>
    public class ValidateTokenCommand : ICommand<ValidateTokenResponse>
    {
        private readonly string _token;
        private readonly IRepository<UserAccount> _userRepository;
        private readonly string _jwtSecret;

        public ValidateTokenCommand(string token, IRepository<UserAccount> userRepository, string jwtSecret)
        {
            _token = token;
            _userRepository = userRepository;
            _jwtSecret = jwtSecret;
        }

        public async Task<ValidateTokenResponse> ExecuteAsync()
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSecret);

                tokenHandler.ValidateToken(_token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == "userId").Value;

                var user = await _userRepository.GetByIdAsync(userId);
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

        private UserInfo MapToUserInfo(UserAccount user)
        {
            return new UserInfo
            {
                Id = user.Id!,
                Email = user.Email!,
                Role = (int)user.Role,
                Organization = user.Organization ?? "default",
                Permissions = user.Permissions,
                LastLoginTime = user.LastLoginTime
            };
        }
    }
}
