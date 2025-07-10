using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IEscrowProcessRepository
    {
        Task AddAsync(EscrowProcess process);
        Task<IEnumerable<EscrowProcess>> GetByEscrowIdAsync(string escrowId);
        Task<EscrowProcess?> GetByIdAsync(string processId);
    }
}