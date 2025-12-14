using ECommerce.Business;
using ECommerce.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthBusiness _authManager;

        public AuthController(AuthBusiness authManager)
        {
            _authManager = authManager;
        }
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            return await _authManager.RegisterAsync(dto);
        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
           return await _authManager.LoginAsync(dto);
        }

        [Authorize]
        [HttpGet("Profile")]
        public IActionResult GetProfile()
        {
            return _authManager.GetProfile(User);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnly()
        {
            return _authManager.AdminEndpoint();
        }

    }

}
