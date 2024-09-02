using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

using Puroguramu.Domains;

namespace Puroguramu.Infrastructures.Roslyn;

public class RoslynAssessor : IAssessExercise
{
    private readonly IExercisesRepository _exercisesRepository;

    private static readonly ScriptOptions Options = ScriptOptions.Default
        .WithImports("System", "System.Linq", "Puroguramu.Domains")
        .WithReferences("System.Core","Puroguramu.Domains");

    public RoslynAssessor(IExercisesRepository repository)
    {
        _exercisesRepository = repository;
    }

    public async Task<ExerciseResult> Assess(Guid exerciseId, string proposal)
    {
        var exercise = _exercisesRepository.GetExercise(exerciseId);
        var codeToRun = exercise.InjectIntoTemplate(proposal);
        try
        {
            ScriptState<TestResult[]> run = await CSharpScript.RunAsync<TestResult[]>(
                codeToRun,
                Options);

            return new ExerciseResult(exercise, proposal, run.ReturnValue);
        }
        catch (CompilationErrorException ex)
        {
            return new ExerciseResult(exercise, proposal,
                ex.Diagnostics.Select(d => new TestResult("Compilation Error", TestStatus.Inconclusive, d.ToString())));
        }
    }

    public async Task<ExerciseResult> StubForExercise(Guid exerciseId)
    {
        var exercise = _exercisesRepository.GetExercise(exerciseId);

        return await Task.FromResult(new ExerciseResult(exercise, exercise.Stub));
    }
}
