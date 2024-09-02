using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.App.ViewModels;
using Puroguramu.Domains;

namespace Puroguramu.App.Pages;

public class IndexModel : PageModel
{
    private readonly IUsersRepository _usersRepository;
    private readonly ILessonsRepository _lessonsRepository;
    private readonly IExercisesRepository _exercisesRepository;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IUsersRepository usersRepository, ILessonsRepository lessonsRepository, IExercisesRepository exercisesRepository,ILogger<IndexModel> logger)
    {
        _usersRepository = usersRepository;
        _lessonsRepository = lessonsRepository;
        _exercisesRepository = exercisesRepository;
        _logger = logger;
    }

    public HomeViewModel Home { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var role = string.Empty;

        if (User?.Identity?.IsAuthenticated ?? false)
        {
            var user = await _usersRepository.GetUserAsync(User);

            if (user == null)
            {
                _logger.LogError($"OnGetAsync() : unable to load user {User?.Identity?.Name}");
                return NotFound("Unable to load user");
            }

            role = await _usersRepository.GetRoleAsync(user);

            if (role == null)
            {
                _logger.LogError($"OnGetAsync() :  unable to load user role for user {user.Matricule}");
                return NotFound("Unable to load user role");
            }
        }

        Home = new HomeViewModel()
        {
            NumberOfStudents = _usersRepository.GetNumberStudents(),
            NumberOfLessons = _lessonsRepository.GetNumberOfLessons(),
            NumberOfExercises = _exercisesRepository.GetNumberOfExercises(),
            IsAuthenticated = User?.Identity?.IsAuthenticated ?? false,
            Role = role,
        };

        return Page();
    }
}
