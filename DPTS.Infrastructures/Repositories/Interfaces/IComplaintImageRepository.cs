using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Interfaces
{
    public interface IComplaintImageRepository
    {
        #region Query

        Task<List<ComplaintImage>> GetByComplaintIdAsync(string complaintId);

        Task<ComplaintImage?> GetByIdAsync(string imageId);

        #endregion

        #region Crud

        Task AddAsync(ComplaintImage image);

        Task RemoveAsync(ComplaintImage image);

        Task RemoveByComplaintIdAsync(string complaintId);

        #endregion
    }
}
