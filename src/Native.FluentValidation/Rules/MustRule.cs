using Native.FluentValidation.Abstractions;

namespace Native.FluentValidation.Rules;

public sealed class MustRule<TValue> : IPropertyRule<TValue>
{
    private readonly Func<TValue, bool> _predicate;

    public MustRule(Func<TValue, bool> predicate)
    {
        _predicate = predicate;
    }

    public string ErrorCode => "Must";
    public string ErrorMessage => "Value did not satisfy the custom predicate.";

    public bool IsValid(TValue value)
    {
        return _predicate(value);
    }
}
