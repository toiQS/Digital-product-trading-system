using DPTS.Applications.Admin.overview.dtos;
using DPTS.Applications.Admin.overview.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Admin.overview.handlers
{
    public class GetOverviewHandler : IRequestHandler<GetOverviewQuery, ServiceResult<IEnumerable<OverviewIndexDto>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IEscrowRepository _escrowRepository;
        private readonly ILogger<GetOverviewHandler> _logger;

        public GetOverviewHandler(IUserRepository userRepository, IProductRepository productRepository, IEscrowRepository escrowRepository, ILogger<GetOverviewHandler> logger)
        {
            _userRepository = userRepository;
            _productRepository = productRepository;
            _escrowRepository = escrowRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<OverviewIndexDto>>> Handle(GetOverviewQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling overview of admin");
            var checkRole = await _userRepository.GetByIdAsync(request.UserId);
            if (checkRole == null)
            {
                _logger.LogError($"Not found user with Id: {request.UserId}");
                return ServiceResult<IEnumerable<OverviewIndexDto>>.Error("Not found user");
            }
            if(checkRole.RoleId != "Admin")
            {
                _logger.LogError($"Current user {request.UserId} don't have enough access books ");
                return ServiceResult<IEnumerable<OverviewIndexDto>>.Error("Người dùng hiện tại không đủ quyển truy cập.");
            }
            
            var result = new List<OverviewIndexDto>();

            // total user without admin
            var totalUserWithoutAdmin = (await _userRepository.GetAllAsync()).Count(x => x.RoleId != "Admin");
            result.Add(new OverviewIndexDto()
            {
                Name = "Tổng người dùng",
                Value = totalUserWithoutAdmin,
            });

            // total seller
            var totalSeller = (await _userRepository.GetAllAsync()).Count(x => x.RoleId == "Seller");
            result.Add(new()
            {
                Name = "Số người bán",
                Value = totalSeller,
            });

            // total products
            var totalproduct = (await _productRepository.SearchAsync()).Count();
            result.Add(new()
            {
                Name = "Số sản phẩm",
                Value = totalproduct,
            });

            // revenue in month
            var (startThisMonth, endThisMonth) = SharedHandle.GetMonthRange(0);
            var escrowDones = (await _escrowRepository.GetAllAsync()).Where(x => x.CreatedAt >= startThisMonth && x.CreatedAt < endThisMonth && x.Status == Domains.EscrowStatus.Done);
            var revenue= escrowDones.Sum(x => x.ActualAmount);
            result.Add(new()
            {
                Name = "Doanh thu tháng",
                Value = revenue,
            });
            return ServiceResult<IEnumerable<OverviewIndexDto>>.Success(result);
        }
    }
}
