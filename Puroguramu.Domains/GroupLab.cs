using System.ComponentModel.DataAnnotations;

namespace Puroguramu.Domains;

public class GroupLab
{
    [Required]
    public Guid IdGroup { get; init; } = Guid.NewGuid();

    [Required]
    [StringLength(255)]
    public string GroupName { get; init; }

    [Required]
    public IEnumerable<SchoolMember> Members { get; set; }
}
