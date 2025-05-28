using DPTS.Applications.Dtos.users;
using DPTS.Applications.Shareds;

namespace DPTS.Applications.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResult<UserDetailModel>> GetUser(string userId);
        Task<ServiceResult<UserDetailModel>> PatchRoleOfUser(string userId, string roleKey, bool isId = false);
    }
}
