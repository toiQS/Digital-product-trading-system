using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Orders
{
    public interface IOrderProcessQuery
    {
        Task<List<OrderProcess>> GetByOrderIdAsync(string orderId);

        Task<OrderProcess?> GetLatestProcessAsync(string orderId);

        Task<List<OrderProcess>> GetProcessesByKeywordAsync(string keyword, DateTime? from = null, DateTime? to = null);
    }
}
