using Puroguramu.Domains;

namespace Puroguramu.App.ViewModels;

public record TestResultViewModel(TestResult Result)
{
    public string Status
        => Result.Status.ToString();

    public string Label
        => Result.Label;

    public bool HasError
        => Result.Status != TestStatus.Passed;

    public string ErrorMessage
        => Result.ErrorMessage;
}
