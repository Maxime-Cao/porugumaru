#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.Domains;

namespace Puroguramu.App.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IUsersRepository _usersRepository;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(IUsersRepository usersRepository, ILogger<LoginModel> logger)
        {
            _usersRepository = usersRepository;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Email or matricule")]
            public string EmailOrMatricule { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToPage("/Index");
            }

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            ReturnUrl = returnUrl;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var result = await _usersRepository.SignInAsync(Input.EmailOrMatricule?.ToLower(), Input.Password, Input.RememberMe);

                if (result == AuthResult.Success)
                {
                    _logger.LogInformation("User logged in.");

                    var role = await _usersRepository.GetRoleAsync(Input.EmailOrMatricule!.ToLower());

                    if (!string.IsNullOrEmpty(role))
                    {
                        if (role.Contains("Teacher"))
                        {
                            return RedirectToPage("/Teacher/Index");
                        }

                        if (role.Contains("Student"))
                        {
                            return RedirectToPage("/Student/Index");
                        }
                    }

                    return LocalRedirect(returnUrl);
                }

                if (result == AuthResult.RequiresTwoFactor)
                {
                    _logger.LogWarning("User account requires two-factor authentication");
                    ModelState.AddModelError(string.Empty, "Your account requires two-factor authentication");
                    return Page();
                }

                if (result == AuthResult.Lockout)
                {
                    _logger.LogWarning("User account locked out");
                    ModelState.AddModelError(string.Empty, "Your account is locked out");
                    return Page();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty,"An error occured, please try again later.");
            }

            return Page();
        }
    }
}
