using System.Collections.Generic;
using Native.FluentValidation.Abstractions;

namespace Native.FluentValidation.Rules;

public sealed class GreaterThanRule<TValue> : IPropertyRule<TValue>
{
    private readonly TValue _threshold;

    public GreaterThanRule(TValue threshold)
    {
        _threshold = threshold;
    }

    public string ErrorCode => "GreaterThan";
    public string ErrorMessage => $"Value must be greater than {_threshold}.";

    public bool IsValid(TValue value)
    {
        if (value is null)
        {
            return false;
        }

        return Comparer<TValue>.Default.Compare(value, _threshold) > 0;
    }
}
