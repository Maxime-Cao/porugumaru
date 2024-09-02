using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.Domains;

namespace Puroguramu.App.Pages.Teacher;

[Authorize(Roles = "Teacher")]
public class CreateExerciseModel : PageModel
{
    private readonly ILessonsRepository _lessonsRepository;
    private readonly IExercisesRepository _exercisesRepository;
    private readonly ILogger<CreateExerciseModel> _logger;

    public CreateExerciseModel(ILessonsRepository lessonsRepository,IExercisesRepository exercisesRepository, ILogger<CreateExerciseModel> logger)
    {
        _lessonsRepository = lessonsRepository;
        _exercisesRepository = exercisesRepository;
        _logger = logger;
    }

    [BindProperty]
    public Guid LessonId { get; set; }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [Display(Name = "Title")]
        [StringLength(255)]
        [RegularExpression(@"^.{5,255}$",ErrorMessage = "The exercise title must contain at least 5 characters")]
        public string Title { get; set; }
    }

    public IActionResult OnGet(Guid lessonId)
    {
        var lesson = _lessonsRepository.GetLesson(lessonId, false);

        if (lesson == null)
        {
            _logger.LogError($"OnGet() : lesson {lessonId} not found");
            return NotFound("Lesson not found");
        }

        LessonId = lesson.LessonId;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var lesson = _lessonsRepository.GetLesson(LessonId, false);

        if (lesson == null)
        {
            _logger.LogError($"OnPostAsync() : lesson {LessonId} not found");
            return NotFound("Lesson not found");
        }

        LessonId = lesson.LessonId;

        if (ModelState.IsValid)
        {
            var newExercise = new Exercise()
            {
                ExerciseId = Guid.NewGuid(),
                Title = Input.Title.Trim(),
                Instructions = "No instructions",
                Difficulty = "Easy",
                Template = "No template",
                Stub = "public class Exercice\n{\n  // Tapez votre code ici\n}\n",
                Solution = "No solution",
                Position = -1,
                IsPublished = false,
                Lesson = lesson,
                ExerciseAttempts = new List<ExerciseAttempt>(),
            };

            var createExerciseResult = await _exercisesRepository.CreateExerciseAsync(newExercise);

            if (!createExerciseResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty,createExerciseResult.Error);
            }
            else
            {
                return RedirectToPage("/Teacher/Index");
            }
        }

        return Page();
    }
}
