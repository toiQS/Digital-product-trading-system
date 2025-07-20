using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.OrderItems
{
    public interface IOrderItemQuery
    {
        Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdsAsync(List<string> orderIds, CancellationToken cancellationToken);
    }
}
