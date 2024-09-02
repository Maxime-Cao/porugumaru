using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Puroguramu.Infrastructures.Data.Configurations;

public class EntityGroupConfiguration : IEntityTypeConfiguration<EntityGroupLab>
{
    public static List<EntityGroupLab> Groups { get; } = new();

    public void Configure(EntityTypeBuilder<EntityGroupLab> builder) => SeedGroups(builder);

    private void SeedGroups(EntityTypeBuilder<EntityGroupLab> builder)
    {
        Groups.Clear();

        var firstGroup = new EntityGroupLab() { IdGroup = Guid.NewGuid(),GroupName = "2i1" };
        var secondGroup = new EntityGroupLab() { IdGroup = Guid.NewGuid(),GroupName = "2i2" };
        var thirdGroup = new EntityGroupLab() { IdGroup = Guid.NewGuid(),GroupName = "2i3" };
        var fourthGroup = new EntityGroupLab() { IdGroup = Guid.NewGuid(),GroupName = "2i4" };

        Groups.AddRange(new[] {firstGroup,secondGroup,thirdGroup,fourthGroup});

        builder.HasData(Groups);
    }
}
