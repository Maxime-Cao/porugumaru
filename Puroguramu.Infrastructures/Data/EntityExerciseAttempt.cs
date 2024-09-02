using System.ComponentModel.DataAnnotations;
using Puroguramu.Domains;

namespace Puroguramu.Infrastructures.Data;

public class EntityExerciseAttempt
{
    [Key]
    public Guid AttemptId { get; init; }

    [Required]
    public string Proposal { get; init; }

    [Required]
    public ExerciseStatus ExerciseStatus { get; set; }

    [Required]
    public DateTime AttemptTime { get; init; }

    [Required]
    public Guid ExerciseId { get; init; }

    public EntityExercise? Exercise { get; set; }

    [Required]
    public string StudentId { get; init; }

    public IdentityStudent? Student { get; set; }
}
