namespace DPTS.Applications.Auth.Dtos
{
   public class RegisterDto
    {
        public string UserId { get; set; } = string.Empty;
        public bool IsEmailConfirmationRequired { get; set; } = true;
    }
}
