using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IPaymentMethodRepository
    {
        Task AddAsync(PaymentMethod method);
        Task DeleteAsync(string methodId);
        Task<PaymentMethod?> GetByIdAsync(string methodId);
        Task<IEnumerable<PaymentMethod>> GetByUserIdAsync(string userId);
        Task<PaymentMethod?> GetDefaultByUserIdAsync(string userId);
        Task<IEnumerable<PaymentMethod>> GetVerifiedByUserIdAsync(string userId);
        Task<bool> IsProviderLinkedAsync(string userId, PaymentProvider provider);
        Task UpdateAsync(PaymentMethod method);
    }
}