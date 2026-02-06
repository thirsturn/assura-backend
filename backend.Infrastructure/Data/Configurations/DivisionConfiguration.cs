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
    }
}
