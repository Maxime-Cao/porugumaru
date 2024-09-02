namespace Puroguramu.App.ViewModels;

public class StudentLessonViewModel
{
    public Guid LessonId { get; init; }

    public string Title { get; init; }

    public string Description { get; init; }

    public IEnumerable<StudentExerciseViewModel> Exercises { get; init; }

    public int ExercisesCount { get; init; }

    public int ExercisesCompletedCount { get; init; }

    public Guid NextExercise { get; init; }
}
