using backend.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.userID);
        builder.Property(u => u.userID).HasMaxLength(36);

        builder.Property(u => u.firstname).HasMaxLength(100).IsRequired();
        builder.Property(u => u.lastname).HasMaxLength(100).IsRequired();
        builder.Property(u => u.email).HasMaxLength(255).IsRequired();
        builder.Property(u => u.telephone).HasMaxLength(20);
        builder.Property(u => u.username).HasMaxLength(100).IsRequired();
        builder.Property(u => u.passwordHash).HasMaxLength(255).IsRequired();
        builder.Property(u => u.refreshToken).HasMaxLength(500);
        builder.Property(u => u.FcmToken).HasMaxLength(500);
        builder.Property(u => u.SocketId).HasMaxLength(100);

        builder.HasIndex(u => u.username).IsUnique();
        builder.HasIndex(u => u.email).IsUnique();

        // Relationship with Division
        builder.HasOne(u => u.Division)
            .WithMany(d => d.Users)
            .HasForeignKey(u => u.divisionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
