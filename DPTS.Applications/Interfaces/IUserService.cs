using DPTS.Applications.Dtos;
using DPTS.Applications.Shareds;

namespace DPTS.Applications.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResult<ProfileDto>> GetUserAsync(string userId);
        Task<ServiceResult<MiniProfileDto>> GetMiniProfileAsync(string userId);
        Task<ServiceResult<string>> PatchRoleOfUserAsync(string adminUserId, string userId, bool isBuyer = true, bool isAdmin = false, CancellationToken cancellationToken = default);
        Task<ServiceResult<string>> ChangePasswordAsync(string userId, string oldPassword, string newPassword, string newPasswordComfirmed);

    }
}
