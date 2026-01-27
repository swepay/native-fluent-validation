using Native.FluentValidation.Core;
using Native.FluentValidation.Results;
using Xunit;

namespace Native.FluentValidation.Tests;

public sealed class Phase2Tests
{
    private sealed class ConditionalModel
    {
        public string? Name { get; set; }
        public bool ValidateName { get; set; }
        public int Age { get; set; }
        public bool SkipAge { get; set; }
    }

    private sealed class ConditionalValidator : NativeValidator<ConditionalModel>
    {
        public ConditionalValidator()
        {
            RuleFor(x => x.Name, nameof(ConditionalModel.Name))
                .NotEmpty()
                .When(x => x.ValidateName);

            RuleFor(x => x.Age, nameof(ConditionalModel.Age))
                .GreaterThan(17)
                .Unless(x => x.SkipAge);
        }
    }

    private sealed class CascadeValidator : NativeValidator<ConditionalModel>
    {
        public CascadeValidator(CascadeMode mode)
        {
            RuleFor(x => x.Name, nameof(ConditionalModel.Name))
                .NotEmpty()
                .Length(2, 4)
                .Cascade(mode);
        }
    }

    private sealed class MessageValidator : NativeValidator<ConditionalModel>
    {
        public MessageValidator()
        {
            RuleFor(x => x.Name, nameof(ConditionalModel.Name))
                .NotEmpty()
                .WithErrorCode("E_NAME_EMPTY")
                .WithMessage("Name is required.");
        }
    }

    [Fact]
    public void When_SkipsRule_WhenPredicateIsFalse()
    {
        var validator = new ConditionalValidator();
        var model = new ConditionalModel { Name = string.Empty, ValidateName = false, Age = 20 };

        ValidationResult result = validator.Validate(model);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Unless_SkipsRule_WhenPredicateIsTrue()
    {
        var validator = new ConditionalValidator();
        var model = new ConditionalModel { Age = 10, SkipAge = true };

        ValidationResult result = validator.Validate(model);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void When_ValidatesRule_WhenPredicateIsTrue()
    {
        var validator = new ConditionalValidator();
        var model = new ConditionalModel { Name = string.Empty, ValidateName = true, Age = 20 };

        ValidationResult result = validator.Validate(model);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
    }

    [Fact]
    public void Cascade_Stop_StopsAfterFirstFailure()
    {
        var validator = new CascadeValidator(CascadeMode.Stop);
        var model = new ConditionalModel { Name = string.Empty };

        ValidationResult result = validator.Validate(model);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
    }

    [Fact]
    public void Cascade_Continue_ReturnsAllFailures()
    {
        var validator = new CascadeValidator(CascadeMode.Continue);
        var model = new ConditionalModel { Name = string.Empty };

        ValidationResult result = validator.Validate(model);

        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
    }

    [Fact]
    public void WithMessage_And_WithErrorCode_OverrideRuleMetadata()
    {
        var validator = new MessageValidator();
        var model = new ConditionalModel { Name = string.Empty };

        ValidationResult result = validator.Validate(model);

        Assert.False(result.IsValid);
        var failure = Assert.Single(result.Errors);
        Assert.Equal("E_NAME_EMPTY", failure.ErrorCode);
        Assert.Equal("Name is required.", failure.ErrorMessage);
    }
}
