using Native.FluentValidation.Builders;
using Native.FluentValidation.Core;

namespace Native.FluentValidation.AwsLambda.Mediator;

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
    }
}
