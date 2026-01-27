using Native.FluentValidation.Rules;
using Xunit;

namespace Native.FluentValidation.Tests;

public sealed class RulesTests
{
    [Fact]
    public void NotNullRule_Null_Fails()
    {
        var rule = new NotNullRule<string?>();
        Assert.False(rule.IsValid(null));
    }

    [Fact]
    public void NotNullRule_Value_Passes()
    {
        var rule = new NotNullRule<string>();
        Assert.True(rule.IsValid("ok"));
    }

    [Fact]
    public void NotEmptyRule_StringEmpty_Fails()
    {
        var rule = new NotEmptyRule<string>();
        Assert.False(rule.IsValid(string.Empty));
    }

    [Fact]
    public void NotEmptyRule_StringValue_Passes()
    {
        var rule = new NotEmptyRule<string>();
        Assert.True(rule.IsValid("x"));
    }

    [Fact]
    public void NotEmptyRule_CollectionEmpty_Fails()
    {
        var rule = new NotEmptyRule<ICollection<int>>();
        Assert.False(rule.IsValid(new List<int>()));
    }

    [Fact]
    public void NotEmptyRule_CollectionValue_Passes()
    {
        var rule = new NotEmptyRule<ICollection<int>>();
        Assert.True(rule.IsValid(new List<int> { 1 }));
    }

    [Fact]
    public void LengthRule_StringWithinRange_Passes()
    {
        var rule = new LengthRule<string>(2, 4);
        Assert.True(rule.IsValid("abcd"));
    }

    [Fact]
    public void LengthRule_StringTooShort_Fails()
    {
        var rule = new LengthRule<string>(2, 4);
        Assert.False(rule.IsValid("a"));
    }

    [Fact]
    public void LengthRule_CollectionWithinRange_Passes()
    {
        var rule = new LengthRule<ICollection<int>>(1, 2);
        Assert.True(rule.IsValid(new List<int> { 1 }));
    }

    [Fact]
    public void GreaterThanRule_ValueGreater_Passes()
    {
        var rule = new GreaterThanRule<int>(10);
        Assert.True(rule.IsValid(11));
    }

    [Fact]
    public void GreaterThanRule_ValueEqual_Fails()
    {
        var rule = new GreaterThanRule<int>(10);
        Assert.False(rule.IsValid(10));
    }

    [Fact]
    public void EmailRule_Valid_Passes()
    {
        var rule = new EmailRule();
        Assert.True(rule.IsValid("user@example.com"));
    }

    [Fact]
    public void EmailRule_Invalid_Fails()
    {
        var rule = new EmailRule();
        Assert.False(rule.IsValid("user@invalid"));
    }

    [Fact]
    public void MustRule_CustomPredicate_Applies()
    {
        var rule = new MustRule<int>(value => value % 2 == 0);
        Assert.True(rule.IsValid(2));
        Assert.False(rule.IsValid(3));
    }
}
