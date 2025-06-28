using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IPaymentMethodRepository
    {
        Task AddAsync(PaymentMethod method);
        Task UpdateAsync(PaymentMethod method);
        Task DeleteAsync(string methodId);
        Task<PaymentMethod?> GetByIdAsync(string methodId);
        Task<IEnumerable<PaymentMethod>> GetsAsync(
            string? userId = null,
            PaymentProvider? provider = null,
            bool? isVerified = null,
            bool? isDefault = null
        );
    }
}