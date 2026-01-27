namespace Native.FluentValidation.Results;

public sealed class ValidationFailure
{
    public string PropertyName { get; }
    public string ErrorCode { get; }
    public string ErrorMessage { get; }

    public ValidationFailure(string propertyName, string errorCode, string errorMessage)
    {
        PropertyName = propertyName;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }
}
