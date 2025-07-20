using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Escrows
{
    public interface IEscrowQuery
    {
        Task<IEnumerable<Escrow>> GetDoneEscrowsAsync(CancellationToken cancellationToken);
    }
}
