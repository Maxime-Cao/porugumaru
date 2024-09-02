using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Puroguramu.Infrastructures.Data.Configurations;

public class IdentityTeacherConfiguration : IEntityTypeConfiguration<IdentityTeacher>
{
    public static readonly string NicolasId = $"{Environment.MachineName}{DateTime.Parse("2024-06-25T23:48:00.0")}Nicolas";
    public static readonly string DorianId = $"{Environment.MachineName}{DateTime.Parse("2024-06-25T23:48:00.0")}Dorian";

    public void Configure(EntityTypeBuilder<IdentityTeacher> builder) => SeedTeachers(builder);

    private void SeedTeachers(EntityTypeBuilder<IdentityTeacher> builder)
    {
        var ph = new PasswordHasher<IdentitySchoolMember>();

        var nicolas = new IdentityTeacher()
        {
            Id = NicolasId,
            Email = "n.hendrikx@helmo.be",
            EmailConfirmed = true,
            UserName = "n.hendrikx@helmo.be",
            NormalizedEmail = "n.hendrikx@helmo.be".ToUpper(),
            Matricule = "p070039",
            Name = "Hendrikx",
            FirstName = "Nicolas"
        };

        nicolas.PasswordHash = ph.HashPassword(nicolas, "Test1234$");

        var dorian = new IdentityTeacher()
        {
            Id = DorianId,
            Email = "d.lauwers@helmo.be",
            EmailConfirmed = true,
            UserName = "d.lauwers@helmo.be",
            NormalizedEmail = "d.lauwers@helmo.be".ToUpper(),
            Matricule = "p180039",
            Name = "Lauwers",
            FirstName = "Dorian"
        };

        dorian.PasswordHash = ph.HashPassword(dorian, "Test1234$");

        builder.HasData(nicolas, dorian);
    }
}
