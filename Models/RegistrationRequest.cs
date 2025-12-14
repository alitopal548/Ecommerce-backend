using System;
namespace ECommerce.Models
{
    public class RegistrationRequest
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
        public string Code { get; set; } = string.Empty;
        public DateTime CodeExpiresAt { get; set; }
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;



    }
}
