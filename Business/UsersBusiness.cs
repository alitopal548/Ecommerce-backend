using ECommerce.Data;
using ECommerce.Dto;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Business
{
    public class UsersBusiness
    {
        private readonly AppDbContext _context;

        public UsersBusiness(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserDto>> GetUsersAsync()
        {
            return await _context.Users
                .Select(u=> new UserDto
                {
                    Id = u.Id,
                    Username=u.Username, 
                    Email=u.Email,
                    Role=u.Role,
                })
                .ToListAsync();
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) 
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UpdateRoleResult> UpdateRoleResultAsync(int id, RoleUpdateDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Role))
                return UpdateRoleResult.BadRequest;

            var user = await _context.Users.FindAsync(id);
            if(user is null)
                return UpdateRoleResult.NotFound;

            user.Role = dto.Role;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return UpdateRoleResult.Success;
        }

        public enum UpdateRoleResult
        {
            Success,
            BadRequest,
            NotFound
        }
    }
}
