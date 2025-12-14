using ECommerce.Business;
using ECommerce.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UsersController : ControllerBase
    {
        private readonly UsersBusiness _business;

        public UsersController(UsersBusiness business)
        {
            _business = business;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _business.GetUsersAsync();
            return Ok(users);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deletedUser = await _business.DeleteUserAsync(id);

            if (!deletedUser) 
                return NotFound();

            return NoContent();
        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] RoleUpdateDto dto)
        {
            var result = await _business.UpdateRoleResultAsync(id, dto);

            if (result == UsersBusiness.UpdateRoleResult.BadRequest)
                return BadRequest("Rol alanı boş olamaz.");

            if(result == UsersBusiness.UpdateRoleResult.NotFound)
                return NotFound();

            return NoContent();
        }


    }
}
