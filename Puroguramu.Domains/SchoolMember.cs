using System.ComponentModel.DataAnnotations;

namespace Puroguramu.Domains;

public class SchoolMember
{
    [Required]
    [StringLength(7)]
    [RegularExpression(@"^[a-zA-Z]{1}[0-9]{6}$")]
    public string Matricule { get; init; }

    [Required]
    [StringLength(255)]
    public string Name { get; init; }

    [Required]
    [StringLength(255)]
    public string FirstName { get; init; }

    [Required]
    [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$")]
    public string Email { get; init; }

    public string? PhotoPath { get; set; }
}
