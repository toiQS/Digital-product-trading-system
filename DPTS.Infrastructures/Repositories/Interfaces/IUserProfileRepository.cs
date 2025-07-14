using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Interfaces
{
    public interface IUserProfileRepository
    {
        #region Query

        Task<UserProfile?> GetByUserIdAsync(string userId);
        Task<bool> ExistsAsync(string userId);

        #endregion

        #region Update

        Task UpdateBasicInfoAsync(string userId, string? fullName, string? phone, string bio, DateOnly birthDate, string? imageUrl);
        Task UpdateAddressAsync(string userId, Address newAddress);
        Task UpdateProfileImageAsync(string userId, string? imageUrl);

        #endregion

        #region Add

        Task AddAsync(UserProfile profile);

        #endregion
    }
}
