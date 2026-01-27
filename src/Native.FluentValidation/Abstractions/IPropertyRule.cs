namespace Native.FluentValidation.Abstractions;

public interface IPropertyRule<in TValue>
{
    bool IsValid(TValue value);
    string ErrorCode { get; }
    string ErrorMessage { get; }
}
