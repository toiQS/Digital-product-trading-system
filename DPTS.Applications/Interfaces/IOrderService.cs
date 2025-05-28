using DPTS.Applications.Dtos.orders;
using DPTS.Applications.Shareds;

namespace DPTS.Applications.Interfaces
{
    public interface IOrderService
    {
        Task<ServiceResult<IEnumerable<OrderIndexModel>>> GetOrdersBySellerId(string sellerId);
    }
}
