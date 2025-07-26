using DPTS.Applications.Admin.manage_category.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Admin.manage_category.handlers
{
    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, ServiceResult<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<UpdateCategoryHandler> _logger;
        private readonly ILogRepository _logRepository;

        public UpdateCategoryHandler(IUserRepository userRepository,
                                     ICategoryRepository categoryRepository,
                                     ILogger<UpdateCategoryHandler> logger,
                                     ILogRepository logRepository)
        {
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
            _logRepository = logRepository;
        }

        public async Task<ServiceResult<string>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
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
            var exist = categories.Where(x => x.CategoryName.ToLower() == request.UpdateCategory.Name).Any();
            if (exist)
            {
                _logger.LogError("A category is existed with a same name");
                return ServiceResult<string>.Error("Đã tồn tại một doanh mục với tên tương tự");
            }
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
            {
                _logger.LogError("Not found category with Id:{id}",request.CategoryId);
                return ServiceResult<string>.Error("Không tìm thấy doanh mục");
            }
            var log = new Log()
            {
                Action = "Cập nhật một doanh mục mới",
                Description = $"doanh mục {category.CategoryId} vừa được cập nhật bởi {request.UserId}",
                CreatedAt = DateTime.UtcNow,
                LogId = Guid.NewGuid().ToString()
            };
            try
            {
                category.CategoryName = request.UpdateCategory.Name ?? category.CategoryName;
                category.Description = request.UpdateCategory.Description ?? category.Description;
                category.CategoryIcon = request.UpdateCategory.Icon ?? category.CategoryIcon;
                category.IsFeatured = request.UpdateCategory.IsFeatured ?? category.IsFeatured;
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
