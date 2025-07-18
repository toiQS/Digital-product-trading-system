using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.ComplaintImages
{
    public interface IComplaintImageQuery
    {
        Task<List<ComplaintImage>> GetByComplaintIdAsync(string complaintId);
        Task<ComplaintImage?> GetByIdAsync(string imageId);
    }
}
