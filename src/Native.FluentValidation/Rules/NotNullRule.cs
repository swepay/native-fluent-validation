using Native.FluentValidation.Abstractions;

namespace Native.FluentValidation.Rules;

public sealed class NotNullRule<TValue> : IPropertyRule<TValue>
{
    public string ErrorCode => "NotNull";
    public string ErrorMessage => "Value must not be null.";

    public bool IsValid(TValue value)
    {
        return value is not null;
    }
}
