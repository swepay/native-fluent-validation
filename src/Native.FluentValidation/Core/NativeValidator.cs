using Native.FluentValidation.Abstractions;
using Native.FluentValidation.Builders;
using Native.FluentValidation.Results;

namespace Native.FluentValidation.Core;

public abstract class NativeValidator<T> : INativeValidator<T>
{
    private readonly List<IValidationRule<T>> _rules = new();

    protected RuleBuilder<T, TValue> RuleFor<TValue>(Func<T, TValue> accessor, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(accessor);

        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException("Property name must be provided explicitly.", nameof(propertyName));
        }

        var validators = new List<IPropertyRule<TValue>>();
        var rule = new Rule<T, TValue>(propertyName, accessor, validators);
        _rules.Add(rule);

        return new RuleBuilder<T, TValue>(rule, validators);
    }

    protected RuleBuilder<T, TValue> RuleFor<TValue>(PropertySelector<T, TValue> property)
    {
        ArgumentNullException.ThrowIfNull(property.Accessor);

        if (string.IsNullOrWhiteSpace(property.Name))
        {
            throw new ArgumentException("Property name must be provided explicitly.", nameof(property));
        }

        var validators = new List<IPropertyRule<TValue>>();
        var rule = new Rule<T, TValue>(property.Name, property.Accessor, validators);
        _rules.Add(rule);

        return new RuleBuilder<T, TValue>(rule, validators);
    }

    public ValidationResult Validate(T instance)
    {
        ArgumentNullException.ThrowIfNull(instance);

        var failures = new List<ValidationFailure>();
        foreach (var rule in _rules)
        {
            rule.Validate(instance, failures);
        }

        return failures.Count == 0
            ? ValidationResult.Success
            : new ValidationResult(failures);
    }
}
