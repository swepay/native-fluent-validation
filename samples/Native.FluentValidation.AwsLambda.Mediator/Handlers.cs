using NativeMediator;

namespace Native.FluentValidation.AwsLambda.Mediator;

public sealed class CreateUserHandler : IRequestHandler<CreateUserRequest, CreateUserResponse>
{
    public ValueTask<CreateUserResponse> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var response = new CreateUserResponse
        {
            IsValid = true,
            Message = $"User {request.Email} accepted."
        };

        return ValueTask.FromResult(response);
    }
}
