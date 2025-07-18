using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.UserProfiles
{
    public interface IUserProfileCommand
    {
        Task UpdateBasicInfoAsync(string userId, string? fullName, string? phone, string bio, DateOnly birthDate, string? imageUrl);
        Task UpdateAddressAsync(string userId, Address newAddress);
        Task UpdateProfileImageAsync(string userId, string? imageUrl);
    }
}
