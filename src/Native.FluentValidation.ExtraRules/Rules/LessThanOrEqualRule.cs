using Native.FluentValidation.Abstractions;

namespace Native.FluentValidation.ExtraRules.Rules;

public sealed class LessThanOrEqualRule<TValue> : IPropertyRule<TValue>
    where TValue : IComparable<TValue>
{
    private readonly TValue _value;

    public LessThanOrEqualRule(TValue value)
    {
        _value = value;
    }

    public string ErrorCode => "LessThanOrEqual";
    public string ErrorMessage => "Value must be less than or equal to the expected value.";

    public bool IsValid(TValue value)
    {
        return value.CompareTo(_value) <= 0;
    }
}
