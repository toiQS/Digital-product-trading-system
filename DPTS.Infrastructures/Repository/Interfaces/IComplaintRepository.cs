using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IComplaintRepository
    {
        Task AddAsync(Complaint complaint);
        Task DeleteAsync(string id);
        Task<Complaint?> GetByIdAsync(string id);
        Task<IEnumerable<Complaint>> GetByOrderIdAsync(string orderId);
        Task<IEnumerable<Complaint>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Complaint>> GetsAsync(string? userId = null, string? productId = null, string? orderId = null, string? text = null, ComplaintStatus? status = null, bool includeUser = false, bool includeOrder = false, bool includeProduct = false, bool includeImages = false);
        Task UpdateAsync(Complaint complaint);
    }
}