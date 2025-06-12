using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IComplaintImageRepository
    {
        Task AddAsync(ComplaintImage image);
        Task DeleteAsync(string id);
        Task<IEnumerable<ComplaintImage>> GetByComplaintIdAsync(string complaintId);
        Task<ComplaintImage?> GetByIdAsync(string id);
        Task<IEnumerable<ComplaintImage>> GetsAsync(string? complaintId = null, string? keyword = null, bool includeComplaint = false);
    }
}