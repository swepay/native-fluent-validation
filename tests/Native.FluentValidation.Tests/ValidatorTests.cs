using Native.FluentValidation.Builders;
using Native.FluentValidation.Core;
using Native.FluentValidation.Results;
using Xunit;

namespace Native.FluentValidation.Tests;

public sealed class ValidatorTests
{
    private sealed class User
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
    }

    private sealed class UserValidator : NativeValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.Email, nameof(User.Email))
                .NotEmpty()
                .Email();

            RuleFor(x => x.Name, nameof(User.Name))
                .NotNull()
                .Length(2, 20);

            RuleFor(x => x.Age, nameof(User.Age))
                .GreaterThan(17)
                .Must(age => age < 120);
        }
    }

    [Fact]
    public void Validate_InvalidUser_ReturnsFailures()
    {
        var validator = new UserValidator();
        var user = new User { Email = "", Name = "A", Age = 10 };

        ValidationResult result = validator.Validate(user);

        Assert.False(result.IsValid);
        Assert.Equal(3, result.Errors.Count);
        Assert.Contains(result.Errors, failure => failure.PropertyName == nameof(User.Email));
        Assert.Contains(result.Errors, failure => failure.PropertyName == nameof(User.Name));
        Assert.Contains(result.Errors, failure => failure.PropertyName == nameof(User.Age));
    }

    [Fact]
    public void Validate_ValidUser_Passes()
    {
        var validator = new UserValidator();
        var user = new User { Email = "user@example.com", Name = "Valid", Age = 30 };

        ValidationResult result = validator.Validate(user);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}
