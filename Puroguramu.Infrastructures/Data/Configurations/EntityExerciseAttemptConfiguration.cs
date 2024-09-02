using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Puroguramu.Domains;

namespace Puroguramu.Infrastructures.Data.Configurations;

public class EntityExerciseAttemptConfiguration : IEntityTypeConfiguration<EntityExerciseAttempt>
{
    private readonly IList<IdentityStudent> _students;
    private readonly IList<EntityExercise> _exercises;

    private readonly string _firstCorrectProposal = @"public class Exercice
{
    public static float Power(float b, int e)
    {
        return (float)Math.Pow(b, e);
    }
}
";

    private readonly string _firstIncorrectProposal = @"public class Exercice
{
    public static float Power(float b, int e)
    {
        return 1.0f;
    }
}
";

    public EntityExerciseAttemptConfiguration(IEnumerable<IdentityStudent> students,IEnumerable<EntityExercise> exercises)
    {
        _students = students.ToList();
        _exercises = exercises.ToList();
    }

    public void Configure(EntityTypeBuilder<EntityExerciseAttempt> builder) => SeedAttempts(builder);

    private void SeedAttempts(EntityTypeBuilder<EntityExerciseAttempt> builder)
    {
        var firstAttemptFirstStudent = new EntityExerciseAttempt()
        {
            AttemptId = Guid.NewGuid(),
            Proposal = _firstCorrectProposal,
            ExerciseStatus = ExerciseStatus.Passed,
            AttemptTime = DateTime.Now,
            ExerciseId = _exercises[1].ExerciseId,
            StudentId = _students[0].Id,
        };

        var secondAttemptFirstStudent = new EntityExerciseAttempt()
        {
            AttemptId = Guid.NewGuid(),
            Proposal = _firstCorrectProposal,
            ExerciseStatus = ExerciseStatus.Passed,
            AttemptTime = DateTime.Now,
            ExerciseId = _exercises[2].ExerciseId,
            StudentId = _students[0].Id,
        };

        var thirdAttemptFirstStudent = new EntityExerciseAttempt()
        {
            AttemptId = Guid.NewGuid(),
            Proposal = _firstCorrectProposal,
            ExerciseStatus = ExerciseStatus.Passed,
            AttemptTime = DateTime.Now,
            ExerciseId = _exercises[3].ExerciseId,
            StudentId = _students[0].Id,
        };

        var fourthAttemptFirstStudent = new EntityExerciseAttempt()
        {
            AttemptId = Guid.NewGuid(),
            Proposal = _firstCorrectProposal,
            ExerciseStatus = ExerciseStatus.Passed,
            AttemptTime = DateTime.Now,
            ExerciseId = _exercises[4].ExerciseId,
            StudentId = _students[0].Id,
        };

        var fithAttemptFirstStudent = new EntityExerciseAttempt()
        {
            AttemptId = Guid.NewGuid(),
            Proposal = _firstCorrectProposal,
            ExerciseStatus = ExerciseStatus.Passed,
            AttemptTime = DateTime.Now,
            ExerciseId = _exercises[6].ExerciseId,
            StudentId = _students[0].Id,
        };

        var sixthAttemptFirstStudent = new EntityExerciseAttempt()
        {
            AttemptId = Guid.NewGuid(),
            Proposal = _firstIncorrectProposal,
            ExerciseStatus = ExerciseStatus.Started,
            AttemptTime = DateTime.Now,
            ExerciseId = _exercises[7].ExerciseId,
            StudentId = _students[0].Id,
        };

        var firstAttemptSecondStudent = new EntityExerciseAttempt()
        {
            AttemptId = Guid.NewGuid(),
            Proposal = _firstIncorrectProposal,
            ExerciseStatus = ExerciseStatus.Started,
            AttemptTime = DateTime.Now,
            ExerciseId = _exercises[1].ExerciseId,
            StudentId = _students[1].Id,
        };

        var secondAttemptSecondStudent = new EntityExerciseAttempt()
        {
            AttemptId = Guid.NewGuid(),
            Proposal = _firstCorrectProposal,
            ExerciseStatus = ExerciseStatus.Passed,
            AttemptTime = DateTime.Now,
            ExerciseId = _exercises[2].ExerciseId,
            StudentId = _students[1].Id,
        };

        var thirdAttemptSecondStudent = new EntityExerciseAttempt()
        {
            AttemptId = Guid.NewGuid(),
            Proposal = _firstCorrectProposal,
            ExerciseStatus = ExerciseStatus.Passed,
            AttemptTime = DateTime.Now,
            ExerciseId = _exercises[7].ExerciseId,
            StudentId = _students[1].Id,
        };

        var fourthAttemptSecondStudent = new EntityExerciseAttempt()
        {
            AttemptId = Guid.NewGuid(),
            Proposal = _firstCorrectProposal,
            ExerciseStatus = ExerciseStatus.Passed,
            AttemptTime = DateTime.Now,
            ExerciseId = _exercises[8].ExerciseId,
            StudentId = _students[1].Id,
        };

        var fithAttemptSecondStudent = new EntityExerciseAttempt()
        {
            AttemptId = Guid.NewGuid(),
            Proposal = _firstIncorrectProposal,
            ExerciseStatus = ExerciseStatus.Started,
            AttemptTime = DateTime.Now,
            ExerciseId = _exercises[9].ExerciseId,
            StudentId = _students[1].Id,
        };

        builder.HasData(firstAttemptFirstStudent);
        builder.HasData(secondAttemptFirstStudent);
        builder.HasData(thirdAttemptFirstStudent);
        builder.HasData(fourthAttemptFirstStudent);
        builder.HasData(fithAttemptFirstStudent);
        builder.HasData(sixthAttemptFirstStudent);
        builder.HasData(firstAttemptSecondStudent);
        builder.HasData(secondAttemptSecondStudent);
        builder.HasData(thirdAttemptSecondStudent);
        builder.HasData(fourthAttemptSecondStudent);
        builder.HasData(fithAttemptSecondStudent);
    }
}
