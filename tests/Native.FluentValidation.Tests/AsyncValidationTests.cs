using Native.FluentValidation.Abstractions;
using Native.FluentValidation.Extensions;
using Native.FluentValidation.Results;
using NativeMediator;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Native.FluentValidation.Tests;

public sealed class AsyncValidationTests
{
    private sealed class CreateUser : IRequest<string>
    {
        public string? Email { get; set; }
    }

    private sealed class AsyncValidator : INativeAsyncValidator<CreateUser>
    {
        public ValueTask<ValidationResult> ValidateAsync(CreateUser instance, CancellationToken cancellationToken = default)
        {
            if (instance.Email == "blocked@example.com")
            {
                var failures = new[]
                {
                    new ValidationFailure(nameof(CreateUser.Email), "Blocked", "Email is blocked.")
                };
                return ValueTask.FromResult(new ValidationResult(failures));
            }

            return ValueTask.FromResult(ValidationResult.Success);
        }
    }

    [Fact]
    public async Task AsyncValidator_ShouldRunInPipeline()
    {
        var services = new ServiceCollection();
        services.AddNativeFluentValidation(builder =>
        {
            builder.AddAsyncValidator<CreateUser, AsyncValidator>();
        });

        var provider = services.BuildServiceProvider();
        var behavior = provider.GetRequiredService<IPipelineBehavior<CreateUser, string>>();

        await Assert.ThrowsAsync<ValidationException>(
            () => behavior.Handle(new CreateUser { Email = "blocked@example.com" }, () => ValueTask.FromResult("ok")).AsTask());
    }
}
