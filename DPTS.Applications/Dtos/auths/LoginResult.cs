namespace DPTS.Applications.Dtos.auths
{
    public class LoginResult
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime Expiry { get; set; }
        
    }
}
