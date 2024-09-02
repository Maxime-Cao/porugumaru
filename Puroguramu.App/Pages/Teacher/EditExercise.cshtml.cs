using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.Domains;

namespace Puroguramu.App.Pages.Teacher;

[Authorize(Roles = "Teacher")]
public class EditExerciseModel : PageModel
{
    private readonly IExercisesRepository _exercisesRepository;
    private readonly IAssessExercise _assessor;
    private readonly ILogger<EditExerciseModel> _logger;

    public EditExerciseModel(IExercisesRepository exercisesRepository,IAssessExercise assessor,ILogger<EditExerciseModel> logger)
    {
        _exercisesRepository = exercisesRepository;
        _assessor = assessor;
        _logger = logger;
    }

    [BindProperty]
    public List<string> Difficulties { get; set; }

    [BindProperty]
    public Guid LessonId { get; set; }

    [BindProperty]
    public Guid ExerciseId { get; set; }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [Display(Name = "Title")]
        [StringLength(255)]
        [RegularExpression(@"^.{5,255}$",ErrorMessage = "The exercise title must contain at least 5 characters")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Instructions")]
        [RegularExpression(@"^.{1,}$",ErrorMessage = "The exercise instructions must contain at least 1 character")]
        public string Instructions { get; set; }

        [Required]
        [Display(Name = "Difficulty")]
        [StringLength(15)]
        [RegularExpression(@"^.{1,15}$",ErrorMessage = "The exercise difficulty must contain at least 1 character")]
        public string Difficulty { get; init; }

        [Required]
        [Display(Name = "Template")]
        [MinLength(1,ErrorMessage = "The exercise template must contain at least 1 character")]
        public string Template { get; set; }

        [Required]
        [Display(Name = "Stub")]
        [MinLength(1,ErrorMessage = "The exercise stub must contain at least 1 character")]
        public string Stub { get; set; }

        [Required]
        [Display(Name = "Solution")]
        [MinLength(1,ErrorMessage = "The exercise solution must contain at least 1 character")]
        public string Solution { get; set; }
    }

    public IActionResult OnGet(Guid lessonId,Guid exerciseId)
    {
        var exercise = _exercisesRepository.GetExercise(exerciseId);

        if (exercise == null)
        {
            _logger.LogError($"OnGet() : exercise {exerciseId} not found");
            return NotFound("Exercise not found");
        }

        LessonId = lessonId;
        ExerciseId = exerciseId;

        Difficulties = new List<string> { "Easy", "Medium", "Hard" };

        Input = new InputModel()
        {
            Title = exercise.Title,
            Instructions = exercise.Instructions,
            Difficulty = exercise.Difficulty,
            Template = exercise.Template,
            Stub = exercise.Stub,
            Solution = exercise.Solution,
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var exercise = _exercisesRepository.GetExercise(ExerciseId);

            if (exercise == null)
            {
                _logger.LogError($"OnPostAsync() : exercise {ExerciseId} not found");
                return NotFound("Exercise not found");
            }

            Difficulties = new List<string> { "Easy", "Medium", "Hard" };

            if (!VerifyDifficulty(Input.Difficulty.Trim()))
            {
                ModelState.AddModelError(string.Empty,"Difficulty must be : Easy, Medium or Hard");
                return OnGet(LessonId, ExerciseId);
            }

            exercise.Title = Input.Title.Trim();
            exercise.Instructions = Input.Instructions.Trim();
            exercise.Difficulty = Input.Difficulty.Trim();
            exercise.Template = Input.Template.Trim();
            exercise.Stub = Input.Stub.Trim();

            var updateExerciseResult = await _exercisesRepository.UpdateExerciseAsync(exercise);

            if (!updateExerciseResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty,updateExerciseResult.Error);
                return OnGet(LessonId, ExerciseId);
            }

            var validateSolutionResult = await _assessor.Assess(exercise.ExerciseId, Input.Solution.Trim());

            if (validateSolutionResult.Status != ExerciseStatus.Passed)
            {
                ModelState.AddModelError(string.Empty,"Invalid solution compared to the given template");

                if (validateSolutionResult?.TestResults != null)
                {
                    foreach (var testResult in validateSolutionResult.TestResults)
                    {
                        if (testResult.Status != TestStatus.Passed)
                        {
                            ModelState.AddModelError(string.Empty,$"Test status : {testResult.Status} => {testResult.ErrorMessage}");
                        }
                    }
                }
            }
            else
            {
                exercise.Solution = Input.Solution.Trim();

                var updateSolutionResult = await _exercisesRepository.UpdateExerciseAsync(exercise);

                if (!updateSolutionResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty,updateSolutionResult.Error);
                    return OnGet(LessonId, ExerciseId);
                }
                else
                {
                    return RedirectToPage("/Teacher/EditLesson", new { lessonId = LessonId });
                }
            }
        }

        return OnGet(LessonId, ExerciseId);
    }

    public async Task<IActionResult> OnGetResetExerciseAsync(Guid lessonId, Guid exerciseId)
    {
        var resetExerciseResult = await _exercisesRepository.ResetExerciseAsync(exerciseId);

        if (!resetExerciseResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty,resetExerciseResult.Error);
            return OnGet(lessonId, exerciseId);
        }

        return RedirectToPage("/Teacher/EditLesson", new { lessonId });
    }

    private bool VerifyDifficulty(string difficulty) => Difficulties.Contains(difficulty);
}
