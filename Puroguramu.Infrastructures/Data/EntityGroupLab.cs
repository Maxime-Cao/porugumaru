using System.ComponentModel.DataAnnotations;

namespace Puroguramu.Infrastructures.Data;

public class EntityGroupLab
{
    [Key]
    public Guid IdGroup { get; init; }

    [Required]
    [StringLength(255)]
    public string GroupName { get; init; }

    [Required]
    public IEnumerable<IdentityStudent> Members { get; init; }
}
