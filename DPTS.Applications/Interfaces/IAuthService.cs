using DPTS.Applications.Shareds;

namespace DPTS.Applications.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<string>> RegisterAsync(string email, string password, string passwordComfirmed, bool isBuyer = true, CancellationToken cancellationToken = default);
        Task<ServiceResult<string>> Auth2FAAsync(string email, string twoFactorSecret, CancellationToken cancellationToken = default);
        Task<ServiceResult<string>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    }
}
