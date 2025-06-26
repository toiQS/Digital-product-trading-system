using DPTS.Applications.NoDistinctionOfRoles.categories.Dtos;
using DPTS.Applications.NoDistinctionOfRoles.categories.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.NoDistinctionOfRoles.categories.Handles
{
    public class GetCategoryIndexListHandle : IRequestHandler<GetCategoryIndexListQuery, ServiceResult<IEnumerable<CategoryIndexListDto>>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<GetCategoryIndexListHandle> _logger;

        public GetCategoryIndexListHandle(ICategoryRepository categoryRepository, ILogger<GetCategoryIndexListHandle> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<CategoryIndexListDto>>> Handle(GetCategoryIndexListQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Bắt đầu xử lý GetCategoryIndexList cho request: {Request}", request);
            try
            {
                var categories = await _categoryRepository.GetsAsync(includeProduct: true);
                _logger.LogInformation("Đã lấy được {Count} danh mục từ repository.", categories.Count());

                var result = categories.Select(x => new CategoryIndexListDto
                {
                    CategoryName = x.CategoryName,
                    CategoryId = x.CategoryId,
                    CategoriesCount = x.Products?.Count ?? 0
                }).ToList();

                _logger.LogInformation("Trả về {ResultCount} CategoryIndexListDto.", result.Count);

                return ServiceResult<IEnumerable<CategoryIndexListDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý GetCategoryIndexList.");
                return ServiceResult<IEnumerable<CategoryIndexListDto>>.Error("Không thể lấy danh sách danh mục.");
            }
        }
    }
}
