using Native.FluentValidation.Abstractions;

namespace Native.FluentValidation.ExtraRules.Rules;

public sealed class GreaterThanOrEqualRule<TValue> : IPropertyRule<TValue>
    where TValue : IComparable<TValue>
{
    private readonly TValue _value;

    public GreaterThanOrEqualRule(TValue value)
    {
        _value = value;
    }

    public string ErrorCode => "GreaterThanOrEqual";
    public string ErrorMessage => "Value must be greater than or equal to the expected value.";

    public bool IsValid(TValue value)
    {
        return value.CompareTo(_value) >= 0;
    }
}
