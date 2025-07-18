using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Complaints
{
    public interface IComplaintQuery
    {
        Task<Complaint?> GetByIdAsync(string complaintId, bool includeImages = false);
        Task<List<Complaint>> GetByUserIdAsync(string userId);
        Task<List<Complaint>> GetByOrderItemAsync(string orderItemId);
        Task<List<Complaint>> GetPendingAsync();
        Task<int> CountByUserIdAsync(string userId);
    }
}
