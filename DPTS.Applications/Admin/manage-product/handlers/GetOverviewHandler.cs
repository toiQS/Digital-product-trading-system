using DPTS.Applications.Admin.manage_product.dtos;
using DPTS.Applications.Admin.manage_product.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Admin.manage_product.handlers
{
    public class GetOverviewHandler : IRequestHandler<GetOverviewQuery, ServiceResult<OverviewDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IComplaintRepository _complaintRepository;
        private readonly ILogger<GetOverviewHandler> _logger;
        public async Task<ServiceResult<OverviewDto>> Handle(GetOverviewQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling get overview product managemnet");
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("Not found user with Id: {id}", request.UserId);
                return ServiceResult<OverviewDto>.Error("Không tìm thấy người dùng");
            }
            if (user.RoleId != "Admin")
            {
                _logger.LogInformation("User current don't have enough access books");
                return ServiceResult<OverviewDto>.Error("Người dùng hiện tại không đủ quyền truy cập");
            }
            var products = await _productRepository.SearchAsync();
            var result = new OverviewDto()
            {
                IndexDtos = new List<OverviewIndexDto>()
                {
                   new OverviewIndexDto()
                    {
                        Name = "Tổng sản phẩm",
                        Value = products.Count()
                    },
                   new OverviewIndexDto()
                   {
                       Name ="Sản phẩm hoạt động",
                       Value = products.Count(x =>x.Status == Domains.ProductStatus.Available)
                   },
                   new OverviewIndexDto()
                   {
                       Name ="Chờ duyệt",
                       Value = products.Count(x => x.Status == Domains.ProductStatus.Pending)
                   }
                }
            };
            return ServiceResult<OverviewDto>.Success(result);
        }
    }
}
