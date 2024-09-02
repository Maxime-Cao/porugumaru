using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.App.ViewModels;
using Puroguramu.Domains;

namespace Puroguramu.App.Pages.Teacher;

[Authorize(Roles = "Teacher")]
public class EditLessonModel : PageModel
{
    private readonly ILessonsRepository _lessonsRepository;
    private readonly IExercisesRepository _exercisesRepository;
    private readonly ILogger<EditLessonModel> _logger;

    public EditLessonModel(ILessonsRepository lessonsRepository,IExercisesRepository exercisesRepository,ILogger<EditLessonModel> logger)
    {
        _lessonsRepository = lessonsRepository;
        _exercisesRepository = exercisesRepository;
        _logger = logger;
    }

    public TeacherLessonViewModel Lesson { get; set; }

    [BindProperty]
    public Guid LessonId { get; set; }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [Display(Name = "Title")]
        [StringLength(255)]
        [RegularExpression(@"^.{5,255}$",ErrorMessage = "The lesson title must contain at least 5 characters")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Description")]
        [StringLength(255, ErrorMessage = "The description must contain at least 1 character", MinimumLength = 1)]
        public string Description { get; set; }
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

        Lesson = new TeacherLessonViewModel()
        {
            LessonId = lesson.LessonId,
            Title = lesson.Title,
            Description = lesson.Description,
        };

        var exercises = lesson.Exercises.ToList();

        if (exercises.Count > 0)
        {
            var minPosExercise = exercises.Min(e => e.Position);
            var maxPosExercise = exercises.Max(e => e.Position);

            Lesson.Exercises = exercises.Select(e => new TeacherExerciseViewModel()
            {
                ExerciseId = e.ExerciseId,
                Title = e.Title,
                IsPublished = e.IsPublished,
                IsFirstExercise = e.Position == minPosExercise,
                IsLastExercise = e.Position == maxPosExercise,
            });
        }
        else
        {
            Lesson.Exercises = new List<TeacherExerciseViewModel>();
        }

        Input = new InputModel()
        {
            Title = Lesson.Title,
            Description = Lesson.Description,
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var lesson = _lessonsRepository.GetLesson(LessonId, false);

            if (lesson == null)
            {
                _logger.LogError($"OnPostAsync() : lesson {LessonId} not found");
                return NotFound("Lesson not found");
            }

            lesson.Title = Input.Title.Trim();
            lesson.Description = Input.Description.Trim();

            var updateLessonResult = await _lessonsRepository.UpdateLessonAsync(lesson);

            if (updateLessonResult.Succeeded)
            {
                return RedirectToPage("/Teacher/Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty,updateLessonResult.Error);
            }
        }

        return OnGet(LessonId);
    }

    public async Task<IActionResult> OnGetUpExerciseAsync(Guid lessonId,Guid exerciseId)
    {
        var upExerciseResult = await _exercisesRepository.UpExerciseAsync(exerciseId);

        if (!upExerciseResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty,upExerciseResult.Error);
            return OnGet(lessonId);
        }

        return RedirectToPage("/Teacher/EditLesson", new { lessonId });
    }

    public async Task<IActionResult> OnGetDownExerciseAsync(Guid lessonId,Guid exerciseId)
    {
        var downExerciseResult = await _exercisesRepository.DownExerciseAsync(exerciseId);

        if (!downExerciseResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty,downExerciseResult.Error);
            return OnGet(lessonId);
        }

        return RedirectToPage("/Teacher/EditLesson", new { lessonId });
    }

    public async Task<IActionResult> OnGetHideExerciseAsync(Guid lessonId,Guid exerciseId)
    {
        var hideExerciseResult = await _exercisesRepository.HideExerciseAsync(exerciseId);

        if (!hideExerciseResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty,hideExerciseResult.Error);
            return OnGet(lessonId);
        }

        return RedirectToPage("/Teacher/EditLesson", new { lessonId });
    }

    public async Task<IActionResult> OnGetPublishExerciseAsync(Guid lessonId,Guid exerciseId)
    {
        var publishExerciseResult = await _exercisesRepository.PublishExerciseAsync(exerciseId);

        if (!publishExerciseResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty,publishExerciseResult.Error);
            return OnGet(lessonId);
        }

        return RedirectToPage("/Teacher/EditLesson", new { lessonId });
    }

    public async Task<IActionResult> OnGetDeleteExerciseAsync(Guid lessonId,Guid exerciseId)
    {
        var deleteExerciseResult = await _exercisesRepository.DeleteExerciseAsync(exerciseId);

        if (!deleteExerciseResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty,deleteExerciseResult.Error);
            return OnGet(lessonId);
        }

        return RedirectToPage("/Teacher/EditLesson", new { lessonId });
    }

    public async Task<IActionResult> OnGetResetLessonAsync(Guid lessonId)
    {
        var resetLessonResult = await _lessonsRepository.ResetLessonAsync(lessonId);

        if (!resetLessonResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty,resetLessonResult.Error);
            return OnGet(lessonId);
        }

        return RedirectToPage("/Teacher/EditLesson", new { lessonId });
    }

    public async Task<IActionResult> OnGetResetExerciseAsync(Guid lessonId, Guid exerciseId)
    {
        var resetExerciseResult = await _exercisesRepository.ResetExerciseAsync(exerciseId);

        if (!resetExerciseResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty,resetExerciseResult.Error);
            return OnGet(lessonId);
        }

        return RedirectToPage("/Teacher/EditLesson", new { lessonId });
    }
}
