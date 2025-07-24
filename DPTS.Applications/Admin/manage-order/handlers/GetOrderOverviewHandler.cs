using DPTS.Applications.Admin.manage_order.dtos;
using DPTS.Applications.Admin.manage_order.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Admin.manage_order.handlers
{
    public class GetOrderOverviewHandler : IRequestHandler<GetOrderOverviewQuery, ServiceResult<OrderOverviewDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetOrderOverviewHandler> _logger;
        private readonly IEscrowRepository _ecrowRepository;

        public GetOrderOverviewHandler(IUserRepository userRepository, ILogger<GetOrderOverviewHandler> logger, IEscrowRepository ecrowRepository)
        {
            _userRepository = userRepository;
            _logger = logger;
            _ecrowRepository = ecrowRepository;
        }

        public async Task<ServiceResult<OrderOverviewDto>> Handle(GetOrderOverviewQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling get order overview");
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("Not found user with Id: {id}", request.UserId);
                return ServiceResult<OrderOverviewDto>.Error("Không tìm thấy người dùng");
            }
            if (user.RoleId != "Admin")
            {
                _logger.LogInformation("User current don't have enough access books");
                return ServiceResult<OrderOverviewDto>.Error("Người dùng hiện tại không đủ quyền truy cập");
            }
            var escrows = await _ecrowRepository.GetAllAsync();
            var newEscrows = escrows.Where(x => x.CreatedAt >= DateTime.Today && x.CreatedAt < DateTime.Today.AddDays(1)).ToList();
            var result = new OrderOverviewDto()
            {
                OrderOverviewIndexDtos = new List<OrderOverviewIndexDto>()
                {
                    new OrderOverviewIndexDto()
                    {
                        Name = "Tổng đơn hàng",
                        Value = escrows.Count(),
                        Information = $"{newEscrows.Count()} đơn hàng mới"
                    },
                    new OrderOverviewIndexDto()
                    {
                        Name = "Chờ xử lý",
                        Value = escrows.Count(x => x.Status == Domains.EscrowStatus.Pending),
                    },
                    new OrderOverviewIndexDto()
                    {
                        Name = "Hoàn thành",
                        Value = escrows.Count(x=> x.Status == Domains.EscrowStatus.Done),
                    },
                    new OrderOverviewIndexDto()
                    {
                        Name = "Đã hủy",
                        Value = escrows.Count(x => x.Status == Domains.EscrowStatus.Canceled)
                    }
                }
            };
            return ServiceResult<OrderOverviewDto>  .Success(result);
        }
    }
}
