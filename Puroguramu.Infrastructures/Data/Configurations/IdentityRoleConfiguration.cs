using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Puroguramu.Infrastructures.Data.Configurations;

public class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public static readonly string TeacherId = $"{Environment.MachineName}{DateTime.Parse("2024-06-25T23:48:00.0")}Teacher";
    public static readonly string StudentId = $"{Environment.MachineName}{DateTime.Parse("2024-06-25T23:48:00.0")}Student";

    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        SeedRoles(builder);
    }

    private void SeedRoles(EntityTypeBuilder<IdentityRole> builder)
    {
        var teacherRole = new IdentityRole() {Id = TeacherId, ConcurrencyStamp = TeacherId, Name = "Teacher", NormalizedName = "TEACHER" };
        var studentRole = new IdentityRole() { Id = StudentId, ConcurrencyStamp = StudentId, Name = "Student", NormalizedName = "STUDENT" };

        builder.HasData(teacherRole, studentRole);
    }
}
