using DPTS.Applications.Buyer.Dtos.product;
using DPTS.Applications.Buyer.Queries.product;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles.product
{
    public class GetCategoryHandler : IRequestHandler<GetCategoryQuery, ServiceResult<IEnumerable<CategoryIndexDto>>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<GetCategoryHandler> _logger;

        public GetCategoryHandler(ICategoryRepository categoryRepository, IProductRepository productRepository, ILogger<GetCategoryHandler> logger)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<CategoryIndexDto>>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling get information of category");
            var result = new List<CategoryIndexDto>();
            IEnumerable<Product> products = (await _productRepository.SearchAsync()).Where(x => x.Status == ProductStatus.Available);
            
            var categories = await _categoryRepository.GetsAsync(includeProduct: true, includeAdjustmentRule: true);

            // Gán thông tin danh mục vào kết quả trả về
            result = categories.Select(c => new CategoryIndexDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                CategoriesCount = products.Count(x => x.CategoryId == c.CategoryId)
            }).ToList();
            return ServiceResult<IEnumerable<CategoryIndexDto>>.Success(result);
        }
    }
}
