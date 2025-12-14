using ECommerce.Dto;
using ECommerce.Models;
using ECommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Business
{
    public class AuthBusiness
    {
        private readonly AuthService _authService; 

        public AuthBusiness( AuthService authService )
        {
            _authService = authService;
        }

        public async Task<IActionResult> RegisterAsync(UserRegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return new BadRequestObjectResult(new {error="Kullanıcı adı ve şifre girilmelidir." });
            }

            try
            {
                var result = await _authService.RegisterAsync(dto);
                return new OkObjectResult(result);
            }
            catch (Exception ex) 
            {
                return new BadRequestObjectResult(new {error=ex.Message});
            }

        }
        public async Task<IActionResult> LoginAsync( UserLoginDto dto)
        {
            try
            {
                var token = await _authService.LoginAsync(dto); //servise git, login isteği at token al

                var user = await _authService.GetUserByUsernameAsync(dto.Username);

                return new OkObjectResult( new
                {
                    token,
                    role = user.Role,
                });
            }
            catch (Exception ex)
            {
                return new UnauthorizedObjectResult( new { error = ex.Message });// giriş başarısızsa 401
            }
        }

        public IActionResult GetProfile(ClaimsPrincipal user)
        {
            var username = user.Identity?.Name ?? "Bilinmeyen";
            return new OkObjectResult($"Merhaba {username} , profil sayfasına hoşgeldin");

        }

        public IActionResult AdminEndpoint()
        {
            return new OkObjectResult("Bu endpoint'e sadece admin erişebiliyooo");
        }
    }
}
