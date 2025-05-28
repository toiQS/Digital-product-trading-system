using DPTS.Applications.Dtos.auths;
using DPTS.Applications.Shareds;

namespace DPTS.Applications.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<string>> RegisterAsync(RegisterModel model);
        Task<ServiceResult<string>> Auth2FAAsync(Auth2FAModel model);
        Task<ServiceResult<LoginResult>> LoginAsync(LoginModel model);

    }
}
