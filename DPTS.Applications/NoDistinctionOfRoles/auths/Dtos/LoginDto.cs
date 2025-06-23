namespace DPTS.Applications.NoDistinctionOfRoles.auths.Dtos
{
    public class LoginDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiry { get; set; }
    }
}
