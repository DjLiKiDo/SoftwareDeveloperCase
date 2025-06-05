using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoftwareDeveloperCase.Domain.Entities.Team;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Configurations;

/// <summary>
/// Entity Framework configuration for the TeamMember entity (Team-User many-to-many)
/// </summary>
public class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        // Table name
        builder.ToTable("TeamMember");

        // Primary key
        builder.HasKey(tm => tm.Id);

        // Properties
        builder.Property(tm => tm.TeamRole)
            .IsRequired();

        builder.Property(tm => tm.Status)
            .IsRequired();

        builder.Property(tm => tm.JoinedDate)
            .IsRequired();

        // Foreign key relationships
        builder.HasOne(tm => tm.Team)
            .WithMany(t => t.TeamMembers)
            .HasForeignKey(tm => tm.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tm => tm.User)
            .WithMany(u => u.TeamMemberships)
            .HasForeignKey(tm => tm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(tm => tm.TeamId)
            .HasDatabaseName("IX_TeamMember_TeamId");

        builder.HasIndex(tm => tm.UserId)
            .HasDatabaseName("IX_TeamMember_UserId");

        builder.HasIndex(tm => tm.Status)
            .HasDatabaseName("IX_TeamMember_Status");

        builder.HasIndex(tm => tm.TeamRole)
            .HasDatabaseName("IX_TeamMember_TeamRole");

        // Unique constraint to prevent duplicate team-user assignments
        builder.HasIndex(tm => new { tm.TeamId, tm.UserId })
            .IsUnique()
            .HasDatabaseName("IX_TeamMember_TeamId_UserId");

        // Composite index for common queries
        builder.HasIndex(tm => new { tm.TeamId, tm.Status })
            .HasDatabaseName("IX_TeamMember_TeamId_Status");

        builder.HasIndex(tm => new { tm.UserId, tm.Status })
            .HasDatabaseName("IX_TeamMember_UserId_Status");
    }
}
