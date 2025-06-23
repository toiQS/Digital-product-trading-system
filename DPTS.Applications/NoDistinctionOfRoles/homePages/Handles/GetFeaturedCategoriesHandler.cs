using DPTS.Applications.NoDistinctionOfRoles.homePages.Dtos;
using DPTS.Applications.NoDistinctionOfRoles.homePages.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.NoDistinctionOfRoles.homePages.Handles
{
    public class GetFeaturedCategoriesHandler : IRequestHandler<GetFeaturedCategoriesQuery, ServiceResult<IEnumerable<FeaturedCategoryDto>>>
    {
        private readonly ILogger<GetFeaturedCategoriesHandler> _logger;
        private readonly ICategoryRepository _categoryRepository;

        public GetFeaturedCategoriesHandler(
            ILogger<GetFeaturedCategoriesHandler> logger,
            ICategoryRepository categoryRepository)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
        }

        public async Task<ServiceResult<IEnumerable<FeaturedCategoryDto>>> Handle(GetFeaturedCategoriesQuery query, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Bắt đầu xử lý yêu cầu lấy danh sách danh mục nổi bật.");

            try
            {
                var categories = await _categoryRepository.GetsAsync(includeProduct: true);

                if (categories == null || !categories.Any())
                {
                    _logger.LogWarning("Không tìm thấy danh mục nào.");
                    return ServiceResult<IEnumerable<FeaturedCategoryDto>>.Success([]);
                }

                var result = categories.Select(category => new FeaturedCategoryDto
                {
                    CategoryId = category.CategoryId,
                    Name = category.CategoryName,
                    CategoryIcon = category.CategoryIcon,
                    ProductCount = category.Products?.Count ?? 0
                }).ToList();

                _logger.LogInformation("Hoàn tất xử lý danh mục nổi bật. Tổng số: {Count}", result.Count);

                return ServiceResult<IEnumerable<FeaturedCategoryDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý GetFeaturedCategoriesHandler.");
                return ServiceResult<IEnumerable<FeaturedCategoryDto>>.Error("Đã xảy ra lỗi khi lấy danh mục nổi bật.");
            }
        }
    }
}
