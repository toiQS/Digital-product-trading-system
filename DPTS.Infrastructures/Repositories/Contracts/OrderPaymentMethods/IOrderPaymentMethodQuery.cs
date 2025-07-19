using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.OrderPayments
{
    public interface IOrderPaymentMethodQuery
    {
        Task<List<OrderPaymentMethod>> GetByOrderIdAsync(string orderId);

        Task<List<OrderPaymentMethod>> GetByWalletIdAsync(string walletId);

        Task<List<OrderPaymentMethod>> GetByPaymentMethodIdAsync(string paymentMethodId);

        Task<decimal> GetTotalPaidByOrderAsync(string orderId);

        Task<bool> HasPaymentFromWalletAsync(string orderId);
    }
}
