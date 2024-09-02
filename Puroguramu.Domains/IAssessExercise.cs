namespace Puroguramu.Domains;

public interface IAssessExercise
{
    Task<ExerciseResult> Assess(Guid exerciseId, string proposal);

    Task<ExerciseResult> StubForExercise(Guid exerciseId);
}
