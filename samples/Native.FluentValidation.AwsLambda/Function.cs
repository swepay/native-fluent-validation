using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Native.FluentValidation.Core;
using Native.FluentValidation.Results;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace Native.FluentValidation.AwsLambda;

public sealed class Function
{
    private readonly CreateUserValidator _validator = new();

    public CreateUserResponse Handler(CreateUserRequest request, ILambdaContext context)
    {
        ValidationResult result = _validator.Validate(request);

        if (!result.IsValid)
        {
            return new CreateUserResponse
            {
                IsValid = false,
                Message = "Validation failed.",
                Errors = result.Errors
            };
        }

        return new CreateUserResponse
        {
            IsValid = true,
            Message = "User accepted.",
            Errors = Array.Empty<ValidationFailure>()
        };
    }
}
