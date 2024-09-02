using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.App.ViewModels;
using Puroguramu.Domains;

namespace Puroguramu.App.Pages.Student;

[Authorize(Roles = "Student")]
public class IndexModel : PageModel
{
    private readonly IUsersRepository _usersRepository;
    private readonly ILessonsRepository _lessonsRepository;
    private readonly IExercisesRepository _exercisesRepository;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IUsersRepository usersRepository,ILessonsRepository lessonsRepository,IExercisesRepository exercisesRepository,ILogger<IndexModel> logger)
    {
        _usersRepository = usersRepository;
        _lessonsRepository = lessonsRepository;
        _exercisesRepository = exercisesRepository;
        _logger = logger;
    }

    public IEnumerable<StudentLessonViewModel> Lessons { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _usersRepository.GetUserAsync(User);

        if (user == null)
        {
            _logger.LogError($"OnGetAsync() : unable to load user {User?.Identity?.Name}");
            return NotFound("Unable to load user");
        }

        var lessons = _lessonsRepository.GetAllLessons(true);

        Lessons = lessons.Select(l => new StudentLessonViewModel()
        {
            LessonId = l.LessonId,
            Title = l.Title,
            ExercisesCount = l.Exercises.Count(),
            ExercisesCompletedCount = _exercisesRepository.GetCompletedExercisesCount(l.LessonId,user.Matricule),
            NextExercise = _exercisesRepository.GetNextPublishedExercise(l.LessonId,user.Matricule),
        });

        return Page();
    }
}
