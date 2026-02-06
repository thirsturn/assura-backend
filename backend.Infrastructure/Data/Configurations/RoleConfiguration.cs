using backend.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.roleID);
        builder.Property(r => r.roleID).HasMaxLength(36);
        builder.Property(r => r.roleName).HasMaxLength(50).IsRequired();

        builder.HasIndex(r => r.roleName).IsUnique();

        // Seed default roles
        builder.HasData(
            new Role { roleID = "1", roleName = "Admin" },
            new Role { roleID = "2", roleName = "StoreKeeper" },
            new Role { roleID = "3", roleName = "Procurement" },
            new Role { roleID = "4", roleName = "Superintend" },
            new Role { roleID = "5", roleName = "HR" },
            new Role { roleID = "6", roleName = "Auditor" },
            new Role { roleID = "7", roleName = "Accountant" }
        );
    }
}
