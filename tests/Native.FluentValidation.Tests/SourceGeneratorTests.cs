using Native.FluentValidation.Builders;
using Native.FluentValidation.Core;
using Native.FluentValidation.SourceGeneration;
using Xunit;

namespace Native.FluentValidation.Tests;

[GeneratePropertyNames]
public sealed class GeneratorUser
{
    public string? Email { get; set; }
    public int Age { get; set; }
}

public sealed class SourceGeneratorTests
{
    [Fact]
    public void GeneratedPropertyNames_ShouldMatchPropertyNames()
    {
        Assert.Equal("Email", GeneratorUserPropertyNames.Email);
        Assert.Equal("Age", GeneratorUserPropertyNames.Age);
    }

    private sealed class GeneratorUserValidator : NativeValidator<GeneratorUser>
    {
        public GeneratorUserValidator()
        {
            RuleFor(GeneratorUserProperties.Email)
                .NotEmpty()
                .Email();
        }
    }

    [Fact]
    public void GeneratedSelectors_ShouldWorkWithRuleFor()
    {
        var validator = new GeneratorUserValidator();
        var result = validator.Validate(new GeneratorUser { Email = "" });

        Assert.False(result.IsValid);
        Assert.Equal("Email", result.Errors[0].PropertyName);
    }
}
