using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.ProductAdjustments
{
    public interface IProductAdjustmentQuery
    {
        Task<IEnumerable<ProductAdjustment>> GetsByProductIdAsync(string productId, CancellationToken cancellationToken);
    }

}
