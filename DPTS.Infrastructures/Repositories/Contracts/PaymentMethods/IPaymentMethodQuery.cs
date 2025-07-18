using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.PaymentMethods
{
    public interface IPaymentMethodQuery
    {
        Task<List<PaymentMethod>> GetByUserIdAsync(string userId);

        Task<PaymentMethod?> GetDefaultAsync(string userId);

        Task<bool> ExistsAsync(string userId, PaymentProvider provider);

        Task<PaymentMethod?> GetByIdAsync(string paymentMethodId);
    }
}
