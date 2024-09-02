namespace Puroguramu.App.ViewModels;

public class HomeViewModel
{
    public int NumberOfStudents { get; init; }

    public int NumberOfLessons { get; init; }

    public int NumberOfExercises { get; init; }

    public bool IsAuthenticated { get; init; }

    public string Role { get; init; }
}
