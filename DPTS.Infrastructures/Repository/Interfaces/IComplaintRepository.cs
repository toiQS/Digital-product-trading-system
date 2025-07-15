using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IComplaintRepository
    {
        Task AddAsync(Complaint complaint);
        Task DeleteAsync(string complaintId);
        Task<bool> ExistsAsync(string complaintId);
        Task<IEnumerable<Complaint>> GetAllAsync(ComplaintStatus? status = null, string? userId = null, bool includeProduct = false, bool includeUser = false, bool includeEscrow = false, bool includeImages = false);
        Task<Complaint?> GetByIdAsync(string complaintId, bool includeProduct = false, bool includeUser = false, bool includeEscrow = false, bool includeImages = false);
        Task<IEnumerable<Complaint>> GetByEscrowIdAsync(string escrowId);
        Task<IEnumerable<Complaint>> GetByProductIdAsync(string productId);
        
        Task UpdateAsync(Complaint complaint);
    }
}