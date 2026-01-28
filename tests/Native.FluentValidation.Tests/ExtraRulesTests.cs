using Native.FluentValidation.Core;
using Native.FluentValidation.ExtraRules;
using Xunit;

namespace Native.FluentValidation.Tests;

public sealed class ExtraRulesTests
{
    private sealed class User
    {
        public string? Email { get; set; }
        public int Age { get; set; }
    }

    private sealed class UserValidator : NativeValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.Email, nameof(User.Email))
                .Matches("@example.com$");

            RuleFor(x => x.Age, nameof(User.Age))
                .InclusiveBetween(18, 120)
                .GreaterThanOrEqual(18)
                .LessThanOrEqual(120)
                .NotEqual(42);
        }
    }

    [Fact]
    public void ExtraRules_ShouldValidate()
    {
        var validator = new UserValidator();
        var valid = new User { Email = "user@example.com", Age = 30 };
        var invalid = new User { Email = "user@other.com", Age = 42 };

        Assert.True(validator.Validate(valid).IsValid);
        Assert.False(validator.Validate(invalid).IsValid);
    }
}
