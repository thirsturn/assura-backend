namespace backend.Core;

public class Role
{
    public string roleID { get; set; } = string.Empty;
    public string roleName { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}