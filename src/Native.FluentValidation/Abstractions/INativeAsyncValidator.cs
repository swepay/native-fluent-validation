using Native.FluentValidation.Results;

namespace Native.FluentValidation.Abstractions;

public interface INativeAsyncValidator<T>
{
    ValueTask<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default);
}
