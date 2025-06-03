using DPTS.Applications.Dtos;
using DPTS.Applications.Shareds;

namespace DPTS.Applications.Interfaces
{
    public interface ICategoryService
    {
        Task<ServiceResult<IEnumerable<CategoryIndexDto>>> GetCategories(int pageSize = 10, int pageNumber = 2);
    }
}
