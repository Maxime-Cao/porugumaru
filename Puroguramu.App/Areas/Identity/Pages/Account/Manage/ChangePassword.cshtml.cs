#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.Domains;

namespace Puroguramu.App.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly IUsersRepository _usersRepository;
        private readonly ILogger<ChangePasswordModel> _logger;

        public ChangePasswordModel(
            IUsersRepository usersRepository,
            ILogger<ChangePasswordModel> logger)
        {
            _usersRepository = usersRepository;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _usersRepository.GetUserAsync(User);

            if (user == null)
            {
                _logger.LogError($"OnGetAsync() : unable to load user {User?.Identity?.Name}");
                return NotFound("Unable to load user");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _usersRepository.GetUserAsync(User);

            if (user == null)
            {
                _logger.LogError($"OnPostAsync() : unable to load user {User?.Identity?.Name}");
                return NotFound("Unable to load user");
            }

            var changePasswordResult = await _usersRepository.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                if(!string.IsNullOrEmpty(changePasswordResult.Error))
                {
                    ModelState.AddModelError(string.Empty, changePasswordResult.Error);
                }

                return Page();
            }

            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";
            return RedirectToPage();
        }
    }
}
