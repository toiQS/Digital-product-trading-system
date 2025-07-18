using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Complaints
{
    public interface IComplaintCommand
    {
        Task AddAsync(Complaint complaint);
        Task UpdateAsync(Complaint complaint);
        Task RemoveAsync(Complaint complaint);
    }
}
