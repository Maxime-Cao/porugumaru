using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.App.ViewModels;
using Puroguramu.Domains;

namespace Puroguramu.App.Pages.Teacher;

[Authorize(Roles = "Teacher")]
public class IndexModel : PageModel
{
    private readonly IUsersRepository _usersRepository;
    private readonly ILessonsRepository _lessonsRepository;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IUsersRepository usersRepository, ILessonsRepository lessonsRepository, ILogger<IndexModel> logger)
    {
        _usersRepository = usersRepository;
        _lessonsRepository = lessonsRepository;
        _logger = logger;
    }

    public IEnumerable<TeacherLessonViewModel> Lessons { get; set; } = new List<TeacherLessonViewModel>();

    public int TotalStudents { get; set; }

    public IActionResult OnGet()
    {
        var lessons = _lessonsRepository.GetAllLessons(false).ToList();

        if (lessons.Count > 0)
        {
            var minPosLesson = lessons.Min(l => l.Position);
            var maxPosLesson = lessons.Max(l => l.Position);

            Lessons = lessons.Select(l => new TeacherLessonViewModel()
            {
                LessonId = l.LessonId,
                Title = l.Title,
                IsPublished = l.IsPublished,
                CompletedStudentCount = _lessonsRepository.GetCompletedStudentCount(l.LessonId),
                IsFirstLesson = l.Position == minPosLesson,
                IsLastLesson = l.Position == maxPosLesson,
            });

            TotalStudents = _usersRepository.GetNumberStudents();
        }

        return Page();
    }

    public async Task<IActionResult> OnGetHideLessonAsync(Guid lessonId)
    {
        var hideLessonResult = await _lessonsRepository.HideLessonAsync(lessonId);

        if (!hideLessonResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty,hideLessonResult.Error);
            return OnGet();
        }

        return RedirectToPage("/Teacher/Index");
    }

    public async Task<IActionResult> OnGetPublishLessonAsync(Guid lessonId)
    {
        var publishLessonResult = await _lessonsRepository.PublishLessonAsync(lessonId);

        if (!publishLessonResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty,publishLessonResult.Error);
            return OnGet();
        }

        return RedirectToPage("/Teacher/Index");
    }

    public async Task<IActionResult> OnGetUpLessonAsync(Guid lessonId)
    {
        var upLessonResult = await _lessonsRepository.UpLessonAsync(lessonId);

        if (!upLessonResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty,upLessonResult.Error);
            return OnGet();
        }

        return RedirectToPage("/Teacher/Index");
    }

    public async Task<IActionResult> OnGetDownLessonAsync(Guid lessonId)
    {
        var downLessonResult = await _lessonsRepository.DownLessonAsync(lessonId);

        if (!downLessonResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty,downLessonResult.Error);
            return OnGet();
        }

        return RedirectToPage("/Teacher/Index");
    }

    public async Task<IActionResult> OnGetDeleteLessonAsync(Guid lessonId)
    {
        var deleteLessonResult = await _lessonsRepository.DeleteLessonAsync(lessonId);

        if (!deleteLessonResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty,deleteLessonResult.Error);
            return OnGet();
        }

        return RedirectToPage("/Teacher/Index");
    }

    public async Task<IActionResult> OnGetResetLessonAsync(Guid lessonId)
    {
        var resetLessonResult = await _lessonsRepository.ResetLessonAsync(lessonId);

        if (!resetLessonResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty, resetLessonResult.Error);
            return OnGet();
        }

        return RedirectToPage("/Teacher/Index");
    }
}
