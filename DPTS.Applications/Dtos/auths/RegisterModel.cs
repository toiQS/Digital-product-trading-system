namespace DPTS.Applications.Dtos.auths
{
    public class RegisterModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ComfirmPassword { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        
    }
}
