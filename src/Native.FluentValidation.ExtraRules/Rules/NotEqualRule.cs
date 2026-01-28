using Native.FluentValidation.Abstractions;

namespace Native.FluentValidation.ExtraRules.Rules;

public sealed class NotEqualRule<TValue> : IPropertyRule<TValue>
{
    private readonly TValue _value;

    public NotEqualRule(TValue value)
    {
        _value = value;
    }

    public string ErrorCode => "NotEqual";
    public string ErrorMessage => "Value must not be equal to the specified value.";

    public bool IsValid(TValue value)
    {
        return !Equals(value, _value);
    }
}
