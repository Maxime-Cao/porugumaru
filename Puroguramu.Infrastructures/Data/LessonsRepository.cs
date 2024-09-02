using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Puroguramu.Domains;

namespace Puroguramu.Infrastructures.Data;

public class LessonsRepository : ILessonsRepository
{
    private readonly PuroguramuDbContext _puroguramuDbContext;
    private readonly IExercisesRepository _exercisesRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<LessonsRepository> _logger;

    public LessonsRepository(PuroguramuDbContext puroguramuDbContext,IExercisesRepository exercisesRepository,IMapper mapper, ILogger<LessonsRepository> logger)
    {
        _puroguramuDbContext = puroguramuDbContext;
        _exercisesRepository = exercisesRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public IEnumerable<Lesson> GetAllLessons(bool onlyPublished)
    {
        var lessonsQuery = _puroguramuDbContext.Lessons.AsQueryable();

        if (onlyPublished)
        {
            lessonsQuery = lessonsQuery.Where(l => l.IsPublished).Include(l => l.Exercises);
        }
        else
        {
            lessonsQuery = lessonsQuery.Include(l => l.Exercises);
        }

        var lessonsFound = lessonsQuery.OrderBy(l => l.Position).ToList();

        foreach (var lesson in lessonsFound)
        {
            if (onlyPublished)
            {
                lesson.Exercises = lesson.Exercises.Where(e => e.IsPublished).OrderBy(e => e.Position).ToList();
            }
            else
            {
                lesson.Exercises = lesson.Exercises.OrderBy(e => e.Position).ToList();
            }
        }

        return _mapper.Map<IEnumerable<Lesson>>(lessonsFound);
    }

    public int GetNumberOfLessons() => _puroguramuDbContext.Lessons.Count();

    public Lesson? GetLesson(Guid lessonId, bool onlyPublishedExercises)
    {
        var lessonFound = _puroguramuDbContext.Lessons.Include(l => l.Exercises).FirstOrDefault(l => l.LessonId == lessonId);

        if (lessonFound != null)
        {
            if (onlyPublishedExercises)
            {
                lessonFound.Exercises = lessonFound.Exercises.Where(e => e.IsPublished).OrderBy(e => e.Position).ToList();
            }
            else
            {
                lessonFound.Exercises = lessonFound.Exercises.OrderBy(e => e.Position).ToList();
            }
        }

        return _mapper.Map<Lesson>(lessonFound);
    }

    public int GetCompletedStudentCount(Guid lessonId)
    {
        var count = 0;

        var students = _puroguramuDbContext.Users.OfType<IdentityStudent>().Include(s => s.ExerciseAttempts).ToList();

        var exercises = _puroguramuDbContext.Exercises.Where(e => e.LessonId == lessonId).ToList();

        foreach (var student in students)
        {
            if (exercises.Count > 0)
            {
                count++;
                foreach (var exercise in exercises)
                {
                    if (!student.ExerciseAttempts.Any(ea => ea.ExerciseId == exercise.ExerciseId && ea.ExerciseStatus == ExerciseStatus.Passed))
                    {
                        count--;
                        break;
                    }
                }
            }
        }

        return count;
    }

    public async Task<(bool Succeeded, string Error)> UpdateLessonAsync(Lesson lesson)
    {
        try
        {
            var lessonFound = _puroguramuDbContext.Lessons.FirstOrDefault(l => l.LessonId == lesson.LessonId);

            if (lessonFound == null)
            {
                _logger.LogError($"UpdateLessonAsync() : lesson with ID {lesson.LessonId} not found");
                return (false, "Lesson not found");
            }

            if (lessonFound.Title != lesson.Title && IsLessonTitleExists(lesson.Title))
            {
                _logger.LogError($"UpdateLessonAsync() : lesson title {lesson.Title} already exists in our system");
                return (false, "This lesson title already exists in our system");
            }

            if (lesson.Title.Length < 5)
            {
                _logger.LogError($"UpdateLessonAsync() : the lesson title {lesson.Title} does not contain 5 characters");
                return (false, "The lesson title does not contain 5 characters");
            }

            lessonFound.Title = lesson.Title;
            lessonFound.Description = lesson.Description;

            _puroguramuDbContext.Lessons.Update(lessonFound);
            await _puroguramuDbContext.SaveChangesAsync();

            return (true, string.Empty);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"UpdateLessonAsync() : an error occurred while updating lesson {lesson.LessonId}");
            return (false, "An error occurred while updating lesson");
        }
    }

    public async Task<(bool Succeeded, string Error)> HideLessonAsync(Guid lessonId)
    {
        try
        {
            var lessonFound = _puroguramuDbContext.Lessons.FirstOrDefault(l => l.LessonId == lessonId);

            if (lessonFound == null)
            {
                _logger.LogError($"HideLessonAsync() : lesson with ID {lessonId} not found");
                return (false, "Lesson not found");
            }

            lessonFound.IsPublished = false;
            await _puroguramuDbContext.SaveChangesAsync();

            return (true, string.Empty);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"HideLessonAsync() : an error occurred while hiding lesson {lessonId}");
            return (false, "An error occurred while hiding lesson");
        }
    }

    public async Task<(bool Succeeded, string Error)> PublishLessonAsync(Guid lessonId)
    {
        try
        {
            var lessonFound = _puroguramuDbContext.Lessons.FirstOrDefault(l => l.LessonId == lessonId);

            if (lessonFound == null)
            {
                _logger.LogError($"PublishLessonAsync() : lesson with ID {lessonId} not found");
                return (false, "Lesson not found");
            }

            lessonFound.IsPublished = true;
            await _puroguramuDbContext.SaveChangesAsync();

            return (true, string.Empty);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"PublishLessonAsync() : an error occurred while publishing lesson {lessonId}");
            return (false, "An error occurred while publishing lesson");
        }
    }

    public async Task<(bool Succeeded, string Error)> UpLessonAsync(Guid lessonId)
    {
        try
        {
            var lessonFound = _puroguramuDbContext.Lessons.FirstOrDefault(l => l.LessonId == lessonId);

            if (lessonFound == null)
            {
                _logger.LogError($"UpLessonAsync() : lesson with ID {lessonId} not found");
                return (false, "Lesson not found");
            }

            var previousLessonFound = _puroguramuDbContext.Lessons.FirstOrDefault(l => l.Position == lessonFound.Position - 1);

            if (previousLessonFound != null)
            {
                var currentPos = lessonFound.Position;
                var maxPos = _puroguramuDbContext.Lessons.Max(l => l.Position);

                lessonFound.Position = maxPos + 1;
                _puroguramuDbContext.Lessons.Update(lessonFound);
                await _puroguramuDbContext.SaveChangesAsync();

                previousLessonFound.Position += 1;
                _puroguramuDbContext.Lessons.Update(previousLessonFound);
                await _puroguramuDbContext.SaveChangesAsync();

                lessonFound.Position = currentPos - 1;
                _puroguramuDbContext.Lessons.Update(lessonFound);
                await _puroguramuDbContext.SaveChangesAsync();
            }

            return (true, string.Empty);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"UpLessonAsync() : an error occurred while updating lesson {lessonId} position");
            return (false, "An error occurred while updating lesson position");
        }
    }

    public async Task<(bool Succeeded, string Error)> DownLessonAsync(Guid lessonId)
    {
        try
        {
            var lessonFound = _puroguramuDbContext.Lessons.FirstOrDefault(l => l.LessonId == lessonId);

            if (lessonFound == null)
            {
                _logger.LogError($"DownLessonAsync() : lesson with ID {lessonId} not found");
                return (false, "Lesson not found");
            }

            var nextLessonFound = _puroguramuDbContext.Lessons.FirstOrDefault(l => l.Position == lessonFound.Position + 1);

            if (nextLessonFound != null)
            {
                var currentPos = lessonFound.Position;
                var maxPos = _puroguramuDbContext.Lessons.Max(l => l.Position);

                lessonFound.Position = maxPos + 1;
                _puroguramuDbContext.Lessons.Update(lessonFound);
                await _puroguramuDbContext.SaveChangesAsync();

                nextLessonFound.Position -= 1;
                _puroguramuDbContext.Lessons.Update(nextLessonFound);
                await _puroguramuDbContext.SaveChangesAsync();

                lessonFound.Position = currentPos + 1;
                _puroguramuDbContext.Lessons.Update(lessonFound);
                await _puroguramuDbContext.SaveChangesAsync();
            }

            return (true, string.Empty);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"DownLessonAsync() : an error occurred while updating lesson {lessonId} position");
            return (false, "An error occurred while updating lesson position");
        }
    }

    public async Task<(bool Succeeded, string Error)> DeleteLessonAsync(Guid lessonId)
    {
        try
        {
            var lessonFound = _puroguramuDbContext.Lessons.FirstOrDefault(l => l.LessonId == lessonId);

            if (lessonFound == null)
            {
                _logger.LogError($"DeleteLessonAsync() : lesson with ID {lessonId} not found");
                return (false, "Lesson not found");
            }

            _puroguramuDbContext.Lessons.Remove(lessonFound);
            await _puroguramuDbContext.SaveChangesAsync();

            return (true, string.Empty);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"DeleteLessonAsync() : an error occurred while deleting lesson {lessonId}");
            return (false, "An error occurred while deleting lesson");
        }
    }

    public async Task<(bool Succeeded, string Error)> CreateLessonAsync(Lesson lesson)
    {
        try
        {
            if (IsLessonTitleExists(lesson.Title))
            {
                _logger.LogError($"CreateLessonAsync() : lesson title {lesson.Title} already exists in our system");
                return (false, "This lesson title already exists in our system");
            }

            if (lesson.Title.Length < 5)
            {
                _logger.LogError($"CreateLessonAsync() : the lesson title {lesson.Title} does not contain 5 characters");
                return (false, "The lesson title does not contain 5 characters");
            }

            var entityLesson = _mapper.Map<EntityLesson>(lesson);
            var maxPosLesson = _puroguramuDbContext.Lessons.Any() ? _puroguramuDbContext.Lessons.Max(l => l.Position) : 0;
            entityLesson.Position = maxPosLesson + 1;

            _puroguramuDbContext.Lessons.Add(entityLesson);
            await _puroguramuDbContext.SaveChangesAsync();

            return (true, string.Empty);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"CreateLessonAsync() : an error occurred while creating lesson {lesson.LessonId}");
            return (false, "An error occurred while creating lesson");
        }
    }

    public async Task<(bool Succeeded, string Error)> ResetLessonAsync(Guid lessonId)
    {
        try
        {
            var exercisesFound = _puroguramuDbContext.Exercises.Where(e => e.LessonId == lessonId).ToList();

            foreach (var exercise in exercisesFound)
            {
                var resetExerciseResult = await _exercisesRepository.ResetExerciseAsync(exercise.ExerciseId);
                if (!resetExerciseResult.Succeeded)
                {
                    _logger.LogError($"ResetLessonAsync() : an error occurred while resetting an exercise in the lesson {lessonId}");
                    return (false, $"ResetLessonAsync() : an error occurred while resetting an exercise in the lesson");
                }
            }

            await _puroguramuDbContext.SaveChangesAsync();

            return (true, string.Empty);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"ResetLessonAsync() : an error occurred during lesson {lessonId} reset");
            return (false, "An error occurred during lesson reset");
        }
    }

    private bool IsLessonTitleExists(string title) => _puroguramuDbContext.Lessons.Any(l => l.Title == title);
}
