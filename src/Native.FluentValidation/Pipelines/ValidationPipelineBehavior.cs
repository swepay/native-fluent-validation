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
        var validator = _serviceProvider.GetService<INativeValidator<TRequest>>();

        if (validator is not null)
        {
            var result = validator.Validate(request);
            if (!result.IsValid)
            {
                throw new ValidationException(result);
            }
        }

        return next();
    }
}
