using DPTS.Applications.Admin.manage_category.dtos;
using DPTS.Applications.Admin.manage_category.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Admin.manage_category.handlers
{
    public class GetOverviewCategoryHandler : IRequestHandler<GetOverviewCategoryQuery, ServiceResult<OverviewCategoryDto>>
    {
        private readonly ILogger<GetOverviewCategoryHandler> _logger;   
        private readonly IUserRepository _userRepository;
        private readonly ICategoryRepository _categoryRepository;

        public GetOverviewCategoryHandler(ILogger<GetOverviewCategoryHandler> logger, IUserRepository userRepository, ICategoryRepository categoryRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<ServiceResult<OverviewCategoryDto>> Handle(GetOverviewCategoryQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling get overview category");
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {   
                _logger.LogError($"Not found user with Id:{request.UserId}");
                return ServiceResult<OverviewCategoryDto>.Error("Không tìm thấy người dùng");
            }
            if (user.RoleId != "Admin")
            {
                _logger.LogError($"User current {request.UserId} don't have enough access book");
                return ServiceResult<OverviewCategoryDto>.Error("Người dùng hiện tại không đủ quyền truy cập");
            }
            var categories = await _categoryRepository.GetsAsync();
            var result = new OverviewCategoryDto()
            {
                IndexDtos = new List<OverviewCategoryIndexDto>()
                {
                    new OverviewCategoryIndexDto()
                    {
                        Name = "Tổng doanh mục",
                        Value = categories.ToList().Count,
                    },
                    new OverviewCategoryIndexDto()
                    {
                        Name = "Doanh mục hoạt động",
                        Value = categories.Where(x => x.IsFeatured).ToList().Count,
                    }
                }
            };
            return ServiceResult<OverviewCategoryDto>.Success(result);
        }
    }
}
