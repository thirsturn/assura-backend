using backend.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Data.Configurations;

public class DivisionConfiguration : IEntityTypeConfiguration<Division>
{
    public void Configure(EntityTypeBuilder<Division> builder)
    {
        builder.ToTable("divisions");

        builder.HasKey(d => d.divisionID);
        builder.Property(d => d.divisionID).HasMaxLength(36);
        builder.Property(d => d.divisionName).HasMaxLength(100).IsRequired();

        builder.HasIndex(d => d.divisionName).IsUnique();

        // Seed default divisions
        builder.HasData(
            new Division { divisionID = "1", divisionName = "Stores" },
            new Division { divisionID = "2", divisionName = "Procurement" },
            new Division { divisionID = "3", divisionName = "Superintend" },
            new Division { divisionID = "4", divisionName = "HR" },
            new Division { divisionID = "5", divisionName = "Auditing" },
            new Division { divisionID = "6", divisionName = "Accounts" },
            new Division { divisionID = "7", divisionName = "Admin" }
        );
    }
}
