namespace DPTS.APIs.Models
{
    public class UserModel
    {
        public string UserId { get; set; } = string.Empty;
    }
    public class PatchRoleOfUserAsync
    {
        public string AdminUserId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } = false;
        public bool IsBuyer { get; set; } = true;
    }
    public class ChangePasswordModel
    {
        public string UserId { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string NewPasswordComfirmed {  get; set; } = string.Empty;
    }
}
