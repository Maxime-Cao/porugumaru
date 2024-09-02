namespace Puroguramu.App.ViewModels;

public class TeacherExerciseViewModel
{
    public Guid ExerciseId { get; init; }

    public string Title { get; init; }

    public bool IsPublished { get; init; }

    public bool IsFirstExercise { get; init; }

    public bool IsLastExercise { get; init; }
}
