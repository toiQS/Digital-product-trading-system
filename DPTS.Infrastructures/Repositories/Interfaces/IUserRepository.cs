using DPTS.Domains;

public interface IUserRepository
{
    #region Query

    Task<User?> GetByIdAsync(string userId);
    Task<User?> GetByEmailAsync(string email);
    Task<List<User>> GetUsersByRoleAsync(string roleId);
    Task<bool> IsEmailTakenAsync(string email);
    Task<bool> IsUsernameTakenAsync(string username);

    #endregion

    #region Crud

    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeactivateUserAsync(string userId);
    Task DeleteAsync(User user);

    #endregion
}
