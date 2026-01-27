using Native.FluentValidation.Abstractions;

namespace Native.FluentValidation.Core;

public sealed class RuleSet<T>
{
    public string Name { get; }
    public IReadOnlyList<IValidationRule<T>> Rules { get; }

    public RuleSet(string name, IReadOnlyList<IValidationRule<T>> rules)
    {
        Name = name;
        Rules = rules;
    }
}
