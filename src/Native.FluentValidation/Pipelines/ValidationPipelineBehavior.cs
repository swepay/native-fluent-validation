using Microsoft.Extensions.DependencyInjection;
using Native.FluentValidation.Abstractions;
using Native.FluentValidation.Results;
using NativeMediator;

namespace Native.FluentValidation.Pipelines;

public sealed class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationPipelineBehavior(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ValueTask<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        var validators = _serviceProvider.GetServices<INativeValidator<TRequest>>();
        List<ValidationFailure>? failures = null;

        foreach (var validator in validators)
        {
            var result = validator.Validate(request);
            if (!result.IsValid)
            {
                failures ??= new List<ValidationFailure>();
                failures.AddRange(result.Errors);
            }
        }

        if (failures is not null)
        {
            throw new ValidationException(new ValidationResult(failures));
        }

        return next();
    }
}
