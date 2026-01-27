namespace Native.FluentValidation.AwsLambda;

public sealed class CreateUserRequest
{
    public string? Email { get; set; }
    public string? Name { get; set; }
    public int Age { get; set; }
}

public sealed class CreateUserResponse
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public IReadOnlyList<Native.FluentValidation.Results.ValidationFailure> Errors { get; set; }
        = Array.Empty<Native.FluentValidation.Results.ValidationFailure>();
}
