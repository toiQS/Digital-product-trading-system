using DPTS.Applications.Admin.manage_category.dtos;
using DPTS.Applications.Admin.manage_category.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Admin.manage_category.handlers
{
    public class GetCategoryHandler : IRequestHandler<GetCategoryQuery, ServiceResult<CategoryDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetCategoryHandler> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;

        public GetCategoryHandler(IUserRepository userRepository,
                                  ILogger<GetCategoryHandler> logger,
                                  ICategoryRepository categoryRepository,
                                  IProductRepository productRepository)
        {
            _userRepository = userRepository;
            _logger = logger;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
        }

        public async Task<ServiceResult<CategoryDto>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling get category with condition");
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError($"Not found user with Id:{request.UserId}");
                return ServiceResult<CategoryDto>.Error("Không tìm thấy người dùng");
            }
            if (user.RoleId != "Admin")
            {
                _logger.LogError($"User current {request.UserId} don't have enough access book");
                return ServiceResult<CategoryDto>.Error("Người dùng hiện tại không đủ quyền truy cập");
            }
            var categories = await _categoryRepository.GetsAsync();
            if (!string.IsNullOrEmpty(request.Condition.Text))
            {
                categories = categories.Where(x => x.CategoryId.Contains(request.Condition.Text) || x.CategoryName.Contains(request.Condition.Text)|| x.Description.Contains(request.Condition.Text));
            }
            if (request.Condition.IsFeatured != null)
            {
                categories = categories.Where(x => x.IsFeatured ==  request.Condition.IsFeatured);
            }
            var products = await _productRepository.SearchAsync();
            var result = new CategoryDto()
            {
                Count = categories.ToList().Count,
                IndexDtos = categories.Select( c => new CategoryIndexDto()
                {
                    Name = c.CategoryName,
                    Description = c.Description,
                    Id = c.CategoryId,
                    ImageUrl = c.CategoryIcon,
                    Information = $"{products.Where(x => x.CategoryId == c.CategoryId).Count()}  sản phẩm",
                    DisplayOrder = c.DisplayOrder
                }).ToList()
            };
            if (request.MethodSord == MethodSord.Sord)
                result.IndexDtos= result.IndexDtos.OrderBy(x => x.DisplayOrder).ToList();
            if(request.MethodSord == MethodSord.SordDescending)
                result.IndexDtos = result.IndexDtos.OrderByDescending(x => x.DisplayOrder).ToList();

            if (request.PageNumber > 0 && request.PageSize > 0)
            {
                result.IndexDtos = result.IndexDtos.Skip((request.PageNumber -1)*request.PageSize).Take(request.PageSize).ToList();
            }
            return ServiceResult<CategoryDto>.Success(result);
        }
    }
}
