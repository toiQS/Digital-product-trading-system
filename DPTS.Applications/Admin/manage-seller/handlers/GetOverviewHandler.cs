using DPTS.Applications.Admin.manage_seller.dtos;
using DPTS.Applications.Admin.manage_seller.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Admin.manage_seller.handlers
{
    public class GetOverviewHandler : IRequestHandler<GetOverviewQuery, ServiceResult<OverviewDto>>
    {
        private readonly ILogger<GetOverviewHandler> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IStoreRepository _storeRepository;

        public GetOverviewHandler(ILogger<GetOverviewHandler> logger, IUserRepository userRepository, IStoreRepository storeRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _storeRepository = storeRepository;
        }

        public async Task<ServiceResult<OverviewDto>> Handle(GetOverviewQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling get overview manage seller");
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("Not found with Id: {d}", request.UserId);
                return ServiceResult<OverviewDto>.Error("Không tìm thấy người dùng");
            }
            if (user.RoleId != "Admin")
            {
                _logger.LogError("User current don't have enough access books");
                return ServiceResult<OverviewDto>.Error("Người dùng hiện tại không đủ quyền truy cập"); 
            }
            var stores = await _storeRepository.GetAllAsync();
            var result = new OverviewDto();
            result.Indexs.Add(new()
            {
                Name = "Tổng người bán",
                Value = stores.Count()
            });

            var storeAvalible = stores.Count(x =>x.Status == Domains.StoreStatus.Active);
            result.Indexs.Add(new OverviewIndexDto()
            {
                Name = "Người bán hoạt động",
                Value = storeAvalible
            });
            var (startThisMonth, endThisMonth) = SharedHandle.GetMonthRange(0);
            var countNewStore = stores.Where(x => x.CreateAt >= startThisMonth && x.CreateAt <= endThisMonth).Count();
            result.Indexs.Add(new()
            {
                Name = "Người bán mới",
                Value = countNewStore
            });

            var storeBlock = stores.Count(x => x.Status == Domains.StoreStatus.Inactive);
            result.Indexs.Add(new OverviewIndexDto()
            {
                Name = "Người bán bị chặn",
                Value = storeBlock
            });
            return ServiceResult<OverviewDto>.Success(result);
        }
    }
}
