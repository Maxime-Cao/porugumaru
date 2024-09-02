using System.ComponentModel.DataAnnotations;

namespace Puroguramu.Domains;

public class Exercise
{
    [Required]
    public Guid ExerciseId { get; init; } = Guid.NewGuid();

    [Required]
    [StringLength(255)]
    [RegularExpression(@"^.{5,255}$")]
    public string Title { get; set; }

    [Required]
    public string Instructions { get; set; }

    [Required]
    [StringLength(15)]
    public string Difficulty { get; set; }

    [Required]
    public string Template { get; set; }

    [Required]
    public string Stub { get; set; }

    [Required]
    public string Solution { get; set; }

    [Required]
    public int Position { get; init; }

    [Required]
    public bool IsPublished { get; init; }

    [Required]
    public Lesson Lesson { get; init; }

    [Required]
    public IEnumerable<ExerciseAttempt> ExerciseAttempts { get; init; }

    public string InjectIntoTemplate(string code)
        => Template.Replace("// code-insertion-point", code);
}
