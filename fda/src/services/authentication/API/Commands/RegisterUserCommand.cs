using Authentication.Models;
using Authentication.DataAccess;
using System.Security.Cryptography;
using System.Text;

namespace Authentication.API.Commands
{
    /// <summary>
    /// Command for user registration operation
    /// </summary>
    public class RegisterUserCommand : ICommand<RegisterResponse>
    {
        private readonly RegisterRequest _request;
        private readonly IRepository<UserAccount> _userRepository;

        public RegisterUserCommand(RegisterRequest request, IRepository<UserAccount> userRepository)
        {
            _request = request;
            _userRepository = userRepository;
        }

        public async Task<RegisterResponse> ExecuteAsync()
        {
            // Validate request
            if (!ValidateRequest())
            {
                throw new ArgumentException("Invalid registration request");
            }

            // Check if user already exists
            var existingUser = await GetExistingUser();
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists.");
            }

            // Create new user account
            var user = CreateUserAccount();

            // Save to database
            await _userRepository.AddAsync(user);

            return new RegisterResponse
            {
                UserId = user.Id!,
                Email = user.Email!,
                Message = "Registration successful"
            };
        }

        private bool ValidateRequest()
        {
            if (string.IsNullOrEmpty(_request.Email) || string.IsNullOrEmpty(_request.Password))
            {
                return false;
            }

            if (!IsValidEmail(_request.Email))
            {
                return false;
            }

            if (_request.Password.Length < 6)
            {
                return false;
            }

            return true;
        }

        private async Task<UserAccount?> GetExistingUser()
        {
            var allUsers = await _userRepository.GetAllAsync();
            return allUsers.FirstOrDefault(u => u.Email == _request.Email);
        }

        private UserAccount CreateUserAccount()
        {
            return new UserAccount
            {
                Email = _request.Email,
                Password = HashPassword(_request.Password!),
                PhoneNumber = _request.PhoneNumber,
                Role = _request.Role,
                Organization = _request.Organization ?? "default",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                LastLoginTime = null,
                FailedLoginAttempts = 0,
                Permissions = GetDefaultPermissions(_request.Role)
            };
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashBytes);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private List<string> GetDefaultPermissions(UserRole role)
        {
            return role switch
            {
                UserRole.Biller => new List<string>
                {
                    "receive_payments",
                    "manage_upi_settings",
                    "view_payment_history",
                    "generate_payment_reports",
                    "manage_restaurant_profile",
                    "view_financial_summary",
                    "process_refunds"
                },
                UserRole.Operator => new List<string>
                {
                    "view_orders",
                    "update_order_status",
                    "manage_inventory",
                    "view_menu",
                    "process_orders"
                },
                UserRole.Worker => new List<string>
                {
                    "view_assigned_orders",
                    "update_order_status",
                    "view_delivery_details"
                },
                UserRole.Customer => new List<string>
                {
                    "browse_menu",
                    "place_order",
                    "view_order_history",
                    "manage_profile",
                    "add_to_cart"
                },
                _ => new List<string>()
            };
        }
    }
}
