using System.Text.RegularExpressions;

namespace Puroguramu.Domains;

public class Teacher : SchoolMember
{
    public static bool IsValidEmail(string mail) => Regex.IsMatch(mail, @"^[a-zA-Z]+\.{1}[a-zA-Z]+@helmo\.be$");

}
