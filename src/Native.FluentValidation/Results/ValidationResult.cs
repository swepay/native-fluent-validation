namespace Native.FluentValidation.Results;

public sealed class ValidationResult
{
    public static ValidationResult Success { get; } = new ValidationResult(Array.Empty<ValidationFailure>());

    public IReadOnlyList<ValidationFailure> Errors { get; }
    public bool IsValid => Errors.Count == 0;

    public ValidationResult(IReadOnlyList<ValidationFailure> errors)
    {
        Errors = errors;
    }
}
