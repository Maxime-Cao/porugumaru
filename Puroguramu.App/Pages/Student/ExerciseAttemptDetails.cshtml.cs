using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.App.ViewModels;
using Puroguramu.Domains;

namespace Puroguramu.App.Pages.Student;

[Authorize(Roles = "Student")]
public class ExerciseAttemptDetailsModel : PageModel
{
    private readonly IUsersRepository _usersRepository;
    private readonly IExercisesRepository _exercisesRepository;
    private readonly ILogger<ExerciseAttemptDetailsModel> _logger;

    public ExerciseAttemptDetailsModel(IUsersRepository usersRepository,IExercisesRepository exercisesRepository, ILogger<ExerciseAttemptDetailsModel> logger)
    {
        _usersRepository = usersRepository;
        _exercisesRepository = exercisesRepository;
        _logger = logger;
    }

    public AttemptViewModel Attempt { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid attemptId)
    {
        var user = await _usersRepository.GetUserAsync(User);

        if (user == null)
        {
            _logger.LogError($"OnGetAsync() => unable to load user {User?.Identity?.Name}");
            return NotFound("Unable to load user");
        }

        var attempt = _exercisesRepository.GetExerciseAttempt(attemptId);

        if (attempt == null)
        {
            _logger.LogError($"OnGetAsync() : attempt with ID {attemptId} not found");
            return NotFound("Attempt not found");
        }

        if (attempt.Student.Matricule != user.Matricule)
        {
            _logger.LogError($"OnGetAsync() : user with matricule {user.Matricule} does not have access to attempt {attempt.AttemptId}");
            return NotFound("Attempt not found");
        }

        Attempt = new AttemptViewModel()
        {
            Time = attempt.AttemptTime.ToString("dd-MM-yyyy HH:mm"),
            AttemptContent = attempt.Proposal,
            Status = attempt.ExerciseStatus switch
            {
                ExerciseStatus.NotStarted => "Not Started",
                ExerciseStatus.Started => "Started",
                ExerciseStatus.Passed => "Succeeded",
                ExerciseStatus.Failed => "Failed",
                _ => "Unknown"
            },
        };

        return Page();
    }
}
