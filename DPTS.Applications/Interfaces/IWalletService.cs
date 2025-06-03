using DPTS.Applications.Dtos;
using DPTS.Applications.Shareds;

namespace DPTS.Applications.Interfaces
{
    public interface IWalletService
    {
        Task<ServiceResult<WalletPersonalDto>> WalletPersonalAsync(string userId);
    }
}
