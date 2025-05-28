using DPTS.Applications.Dtos.categories;
using DPTS.Applications.Shareds;
using DPTS.Applications.Shareds.Interfaces;
using DPTS.Domains;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DPTS.Applications.Implements
{
    public class CategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoryService> _logger;
        public CategoryService(IUnitOfWork unitOfWork, ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<ServiceResult<IEnumerable<IndexCategoryModel>>> GetCategoriesAsync()
        {
            try
            {
                _logger.LogInformation("");
                var categories = await _unitOfWork.Repository<Category>().GetAllAsync();
                if(!categories.Any()) return ServiceResult<IEnumerable<IndexCategoryModel>>.Success(null!);
                var indexcategories = new List<IndexCategoryModel>();
                foreach (var category in categories)
                {
                    var products = await _unitOfWork.Repository<Product>().GetManyAsync(nameof(Product.CategoryId), category.CategoryId);
                    var indexCategoryModel = new IndexCategoryModel()
                    {
                        CategoryName = category.Name,
                        CategoryId = category.CategoryId,
                        ProductQuantity = $"{products.Count()} sản phẩm"
                    };
                    indexcategories.Add(indexCategoryModel);
                }
                return ServiceResult<IEnumerable<IndexCategoryModel>>.Success(indexcategories);
            }
            catch (Exception ex)
            {
                _logger.LogError("");
                return ServiceResult<IEnumerable<IndexCategoryModel>>.Error("");
            }
        }
    }
}
