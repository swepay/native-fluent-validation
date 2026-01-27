using Native.FluentValidation.Rules;

namespace Native.FluentValidation.Builders;

public static class RuleBuilderExtensions
{
    public static RuleBuilder<T, string?> Email<T>(this RuleBuilder<T, string?> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddRule(new EmailRule());
    }
}
