using Native.FluentValidation.Abstractions;

namespace Native.FluentValidation.Rules;

public sealed class ConfiguredRule<TValue> : IPropertyRule<TValue>
{
    public IPropertyRule<TValue> Inner { get; }
    public string? ErrorCodeOverride { get; }
    public string? ErrorMessageOverride { get; }

    public ConfiguredRule(IPropertyRule<TValue> inner, string? errorCodeOverride, string? errorMessageOverride)
    {
        Inner = inner;
        ErrorCodeOverride = errorCodeOverride;
        ErrorMessageOverride = errorMessageOverride;
    }

    public string ErrorCode => ErrorCodeOverride ?? Inner.ErrorCode;
    public string ErrorMessage => ErrorMessageOverride ?? Inner.ErrorMessage;

    public bool IsValid(TValue value)
    {
        return Inner.IsValid(value);
    }
}
