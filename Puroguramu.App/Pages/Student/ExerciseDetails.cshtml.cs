using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.App.ViewModels;
using Puroguramu.Domains;

namespace Puroguramu.App.Pages.Student;

[Authorize(Roles = "Student")]
public class ExerciseDetailsModel : PageModel
{
    private readonly IAssessExercise _assesor;
    private readonly IUsersRepository _usersRepository;
    private readonly IExercisesRepository _exercisesRepository;
    private readonly ILogger<ExerciseDetailsModel> _logger;

    private ExerciseResult? _result;

    public StudentExerciseViewModel StudentExercise { get; set; }

    public IEnumerable<AttemptViewModel> Attempts { get; set; }

    [BindProperty]
    public Guid ExerciseId { get; set; }

    public string ExerciseResultStatus { get; set; }

    public Guid NextExercise { get; set; }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        public string Proposal { get; set; } = string.Empty;
    }

    public IEnumerable<TestResultViewModel> TestResult
        => _result
            ?.TestResults
            ?.Select(result => new TestResultViewModel(result)) ?? Array.Empty<TestResultViewModel>();

    public ExerciseDetailsModel(IAssessExercise assesor,IUsersRepository usersRepository,IExercisesRepository exercisesRepository,ILogger<ExerciseDetailsModel> logger)
    {
        _assesor = assesor;
        _usersRepository = usersRepository;
        _exercisesRepository = exercisesRepository;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(Guid exerciseId)
    {
        var user = await _usersRepository.GetUserAsync(User);

        if (user == null)
        {
            _logger.LogError($"OnGetAsync() : unable to load user {User?.Identity?.Name}");
            return NotFound("Unable to load user");
        }

        var exercise = _exercisesRepository.GetExercise(exerciseId);

        if (exercise == null || !exercise.IsPublished)
        {
            _logger.LogError($"OnGetAsync() : unable to load exercise {exerciseId}");
            return NotFound("Exercise not found");
        }

        StudentExercise = new StudentExerciseViewModel()
        {
            Title = exercise.Title,
            Difficulty = exercise.Difficulty,
            Instructions = exercise.Instructions,
            Solution = exercise.Solution,
        };

        ExerciseId = exercise.ExerciseId;

        var attempt = _exercisesRepository.GetLastExerciceAttempt(exerciseId, user.Matricule);

        if (attempt == null)
        {
            _result = await _assesor.StubForExercise(exerciseId);
            Input = new InputModel() { Proposal = _result.Proposal };
            SetExerciseStatus(_result.Status);
        }
        else
        {
            Input = new InputModel() { Proposal = attempt.Proposal };
            SetExerciseStatus(attempt.ExerciseStatus);
        }

        NextExercise = _exercisesRepository.GetNextPublishedExercise(exercise.Lesson.LessonId, user.Matricule);

        var attempts = _exercisesRepository.GetAllAttempts(exerciseId, user.Matricule).OrderBy(a => a.AttemptTime).ToList();

        Attempts = attempts.Select(exerciseAttempt => new AttemptViewModel()
        {
            AttemptId = exerciseAttempt.AttemptId,
        });

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var user = await _usersRepository.GetUserAsync(User);

            if (user == null)
            {
                _logger.LogError($"OnPostAsync() : unable to load user {User?.Identity?.Name}");
                return NotFound("Unable to load user");
            }

            var student = _usersRepository.LoadStudent(user.Matricule);

            var exercise = _exercisesRepository.GetExercise(ExerciseId);

            if (exercise == null || !exercise.IsPublished)
            {
                _logger.LogError($"OnPostAsync() : unable to load exercise {ExerciseId}");
                return NotFound("Exercise not found");
            }

            var attempt = _exercisesRepository.GetLastExerciceAttempt(exercise.ExerciseId, user.Matricule);

            if (attempt != null && (attempt.ExerciseStatus == ExerciseStatus.Failed || attempt.ExerciseStatus == ExerciseStatus.Passed))
            {
                return await OnGetAsync(ExerciseId);
            }

            _result = await _assesor.Assess(exercise.ExerciseId, Input.Proposal);

            var createResult = await _exercisesRepository.CreateExerciseAttemptAsync(new ExerciseAttempt()
            {
                AttemptId = Guid.NewGuid(),
                Proposal = _result.Proposal,
                ExerciseStatus = _result.Status,
                AttemptTime = DateTime.Now,
                Exercise = exercise,
                Student = student!,
            });

            if (!string.IsNullOrEmpty(createResult.Error))
            {
                ModelState.AddModelError(string.Empty,createResult.Error);
            }

            StudentExercise = new StudentExerciseViewModel()
            {
                Title = exercise.Title,
                Difficulty = exercise.Difficulty,
                Instructions = exercise.Instructions,
                Solution = exercise.Solution,
            };

            Input = new InputModel() { Proposal = _result.Proposal };
            SetExerciseStatus(_result.Status);
            NextExercise = _exercisesRepository.GetNextPublishedExercise(exercise.Lesson.LessonId, user.Matricule);

            var attempts = _exercisesRepository.GetAllAttempts(exercise.ExerciseId, user.Matricule).OrderBy(a => a.AttemptTime).ToList();

            Attempts = attempts.Select(exerciseAttempt => new AttemptViewModel()
            {
                AttemptId = exerciseAttempt.AttemptId,
            });

            return Page();
        }
        else
        {
            return await OnGetAsync(ExerciseId);
        }
    }

    public async Task<IActionResult> OnGetShowSolutionAsync(Guid exerciseId)
    {
        var user = await _usersRepository.GetUserAsync(User);

        if (user == null)
        {
            _logger.LogError($"OnGetShowSolutionAsync() : unable to load user {User?.Identity?.Name}");
            return NotFound("Unable to load user");
        }

        var student = _usersRepository.LoadStudent(user.Matricule);

        var exercise = _exercisesRepository.GetExercise(exerciseId);

        if (exercise == null || !exercise.IsPublished)
        {
            _logger.LogError($"OnGetShowSolutionAsync() : unable to load exercise {exerciseId}");
            return NotFound("Exercise not found");
        }

        var createResult = await _exercisesRepository.CreateExerciseAttemptAsync(new ExerciseAttempt()
        {
            AttemptId = Guid.NewGuid(),
            Proposal = exercise.Stub,
            ExerciseStatus = ExerciseStatus.Failed,
            AttemptTime = DateTime.Now,
            Exercise = exercise,
            Student = student!,
        });

        if (!string.IsNullOrEmpty(createResult.Error))
        {
            ModelState.AddModelError(string.Empty,createResult.Error);
            return await OnGetAsync(exerciseId);
        }

        return RedirectToPage("/Student/ExerciseDetails", new { exerciseId });
    }

    private string SetExerciseStatus(ExerciseStatus exerciseStatus) =>
        ExerciseResultStatus = exerciseStatus switch
        {
            ExerciseStatus.NotStarted => "Not Started",
            ExerciseStatus.Started => "Started",
            ExerciseStatus.Passed => "Succeeded",
            ExerciseStatus.Failed => "Failed",
            _ => "Unknown"
        };
}
