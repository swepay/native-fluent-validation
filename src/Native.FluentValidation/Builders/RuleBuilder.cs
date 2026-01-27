using Native.FluentValidation.Abstractions;
using Native.FluentValidation.Core;
using Native.FluentValidation.Rules;

namespace Native.FluentValidation.Builders;

public sealed class RuleBuilder<T, TValue>
{
    private readonly List<IPropertyRule<TValue>> _validators;
    private int _lastIndex = -1;

    public Rule<T, TValue> Rule { get; }

    internal RuleBuilder(Rule<T, TValue> rule, List<IPropertyRule<TValue>> validators)
    {
        Rule = rule;
        _validators = validators;
    }

    public RuleBuilder<T, TValue> NotNull()
    {
        return AddRule(new NotNullRule<TValue>());
    }

    public RuleBuilder<T, TValue> NotEmpty()
    {
        return AddRule(new NotEmptyRule<TValue>());
    }

    public RuleBuilder<T, TValue> Length(int min, int max)
    {
        return AddRule(new LengthRule<TValue>(min, max));
    }

    public RuleBuilder<T, TValue> GreaterThan(TValue value)
    {
        return AddRule(new GreaterThanRule<TValue>(value));
    }

    public RuleBuilder<T, TValue> Must(Func<TValue, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return AddRule(new MustRule<TValue>(predicate));
    }

    public RuleBuilder<T, TValue> WithMessage(string message)
    {
        ArgumentNullException.ThrowIfNull(message);
        ReplaceLast((inner, code, _) => new ConfiguredRule<TValue>(inner, code, message));
        return this;
    }

    public RuleBuilder<T, TValue> WithErrorCode(string errorCode)
    {
        ArgumentNullException.ThrowIfNull(errorCode);
        ReplaceLast((inner, _, message) => new ConfiguredRule<TValue>(inner, errorCode, message));
        return this;
    }

    public RuleBuilder<T, TValue> Cascade(CascadeMode mode)
    {
        Rule.SetCascadeMode(mode);
        return this;
    }

    public RuleBuilder<T, TValue> When(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        Rule.SetCondition(predicate, negated: false);
        return this;
    }

    public RuleBuilder<T, TValue> Unless(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        Rule.SetCondition(predicate, negated: true);
        return this;
    }

    internal RuleBuilder<T, TValue> AddRule(IPropertyRule<TValue> rule)
    {
        _validators.Add(rule);
        _lastIndex = _validators.Count - 1;
        return this;
    }

    private void ReplaceLast(Func<IPropertyRule<TValue>, string?, string?, IPropertyRule<TValue>> factory)
    {
        if (_lastIndex < 0)
        {
            throw new InvalidOperationException("No validator has been added yet.");
        }

        var current = _validators[_lastIndex];
        var inner = current;
        string? errorCode = null;
        string? errorMessage = null;

        if (current is ConfiguredRule<TValue> configured)
        {
            inner = configured.Inner;
            errorCode = configured.ErrorCodeOverride;
            errorMessage = configured.ErrorMessageOverride;
        }

        _validators[_lastIndex] = factory(inner, errorCode, errorMessage);
    }
}
