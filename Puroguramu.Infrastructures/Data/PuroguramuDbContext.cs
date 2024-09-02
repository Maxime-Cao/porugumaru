using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Puroguramu.Infrastructures.Data.Configurations;

namespace Puroguramu.Infrastructures.Data;

public class PuroguramuDbContext : IdentityDbContext<IdentitySchoolMember,IdentityRole,string>
{
    public DbSet<EntityGroupLab> Groups { get; set; }

    public DbSet<EntityLesson> Lessons { get; set; }

    public DbSet<EntityExercise> Exercises { get; set; }

    public DbSet<EntityExerciseAttempt> ExerciseAttempts { get; set; }

    public PuroguramuDbContext(DbContextOptions<PuroguramuDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EntityGroupLab>()
            .HasIndex(g => g.GroupName)
            .IsUnique();

        modelBuilder.Entity<EntityLesson>()
            .HasIndex(l => l.Position)
            .IsUnique();

        modelBuilder.Entity<EntityLesson>()
            .HasIndex(l => l.Title)
            .IsUnique();

        modelBuilder.Entity<IdentitySchoolMember>()
            .HasIndex(m => m.Matricule)
            .IsUnique();

        modelBuilder.Entity<IdentitySchoolMember>()
            .HasIndex(m => m.Email)
            .IsUnique();

        modelBuilder.Entity<IdentityStudent>()
            .HasOne(m => m.LabGroup)
            .WithMany(g => g.Members);

        modelBuilder.Entity<IdentityStudent>()
            .HasMany(s => s.ExerciseAttempts)
            .WithOne(e => e.Student)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EntityLesson>()
            .HasMany(lesson => lesson.Exercises)
            .WithOne(exercise => exercise.Lesson)
            .HasForeignKey(exercise => exercise.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EntityExercise>()
            .HasMany(exercise => exercise.ExerciseAttempts)
            .WithOne(attempt => attempt.Exercise)
            .HasForeignKey(attempt => attempt.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);

        var groups = new List<EntityGroupLab>();
        var students = new List<IdentityStudent>();
        var lessons = new List<EntityLesson>();
        var exercises = new List<EntityExercise>();

        modelBuilder.ApplyConfiguration(new EntityGroupConfiguration());
        groups.AddRange(EntityGroupConfiguration.Groups);

        modelBuilder.ApplyConfiguration(new IdentityRoleConfiguration());
        modelBuilder.ApplyConfiguration(new IdentityUserConfiguration());
        modelBuilder.ApplyConfiguration(new IdentityTeacherConfiguration());

        modelBuilder.ApplyConfiguration(new IdentityStudentConfiguration(groups));
        students.AddRange(IdentityStudentConfiguration.Students);

        modelBuilder.ApplyConfiguration(new EntityLessonConfiguration());
        lessons.AddRange(EntityLessonConfiguration.Lessons);

        modelBuilder.ApplyConfiguration(new EntityExerciseConfiguration(lessons));
        exercises.AddRange(EntityExerciseConfiguration.Exercises);

        modelBuilder.ApplyConfiguration(new EntityExerciseAttemptConfiguration(students,exercises));

        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string> {RoleId = IdentityRoleConfiguration.TeacherId, UserId = IdentityTeacherConfiguration.NicolasId },
            new IdentityUserRole<string> {RoleId = IdentityRoleConfiguration.TeacherId, UserId = IdentityTeacherConfiguration.DorianId },
            new IdentityUserRole<string> {RoleId = IdentityRoleConfiguration.StudentId, UserId = IdentityStudentConfiguration.MaximeId },
            new IdentityUserRole<string> {RoleId = IdentityRoleConfiguration.StudentId, UserId = IdentityStudentConfiguration.MeganId });
    }

    public override int SaveChanges()
    {
        AdjustLessonsPosition();
        AdjustExercisesPosition();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AdjustLessonsPosition();
        AdjustExercisesPosition();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void AdjustLessonsPosition()
    {
        var deletedLessons = ChangeTracker.Entries<EntityLesson>()
            .Where(e => e.State == EntityState.Deleted)
            .Select(e => e.Entity)
            .ToList();

        foreach (var deletedLesson in deletedLessons)
        {
            var affectedLessons = Lessons
                .Where(l => l.Position > deletedLesson.Position)
                .ToList();
            foreach (var lesson in affectedLessons)
            {
                lesson.Position--;
                Entry(lesson).State = EntityState.Modified;
            }
        }
    }

    private void AdjustExercisesPosition()
    {
        var deletedExercises = ChangeTracker.Entries<EntityExercise>()
            .Where(e => e.State == EntityState.Deleted)
            .Select(e => e.Entity)
            .ToList();

        foreach (var deletedExercise in deletedExercises)
        {
            var affectedExercises = Exercises
                .Where(e => e.LessonId == deletedExercise.LessonId && e.Position > deletedExercise.Position)
                .ToList();

            foreach (var exercise in affectedExercises)
            {
                exercise.Position--;
                Entry(exercise).State = EntityState.Modified;
            }
        }
    }
}
