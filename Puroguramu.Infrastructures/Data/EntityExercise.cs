using System.ComponentModel.DataAnnotations;

namespace Puroguramu.Infrastructures.Data;

public class EntityExercise
{

    [Key]
    public Guid ExerciseId { get; init; }

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
    public int Position { get; set; }

    [Required]
    public bool IsPublished { get; set; }

    [Required]
    public Guid LessonId { get; init; }

    public EntityLesson? Lesson { get; set; }

    [Required]
    public IEnumerable<EntityExerciseAttempt> ExerciseAttempts { get; init; }
}
