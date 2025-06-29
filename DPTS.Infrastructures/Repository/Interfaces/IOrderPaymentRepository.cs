using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IOrderPaymentRepository
    {
        Task AddAsync(OrderPayment payment);
        Task AddRangeAsync(IEnumerable<OrderPayment> payments);
        Task<IEnumerable<OrderPayment>> GetByOrderIdAsync(string orderId);
        Task<IEnumerable<OrderPayment>> GetByPaymentMethodIdAsync(string paymentMethodId, DateTime? from = null, DateTime? to = null);
        Task<IEnumerable<OrderPayment>> GetByWalletIdAsync(string walletId, DateTime? from = null, DateTime? to = null);
        Task<decimal> GetTotalPaidByOrderIdAsync(string orderId);
    }
}