using DPTS.Applications.Dtos.statisticals;
using DPTS.Applications.Shareds;

namespace DPTS.Applications.Interfaces
{
    public interface IStatictisAdminService
    {
        Task<ServiceResult<UserFractionsByRoleModel>> GetMembersInAdminRole();
        Task<ServiceResult<UserFractionsByRoleModel>> GetMembersInBuyerRole();
        Task<ServiceResult<UserFractionsByRoleModel>> GetMembersInSellerRole();
        Task<ServiceResult<IEnumerable<UserFractionsByRoleModel>>> GetMembersAsync();
    }
}
