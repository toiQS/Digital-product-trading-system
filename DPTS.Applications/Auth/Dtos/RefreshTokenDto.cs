namespace DPTS.Applications.Auth.Dtos
{
    public class RefreshTokenDto
    {
        public JwtTokenDto Token { get; set; } = new();
    }
}
