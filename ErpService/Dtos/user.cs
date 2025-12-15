using System.ComponentModel.DataAnnotations;

namespace ErpService.Dtos
{
    public class LoginRequest
    {

        [Required(ErrorMessage = "Username is required")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;
    }


    public class LoginResponse
    {
   
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }

        public string TokenType { get; set; } = "Bearer";
        public string Username { get; set; } = string.Empty;
    }

}