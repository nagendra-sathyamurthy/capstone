using Authentication.Models;
using Authentication.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Authentication.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserAccount loginRequest)
        {
            var user = await _authService.Login(loginRequest.Email, loginRequest.Password);
            if (user == null)
                return Unauthorized();
            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserAccount registerRequest)
        {
            try
            {
                await _authService.Register(registerRequest);
                return Ok();
            }
            catch (System.InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            await _authService.ForgotPassword(email);
            return Ok();
        }
    }
}