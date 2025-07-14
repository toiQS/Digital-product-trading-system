using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Interfaces
{
    public interface IOrderPaymentMethodRepository
    {
        #region Query

        Task<List<OrderPaymentMethod>> GetByOrderIdAsync(string orderId);

        Task<List<OrderPaymentMethod>> GetByWalletIdAsync(string walletId);

        Task<List<OrderPaymentMethod>> GetByPaymentMethodIdAsync(string paymentMethodId);

        Task<decimal> GetTotalPaidByOrderAsync(string orderId);

        Task<bool> HasPaymentFromWalletAsync(string orderId);

        #endregion

        #region Crud

        Task AddAsync(OrderPaymentMethod payment);
        Task AddRangeAsync(List<OrderPaymentMethod> payments);

        #endregion
    }
}
