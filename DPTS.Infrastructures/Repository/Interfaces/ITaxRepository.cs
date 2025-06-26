using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface ITaxRepository
    {
        Task<IEnumerable<Tax>> GetsAsync(string? text = null, bool includeCategory = false);
        Task<Tax?> GetByIdAsync(string id, bool includeCategory = false);
        Task AddAsync(Tax tax);
        Task UpdateAsync(Tax tax);
        Task DeleteAsync(string id);
    }
}
