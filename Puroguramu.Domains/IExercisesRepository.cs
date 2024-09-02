namespace Puroguramu.Domains;

public interface IExercisesRepository
{
    Exercise? GetExercise(Guid exerciseId);

    int GetNumberOfExercises();

    ExerciseStatus? GetExerciseStatus(Guid exerciseId, string matricule);

    int GetCompletedExercisesCount(Guid lessonId, string matricule);

    Guid GetNextPublishedExercise(Guid lessonId, string matricule);

    ExerciseAttempt? GetExerciseAttempt(Guid attemptId);

    ExerciseAttempt? GetLastExerciceAttempt(Guid exerciseId, string matricule);

    IEnumerable<ExerciseAttempt> GetAllAttempts(Guid exerciseId, string matricule);

    Task<(bool Succeeded, string Error)> CreateExerciseAttemptAsync(ExerciseAttempt exerciseAttempt);

    Task<(bool Succeeded, string Error)> HideExerciseAsync(Guid exerciseId);

    Task<(bool Succeeded, string Error)> PublishExerciseAsync(Guid exerciseId);

    Task<(bool Succeeded, string Error)> UpExerciseAsync(Guid exerciseId);

    Task<(bool Succeeded, string Error)> DownExerciseAsync(Guid exerciseId);

    Task<(bool Succeeded, string Error)> CreateExerciseAsync(Exercise exercise);

    Task<(bool Succeeded, string Error)> UpdateExerciseAsync(Exercise exercise);

    Task<(bool Succeeded, string Error)> DeleteExerciseAsync(Guid exerciseId);

    Task<(bool Succeeded, string Error)> ResetExerciseAsync(Guid exerciseId);
}
