using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.ComplaintImages
{
    public interface IComplaintImageCommand
    {
        Task RemoveByComplaintIdAsync(string complaintId);
        Task AddAsync(ComplaintImage complaintImage);
        Task UpdateAsync(ComplaintImage complaintImage);
        Task CheckExistedAsync(string complaintImageId);
    }
}
