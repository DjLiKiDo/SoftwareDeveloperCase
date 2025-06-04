using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoftwareDeveloperCase.Domain.Entities.Identity;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Configurations;

/// <summary>
/// Entity Framework configuration for the RefreshToken entity
/// </summary>
public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        // Table name
        builder.ToTable("RefreshToken");
        
        // Primary key
        builder.HasKey(rt => rt.Id);
        
        // Properties
        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(rt => rt.ExpiresAt)
            .IsRequired();
            
        builder.Property(rt => rt.IsRevoked)
            .IsRequired()
            .HasDefaultValue(false);
            
        builder.Property(rt => rt.JwtId)
            .HasMaxLength(100);
            
        // Ignore computed property
        builder.Ignore(rt => rt.IsActive);
            
        // Foreign key relationship
        builder.HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Indexes
        builder.HasIndex(rt => rt.Token)
            .IsUnique()
            .HasDatabaseName("IX_RefreshToken_Token");
            
        builder.HasIndex(rt => rt.UserId)
            .HasDatabaseName("IX_RefreshToken_UserId");
            
        builder.HasIndex(rt => rt.ExpiresAt)
            .HasDatabaseName("IX_RefreshToken_ExpiresAt");
            
        builder.HasIndex(rt => rt.IsRevoked)
            .HasDatabaseName("IX_RefreshToken_IsRevoked");
            
        builder.HasIndex(rt => rt.JwtId)
            .HasDatabaseName("IX_RefreshToken_JwtId");
    }
}