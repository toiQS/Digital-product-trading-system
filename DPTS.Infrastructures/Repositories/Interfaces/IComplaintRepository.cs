using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Interfaces
{
    public interface IComplaintRepository
    {
        #region Query

        Task<Complaint?> GetByIdAsync(string complaintId, bool includeImages = false);

        Task<List<Complaint>> GetByUserIdAsync(string userId);

        Task<List<Complaint>> GetByOrderItemAsync(string orderItemId);

        Task<List<Complaint>> GetPendingAsync();

        Task<int> CountByUserIdAsync(string userId);

        #endregion

        #region Crud

        Task AddAsync(Complaint complaint);
        Task UpdateAsync(Complaint complaint);
        Task RemoveAsync(Complaint complaint);

        #endregion
    }
}
