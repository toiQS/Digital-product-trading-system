using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.ProductImages
{
    public interface IProductImageCommand
    {
        Task RemoveByProductIdAsync(string productId);
        Task RemoveByIdAsync(string productImageId);
        Task AddAsync(ProductImage productImage);
        Task UpdateAsync(ProductImage productImage);
    }
}
