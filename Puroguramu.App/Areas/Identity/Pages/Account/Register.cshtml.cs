// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.Domains;

namespace Puroguramu.App.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IGroupsRepository _groupsRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            IGroupsRepository groupsRepository,
            IUsersRepository usersRepository,
            ILogger<RegisterModel> logger)
        {
            _groupsRepository = groupsRepository;
            _usersRepository = usersRepository;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty]
        public List<GroupLab> Groups { get; set; }
        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Matricule")]
            [RegularExpression(@"^[a-zA-Z]{1}[0-9]{6}$",ErrorMessage = "Matricule must be 1 letter & 6 digits")]
            public string Matricule { get; set; }

            [Required]
            [Display(Name = "Name")]
            [RegularExpression(@"^.{1,}$",ErrorMessage = "Name must be at least 1 character")]
            public string Name { get; set; }

            [Required]
            [Display(Name = "Firstname")]
            [RegularExpression(@"^.{1,}$",ErrorMessage = "Firstname must be at least 1 character")]
            public string Firstname { get; set; }

            [Required]
            [RegularExpression(@"^[a-zA-Z]+\.{1}[a-zA-Z]+@student\.helmo\.be$",ErrorMessage = "Please provide a valid HELMo student e-mail address")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "Group")]
            public string Group { get; set; }
        }


        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToPage("/Index");
            }

            Groups = _groupsRepository.GetAllGroups().ToList();
            ReturnUrl = returnUrl;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            try
            {
                Groups = _groupsRepository.GetAllGroups().ToList();
                returnUrl ??= Url.Content("~/");

                if (ModelState.IsValid)
                {
                    var user = CreateStudent(Input.Matricule,Input.Name,Input.Firstname,Input.Email,Input.Group);

                    if (user == null)
                    {
                        return Page();
                    }

                    var result = await _usersRepository.CreateStudentAsync(user,Input.Password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");
                        return LocalRedirect(returnUrl);
                    }

                    if (!string.IsNullOrEmpty(result.Error))
                    {
                        ModelState.AddModelError(string.Empty,result.Error);
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"OnPostAsync() : an error has occurred during user registration.");
                ModelState.AddModelError(string.Empty, "An error has occurred during user registration. Please try again.");
                return Page();
            }
        }

        private Student CreateStudent(string matricule, string name, string firstname, string mail, string group)
        {
            try
            {
                VerifyGroup(group);
                var newStudent = new Student()
                {
                    Matricule = matricule.Trim()?.ToLower(),
                    Name = name.Trim(),
                    FirstName = firstname.Trim(),
                    Email = mail.Trim().ToLower(),
                    LabGroup = Groups.FirstOrDefault(g => g.GroupName == group),
                };

                return newStudent;
            }
            catch (Exception e)
            {
                _logger.LogError("CreateStudent() : {0}",e.Message);
                ModelState.AddModelError(string.Empty,"An error occurred while creating the user. Please try again");
            }

            return null;
        }

        private void VerifyGroup(string group)
        {
            if (!Groups.Any(g => g.GroupName == group))
            {
                _logger.LogError($"VerifyGroup() : Group name {group} does not exist");
                ModelState.AddModelError(string.Empty,"Your group name does not exist");
                throw new Exception("Your group name does not exist");
            }
        }
    }
}
