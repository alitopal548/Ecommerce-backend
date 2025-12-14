namespace ECommerce.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = [];
        public byte[] PasswordSalt { get; set; } = [];
        public string Role { get; set; } = "user";
        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; } = false;
    }
}
