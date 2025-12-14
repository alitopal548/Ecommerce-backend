using ECommerce.Data;
using ECommerce.Dto;
using ECommerce.Helpers;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ECommerce.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtTokenGenerator _tokenGenerator;

        public AuthService(AppDbContext context, JwtTokenGenerator tokenGenerator)
        {
            _context = context;
            _tokenGenerator = tokenGenerator;
        }
        public async Task<string> LoginAsync(UserLoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null)
                throw new Exception("Kullanıcı Bulunamadı.");

            using var hmac = new HMACSHA512(user.PasswordSalt); //şifreyi aynı salt ile tekrar hashle
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

            for(int i=0; i<computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                    throw new Exception("Şifre Hatalı. Lütfen tekrar deneyiniz.");
            }

            return _tokenGenerator.GenerateToken(user.Id ,user.Username, user.Role); //giriş başarılıysa token üret
        }

        public async Task<string> RegisterAsync(UserRegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                throw new Exception("Kullanıcı adı mevcut yeni bir kullanıcı adı deneyiniz");

            using var hmac = new HMACSHA512();

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
                PasswordSalt = hmac.Key,
                Role = "user",
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return "Kayıt başarılı";
        }

        public async Task<User> GetUserByUsernameAsync(string username)
            => await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
    }
}
