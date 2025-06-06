using DPTS.Applications.Dtos;
using DPTS.Applications.Shareds;

namespace DPTS.Applications.Interfaces
{
    public interface IMessageService
    {
        Task<ServiceResult<IEnumerable<RecentMessageIndexDto>>> GetRecentMessagesAsync(string sellerId, int pageNumber = 2, int pageSize = 10);
    }
}
