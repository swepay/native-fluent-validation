using NativeMediator;

namespace Native.FluentValidation.AwsLambda.Mediator;

public sealed class CreateUserRequest : IRequest<CreateUserResponse>
{
    public string? Email { get; set; }
    public string? Name { get; set; }
}

public sealed class CreateUserResponse
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
}
