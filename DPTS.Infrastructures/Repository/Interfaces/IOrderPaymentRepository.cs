using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IOrderPaymentRepository
    {
        Task AddAsync(OrderPayment payment);
        Task UpdateAsync(OrderPayment payment);
        Task DeleteAsync(string orderPaymentId);
        Task<OrderPayment?> GetByIdAsync(string orderPaymentId);
        Task<IEnumerable<OrderPayment>> GetsAsync(
            string? orderId = null,
            PaymentSourceType? sourceType = null,
            DateTime? from = null,
            DateTime? to = null,
            decimal? minAmount = null,
            decimal? maxAmount = null
        );
    }

}