using Native.FluentValidation.Results;

namespace Native.FluentValidation.Abstractions;

public interface IValidationRule<in T>
{
    void Validate(T instance, IList<ValidationFailure> failures);
}
