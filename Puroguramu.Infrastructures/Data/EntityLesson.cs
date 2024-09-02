using System.ComponentModel.DataAnnotations;

namespace Puroguramu.Infrastructures.Data;

public class EntityLesson
{
    [Key]
    public Guid LessonId { get; init; }

    [Required]
    [StringLength(255)]
    [RegularExpression(@"^.{5,255}$")]
    public string Title { get; set;}

    [Required]
    [StringLength(255)]
    public string Description { get; set; }

    [Required]
    public int Position { get; set; }

    [Required]
    public bool IsPublished { get; set; }

    [Required]
    public IEnumerable<EntityExercise> Exercises { get; set; }
}
