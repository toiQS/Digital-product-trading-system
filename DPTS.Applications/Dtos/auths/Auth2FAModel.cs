namespace DPTS.Applications.Dtos.auths
{
    public class Auth2FAModel
    {
        public string Email { get; set; } = string.Empty;
        public string TwoFactorSecret { get; set; } = string.Empty;
    }
}
