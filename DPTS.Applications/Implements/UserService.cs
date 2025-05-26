using DPTS.Applications.Dtos.users;
using DPTS.Applications.Shareds;
using DPTS.Applications.Shareds.Interfaces;
using DPTS.Domains;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Implements
{
    public class UserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        
        /// <summary>
        /// Retrieves user detail information by user ID.
        /// </summary>
        public async Task<ServiceResult<UserDetailModel>> GetUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return ServiceResult<UserDetailModel>.Error("User ID is required.");

            try
            {
                _logger.LogInformation("Retrieving user with ID: {UserId}", userId);

                var user = await _unitOfWork.Repository<User>().GetOneAsync("UserId", userId);
                if (user == null)
                    return ServiceResult<UserDetailModel>.Error("User not found.");

                var role = await _unitOfWork.Repository<Role>().GetOneAsync("RoleId", user.RoleId);
                if (role == null)
                    return ServiceResult<UserDetailModel>.Error("Role not found.");

                var response = new UserDetailModel
                {
                    UserId = userId,
                    NumberPhone = user.Phone ?? "",
                    FullName = user.FullName ?? "",
                    RoleName = role.RoleName,
                    UserName = user.Username,
                    Address = user.Address ?? "",
                    Email = user.Email
                };

                return ServiceResult<UserDetailModel>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving user details.");
                return ServiceResult<UserDetailModel>.Error("Failed to retrieve user details.");
            }
        }

        /// <summary>
        /// Updates the role of a given user by either role name or ID.
        /// </summary>
        public async Task<ServiceResult<UserDetailModel>> PatchRoleOfUser(string userId, string roleKey, bool isId = false)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(roleKey))
                return ServiceResult<UserDetailModel>.Error("User ID and Role are required.");

            try
            {
                _logger.LogInformation("Updating role for user ID: {UserId}", userId);

                string roleId = roleKey;

                if (!isId)
                {
                    var role = await _unitOfWork.Repository<Role>().GetOneAsync("RoleName", roleKey);
                    if (role == null)
                        return ServiceResult<UserDetailModel>.Error("Role not found.");

                    roleId = role.RoleId;
                }

                var user = await _unitOfWork.Repository<User>().GetOneAsync("UserId", userId);
                if (user == null)
                    return ServiceResult<UserDetailModel>.Error("User not found.");

                var patchData = new Dictionary<string, object>
                {
                    { nameof(User.RoleId), roleId }
                };

                await _unitOfWork.ExecuteTransactionAsync(() =>
                    _unitOfWork.Repository<User>().PatchAsync(user, patchData));

                return await GetUser(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user role.");
                return ServiceResult<UserDetailModel>.Error("Failed to update user role.");
            }
        }
    }
}
