using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.Domains;

namespace Puroguramu.App.Pages.Teacher;

[Authorize(Roles = "Teacher")]
public class CreateLessonModel : PageModel
{
    private readonly ILessonsRepository _lessonsRepository;
    private readonly ILogger<CreateLessonModel> _logger;

    public CreateLessonModel(ILessonsRepository lessonsRepository, ILogger<CreateLessonModel> logger)
    {
        _lessonsRepository = lessonsRepository;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [Display(Name = "Title")]
        [StringLength(255)]
        [RegularExpression(@"^.{5,255}$",ErrorMessage = "The lesson title must contain at least 5 characters")]
        public string Title { get; set; }
    }

    public IActionResult OnGet() => Page();

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var newLesson = new Lesson()
            {
                LessonId = Guid.NewGuid(),
                Title = Input.Title!.Trim(),
                Description = "No description",
                Position = -1,
                IsPublished = false,
                Exercises = new List<Exercise>(),
            };

            var createLessonResult = await _lessonsRepository.CreateLessonAsync(newLesson);

            if (!createLessonResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty,createLessonResult.Error);
            }
            else
            {
                return RedirectToPage("/Teacher/Index");
            }
        }

        return Page();
    }

}
