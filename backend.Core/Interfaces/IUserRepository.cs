namespace backend.Core.Interfaces;

/// <summary>
/// Repository interface for User data access
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(string userId);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(string userId);
    Task<bool> ExistsAsync(string userId);
    Task<bool> UsernameExistsAsync(string username);
}
