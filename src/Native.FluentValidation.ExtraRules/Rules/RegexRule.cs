using System.Text.RegularExpressions;
using Native.FluentValidation.Abstractions;

namespace Native.FluentValidation.ExtraRules.Rules;

public sealed class RegexRule : IPropertyRule<string?>
{
    private readonly Regex _regex;

    public RegexRule(string pattern)
    {
        _regex = new Regex(pattern, RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);
    }

    public RegexRule(Regex regex)
    {
        _regex = regex;
    }

    public string ErrorCode => "Regex";
    public string ErrorMessage => "Value does not match the required pattern.";

    public bool IsValid(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        return _regex.IsMatch(value);
    }
}
