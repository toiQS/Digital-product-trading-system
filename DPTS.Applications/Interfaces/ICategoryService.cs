using DPTS.Applications.Dtos.categories;
using DPTS.Applications.Shareds;

namespace DPTS.Applications.Interfaces
{
    public interface ICategoryService
    {
        Task<ServiceResult<IEnumerable<IndexCategoryModel>>> GetCategoriesAsync();
    }
}
