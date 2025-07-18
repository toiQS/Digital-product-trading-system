namespace DPTS.Infrastructures.Repositories.Contracts.Users
{
    public interface IUserSecurityLoginHandler
    {
        Task IncrementFailedLoginAttemptsAsync(string userId);
        Task ResetFailedLoginAttemptsAsync(string userId);
        Task LockoutUserAsync(string userId, DateTime until);
        Task MarkEmailVerifiedAsync(string userId);
        Task UpdatePasswordAsync(string userId, string newPasswordHash);
        Task UpdateTwoFactorAsync(string userId, bool enabled, string? secret);
    }
}
