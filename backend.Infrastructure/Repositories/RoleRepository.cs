using backend.Core;
using backend.Core.Interfaces;
using backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Role data access
/// </summary>
public class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext _context;

    public RoleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByIdAsync(string roleId)
    {
        return await _context.Roles.FindAsync(roleId);
    }

    public async Task<Role?> GetByNameAsync(string roleName)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.roleName == roleName);
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task<Role> CreateAsync(Role role)
    {
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task<bool> ExistsAsync(string roleId)
    {
        return await _context.Roles.AnyAsync(r => r.roleID == roleId);
    }
}
