namespace backend.Core.Interfaces;

/// <summary>
/// Repository interface for Role data access
/// </summary>
public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(string roleId);
    Task<Role?> GetByNameAsync(string roleName);
    Task<IEnumerable<Role>> GetAllAsync();
    Task<Role> CreateAsync(Role role);
    Task<bool> ExistsAsync(string roleId);
}
