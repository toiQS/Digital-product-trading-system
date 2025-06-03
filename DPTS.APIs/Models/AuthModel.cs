using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DPTS.APIs.Models
{
    public class AuthModel
    {
        
    }
    public class LoginModel
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [PasswordPropertyText]
        public string Password { get; set; } = string.Empty;
    }
    public class RegistrationModel
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [PasswordPropertyText]
        public string Password { get; set; } = string.Empty;
        [PasswordPropertyText]
        public string PasswordComfirmed {  get; set; } = string.Empty;
    }
    public class Auth2FAModel
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string TwoFactorSecret {  get; set; } = string.Empty;
    }
}
