using Azure.Core;
using DPTS.Applications.Admin.manage_user.dtos;
using DPTS.Applications.Admin.manage_user.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using LinqKit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Admin.manage_user.handlers
{
    public class GetUsersHandler : IRequestHandler<GetUsersQuery, ServiceResult<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IEscrowRepository _escrowRepository;
        private readonly ILogger<GetUsersHandler> _logger;

        public GetUsersHandler(IUserRepository userRepository,
                               IOrderRepository orderRepository,
                               IUserProfileRepository userProfileRepository,
                               IEscrowRepository escrowRepository,
                               ILogger<GetUsersHandler> logger)
        {
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _userProfileRepository = userProfileRepository;
            _escrowRepository = escrowRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling get users with condition");
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("Not found user with Id: {id}", request.UserId);
                return ServiceResult<UserDto>.Error("Không tìm thấy người dùng");
            }
            if (user.RoleId != "Admin")
            {
                _logger.LogInformation("User current don't have enough access books");
                return ServiceResult<UserDto>.Error("Người dùng hiện tại không đủ quyền truy cập");
            }
            var users = await _userRepository.GetAllAsync();
            if (request.Condition.Text != null)
            {
                users = users.Where(x => x.Username.Contains(request.Condition.Text) || x.Email.Contains(request.Condition.Text)).ToList();
            }
            if (request.Condition.IsAvalible != null)
            {
                users = users.Where(x => x.IsActive  == request.Condition.IsAvalible);
            }
            var result = new UserDto();
            users.ForEach(async u =>
            {
                var orders = await _orderRepository.GetByBuyerAsync(request.UserId);
                var userProfile = await _userProfileRepository.GetByUserIdAsync(request.UserId);
                if (userProfile == null)
                {
                    _logger.LogError("Error when get profile user by Id:{d}", u.UserId);

                }
                int totalOrder = 0;
                orders.ForEach(async o =>
                {
                    var countEsrow = await _escrowRepository.GetByOrderIdAsync(o.OrderId);
                    totalOrder += countEsrow.Count();
                });
                var index = new UserIndexDto()
                {
                    Name = userProfile.FullName ?? "Error",
                    Id = u.UserId,
                    Image = userProfile.ImageUrl ??"Error",
                    CountOrder = totalOrder,
                    JoinAt = u.CreatedAt,
                    Status = u.IsActive ?"Hoạt động":"Không hoạt động",
                    Expenditure = orders.Sum(x => x.TotalAmount)
                };
                result.Users.Add(index);
            });
           result.Users = result.Users.OrderByDescending(x => x.JoinAt)
                    .OrderByDescending(x => x.CountOrder)
                    .OrderByDescending(x => x.Expenditure).ToList();
           if (request.PageCount > 0 && request.PageSize > 0)
           {
               result.Users =  result.Users.Skip((request.PageCount - 1) * request.PageSize).Take(request.PageSize).ToList();
           }
            return ServiceResult<UserDto>.Success(result);
        }
    }
}
