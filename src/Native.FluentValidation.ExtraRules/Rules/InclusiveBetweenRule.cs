using Native.FluentValidation.Abstractions;

namespace Native.FluentValidation.ExtraRules.Rules;

public sealed class InclusiveBetweenRule<TValue> : IPropertyRule<TValue>
    where TValue : IComparable<TValue>
{
    private readonly TValue _min;
    private readonly TValue _max;

    public InclusiveBetweenRule(TValue min, TValue max)
    {
        _min = min;
        _max = max;
    }

    public string ErrorCode => "InclusiveBetween";
    public string ErrorMessage => "Value is outside the allowed range.";

    public bool IsValid(TValue value)
    {
        return value.CompareTo(_min) >= 0 && value.CompareTo(_max) <= 0;
    }
}
