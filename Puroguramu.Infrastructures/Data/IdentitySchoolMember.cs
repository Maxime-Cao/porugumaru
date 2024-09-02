using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Puroguramu.Infrastructures.Data;

public abstract class IdentitySchoolMember : IdentityUser
{
    [Required]
    [StringLength(7)]
    [RegularExpression(@"^[a-zA-Z]{1}[0-9]{6}$")]
    public string Matricule { get; init; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    [Required]
    [StringLength(255)]
    public string FirstName { get; set; }

    public string? PhotoPath { get; set; }

}
