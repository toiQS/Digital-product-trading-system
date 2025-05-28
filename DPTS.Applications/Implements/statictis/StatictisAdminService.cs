using DPTS.Applications.Dtos.statisticals;
using DPTS.Applications.Interfaces;
using DPTS.Applications.Shareds;
using DPTS.Applications.Shareds.Interfaces;
using DPTS.Domains;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Implements.statictis
{
    public class StatictisAdminService : IStatictisAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StatictisAdminService> _logger;

        public StatictisAdminService(IUnitOfWork unitOfWork, ILogger<StatictisAdminService> logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// Truy xuất số lượng và phần trăm người dùng theo vai trò.
        /// </summary>
        private async Task<ServiceResult<UserFractionsByRoleModel>> GetMembersByRoleAsync(string roleKey, bool isById = false)
        {
            if (string.IsNullOrWhiteSpace(roleKey))
                return ServiceResult<UserFractionsByRoleModel>.Error("Giá trị truyền vào không hợp lệ.");

            try
            {
                _logger.LogInformation("Đang lấy người dùng theo vai trò: {RoleKey}", roleKey);

                string roleId = roleKey;

                if (!isById)
                {
                    var role = await _unitOfWork.Repository<Role>().GetOneAsync("RoleName", roleKey);
                    if (role == null) return ServiceResult<UserFractionsByRoleModel>.Success(null!);
                    roleId = role.RoleId;
                }

                var users = await _unitOfWork.Repository<User>().GetManyAsync("RoleId", roleId);
                var allUsers = await _unitOfWork.Repository<User>().GetAllAsync();

                int quantity = users.Count();
                int total = allUsers.Count();
                int percentage = total == 0 ? 0 : quantity * 100 / total;

                return ServiceResult<UserFractionsByRoleModel>.Success(new UserFractionsByRoleModel
                {
                    Quantity = quantity,
                    RoleName = roleKey,
                    Percentage = percentage
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy người dùng theo vai trò: {RoleKey}", roleKey);
                return ServiceResult<UserFractionsByRoleModel>.Error("Đã xảy ra lỗi khi lấy dữ liệu người dùng.");
            }
        }

        // Shortcut methods cho các vai trò cụ thể
        public Task<ServiceResult<UserFractionsByRoleModel>> GetMembersInAdminRole() => GetMembersByRoleAsync("Admin");
        public Task<ServiceResult<UserFractionsByRoleModel>> GetMembersInBuyerRole() => GetMembersByRoleAsync("Buyer");
        public Task<ServiceResult<UserFractionsByRoleModel>> GetMembersInSellerRole() => GetMembersByRoleAsync("Seller");

        /// <summary>
        /// Lấy thông tin tổng hợp số lượng người dùng theo vai trò chính.
        /// </summary>
        public async Task<ServiceResult<IEnumerable<UserFractionsByRoleModel>>> GetMembersAsync()
        {
            var resultList = new List<UserFractionsByRoleModel>();
            var roles = new[] { GetMembersInAdminRole(), GetMembersInSellerRole(), GetMembersInBuyerRole() };

            foreach (var roleTask in roles)
            {
                var result = await roleTask;
                if (result.Data != null) resultList.Add(result.Data);
            }

            return ServiceResult<IEnumerable<UserFractionsByRoleModel>>.Success(resultList);
        }
    }
}
