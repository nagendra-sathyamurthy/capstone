using Authentication.Models;
using Authentication.DataAccess;
using System.Threading.Tasks;

namespace Authentication.Services
{
    public class AuthService
    {
        private readonly IRepository<UserAccount> _repository;

        public AuthService(IRepository<UserAccount> repository)
        {
            _repository = repository;
        }

        public async Task<string?> Login(string email, string password)
        {
            var user = await _repository.GetByIdAsync(email);
            if (user != null && user.Password == password)
            {
                // Generate JWT token
                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var key = System.Text.Encoding.ASCII.GetBytes("GJ0VFqmRVBR0iE2ojyzh28HlayZgRcUI");
                var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
                {
                    Subject = new System.Security.Claims.ClaimsIdentity(new[]
                    {
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Email)
                    }),
                    Expires = System.DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                        new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                        Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            return null;
        }

        public async Task Register(UserAccount user)
        {
            var existing = await _repository.GetByIdAsync(user.Email);
            if (existing != null)
            {
                throw new System.InvalidOperationException("User with this email already exists.");
            }
            await _repository.AddAsync(user);
        }

        public async Task ForgotPassword(string email)
        {
            // Implement forgot password logic (e.g., send reset email)
        }
    }
}