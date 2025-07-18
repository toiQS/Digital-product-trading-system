using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Orders
{
    public interface IOrderProcessCommand
    {
        Task AddAsync(OrderProcess process);

        Task AddRangeAsync(List<OrderProcess> processes);
    }
}
