namespace Puroguramu.App.ViewModels;

public class TeacherLessonViewModel
{
    public Guid LessonId { get; init; }

    public string Title { get; init; }

    public string Description { get; init; }

    public IEnumerable<TeacherExerciseViewModel> Exercises { get; set; }

    public bool IsPublished { get; init; }

    public int CompletedStudentCount { get; init; }

    public bool IsFirstLesson { get; init; }

    public bool IsLastLesson { get; init; }
}
