using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.OrderPayments
{
    public interface IOrderPaymentMethodCommand
    {
        Task AddAsync(OrderPaymentMethod payment);

        Task AddRangeAsync(List<OrderPaymentMethod> payments);
        Task<bool> IsFullyPaidAsync(string orderId, decimal orderTotal);

    }
}
