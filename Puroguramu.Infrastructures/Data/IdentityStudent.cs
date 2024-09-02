using System.ComponentModel.DataAnnotations;

namespace Puroguramu.Infrastructures.Data;

public class IdentityStudent : IdentitySchoolMember
{
    [Required]
    public Guid LabGroupId { get; set; }

    public EntityGroupLab? LabGroup { get; set; }

    [Required]
    public IEnumerable<EntityExerciseAttempt> ExerciseAttempts { get; set; }

}
