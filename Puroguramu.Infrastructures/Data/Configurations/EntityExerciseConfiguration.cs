using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Puroguramu.Infrastructures.Data.Configurations;

public class EntityExerciseConfiguration : IEntityTypeConfiguration<EntityExercise>
{
    public static readonly List<EntityExercise> Exercises = new();
    private readonly IList<EntityLesson> _lessons;
    private readonly string _firstExerciseTemplate = @"// code-insertion-point

public class Test
{
    public static TestResult Ensure(float b, int exponent, float expected)
    {
      TestStatus status = TestStatus.Passed;
      float actual = float.NaN;
      try
      {
         actual = Exercice.Power(b, exponent);
         if(Math.Abs(actual - expected) > 0.00001f)
         {
             status = TestStatus.Failed;
         }
      }
      catch(Exception ex)
      {
         status = TestStatus.Inconclusive;
      }

      return new TestResult(
        string.Format(""Power of {0} by {1} should be {2}"", b, exponent, expected),
        status,
        status == TestStatus.Passed ? string.Empty : string.Format(""Expected {0}. Got {1}."", expected, actual)
      );
    }
}

return new TestResult[] {
  Test.Ensure(2, 4, 16.0f),
  Test.Ensure(2, -4, 1.0f/16.0f)
};
";

    private readonly string _firstExerciseStub = @"public class Exercice
{
  // Tapez votre code ici
}
";

    private readonly string _firstExerciseSolution = @"public class Exercice
{
    public static float Power(float b, int e)
    {
        return (float)Math.Pow(b, e);
    }
}
";

    public EntityExerciseConfiguration(IEnumerable<EntityLesson> lessons)
    {
        _lessons = lessons.ToList();
    }

    public void Configure(EntityTypeBuilder<EntityExercise> builder)
    {
        builder.HasIndex(e => new { e.LessonId, e.Title }).IsUnique();
        builder.HasIndex(e => new { e.LessonId, e.Position }).IsUnique();
        SeedExercises(builder);
    }

    private void SeedExercises(EntityTypeBuilder<EntityExercise> builder)
    {
        Exercises.Clear();

        var firstExercise = new EntityExercise()
        {
            ExerciseId = Guid.NewGuid(),
            Title = "Calcul de puissance en C# v1" ,
            Instructions = "Créez une fonction Power C# prenant en paramètre une base b de type float et un exposant e de type int. Power(b, e) retourne le float b exposant e.",
            Difficulty = "Easy",
            Template = _firstExerciseTemplate,
            Stub = _firstExerciseStub,
            Solution = _firstExerciseSolution,
            Position = 1,
            IsPublished = true,
            LessonId = _lessons[0].LessonId,
        };

        var secondExercise = new EntityExercise()
        {
            ExerciseId = Guid.NewGuid(),
            Title = "Calcul de puissance en C# v2" ,
            Instructions = "Créez une fonction Power C# prenant en paramètre une base b de type float et un exposant e de type int. Power(b, e) retourne le float b exposant e.",
            Difficulty = "Medium",
            Template = _firstExerciseTemplate,
            Stub = _firstExerciseStub,
            Solution = _firstExerciseSolution,
            Position = 2,
            IsPublished = true,
            LessonId = _lessons[0].LessonId,
        };

        var thirdExercise = new EntityExercise()
        {
            ExerciseId = Guid.NewGuid(),
            Title = "Calcul de puissance en C# v3" ,
            Instructions = "Créez une fonction Power C# prenant en paramètre une base b de type float et un exposant e de type int. Power(b, e) retourne le float b exposant e.",
            Difficulty = "Easy",
            Template = _firstExerciseTemplate,
            Stub = _firstExerciseStub,
            Solution = _firstExerciseSolution,
            Position = 1,
            IsPublished = true,
            LessonId = _lessons[1].LessonId,
        };

        var fourthExercise = new EntityExercise()
        {
            ExerciseId = Guid.NewGuid(),
            Title = "Calcul de puissance en C# v4" ,
            Instructions = "Créez une fonction Power C# prenant en paramètre une base b de type float et un exposant e de type int. Power(b, e) retourne le float b exposant e.",
            Difficulty = "Medium",
            Template = _firstExerciseTemplate,
            Stub = _firstExerciseStub,
            Solution = _firstExerciseSolution,
            Position = 2,
            IsPublished = true,
            LessonId = _lessons[1].LessonId,
        };

        var fithExercise = new EntityExercise()
        {
            ExerciseId = Guid.NewGuid(),
            Title = "Calcul de puissance en C# v5" ,
            Instructions = "Créez une fonction Power C# prenant en paramètre une base b de type float et un exposant e de type int. Power(b, e) retourne le float b exposant e.",
            Difficulty = "Hard",
            Template = _firstExerciseTemplate,
            Stub = _firstExerciseStub,
            Solution = _firstExerciseSolution,
            Position = 3,
            IsPublished = true,
            LessonId = _lessons[1].LessonId,
        };

        var sixthExercise = new EntityExercise()
        {
            ExerciseId = Guid.NewGuid(),
            Title = "Calcul de puissance en C# v6" ,
            Instructions = "Créez une fonction Power C# prenant en paramètre une base b de type float et un exposant e de type int. Power(b, e) retourne le float b exposant e.",
            Difficulty = "Medium",
            Template = _firstExerciseTemplate,
            Stub = _firstExerciseStub,
            Solution = _firstExerciseSolution,
            Position = 1,
            IsPublished = true,
            LessonId = _lessons[2].LessonId,
        };

        var seventhExercise = new EntityExercise()
        {
            ExerciseId = Guid.NewGuid(),
            Title = "Calcul de puissance en C# v7" ,
            Instructions = "Créez une fonction Power C# prenant en paramètre une base b de type float et un exposant e de type int. Power(b, e) retourne le float b exposant e.",
            Difficulty = "Easy",
            Template = _firstExerciseTemplate,
            Stub = _firstExerciseStub,
            Solution = _firstExerciseSolution,
            Position = 1,
            IsPublished = true,
            LessonId = _lessons[3].LessonId,
        };

        var eighthExercise = new EntityExercise()
        {
            ExerciseId = Guid.NewGuid(),
            Title = "Calcul de puissance en C# v8" ,
            Instructions = "Créez une fonction Power C# prenant en paramètre une base b de type float et un exposant e de type int. Power(b, e) retourne le float b exposant e.",
            Difficulty = "Medium",
            Template = _firstExerciseTemplate,
            Stub = _firstExerciseStub,
            Solution = _firstExerciseSolution,
            Position = 2,
            IsPublished = true,
            LessonId = _lessons[3].LessonId,
        };

        var ninthExercise = new EntityExercise()
        {
            ExerciseId = Guid.NewGuid(),
            Title = "Calcul de puissance en C# v9" ,
            Instructions = "Créez une fonction Power C# prenant en paramètre une base b de type float et un exposant e de type int. Power(b, e) retourne le float b exposant e.",
            Difficulty = "Medium",
            Template = _firstExerciseTemplate,
            Stub = _firstExerciseStub,
            Solution = _firstExerciseSolution,
            Position = 3,
            IsPublished = true,
            LessonId = _lessons[3].LessonId,
        };

        var tenthExercise = new EntityExercise()
        {
            ExerciseId = Guid.NewGuid(),
            Title = "Calcul de puissance en C# v10" ,
            Instructions = "Créez une fonction Power C# prenant en paramètre une base b de type float et un exposant e de type int. Power(b, e) retourne le float b exposant e.",
            Difficulty = "Hard",
            Template = _firstExerciseTemplate,
            Stub = _firstExerciseStub,
            Solution = _firstExerciseSolution,
            Position = 4,
            IsPublished = true,
            LessonId = _lessons[3].LessonId,
        };

        Exercises.AddRange(new[] {firstExercise,secondExercise,thirdExercise,fourthExercise,fithExercise,sixthExercise,seventhExercise,eighthExercise,ninthExercise,tenthExercise});

        builder.HasData(Exercises);
    }
}
