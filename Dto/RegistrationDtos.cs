namespace ECommerce.Dto
{
    /// <summary>
    /// Kayıt isteği sırasında kullanıcıdan alınacak bilgiler
    /// </summary>
    public class RequestRegisterDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Kayıt onayı sırasında kullanıcıdan alınacak doğrulama kodu
    /// </summary>
    public class ConfirmRegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
