using Native.FluentValidation.Abstractions;
using Native.FluentValidation.Results;

namespace Native.FluentValidation.Core;

public sealed class Rule<T, TValue> : IValidationRule<T>
{
    private CascadeMode _cascadeMode = CascadeMode.Continue;
    private Func<T, bool>? _condition;
    private bool _conditionNegated;

    public string PropertyName { get; }
    public Func<T, TValue> Accessor { get; }
    public IReadOnlyList<IPropertyRule<TValue>> Validators { get; }

    public Rule(
        string propertyName,
        Func<T, TValue> accessor,
        IReadOnlyList<IPropertyRule<TValue>> validators)
    {
        PropertyName = propertyName;
        Accessor = accessor;
        Validators = validators;
    }

    internal void SetCascadeMode(CascadeMode mode)
    {
        _cascadeMode = mode;
    }

    internal void SetCondition(Func<T, bool> predicate, bool negated)
    {
        _condition = predicate;
        _conditionNegated = negated;
    }

    public void Validate(T instance, IList<ValidationFailure> failures)
    {
        if (_condition is not null)
        {
            var shouldValidate = _condition(instance);
            if (_conditionNegated)
            {
                shouldValidate = !shouldValidate;
            }

            if (!shouldValidate)
            {
                return;
            }
        }

        var value = Accessor(instance);

        foreach (var validator in Validators)
        {
            if (!validator.IsValid(value))
            {
                failures.Add(new ValidationFailure(PropertyName, validator.ErrorCode, validator.ErrorMessage));

                if (_cascadeMode == CascadeMode.Stop)
                {
                    break;
                }
            }
        }
    }
}
