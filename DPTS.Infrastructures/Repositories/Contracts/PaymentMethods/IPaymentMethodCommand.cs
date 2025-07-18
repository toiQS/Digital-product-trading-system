using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.PaymentMethods
{
    public interface IPaymentMethodCommand
    {
        Task AddAsync(PaymentMethod paymentMethod);
        Task UpdateAsync(PaymentMethod paymentMethod);
        Task RemoveAsync(PaymentMethod paymentMethod);
    }
}
