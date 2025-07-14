using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Interfaces
{
    public interface IPaymentMethodRepository
    {
        #region Query

        Task<List<PaymentMethod>> GetByUserIdAsync(string userId);

        Task<PaymentMethod?> GetDefaultAsync(string userId);

        Task<bool> ExistsAsync(string userId, PaymentProvider provider);

        Task<PaymentMethod?> GetByIdAsync(string paymentMethodId);

        #endregion

        #region Crud

        Task AddAsync(PaymentMethod method);
        Task UpdateAsync(PaymentMethod method);
        Task RemoveAsync(PaymentMethod method);

        #endregion
    }
}
