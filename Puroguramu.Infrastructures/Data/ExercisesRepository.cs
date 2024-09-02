using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Puroguramu.Domains;
using Puroguramu.Infrastructures.Data;

public class ExercisesRepository : IExercisesRepository
{
    private readonly PuroguramuDbContext _puroguramuDbContext;
    private readonly ILogger<ExercisesRepository> _logger;
    private readonly IMapper _mapper;

    public ExercisesRepository(PuroguramuDbContext puroguramuDbContext,ILogger<ExercisesRepository> logger,IMapper mapper)
    {
        _puroguramuDbContext = puroguramuDbContext;
        _logger = logger;
        _mapper = mapper;
    }

    public Exercise? GetExercise(Guid exerciseId)
    {
        var exerciseFound = _puroguramuDbContext.Exercises.Include(e => e.Lesson).FirstOrDefault(e => e.ExerciseId == exerciseId);
        return _mapper.Map<Exercise>(exerciseFound);
    }

    public int GetNumberOfExercises() => _puroguramuDbContext.Exercises.Count();

    public ExerciseStatus? GetExerciseStatus(Guid exerciseId, string matricule)
    {
        var user = _puroguramuDbContext.Users.OfType<IdentityStudent>().Include(u => u.ExerciseAttempts).FirstOrDefault(u => u.Matricule == matricule);

        if (user == null)
        {
            _logger.LogError($"GetExerciseStatus() : user with matricule {matricule} not found");
            return null;
        }

        var exerciseAttempt = user.ExerciseAttempts.Where(ea => ea.ExerciseId == exerciseId).MaxBy(ea => ea.AttemptTime);

        return exerciseAttempt?.ExerciseStatus ?? ExerciseStatus.NotStarted;
    }

    public int GetCompletedExercisesCount(Guid lessonId, string matricule)
    {
        var count = 0;

        var user = _puroguramuDbContext.Users.FirstOrDefault(u => u.Matricule == matricule);

        if (user == null)
        {
            return count;
        }

        var exercises = _puroguramuDbContext.Exercises.Where(e => e.LessonId == lessonId).ToList();

        foreach (var exercise in exercises)
        {
            if (_puroguramuDbContext.ExerciseAttempts.Any(ea => ea.ExerciseId == exercise.ExerciseId && ea.StudentId == user.Id && ea.ExerciseStatus == ExerciseStatus.Passed && exercise.IsPublished))
            {
                count++;
            }
        }

        return count;
    }

    public Guid GetNextPublishedExercise(Guid lessonId, string matricule)
    {
        var user = _puroguramuDbContext.Users.OfType<IdentityStudent>().Include(u => u.ExerciseAttempts).FirstOrDefault(u => u.Matricule == matricule);

        if (user == null)
        {
            return Guid.Empty;
        }

        var currentLessonPosition = _puroguramuDbContext.Lessons
            .Where(l => l.LessonId == lessonId && l.IsPublished)
            .Select(l => l.Position)
            .FirstOrDefault();

        if (currentLessonPosition < 1)
        {
            return Guid.Empty;
        }

        var completedAndFailedExerciseIds = _puroguramuDbContext.ExerciseAttempts
            .Where(ea => ea.StudentId == user.Id && (ea.ExerciseStatus == ExerciseStatus.Passed || ea.ExerciseStatus == ExerciseStatus.Failed))
            .Select(ea => ea.ExerciseId)
            .ToList();

        var nextExerciseInCurrentLesson = _puroguramuDbContext.Exercises
            .Where(e => e.LessonId == lessonId && !completedAndFailedExerciseIds.Contains(e.ExerciseId) && e.IsPublished)
            .OrderBy(e => e.Position)
            .FirstOrDefault();

        if (nextExerciseInCurrentLesson != null)
        {
            return nextExerciseInCurrentLesson.ExerciseId;
        }

        var nextExerciseInNextLessons = _puroguramuDbContext.Exercises
            .Include(e => e.Lesson)
            .Where(e => e.IsPublished && e.Lesson.IsPublished && e.Lesson.Position > currentLessonPosition && !completedAndFailedExerciseIds.Contains(e.ExerciseId))
            .OrderBy(e => e.Lesson.Position)
            .ThenBy(e => e.Position)
            .FirstOrDefault();

        return nextExerciseInNextLessons?.ExerciseId ?? Guid.Empty;
    }

    public ExerciseAttempt? GetExerciseAttempt(Guid attemptId)
    {
        var attemptFound = _puroguramuDbContext.ExerciseAttempts.Include(ea => ea.Student).FirstOrDefault(ea => ea.AttemptId == attemptId);

        if (attemptFound == null)
        {
            return null;
        }

        return _mapper.Map<ExerciseAttempt>(attemptFound);
    }

    public ExerciseAttempt? GetLastExerciceAttempt(Guid exerciseId, string matricule)
    {
        var user = _puroguramuDbContext.Users.OfType<IdentityStudent>().Include(u => u.ExerciseAttempts).FirstOrDefault(u => u.Matricule == matricule);

        if (user == null)
        {
            _logger.LogError($"GetLastExerciceAttempt() : user with matricule {matricule} not found");
            return null;
        }

        var attemptFound = user.ExerciseAttempts
            .Where(ea => ea.ExerciseId == exerciseId).MaxBy(ea => ea.AttemptTime);

        return _mapper.Map<ExerciseAttempt>(attemptFound);
    }

    public IEnumerable<ExerciseAttempt> GetAllAttempts(Guid exerciseId, string matricule)
    {
       var attempts = new List<ExerciseAttempt>();

       var userFound = _puroguramuDbContext.Users.FirstOrDefault(u => u.Matricule == matricule);

       if (userFound != null)
       {
           var attemptsFound = _puroguramuDbContext.ExerciseAttempts.Where(ea => ea.ExerciseId == exerciseId && ea.StudentId == userFound.Id);

           foreach(var attempt in attemptsFound)
           {
               attempts.Add(_mapper.Map<ExerciseAttempt>(attempt));
           }
       }

       return attempts;
    }

    public async Task<(bool Succeeded, string Error)> CreateExerciseAttemptAsync(ExerciseAttempt exerciseAttempt)
    {
        try
        {
            var entityExerciseAttempt = _mapper.Map<EntityExerciseAttempt>(exerciseAttempt);

            var student = await _puroguramuDbContext.Users.OfType<IdentityStudent>()
                .FirstOrDefaultAsync(s => s.Matricule == entityExerciseAttempt!.Student!.Matricule);

            if (student == null)
            {
                _logger.LogError($"CreateExerciseAttemptAsync() : student with matricule {exerciseAttempt.Student.Matricule} not found");
                return (false, "Student not found");
            }

            entityExerciseAttempt.Student = student;

            var exercise = await _puroguramuDbContext.Exercises
                .FirstOrDefaultAsync(e => e.ExerciseId == entityExerciseAttempt!.Exercise!.ExerciseId);

            if (exercise == null)
            {
                _logger.LogError($"CreateExerciseAttemptAsync() : exercise with ID {entityExerciseAttempt?.Exercise?.ExerciseId} not found");
                return (false, "Exercise not found");
            }

            if(_puroguramuDbContext.ExerciseAttempts.Any(ea => ea.ExerciseId == exercise.ExerciseId && ea.StudentId == student.Id && (ea.ExerciseStatus == ExerciseStatus.Failed || ea.ExerciseStatus == ExerciseStatus.Passed)))
            {
                _logger.LogError($"CreateExerciseAttemptAsync() : database already contains a failed or successful attempt for exercise {exercise.ExerciseId} and for user {student.Matricule}");
                return (false, "Our system already contains a failed or successful attempt for this exercise");
            }

            entityExerciseAttempt.Exercise = exercise;

            _puroguramuDbContext.Add(entityExerciseAttempt);
            await _puroguramuDbContext.SaveChangesAsync();
            return (true, string.Empty);
        }
        catch (Exception e)
        {
            _logger.LogError(e,$"CreateExerciseAttemptAsync() : an error occurred while creating exercise attempt for user {exerciseAttempt.Student.Matricule}");
            return (false, "An error occurred while creating exercise attempt");
        }
    }

    public async Task<(bool Succeeded, string Error)> HideExerciseAsync(Guid exerciseId)
    {
        try
        {
            var exerciseFound = _puroguramuDbContext.Exercises.FirstOrDefault(e => e.ExerciseId == exerciseId);

            if (exerciseFound == null)
            {
                _logger.LogError($"HideExerciseAsync() : exercise with ID {exerciseId} not found");
                return (false, "Exercise not found");
            }

            exerciseFound.IsPublished = false;
            await _puroguramuDbContext.SaveChangesAsync();

            return (true, string.Empty);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"HideExerciseAsync() : an error occurred while hiding exercise {exerciseId}");
            return (false, "An error occurred while hiding exercise");
        }
    }

    public async Task<(bool Succeeded, string Error)> PublishExerciseAsync(Guid exerciseId)
    {
        try
        {
            var exerciseFound = _puroguramuDbContext.Exercises.FirstOrDefault(e => e.ExerciseId == exerciseId);

            if (exerciseFound == null)
            {
                _logger.LogError($"PublishExerciseAsync() : exercise with ID {exerciseFound} not found");
                return (false, "Exercise not found");
            }

            exerciseFound.IsPublished = true;
            await _puroguramuDbContext.SaveChangesAsync();

            return (true, string.Empty);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"PublishExerciseAsync() : an error occurred while publishing exercise {exerciseId}");
            return (false, "An error occurred while publishing exercise");
        }
    }

    public async Task<(bool Succeeded, string Error)> UpExerciseAsync(Guid exerciseId)
    {
        try
        {
            var exerciseFound = _puroguramuDbContext.Exercises.FirstOrDefault(e => e.ExerciseId == exerciseId);

            if (exerciseFound == null)
            {
                _logger.LogError($"UpExerciseAsync() : exercise with ID {exerciseId} not found");
                return (false, "Exercise not found");
            }

            var previousExerciseFound = _puroguramuDbContext.Exercises.FirstOrDefault(e => e.LessonId == exerciseFound.LessonId && e.Position == exerciseFound.Position - 1);

            if (previousExerciseFound != null)
            {
                var currentPos = exerciseFound.Position;
                var maxPos = _puroguramuDbContext.Exercises.Where(e => e.LessonId == exerciseFound.LessonId).Max(e => e.Position);

                exerciseFound.Position = maxPos + 1;
                _puroguramuDbContext.Exercises.Update(exerciseFound);
                await _puroguramuDbContext.SaveChangesAsync();

                previousExerciseFound.Position += 1;
                _puroguramuDbContext.Exercises.Update(previousExerciseFound);
                await _puroguramuDbContext.SaveChangesAsync();

                exerciseFound.Position = currentPos - 1;
                _puroguramuDbContext.Exercises.Update(exerciseFound);
                await _puroguramuDbContext.SaveChangesAsync();
            }

            return (true, string.Empty);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"UpExerciseAsync() : an error occurred while updating exercise {exerciseId} position");
            return (false, "An error occurred while updating exercise position");
        }
    }

    public async Task<(bool Succeeded, string Error)> DownExerciseAsync(Guid exerciseId)
    {
        try
        {
            var exerciseFound = _puroguramuDbContext.Exercises.FirstOrDefault(e => e.ExerciseId == exerciseId);

            if (exerciseFound == null)
            {
                _logger.LogError($"DownExerciseAsync() : exercise with ID {exerciseId} not found");
                return (false, "Exercise not found");
            }

            var nextExerciseFound = _puroguramuDbContext.Exercises.FirstOrDefault(e => e.LessonId == exerciseFound.LessonId && e.Position == exerciseFound.Position + 1);

            if (nextExerciseFound != null)
            {
                var currentPos = exerciseFound.Position;
                var maxPos = _puroguramuDbContext.Exercises.Where(e => e.LessonId == exerciseFound.LessonId).Max(e => e.Position);

                exerciseFound.Position = maxPos + 1;
                _puroguramuDbContext.Exercises.Update(exerciseFound);
                await _puroguramuDbContext.SaveChangesAsync();

                nextExerciseFound.Position -= 1;
                _puroguramuDbContext.Exercises.Update(nextExerciseFound);
                await _puroguramuDbContext.SaveChangesAsync();

                exerciseFound.Position = currentPos + 1;
                _puroguramuDbContext.Exercises.Update(exerciseFound);
                await _puroguramuDbContext.SaveChangesAsync();
            }

            return (true, string.Empty);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"DownExerciseAsync() : an error occurred while updating exercise {exerciseId} position");
            return (false, "An error occurred while updating exercise position");
        }
    }

    public async Task<(bool Succeeded, string Error)> CreateExerciseAsync(Exercise exercise)
    {
        try
        {
            var lessonId = exercise.Lesson.LessonId;

            var lessonFound = _puroguramuDbContext.Lessons.FirstOrDefault(l => l.LessonId == lessonId);

            if (lessonFound == null)
            {
                _logger.LogError($"CreateExerciseAsync() : lesson with ID {lessonId} not found");
                return (false, "Lesson not found");
            }

            if (IsExerciseTitleExists(lessonId,exercise.Title))
            {
                _logger.LogError($"CreateExerciseAsync() : exercise title {exercise.Title} already exists in our system for lesson {lessonId}");
                return (false, "This exercise title already exists in our system for this lesson");
            }

            if (exercise.Title.Length < 5)
            {
                _logger.LogError($"CreateExerciseAsync() : the exercise title {exercise.Title} does not contain 5 characters");
                return (false, "The exercise title does not contain 5 characters");
            }

            var entityExercise = _mapper.Map<EntityExercise>(exercise);

            entityExercise.Lesson = lessonFound;

            var maxPosExercise = _puroguramuDbContext.Exercises.Any(e => e.LessonId == lessonId) ? _puroguramuDbContext.Exercises.Where(l => l.LessonId == lessonId).Max(l => l.Position) : 0;
            entityExercise.Position = maxPosExercise + 1;

            _puroguramuDbContext.Exercises.Add(entityExercise);
            await _puroguramuDbContext.SaveChangesAsync();

            return (true, string.Empty);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"CreateExerciseAsync() : an error occurred while creating exercise {exercise.ExerciseId} for lesson {exercise.Lesson.LessonId}");
            return (false, "An error occurred while creating exercise");
        }
    }

    public async Task<(bool Succeeded, string Error)> UpdateExerciseAsync(Exercise exercise)
    {
        try
        {
            var exerciseFound = _puroguramuDbContext.Exercises.FirstOrDefault(e => e.ExerciseId == exercise.ExerciseId);

            if (exerciseFound == null)
            {
                _logger.LogError($"UpdateExerciseAsync() : exercise with ID {exercise.ExerciseId} not found");
                return (false, "Exercise not found");
            }

            if (exercise.Title != exerciseFound.Title && IsExerciseTitleExists(exerciseFound.LessonId,exercise.Title))
            {
                _logger.LogError($"UpdateExerciseAsync() : exercise title {exercise.Title} already exists in our system for lesson {exerciseFound.LessonId}");
                return (false, "This exercise title already exists in our system for this lesson");
            }

            if (exercise.Title.Length < 5)
            {
                _logger.LogError($"UpdateExerciseAsync() : the exercise title {exercise.Title} does not contain 5 characters");
                return (false, "The exercise title does not contain 5 characters");
            }

            exerciseFound.Title = exercise.Title;
            exerciseFound.Instructions = exercise.Instructions;
            exerciseFound.Difficulty = exercise.Difficulty;
            exerciseFound.Template = exercise.Template;
            exerciseFound.Stub = exercise.Stub;
            exerciseFound.Solution = exercise.Solution;

            _puroguramuDbContext.Exercises.Update(exerciseFound);
            await _puroguramuDbContext.SaveChangesAsync();

            var attempts = _puroguramuDbContext.ExerciseAttempts.Where(ea => ea.ExerciseId == exerciseFound.ExerciseId && ea.ExerciseStatus == ExerciseStatus.Passed);

            foreach (var attempt in attempts)
            {
                attempt.ExerciseStatus = ExerciseStatus.Started;
                _puroguramuDbContext.ExerciseAttempts.Update(attempt);
            }

            await _puroguramuDbContext.SaveChangesAsync();

            return (true, string.Empty);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"UpdateExerciseAsync() : an error occurred while updating exercise {exercise.ExerciseId} for lesson {exercise.Lesson.LessonId}");
            return (false, "An error occurred while updating exercise");
        }
    }

    public async Task<(bool Succeeded, string Error)> DeleteExerciseAsync(Guid exerciseId)
    {
        try
        {
            var exerciseFound = _puroguramuDbContext.Exercises.FirstOrDefault(e => e.ExerciseId == exerciseId);

            if (exerciseFound == null)
            {
                _logger.LogError($"DeleteExerciseAsync() : exercise with ID {exerciseId} not found");
                return (false, "Exercise not found");
            }

            _puroguramuDbContext.Exercises.Remove(exerciseFound);
            await _puroguramuDbContext.SaveChangesAsync();

            return (true, string.Empty);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"DeleteExerciseAsync() : an error occurred while deleting exercise {exerciseId}");
            return (false, "An error occurred while deleting exercise");
        }
    }

    public async Task<(bool Succeeded, string Error)> ResetExerciseAsync(Guid exerciseId)
    {
        try
        {
            var attemptsFound = _puroguramuDbContext.ExerciseAttempts.Where(ea => ea.ExerciseId == exerciseId);

            foreach (var attempt in attemptsFound)
            {
                _puroguramuDbContext.ExerciseAttempts.Remove(attempt);
            }

            await _puroguramuDbContext.SaveChangesAsync();

            return (true, string.Empty);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"ResetExerciseAsync() : an error occurred during exercise {exerciseId} reset");
            return (false, "An error occurred during exercise reset");
        }
    }

    private bool IsExerciseTitleExists(Guid lessonId, string title)
        => _puroguramuDbContext.Exercises.Any(e => e.LessonId == lessonId && e.Title == title);
}
