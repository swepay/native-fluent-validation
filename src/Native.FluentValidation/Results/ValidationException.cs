namespace Native.FluentValidation.Results;

public sealed class ValidationException : Exception
{
    public ValidationResult Result { get; }

    public ValidationException(ValidationResult result)
        : base("Validation failed.")
    {
        Result = result;
    }
}
