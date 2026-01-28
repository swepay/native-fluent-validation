using System.Text.RegularExpressions;
using Native.FluentValidation.Abstractions;
using Native.FluentValidation.Builders;
using Native.FluentValidation.ExtraRules.Rules;

namespace Native.FluentValidation.ExtraRules;

public static class RuleBuilderExtraExtensions
{
    public static RuleBuilder<T, string?> Matches<T>(this RuleBuilder<T, string?> builder, string pattern)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(pattern);
        return builder.AddRule(new RegexRule(pattern));
    }

    public static RuleBuilder<T, string?> Matches<T>(this RuleBuilder<T, string?> builder, Regex regex)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(regex);
        return builder.AddRule(new RegexRule(regex));
    }

    public static RuleBuilder<T, TValue> InclusiveBetween<T, TValue>(
        this RuleBuilder<T, TValue> builder,
        TValue min,
        TValue max)
        where TValue : IComparable<TValue>
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddRule(new InclusiveBetweenRule<TValue>(min, max));
    }

    public static RuleBuilder<T, TValue> LessThanOrEqual<T, TValue>(
        this RuleBuilder<T, TValue> builder,
        TValue value)
        where TValue : IComparable<TValue>
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddRule(new LessThanOrEqualRule<TValue>(value));
    }

    public static RuleBuilder<T, TValue> GreaterThanOrEqual<T, TValue>(
        this RuleBuilder<T, TValue> builder,
        TValue value)
        where TValue : IComparable<TValue>
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddRule(new GreaterThanOrEqualRule<TValue>(value));
    }

    public static RuleBuilder<T, TValue> NotEqual<T, TValue>(
        this RuleBuilder<T, TValue> builder,
        TValue value)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddRule(new NotEqualRule<TValue>(value));
    }
}
