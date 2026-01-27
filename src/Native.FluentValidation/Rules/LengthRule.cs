using System.Collections;
using Native.FluentValidation.Abstractions;

namespace Native.FluentValidation.Rules;

public sealed class LengthRule<TValue> : IPropertyRule<TValue>
{
    private readonly int _min;
    private readonly int _max;

    public LengthRule(int min, int max)
    {
        if (min < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(min), "Min must be non-negative.");
        }

        if (max < min)
        {
            throw new ArgumentOutOfRangeException(nameof(max), "Max must be greater than or equal to min.");
        }

        _min = min;
        _max = max;
    }

    public string ErrorCode => "Length";
    public string ErrorMessage => $"Value length must be between {_min} and {_max}.";

    public bool IsValid(TValue value)
    {
        if (value is null)
        {
            return false;
        }

        var length = value switch
        {
            string text => text.Length,
            ICollection collection => collection.Count,
            _ => throw new NotSupportedException("Length supports string or ICollection values.")
        };

        return length >= _min && length <= _max;
    }
}
