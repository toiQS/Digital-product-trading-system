using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Escrows
{
    public interface IEscrowCommand
    {
        Task AddAsync(Escrow escrow);
        Task UpdateAsync(Escrow escrow);
        Task RemoveAsync(Escrow escrow);
    }
}
