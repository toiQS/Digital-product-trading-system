using DPTS.Applications.Admin.manage_category.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Admin.manage_category.handlers
{
    public class AddCategoryHandler : IRequestHandler<AddCategoryCommand, ServiceResult<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<AddCategoryHandler> _logger;
        private readonly ILogRepository _logRepository;

        public AddCategoryHandler(IUserRepository userRepository,
                                  ICategoryRepository categoryRepository,
                                  ILogger<AddCategoryHandler> logger,
                                  ILogRepository logRepository)
        {
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
            _logRepository = logRepository;
        }

        public async Task<ServiceResult<string>> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling add new catgory");
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError($"Not found user with Id:{request.UserId}");
                return ServiceResult<string>.Error("Không tìm thấy người dùng");
            }
            if (user.RoleId != "Admin")
            {
                _logger.LogError($"User current {request.UserId} don't have enough access book");
                return ServiceResult<string>.Error("Người dùng hiện tại không đủ quyền truy cập");
            }
            var categories = await _categoryRepository.GetsAsync();
            var exist = categories.Where(x => x.CategoryName.ToLower() == request.AddCategory.CategoryName).Any();
            if (exist)
            {
                _logger.LogError("A category is existed with a same name");
                return ServiceResult<string>.Error("Đã tồn tại một doanh mục với tên tương tự");
            }
            var category = new Category()
            {
                CategoryId = Guid.NewGuid().ToString(),
                CategoryName = request.AddCategory.CategoryName,
                CategoryIcon = request.AddCategory.CategoryIcon,
                Description = request.AddCategory.CategoryDescription,
                DisplayOrder = categories.Count() +1,
                CreateAt = DateTime.UtcNow,
                IsFeatured = true,
                UpdatedAt = DateTime.UtcNow,
            };
            var log = new Log()
            {
                Action="Thêm một doanh mục mới",
                Description = $"doanh mục {category.CategoryId} vừa được thêm bởi {request.UserId}",
                CreatedAt = DateTime.UtcNow,
                LogId = Guid.NewGuid().ToString()
            };
            try
            {
                await _categoryRepository.AddAsync(category);
                await _logRepository.AddAsync(log);
                return ServiceResult<string>.Success("Thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Error when add new cateogry");
                return ServiceResult<string>.Error("Cập nhật không thành công");
            }
        }
    }
}
