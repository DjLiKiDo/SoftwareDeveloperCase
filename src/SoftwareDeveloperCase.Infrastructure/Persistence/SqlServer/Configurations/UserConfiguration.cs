using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoftwareDeveloperCase.Domain.Entities;
using SoftwareDeveloperCase.Domain.ValueObjects;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Configurations;

/// <summary>
/// Entity Framework configuration for the User entity
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table name
        builder.ToTable("User");

        // Primary key
        builder.HasKey(u => u.Id);

        // Properties
        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Password)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Configure Email value object
        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => new Email(value))
            .IsRequired()
            .HasMaxLength(255);

        // Indexes
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_User_Email");

        builder.HasIndex(u => u.Name)
            .HasDatabaseName("IX_User_Name");

        builder.HasIndex(u => u.IsActive)
            .HasDatabaseName("IX_User_IsActive");
    }
}
