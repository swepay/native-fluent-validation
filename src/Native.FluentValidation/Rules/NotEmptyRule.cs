using System.Collections;
using Native.FluentValidation.Abstractions;

namespace Native.FluentValidation.Rules;

public sealed class NotEmptyRule<TValue> : IPropertyRule<TValue>
{
    public string ErrorCode => "NotEmpty";
    public string ErrorMessage => "Value must not be empty.";

    public bool IsValid(TValue value)
    {
        if (value is null)
        {
            return false;
        }

        if (value is string text)
        {
            return text.Length > 0;
        }

        if (value is ICollection collection)
        {
            return collection.Count > 0;
        }

        throw new NotSupportedException("NotEmpty supports string or ICollection values.");
    }
}
