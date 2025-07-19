using DPTS.Applications.case_buyer.homepage.dtos;
using DPTS.Applications.case_buyer.homepage.models;
using DPTS.Applications.shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repositories.Contracts.Categories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.case_buyer.homepage.handlers
{
    public class GetCategoriesOutstandingIndexHandler : IRequestHandler<GetCategoriesOutStandingIndexQuery, ServiceResult<IEnumerable<CategoriesOutstandingIndexDto>>>
    {
        private readonly ICategoryQuery _categoryQuery;
        private readonly ILogger<GetCategoriesOutstandingIndexHandler> _logger;

        public GetCategoriesOutstandingIndexHandler(ICategoryQuery categoryQuery, ILogger<GetCategoriesOutstandingIndexHandler> logger)
        {
            _categoryQuery = categoryQuery;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<CategoriesOutstandingIndexDto>>> Handle(GetCategoriesOutStandingIndexQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetCategoriesOutstandingItemQuery");
            IEnumerable<Category> categories =await _categoryQuery.GetCategoriesAsync(includeProduct: true, isAvailible: true,sortByCountProductAvalible: true,take:10, cancellationToken);
            if (categories == null || !categories.Any())
            {
                _logger.LogWarning("No categories found.");
                return ServiceResult<IEnumerable<CategoriesOutstandingIndexDto>>.Error("Không tìm thấy doanh mục nào.");
            }
            var categoryDtos = categories.Select(c => new CategoriesOutstandingIndexDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                CategoryImage = c.CategoryIcon,
                CountProductsAvailable = c.Products.Count,
            }).ToList();
            
            return ServiceResult<IEnumerable<CategoriesOutstandingIndexDto>>.Success(categoryDtos);
        }
    }
}
