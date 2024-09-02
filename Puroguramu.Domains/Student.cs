using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Puroguramu.Domains;

public class Student : SchoolMember
{
    [Required]
    public GroupLab LabGroup { get; init; }

    [Required]
    public IEnumerable<ExerciseAttempt> ExerciseAttempts { get; init; }

    public static bool IsValidEmail(string mail) => Regex.IsMatch(mail, @"^[a-zA-Z]+\.{1}[a-zA-Z]+@student\.helmo\.be$");
}
