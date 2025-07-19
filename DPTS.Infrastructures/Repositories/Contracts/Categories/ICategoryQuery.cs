using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Categories
{
    public interface ICategoryQuery
    {
        Task<IEnumerable<Category>> GetCategoriesAsync(bool includeProduct, bool isAvailible, bool sortByCountProductAvalible, int take, CancellationToken cancellationToken);
    }

}
