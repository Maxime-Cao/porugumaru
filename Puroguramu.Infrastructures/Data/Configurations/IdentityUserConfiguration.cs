using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Puroguramu.Infrastructures.Data.Configurations;

public class IdentityUserConfiguration : IEntityTypeConfiguration<IdentitySchoolMember>
{
    public void Configure(EntityTypeBuilder<IdentitySchoolMember> builder)
    {
        builder.Property<string>(user => user.Matricule)
            .IsRequired()
            .HasMaxLength(7)
            .IsFixedLength();

        builder.HasIndex(user => user.Matricule).IsUnique();
    }
}
