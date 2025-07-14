using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Interfaces
{
    public interface IOrderProcessRepository
    {
        #region Query

        Task<List<OrderProcess>> GetByOrderIdAsync(string orderId);

        Task<OrderProcess?> GetLatestProcessAsync(string orderId);

        Task<List<OrderProcess>> GetProcessesByKeywordAsync(string keyword, DateTime? from = null, DateTime? to = null);

        #endregion

        #region Crud

        Task AddAsync(OrderProcess process);
        Task AddRangeAsync(List<OrderProcess> processes);

        #endregion
    }
}
