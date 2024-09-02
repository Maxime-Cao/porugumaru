using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Puroguramu.Infrastructures.Data.Configurations;

public class EntityLessonConfiguration : IEntityTypeConfiguration<EntityLesson>
{
    public static List<EntityLesson> Lessons = new();

    public void Configure(EntityTypeBuilder<EntityLesson> builder) => SeedLessons(builder);

    private void SeedLessons(EntityTypeBuilder<EntityLesson> builder)
    {
        Lessons.Clear();

        var firstLesson = new EntityLesson()
        {
            LessonId = Guid.NewGuid(),
            Title = "Mon incroyable leçon v1",
            Description = "Ceci est la description de mon incroyable leçon v1",
            Position = 1,
            IsPublished = true,
        };

        var secondLesson = new EntityLesson()
        {
            LessonId = Guid.NewGuid(),
            Title = "Mon incroyable leçon v2",
            Description = "Ceci est la description de mon incroyable leçon v2",
            Position = 2,
            IsPublished = true,
        };

        var thirdLesson = new EntityLesson()
        {
            LessonId = Guid.NewGuid(),
            Title = "Mon incroyable leçon v3",
            Description = "Ceci est la description de mon incroyable leçon v3",
            Position = 3,
            IsPublished = true,
        };

        var fourthLesson = new EntityLesson()
        {
            LessonId = Guid.NewGuid(),
            Title = "Mon incroyable leçon v4",
            Description = "Ceci est la description de mon incroyable leçon v4",
            Position = 4,
            IsPublished = true,
        };

        Lessons.AddRange(new[] {firstLesson,secondLesson,thirdLesson,fourthLesson});

        builder.HasData(Lessons);
    }
}
