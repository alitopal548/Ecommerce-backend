using ECommerce.Business;
using ECommerce.Dto;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly RegistrationBusiness _business;
        public RegistrationController(RegistrationBusiness business)
        {
            _business = business;
        }

        [HttpPost("request-register")]
        public async Task<IActionResult> RequestRegister([FromBody] RequestRegisterDto dto)
        {
            var error = await _business.RequestRegisterAsync(dto);
            if (error != null)
                return BadRequest(error);

            return Ok(new { Message = "Kayıt kodu e-posta adresinize gönderildi." });
        }

        [HttpPost("confirm-register")]
        public async Task<IActionResult> ConfirmRegister([FromBody] ConfirmRegisterDto dto)
        {
            var error = await _business.ConfirmRegisterAsync(dto);
            if (error != null)
                return BadRequest(error);

            return Ok(new { Message = "Kayıt tamamlandı. Giriş yapabilirsiniz." });
        }
    }
}
