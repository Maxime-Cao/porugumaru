#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.Domains;

namespace Puroguramu.App.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IGroupsRepository _groupsRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            IUsersRepository usersRepository,IGroupsRepository groupsRepository,IWebHostEnvironment webHostEnvironment,ILogger<IndexModel> logger)
        {
            _usersRepository = usersRepository;
            _groupsRepository = groupsRepository;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public string Role { get; set; }

        public string Matricule { get; set; }

        [BindProperty]
        public List<GroupLab> Groups { get; set; }

        public string? MemberPhotoPath { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Name")]
            [RegularExpression(@"^.{1,}$",ErrorMessage = "Name must be at least 1 character")]
            public string Name { get; set; }

            [Required]
            [Display(Name = "Firstname")]
            [RegularExpression(@"^.{1,}$",ErrorMessage = "Firstname must be at least 1 character")]
            public string Firstname { get; set; }

            [Required]
            [RegularExpression(@"^[a-zA-Z]+\.{1}[a-zA-Z]+(@student\.helmo\.be|@helmo\.be)$",ErrorMessage = "Please provide a valid HELMo e-mail address")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Display(Name = "Group")]
            public string Group { get; set; }

            [Display(Name = "Profile picture")]
            [DataType(DataType.Upload)]
            public IFormFile ProfilePicture { get; set; }

            public bool DeleteProfilePicture { get; set; }
        }

        private async Task<IActionResult> ReloadPage(SchoolMember user)
        {
            await LoadAsync(user);
            return Page();
        }

        private bool ValidateEmail(string mail) => Role.Contains("Student") ? Student.IsValidEmail(mail) : Teacher.IsValidEmail(mail);

        private void VerifyGroup(string group)
        {
            if (!Groups.Any(g => g.GroupName == group))
            {
                _logger.LogError($"VerifyGroup() : group name {group} does not exist");
                ModelState.AddModelError(string.Empty,"Your group name does not exist");
                throw new Exception("Your group name does not exist");
            }
        }

        private Teacher CreateTeacher(string matricule, string name, string firstname, string mail)
        {
            try
            {
                var teacher = new Teacher()
                {
                    Matricule = Matricule,
                    Name = Input.Name.Trim(),
                    FirstName = Input.Firstname.Trim(),
                    Email = Input.Email.Trim().ToLower(),
                };

                return teacher;
            }
            catch (Exception e)
            {
                _logger.LogError("CreateTeacher() : {0}",e.Message);
                ModelState.AddModelError(string.Empty,"An error occurred during user update. Please try again");
            }

            return null;
        }

        private Student CreateStudent(string matricule, string name, string firstname, string mail, string group)
        {
            try
            {
                VerifyGroup(Input.Group);
                var student = new Student()
                {
                    Matricule = Matricule,
                    Name = Input.Name.Trim(),
                    FirstName = Input.Firstname.Trim(),
                    Email = Input.Email.Trim().ToLower(),
                    LabGroup = Groups.FirstOrDefault(g => g.GroupName == Input.Group),
                };

                return student;
            }
            catch (Exception e)
            {
                _logger.LogError("CreateStudent() : {0}",e.Message);
                ModelState.AddModelError(string.Empty,"An error occurred during user update. Please try again");
            }

            return null;
        }

        private async Task<string> UpdateImageAsync()
        {
            var updatedFilePath = !string.IsNullOrEmpty(MemberPhotoPath) ? MemberPhotoPath : string.Empty;

            var pictureFile = Input.ProfilePicture;

            if (pictureFile != null && pictureFile.Length > 0)
            {
                if (pictureFile.Length > 1 * 1024 * 1024)
                {
                    _logger.LogError("UpdateImageAsync() : file size must not exceed 1 MB.");
                    ModelState.AddModelError(string.Empty,"The file size must not exceed 1 MB.");
                    return null;
                }

                byte[] buffer = new byte[8];
                using (var stream = pictureFile.OpenReadStream())
                {
                    await stream.ReadAsync(buffer, 0, buffer.Length);
                }

                if (!IsValidImage(buffer))
                {
                    _logger.LogError("UpdateImageAsync() : incorrect image format (should be png or jpeg or jpg).");
                    ModelState.AddModelError(string.Empty,"Please provide a correct image (png/jpg/jpeg)");
                    return null;
                }

                var fileName = $"{Matricule}-{DateTime.Now.ToString("yyyyMMddHHmmss")}{Path.GetExtension(pictureFile.FileName).ToLower()}";

                var uploadDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "img", "upload");

                Directory.CreateDirectory(uploadDirectory);

                updatedFilePath = Path.Combine(uploadDirectory, fileName);

                await using (var stream = new FileStream(updatedFilePath, FileMode.Create))
                {
                    await pictureFile.CopyToAsync(stream);
                }

                updatedFilePath = Path.Combine("img", "upload", fileName);
            }
            else if (Input.DeleteProfilePicture)
            {
                updatedFilePath = string.Empty;
            }

            return updatedFilePath;
        }

        private bool IsValidImage(byte[] buffer)
        {
            var png = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
            var jpeg = new byte[] { 0xFF, 0xD8, 0xFF };

            return buffer.Take(png.Length).SequenceEqual(png) || buffer.Take(jpeg.Length).SequenceEqual(jpeg);
        }

        private async Task<IActionResult> LoadAsync(SchoolMember user)
        {
            Role = await _usersRepository.GetRoleAsync(user);

            if (Role == null)
            {
                _logger.LogError($"LoadAsync() :  unable to load user role for user {user.Matricule}");
                return NotFound("Unable to load user role");
            }

            Groups = _groupsRepository.GetAllGroups().ToList();

            if (Role.Contains("Student"))
            {
                var student = _usersRepository.LoadStudent(user.Matricule);

                if (student == null)
                {
                    _logger.LogError($"LoadAsync() : student with matricule {user.Matricule} not found");
                    return NotFound("Unable to load user");
                }

                Matricule = student.Matricule;
                Input = new InputModel
                {
                    Name = student.Name,
                    Firstname = student.FirstName,
                    Email = student.Email,
                    Group = student?.LabGroup?.GroupName,
                    DeleteProfilePicture = false,
                };
                MemberPhotoPath = student.PhotoPath;
            }
            else if (Role.Contains("Teacher"))
            {
                var teacher = _usersRepository.LoadTeacher(user.Matricule);

                if (teacher == null)
                {
                    _logger.LogError($"LoadAsync() : teacher with matricule {user.Matricule} not found");
                    return NotFound("Unable to load user");
                }

                Matricule = teacher.Matricule;
                Input = new InputModel()
                {
                    Name = teacher.Name,
                    Firstname = teacher.FirstName,
                    Email = teacher.Email,
                    DeleteProfilePicture = false,
                };
                MemberPhotoPath = teacher.PhotoPath;
            }
            else
            {
                _logger.LogError($"LoadAsync() : user role {Role} undefined");
                return NotFound("User role undefined");
            }

            return Page();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _usersRepository.GetUserAsync(User);

            if (user == null)
            {
                _logger.LogError($"OnGetAsync() : unable to load user {User?.Identity?.Name}");
                return NotFound("Unable to load user");
            }

            return await LoadAsync(user);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            SchoolMember user = null;
            try
            {
                user = await _usersRepository.GetUserAsync(User);

                if (user == null)
                {
                    _logger.LogError($"OnPostAsync() : unable to load user {User?.Identity?.Name}");
                    return NotFound("Unable to load user");
                }

                Matricule = user.Matricule;
                MemberPhotoPath = user.PhotoPath;
                Role = await _usersRepository.GetRoleAsync(user);

                if (Role == null)
                {
                    _logger.LogError($"OnPostAsync() :  unable to load user role for user {user.Matricule}");
                    return NotFound("Unable to load user role");
                }

                Groups = _groupsRepository.GetAllGroups().ToList();

                if (!ModelState.IsValid)
                {
                    return await ReloadPage(user);
                }

                if (!ValidateEmail(Input.Email))
                {
                    ModelState.AddModelError(string.Empty,$"Please provide a valid HELMo {Role.ToLower()} e-mail address");
                    return await ReloadPage(user);
                }

                if (Role.Contains("Student"))
                {
                    var student = CreateStudent(Matricule, Input.Name, Input.Firstname, Input.Email, Input.Group);

                    if (student == null)
                    {
                        return await ReloadPage(user);
                    }

                    var oldImagePath = MemberPhotoPath;
                    var newImagePath = await UpdateImageAsync();

                    if (newImagePath == null)
                    {
                        return await ReloadPage(user);
                    }

                    student.PhotoPath = newImagePath;

                    var updateResult = await _usersRepository.UpdateStudentAsync(student);

                    if (!string.IsNullOrEmpty(updateResult.Error))
                    {
                        if (newImagePath != oldImagePath)
                        {
                            var realImageNewPath = Path.Combine(_webHostEnvironment.WebRootPath, newImagePath);
                            if (!string.IsNullOrEmpty(realImageNewPath) && System.IO.File.Exists(realImageNewPath))
                            {
                                System.IO.File.Delete(realImageNewPath);
                            }
                        }

                        ModelState.AddModelError(string.Empty,updateResult.Error);
                        return await ReloadPage(user);
                    }

                    if (!string.IsNullOrEmpty(oldImagePath) && oldImagePath != newImagePath)
                    {
                        var realImageOldPath = Path.Combine(_webHostEnvironment.WebRootPath, oldImagePath);
                        if (System.IO.File.Exists(realImageOldPath))
                        {
                            System.IO.File.Delete(realImageOldPath);
                        }
                    }

                    StatusMessage = "Your profile has been updated";
                }
                else if (Role.Contains("Teacher"))
                {
                    var teacher = CreateTeacher(Matricule, Input.Name, Input.Firstname, Input.Email);

                    if (teacher == null)
                    {
                        return await ReloadPage(user);
                    }

                    var oldImagePath = MemberPhotoPath;
                    var newImagePath = await UpdateImageAsync();

                    if (newImagePath == null)
                    {
                        return await ReloadPage(user);
                    }

                    teacher.PhotoPath = newImagePath;

                    var updateResult = await _usersRepository.UpdateTeacherAsync(teacher);

                    if (!string.IsNullOrEmpty(updateResult.Error))
                    {
                        var realImageNewPath = Path.Combine(_webHostEnvironment.WebRootPath, newImagePath);
                        if (!string.IsNullOrEmpty(realImageNewPath) && System.IO.File.Exists(realImageNewPath))
                        {
                            System.IO.File.Delete(realImageNewPath);
                        }

                        ModelState.AddModelError(string.Empty,updateResult.Error);
                        return await ReloadPage(user);
                    }

                    if (!string.IsNullOrEmpty(oldImagePath) && oldImagePath != newImagePath)
                    {
                        var realImageOldPath = Path.Combine(_webHostEnvironment.WebRootPath, oldImagePath);
                        if (System.IO.File.Exists(realImageOldPath))
                        {
                            System.IO.File.Delete(realImageOldPath);
                        }
                    }

                    StatusMessage = "Your profile has been updated";
                }

                return RedirectToPage();
            }
            catch (Exception e)
            {
                _logger.LogError(e,"OnPostAsync() : an error has occurred during user update.");
                ModelState.AddModelError(string.Empty, "An error has occurred during user update. Please try again.");
                return await ReloadPage(user);
            }
        }
    }
}
