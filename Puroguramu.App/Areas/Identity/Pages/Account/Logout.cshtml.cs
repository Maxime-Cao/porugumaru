#nullable disable

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.Domains;

namespace Puroguramu.App.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly IUsersRepository _usersRepository;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(IUsersRepository usersRepository,ILogger<LogoutModel> logger)
        {
            _usersRepository = usersRepository;
            _logger = logger;
        }

        public bool IsAuthenticated { get; set; }

        public async Task<IActionResult> OnGet()
        {
            IsAuthenticated = User?.Identity?.IsAuthenticated ?? false;
            return Page();
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _usersRepository.SignOutAsync();
            _logger.LogInformation("OnPost() : user logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage();
            }
        }
    }
}
