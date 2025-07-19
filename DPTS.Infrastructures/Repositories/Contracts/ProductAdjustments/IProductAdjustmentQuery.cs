using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.ProductAdjustments
{
    public interface IProductAdjustmentQuery
    {
        Task<IEnumerable<ProductAdjustment>> GetByProductIdAsync(string productId, CancellationToken cancellationToken);
    }

}
