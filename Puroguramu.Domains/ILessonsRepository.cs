namespace Puroguramu.Domains;

public interface ILessonsRepository
{
    IEnumerable<Lesson> GetAllLessons(bool onlyPublished);

    int GetNumberOfLessons();

    Lesson? GetLesson(Guid lessonId, bool onlyPublishedExercises);

    int GetCompletedStudentCount(Guid lessonId);

    Task<(bool Succeeded, string Error)> UpdateLessonAsync(Lesson lesson);

    Task<(bool Succeeded, string Error)> HideLessonAsync(Guid lessonId);

    Task<(bool Succeeded, string Error)> PublishLessonAsync(Guid lessonId);

    Task<(bool Succeeded, string Error)> UpLessonAsync(Guid lessonId);

    Task<(bool Succeeded, string Error)> DownLessonAsync(Guid lessonId);

    Task<(bool Succeeded, string Error)> DeleteLessonAsync(Guid lessonId);

    Task<(bool Succeeded, string Error)> CreateLessonAsync(Lesson lesson);

    Task<(bool Succeeded, string Error)> ResetLessonAsync(Guid lessonId);
}
