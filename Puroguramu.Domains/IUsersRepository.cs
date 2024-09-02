using System.Security.Claims;

namespace Puroguramu.Domains;

public interface IUsersRepository
{
    Task<AuthResult> SignInAsync(string emailOrMatricule, string password, bool rememberMe);

    Task SignOutAsync();

    Task<(bool Succeeded, string Error)> CreateStudentAsync(Student student,string password);

    Task<SchoolMember?> GetUserAsync(ClaimsPrincipal user);

    Task<(bool Succeeded, string Error)> ChangePasswordAsync(SchoolMember schoolMember, string oldPassword, string newPassword);

    Task<string?> GetRoleAsync(SchoolMember member);

    Task<string?> GetRoleAsync(string emailOrMatricule);

    Student? LoadStudent(string matricule);

    Teacher? LoadTeacher(string matricule);

    Task<(bool Succeeded, string Error)> UpdateStudentAsync(Student student);

    Task<(bool Succeeded, string Error)> UpdateTeacherAsync(Teacher teacher);

    int GetNumberStudents();
}
