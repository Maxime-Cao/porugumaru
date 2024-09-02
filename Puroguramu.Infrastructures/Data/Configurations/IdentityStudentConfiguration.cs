using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Puroguramu.Infrastructures.Data.Configurations;

public class IdentityStudentConfiguration : IEntityTypeConfiguration<IdentityStudent>
{
    public static readonly List<IdentityStudent> Students = new();
    public static readonly string MaximeId = $"{Environment.MachineName}{DateTime.Parse("2024-06-25T23:48:00.0")}Maxime";
    public static readonly string MeganId = $"{Environment.MachineName}{DateTime.Parse("2024-06-25T23:48:00.0")}Megan";

    private readonly IList<EntityGroupLab> _groups;

    public IdentityStudentConfiguration(IEnumerable<EntityGroupLab> groups)
    {
        _groups = groups.ToList();
    }

    public void Configure(EntityTypeBuilder<IdentityStudent> builder) => SeedStudents(builder);

    private void SeedStudents(EntityTypeBuilder<IdentityStudent> builder)
    {
        Students.Clear();

        var ph = new PasswordHasher<IdentitySchoolMember>();

        var maxime = new IdentityStudent()
        {
            Id = MaximeId,
            Email = "m.cao@student.helmo.be",
            EmailConfirmed = true,
            UserName = "m.cao@student.helmo.be",
            NormalizedEmail = "m.cao@student.helmo.be".ToUpper(),
            Matricule = "d170051",
            Name = "Cao",
            FirstName = "Maxime",
            LabGroupId = _groups[0].IdGroup,
        };

        maxime.PasswordHash = ph.HashPassword(maxime, "Test1234$");

        var megan = new IdentityStudent()
        {
            Id = MeganId,
            Email = "m.levieux@student.helmo.be",
            EmailConfirmed = true,
            UserName = "m.levieux@student.helmo.be",
            NormalizedEmail = "m.levieux@student.helmo.be".ToUpper(),
            Matricule = "d170000",
            Name = "Levieux",
            FirstName = "Megan",
            LabGroupId = _groups[1].IdGroup,
        };

        megan.PasswordHash = ph.HashPassword(megan, "Test1234$");

        Students.AddRange(new[] {maxime,megan});

        builder.HasData(Students);
    }
}
