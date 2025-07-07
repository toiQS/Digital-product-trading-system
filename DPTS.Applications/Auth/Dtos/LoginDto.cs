namespace DPTS.Applications.Auth.Dtos
{
   public class LoginDto 
    {
        public JwtTokenDto Token { get; set; } = new();
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = "Buyer";
        public bool IsTwoFactorRequired { get; set; } = false;
        public bool IsEmailVerified { get; set; } = false;
    }
}
