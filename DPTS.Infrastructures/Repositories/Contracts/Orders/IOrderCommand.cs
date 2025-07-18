using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Orders
{
    public interface IOrderCommand
    {
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task RemoveAsync(Order order);
        Task RemoveByIdAsync(string orderId);
    }
}
