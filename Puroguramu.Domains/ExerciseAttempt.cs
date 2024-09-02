using System.ComponentModel.DataAnnotations;

namespace Puroguramu.Domains;

public class ExerciseAttempt
{
    [Required]
    public Guid AttemptId { get; init; } = Guid.NewGuid();

    [Required]
    public string Proposal { get; init; }

    [Required]
    public ExerciseStatus ExerciseStatus { get; init; }

    [Required]
    public DateTime AttemptTime { get; init; } = DateTime.Now;

    [Required]
    public Exercise Exercise { get; init; }

    [Required]
    public Student Student { get; init; }
}
