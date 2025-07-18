using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.OrderItems
{
    public interface IOrderItemCommand
    {
        Task AddAsync(OrderItem orderItem);
        Task UpdateAsync(OrderItem orderItem);
        Task RemoveAsync(OrderItem orderItem);
        Task RemoveByOrderIdAsync(string orderId);
    }
}
