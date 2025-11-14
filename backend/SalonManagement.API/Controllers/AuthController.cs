// SalonManagement.API/Controllers/AuthController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalonManagement.API.DTOs.Auth;
using SalonManagement.API.Repositories.Interfaces;

namespace SalonManagement.API.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);
            return HandleResult(result);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.RegisterCustomerAsync(request);
            return HandleResult(result);
        }

        [HttpPost("login-salonmanager")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginSalonManager([FromBody] LoginRequestDto request)
        {
            var result = await _authService.LoginSalonManagerAsync(request);
            return HandleResult(result);
        }

        [HttpPost("login-employee")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginEmpoyee([FromBody] LoginRequestDto request)
        {
            var result = await _authService.LoginEmployeeAsync(request);
            return HandleResult(result);
        }


        [HttpPost("login-sysadmin")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginSysAdmin([FromBody] LoginRequestDto request)
        {
            var result = await _authService.LoginSystemAdminAsync(request);
            return HandleResult(result);
        }

        [HttpPost("register-sysadmin")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterSysAdmin([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.RegisterSystemAdminAsync(request);
            return HandleResult(result);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = Guid.Parse(User.FindFirst("userId")?.Value ?? string.Empty);
            var result = await _authService.GetCurrentUserAsync(userId);
            
            return HandleResult(result);
        }
    }
}