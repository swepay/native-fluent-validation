using FluentValidation;
using Native.FluentValidation.Builders;
using Native.FluentValidation.Core;
using Xunit;

namespace Native.FluentValidation.Tests;

public sealed class ComparisonTests
{
    private sealed class User
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
    }

    private sealed class NativeUserValidator : NativeValidator<User>
    {
        public NativeUserValidator()
        {
            RuleFor(x => x.Email, nameof(User.Email))
                .NotEmpty()
                .Email();

            RuleFor(x => x.Name, nameof(User.Name))
                .NotNull()
                .Length(2, 40);

            RuleFor(x => x.Age, nameof(User.Age))
                .GreaterThan(17)
                .Must(age => age < 120);
        }
    }

    private sealed class FluentUserValidator : AbstractValidator<User>
    {
        public FluentUserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Name)
                .NotNull()
                .Length(2, 40);

            RuleFor(x => x.Age)
                .GreaterThan(17)
                .Must(age => age < 120);
        }
    }

    [Fact]
    public void Validators_ShouldAgree_OnValidUser()
    {
        var user = new User { Email = "user@example.com", Name = "Alice", Age = 30 };
        var native = new NativeUserValidator();
        var fluent = new FluentUserValidator();

        var nativeResult = native.Validate(user);
        var fluentResult = fluent.Validate(user);

        Assert.True(nativeResult.IsValid);
        Assert.True(fluentResult.IsValid);
    }

    [Fact]
    public void Validators_ShouldAgree_OnInvalidUser()
    {
        var user = new User { Email = "", Name = "", Age = 10 };
        var native = new NativeUserValidator();
        var fluent = new FluentUserValidator();

        var nativeResult = native.Validate(user);
        var fluentResult = fluent.Validate(user);

        Assert.False(nativeResult.IsValid);
        Assert.False(fluentResult.IsValid);
    }
}
