using System.ComponentModel.DataAnnotations;

namespace Puroguramu.Domains;

public class Lesson
{
    [Required]
    public Guid LessonId { get; init; } = Guid.NewGuid();

    [Required]
    [StringLength(255)]
    [RegularExpression(@"^.{5,255}$")]
    public string Title { get; set;}

    [Required]
    [StringLength(255)]
    public string Description { get; set; }

    [Required]
    public int Position { get; init; }

    [Required]
    public bool IsPublished { get; init; }

    [Required]
    public IEnumerable<Exercise> Exercises { get; init; }
}
