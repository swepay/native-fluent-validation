using Native.FluentValidation.Results;

namespace Native.FluentValidation.Abstractions;

public interface INativeValidator<in T>
{
    ValidationResult Validate(T instance);
}
