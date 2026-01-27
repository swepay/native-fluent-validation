using Native.FluentValidation.Abstractions;

namespace Native.FluentValidation.Rules;

public sealed class EmailRule : IPropertyRule<string?>
{
    public string ErrorCode => "Email";
    public string ErrorMessage => "Value must be a valid email address.";

    public bool IsValid(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return true;
        }

        var atIndex = value.IndexOf('@');
        if (atIndex <= 0 || atIndex == value.Length - 1)
        {
            return false;
        }

        var domainPart = value[(atIndex + 1)..];
        var dotIndex = domainPart.IndexOf('.');

        return dotIndex > 0 && dotIndex < domainPart.Length - 1;
    }
}
