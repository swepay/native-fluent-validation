using Native.FluentValidation.Core;

namespace Native.FluentValidation.AwsLambda;

public sealed class CreateUserValidator : NativeValidator<CreateUserRequest>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Email, nameof(CreateUserRequest.Email))
            .NotEmpty()
            .Email();

        RuleFor(x => x.Name, nameof(CreateUserRequest.Name))
            .NotEmpty()
            .Length(2, 100);

        RuleFor(x => x.Age, nameof(CreateUserRequest.Age))
            .GreaterThan(17);
    }
}
