using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.App.ViewModels;
using Puroguramu.Domains;

namespace Puroguramu.App.Pages.Student;

[Authorize(Roles = "Student")]
public class DetailsModel : PageModel
{
    private readonly IUsersRepository _usersRepository;
    private readonly ILessonsRepository _lessonsRepository;
    private readonly IExercisesRepository _exercisesRepository;
    private readonly ILogger<DetailsModel> _logger;

    public DetailsModel(IUsersRepository usersRepository, ILessonsRepository lessonsRepository, IExercisesRepository exercisesRepository, ILogger<DetailsModel> logger)
    {
        _usersRepository = usersRepository;
        _lessonsRepository = lessonsRepository;
        _exercisesRepository = exercisesRepository;
        _logger = logger;
    }

    public StudentLessonViewModel StudentLesson { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid lessonId)
    {
        var user = await _usersRepository.GetUserAsync(User);

        if (user == null)
        {
            _logger.LogError($"OnGetAsync() : unable to load user {User?.Identity?.Name}");
            return NotFound("Unable to load user");
        }

        var lesson = _lessonsRepository.GetLesson(lessonId,true);

        if (lesson == null || !lesson.IsPublished)
        {
            _logger.LogError($"OnGetAsync() : unable to load lesson {lessonId}");
            return NotFound("Lesson not found");
        }

        var exercises = lesson.Exercises;

        StudentLesson = new StudentLessonViewModel()
        {
            Title = lesson.Title,
            Description = lesson.Description,
            Exercises = exercises.Select(e => new StudentExerciseViewModel()
            {
                ExerciseId = e.ExerciseId,
                Title = e.Title,
                Difficulty = e.Difficulty,
                ExerciseStatus = _exercisesRepository.GetExerciseStatus(e.ExerciseId,user.Matricule) switch
                {
                    ExerciseStatus.NotStarted => "Not Started",
                    ExerciseStatus.Started => "Started",
                    ExerciseStatus.Passed => "Succeeded",
                    ExerciseStatus.Failed => "Failed",
                    _ => "Unknown"
                },
            }),
        };

        return Page();
    }
}
