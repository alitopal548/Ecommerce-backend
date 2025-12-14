using ECommerce.Data;
using ECommerce.Dto;
using ECommerce.Models;
using ECommerce.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ECommerce.Business
{
    public class RegistrationBusiness
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        public RegistrationBusiness(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<string?> RequestRegisterAsync(RequestRegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                    return "Kullanıcı adı, Email ve Şifre alanları zorunludur.";

            bool userExists = await _context.Users
                .AnyAsync(u=>u.Username==dto.Username || u.Email==dto.Email);

            bool requestExists = await _context.RegistrationRequests
                .AnyAsync(r=>r.Email==dto.Email);

            if (userExists || requestExists)
                return "Bu kullanıcı adı veya e-posta zaten mevcut.";

            //şifreyi hashla
            using var hmac = new HMACSHA512();
            var salt = hmac.Key;
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

            // 6 haneli rastgele kod üretme
            var rng = RandomNumberGenerator.Create();
            byte[] nb = new byte[4];
            rng.GetBytes(nb);
            int codeInt = new Random(BitConverter.ToInt32(nb, 0)).Next(100000, 999999);
            string code = codeInt.ToString();

            var req = new RegistrationRequest
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordSalt = salt,
                PasswordHash = hash,
                Code = code,
                CodeExpiresAt = DateTime.UtcNow.AddMinutes(10)
            };

            _context.RegistrationRequests.Add(req);
            await _context.SaveChangesAsync();

            //eposta gönderimi
            await _emailService.SendEmailAsync(
                dto.Email,
                "Kayıt Kodunuz",
                $"Kayıt işleminiz için kodunuz: {code}");
            return null;
        }

        public async Task<string?> ConfirmRegisterAsync(ConfirmRegisterDto dto)
        {
            var req = await _context.RegistrationRequests.SingleOrDefaultAsync(r=>r.Email == dto.Email && r.Code==dto.Code);

            if (req == null || req.CodeExpiresAt < DateTime.UtcNow)
                return "Girdiğiniz kod geçersiz veya süresi dolmuş";

            //Yeni kullanıcıyı Users tablosuna ekle
            var user = new User
            {
                Username = req.Username,
                Email = req.Email,
                PasswordHash = req.PasswordHash,
                PasswordSalt = req.PasswordSalt,
                Role = "user",
                EmailConfirmed = true,
            };
            _context.Users.Add(user);

            //Geçici isteği sil
            _context.RegistrationRequests.Remove(req);
            await _context.SaveChangesAsync();

            return null;
        }
    }
}
