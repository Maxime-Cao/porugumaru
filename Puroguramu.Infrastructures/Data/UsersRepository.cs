using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Puroguramu.Domains;

namespace Puroguramu.Infrastructures.Data;

public class UsersRepository : IUsersRepository
{
    private readonly SignInManager<IdentitySchoolMember> _signInManager;
    private readonly UserManager<IdentitySchoolMember> _userManager;
    private readonly PuroguramuDbContext _puroguramuDbContext;
    private readonly ILogger<UsersRepository> _logger;

    public UsersRepository(SignInManager<IdentitySchoolMember> signInManager,UserManager<IdentitySchoolMember> userManager,PuroguramuDbContext puroguramuDbContext, ILogger<UsersRepository> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _puroguramuDbContext = puroguramuDbContext;
        _logger = logger;
    }

    public async Task<AuthResult> SignInAsync(string emailOrMatricule, string password, bool rememberMe)
    {
        var user = await _signInManager.UserManager.FindByEmailAsync(emailOrMatricule);

        if (user == null)
        {
            user = _signInManager.UserManager.Users.FirstOrDefault(u => u.Matricule == emailOrMatricule);
            if (user == null)
            {
                return AuthResult.Failed;
            }
        }

        var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            return AuthResult.Success;
        }

        if (result.RequiresTwoFactor)
        {
            return AuthResult.RequiresTwoFactor;
        }

        else if (result.IsLockedOut)
        {
            return AuthResult.Lockout;
        }
        else
        {
            return AuthResult.Failed;
        }
    }

    public async Task SignOutAsync() => await _signInManager.SignOutAsync();

    public async Task<(bool Succeeded,string Error)> CreateStudentAsync(Student student,string password)
    {
        if (IsMatriculeExists(student.Matricule) || IsMailExists(student.Email))
        {
            _logger.LogError($"CreateStudentAsync() : email address {student.Email} and/or matricule {student.Matricule} already exists in the system");
            return (false, "This e-mail address and/or matricule already exists in our system");
        }

        var groupFound = _puroguramuDbContext.Groups.FirstOrDefault(g => g.GroupName == student.LabGroup.GroupName);

        if (groupFound == null)
        {
            _logger.LogError($"CreateStudentAsync() : group with groupName {student.LabGroup.GroupName} not found");
            return (false, "An error occured while creating the user : group not found");
        }

        var user = new IdentityStudent()
        {
            Email = student.Email,
            EmailConfirmed = true,
            UserName = student.Email,
            NormalizedEmail = student.Email.ToUpper(),
            Matricule = student.Matricule,
            Name = student.Name,
            FirstName = student.FirstName,
            LabGroup = groupFound,
        };
        return await CreateUserAsync(user, UserRole.Student, password);
    }

    public async Task<SchoolMember?> GetUserAsync(ClaimsPrincipal user)
    {
        var userFound = await _userManager.GetUserAsync(user);

        if (userFound == null)
        {
            return null;
        }

        var domainUser = new SchoolMember()
        {
            Matricule = userFound.Matricule,
            Name = userFound.Name,
            FirstName = userFound.FirstName,
            Email = userFound.Email,
            PhotoPath = userFound.PhotoPath,
        };

        return domainUser;
    }

    public async Task<(bool Succeeded, string Error)> ChangePasswordAsync(SchoolMember schoolMember, string oldPassword, string newPassword)
    {
        var errors = new List<string>();

        var userFound = _userManager.Users.FirstOrDefault(u => u.Matricule == schoolMember.Matricule);

        if (userFound == null)
        {
            _logger.LogError($"ChangePasswordAsync() : unable to find user {schoolMember.Matricule}");
            return (false, "An error occurred while changing the password: user not found");
        }

        var result = await _userManager.ChangePasswordAsync(userFound, oldPassword,newPassword);

        if (!result.Succeeded)
        {
            foreach (var error in result?.Errors)
            {
                _logger.LogError("ChangePasswordAsync() : {0}", error.Description);
            }

            return (false, "An error occurred while changing the password");
        }

        await _signInManager.RefreshSignInAsync(userFound);
        return (true, string.Empty);
    }

    public async Task<string?> GetRoleAsync(SchoolMember member)
    {
        var userFound = _userManager.Users.FirstOrDefault(u => u.Matricule.
            Equals(member.Matricule));

        if (userFound == null)
        {
            return null;
        }

        var result = await _userManager.GetRolesAsync(userFound);

        return result?[0];
    }

    public async Task<string?> GetRoleAsync(string emailOrMatricule)
    {
        var userFound = _userManager.Users.FirstOrDefault(u => u.Matricule == emailOrMatricule || u.Email == emailOrMatricule);

        if (userFound == null)
        {
            return null;
        }

        var result = await _userManager.GetRolesAsync(userFound);

        return result?[0];
    }

    public Student? LoadStudent(string matricule)
    {
        var studentFound = _puroguramuDbContext.Users.OfType<IdentityStudent>().FirstOrDefault(s => s.Matricule == matricule);

        if (studentFound == null)
        {
            return null;
        }

        var groupName = studentFound.LabGroup?.GroupName;

        var domainStudent = new Student()
        {
            Matricule = studentFound.Matricule,
            Name = studentFound.Name,
            FirstName = studentFound.FirstName,
            Email = studentFound.Email,
            LabGroup = string.IsNullOrEmpty(groupName) ? null : new GroupLab() {GroupName = groupName},
            PhotoPath = studentFound.PhotoPath,
        };

        return domainStudent;
    }

    public Teacher? LoadTeacher(string matricule)
    {
        var teacherFound = _puroguramuDbContext.Users.OfType<IdentityTeacher>().FirstOrDefault(t => t.Matricule == matricule);

        if (teacherFound == null)
        {
            return null;
        }

        var domainTeacher = new Teacher()
        {
            Matricule = teacherFound.Matricule,
            Name = teacherFound.Name,
            FirstName = teacherFound.FirstName,
            Email = teacherFound.Email,
            PhotoPath = teacherFound.PhotoPath,
        };

        return domainTeacher;
    }

    public async Task<(bool Succeeded, string Error)> UpdateStudentAsync(Student student)
    {
        var studentFound = _puroguramuDbContext.Users.OfType<IdentityStudent>().FirstOrDefault(s => s.Matricule == student.Matricule);

        if (studentFound == null)
        {
            _logger.LogError($"UpdateStudentAsync() : unable to find user {student.Matricule}");
            return (false, "An error occurred during user update: user not found");
        }

        var groupFound = _puroguramuDbContext.Groups.FirstOrDefault(g => g.GroupName == student.LabGroup.GroupName);

        if (groupFound == null)
        {
            _logger.LogError($"UpdateStudentAsync() : unable to find group {student.LabGroup.GroupName}");
            return (false, "An error occured during user update: group not found");
        }

        if (student.Email != studentFound.Email && IsMailExists(student.Email))
        {
            _logger.LogError($"UpdateStudentAsync() : email address {student.Email} already exists in the system");
            return (false, "This e-mail address already exists in our system");
        }

        studentFound.Name = student.Name;
        studentFound.FirstName = student.FirstName;
        studentFound.Email = student.Email;
        studentFound.UserName = student.Email;
        studentFound.NormalizedEmail = student.Email.ToUpper();

        studentFound.PhotoPath = student.PhotoPath;
        studentFound.LabGroup = groupFound;

        await _puroguramuDbContext.SaveChangesAsync();
        await _signInManager.RefreshSignInAsync(studentFound);

        return (true, string.Empty);
    }

    public async Task<(bool Succeeded, string Error)> UpdateTeacherAsync(Teacher teacher)
    {
        var teacherFound = _puroguramuDbContext.Users.OfType<IdentityTeacher>().FirstOrDefault(t => t.Matricule == teacher.Matricule);

        if (teacherFound == null)
        {
            _logger.LogError($"UpdateTeacherAsync() : unable to find user {teacher.Matricule}");
            return (false, "An error occurred during user update: user not found");
        }

        if (teacher.Email != teacherFound.Email && IsMailExists(teacher.Email))
        {
            _logger.LogError($"UpdateTeacherAsync() : email address {teacher.Email} already exists in the system");
            return (false, "This e-mail address already exists in our system");
        }

        teacherFound.Name = teacher.Name;
        teacherFound.FirstName = teacher.FirstName;
        teacherFound.Email = teacher.Email;
        teacherFound.UserName = teacher.Email;
        teacherFound.NormalizedEmail = teacher.Email.ToUpper();

        teacherFound.PhotoPath = teacher.PhotoPath;

        await _puroguramuDbContext.SaveChangesAsync();
        await _signInManager.RefreshSignInAsync(teacherFound);

        return (true, string.Empty);
    }

    public int GetNumberStudents() => _puroguramuDbContext.Users.OfType<IdentityStudent>().Count();

    private async Task<(bool Succeeded, string Error)> CreateUserAsync(IdentitySchoolMember user,UserRole role,string password)
    {
        await _userManager.SetUserNameAsync(user,user.Email);
        await _userManager.SetEmailAsync(user,user.Email);

        var result = await _userManager.CreateAsync(user,password);

        if (result?.Succeeded ?? false)
        {
            if (role == UserRole.Student)
            {
                await _userManager.AddToRoleAsync(user, "Student");
            }
            else if (role == UserRole.Teacher)
            {
                await _userManager.AddToRoleAsync(user, "Teacher");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return (true, string.Empty);
        }

        foreach (var error in result?.Errors)
        {
            _logger.LogError("CreateUserAsync() : {0}", error.Description);
        }

        return (false, "An error occurred while creating the user");
    }

    private bool IsMatriculeExists(string matricule) => _puroguramuDbContext.Users.Any(u => u.Matricule == matricule);

    private bool IsMailExists(string mail) => _puroguramuDbContext.Users.Any(u => u.Email == mail);
}
