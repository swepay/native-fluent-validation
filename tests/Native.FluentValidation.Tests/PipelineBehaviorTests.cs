using Microsoft.Extensions.DependencyInjection;
using Native.FluentValidation.Builders;
using Native.FluentValidation.Core;
using Native.FluentValidation.Extensions;
using Native.FluentValidation.Results;
using NativeMediator;
using Xunit;

namespace Native.FluentValidation.Tests;

public sealed class PipelineBehaviorTests
{
    private sealed class CreateUser : IRequest<string>
    {
        public string? Email { get; set; }
    }

    private sealed class CreateUserValidator : NativeValidator<CreateUser>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Email, nameof(CreateUser.Email))
                .NotEmpty()
                .Email();
        }
    }

    private sealed class CreateUserDomainValidator : NativeValidator<CreateUser>
    {
        public CreateUserDomainValidator()
        {
            RuleFor(x => x.Email, nameof(CreateUser.Email))
                .Must(email => email is not null && email.EndsWith("@example.com", StringComparison.OrdinalIgnoreCase))
                .WithErrorCode("E_DOMAIN")
                .WithMessage("Email must be from example.com.");
        }
    }

    [Fact]
    public async Task Pipeline_ValidRequest_CallsNext()
    {
        var services = new ServiceCollection();
        services.AddNativeFluentValidation<CreateUser, CreateUserValidator>();
        var provider = services.BuildServiceProvider();

        var behavior = provider.GetRequiredService<IPipelineBehavior<CreateUser, string>>();
        var request = new CreateUser { Email = "user@example.com" };

        var result = await behavior.Handle(request, () => ValueTask.FromResult("ok"));

        Assert.Equal("ok", result);
    }

    [Fact]
    public async Task Pipeline_InvalidRequest_Throws()
    {
        var services = new ServiceCollection();
        services.AddNativeFluentValidation<CreateUser, CreateUserValidator>();
        var provider = services.BuildServiceProvider();

        var behavior = provider.GetRequiredService<IPipelineBehavior<CreateUser, string>>();
        var request = new CreateUser { Email = "invalid" };

        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => behavior.Handle(request, () => ValueTask.FromResult("ok")).AsTask());

        Assert.False(exception.Result.IsValid);
    }

    [Fact]
    public async Task Pipeline_MultipleValidators_AggregatesFailures()
    {
        var services = new ServiceCollection();
        services.AddNativeFluentValidation(builder =>
        {
            builder.AddValidator<CreateUser, CreateUserValidator>();
            builder.AddValidator<CreateUser, CreateUserDomainValidator>();
        });

        var provider = services.BuildServiceProvider();

        var behavior = provider.GetRequiredService<IPipelineBehavior<CreateUser, string>>();
        var request = new CreateUser { Email = "invalid" };

        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => behavior.Handle(request, () => ValueTask.FromResult("ok")).AsTask());

        Assert.Equal(2, exception.Result.Errors.Count);
    }
}
